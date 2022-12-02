using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class SnowTerrain : MonoBehaviour
{
    //public Texture2D InitialTexture;

    public int Resolution = 128; // configurable?

    private const float c_maxDensityKgm3 = 480;

    // todo: density map

    Mesh m_snowMesh;
    NativeArray<Vector3> m_snowVertices;

    public void Setup()
    {
        Reset();
    }

    public void Reset()
    {
        if (m_snowMesh != null)
        {
            Resources.UnloadAsset(m_snowMesh);
        }

        /*
        //m_snowTexture = new Texture2D(InitialTexture.width, InitialTexture.height, TextureFormat.RGBA32, false, true);
        m_snowTexture = new Texture2D(Resolution, Resolution, TextureFormat.RGBA32, false, true);

        byte initialDepth = (byte)(0.6f / c_maxDepthMeters * byte.MaxValue);
        byte initialDensity = (byte)(300 / c_maxDensityKgm3 * byte.MaxValue);
        */

        m_snowVertices = new NativeArray<Vector3>(Resolution * Resolution, Allocator.Persistent);
        var uvs = new Vector2[Resolution * Resolution];

        // todo: triangle grid?

        var fResolution = (float)Resolution;
        var offset = new Vector3(-0.5f, 0.5f, -0.5f);
        for (int r = 0; r < Resolution; ++r)
        {
            for (int c = 0; c < Resolution; ++c)
            {
                int i = r * Resolution + c; // add 1px border around map?
                uvs[i] = new Vector2(c / fResolution, r / fResolution);
                m_snowVertices[i] = offset + new Vector3(uvs[i].x, transform.localScale.y, uvs[i].y);
            }
        }

        int numTris = Resolution - 1;
        var tris = new int[(numTris * numTris) * 6];

        for (int ti = 0, vi = 0, y = 0; y < numTris; ++y, ++vi)
        {
            for (int x = 0; x < numTris; ++x, ti += 6, ++vi)
            {
                tris[ti] = vi;
                tris[ti + 3] = tris[ti + 2] = vi + 1;
                tris[ti + 4] = tris[ti + 1] = vi + numTris + 1;
                tris[ti + 5] = vi + numTris + 2;
            }
        }

        //var pixels = m_snowTexture.GetRawTextureData<Color>();
        //for (int i = 0; i < pixels.Length; ++i)
        //{
        //    pixels[i] = new Color32(initialDepth, initialDensity, 0, 0);
        //}
        //m_snowTexture.Apply();

        m_snowMesh = new Mesh();
        m_snowMesh.MarkDynamic();
        m_snowMesh.SetVertices(m_snowVertices);
        m_snowMesh.uv = uvs;
        m_snowMesh.triangles = tris;
        m_snowMesh.RecalculateNormals();
        m_snowMesh.RecalculateBounds();

        // requires mesh filter
        if (!TryGetComponent<MeshFilter>(out var filter))
            filter = gameObject.AddComponent<MeshFilter>();
        filter.mesh = m_snowMesh;
    }

    /// <summary>Deform the snow with the given pattern applying the specified force</summary>
    // e.g. for foot steps
    public void Deform(Vector2 worldCenter, Texture2D pattern, float forceNewtons) // amount is in newtons?
    {
        // snow pattern should push snow up on edges?

    }

    /// <summary>Carve the snow with the given pattern to the max depth of toDepth</summary>
    // e.g. for a snowblower
    public void Carve(Vector2 worldCenter, Texture2D pattern, float toDepth)
    {
        
    }

    void Start()
    {
        Reset();
    }

    void RecalculateMesh(Rect region)
    {
        
    }

    Vector3? lastHitPoint;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position + new Vector3(0, transform.localScale.y / 2, 0), transform.localScale);
        Gizmos.DrawCube(transform.position, new Vector3(0.5f, 0.5f, 0.5f));
        if (lastHitPoint.HasValue)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(lastHitPoint.Value, 0.5f);
        }
    }

    /// <summary>
    /// Check and recreate <see cref="m_snowVertices"/>. This is needed after a hot-reload
    /// </summary>
    private void RecreateMeshVertices()
    {
        if (m_snowVertices.Length > 0)
            return;

        // there is probably a better way to do this (supposedly ScriptableSingleton is an option)
        List<Vector3> verts = new();
        m_snowMesh.GetVertices(verts);
        m_snowVertices = new NativeArray<Vector3>(verts.ToArrayPooled(), Allocator.Persistent);
        Debug.Log("Recreated snow vertices from mesh");
    }

    // debugging
    private void Update()
    {
        // crude testing
        if (Input.GetMouseButton(0))
        {
            var ray = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y));
            var plane = new Plane(transform.up, transform.position);
            if (plane.Raycast(ray, out var dist))
            {
                var pos = ray.GetPoint(dist);
                lastHitPoint = pos;
                //var pos = Vector3.ProjectOnPlane(lastHitPoint.Value, plane.normal);
                pos -= transform.position;

                var x = Vector3.Dot(pos, transform.right) / transform.localScale.x + 0.5f;
                var y = Vector3.Dot(pos, transform.forward) / transform.localScale.z + 0.5f;

                var xv = (int)Mathf.Round(x * Resolution);
                var yv = (int)Mathf.Round(y * Resolution);
                if (xv >= 0 && yv >= 0 && xv < Resolution && yv < Resolution)
                {
                    RecreateMeshVertices();

                    var vi = xv + yv * Resolution;
                    var v = m_snowVertices[vi];
                    v.y = Mathf.Max(0, v.y - (6f * Time.deltaTime));
                    m_snowVertices[vi] = v;
                    m_snowMesh.SetVertices(m_snowVertices);
                    m_snowMesh.UploadMeshData(false);
                }
            }
            else
            {
                lastHitPoint = null;
            }
        }
    }
}
