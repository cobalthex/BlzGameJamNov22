using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class SnowTerrain : MonoBehaviour
{
    //public Texture2D InitialTexture;

    public int Resolution = 128; // configurable?

    public Texture2D Paintbrush;
    public float BrushSizeFraction = 0.1f;

    // todo: density map

    Mesh m_snowMesh;
    NativeArray<Vector3> m_snowVertices;

    public void Setup()
    {
        Reset();
    }

    public void Reset(float surfaceVariability = 0.005f) // initial texture?
    {
        // windswept patterns?

        m_snowMesh?.Clear();

        m_snowVertices = new NativeArray<Vector3>(Resolution * Resolution, Allocator.Persistent);
        var uvs = new Vector2[Resolution * Resolution];

        // todo: trianglular grid?

        var fResolution = (float)Resolution;
        var offset = new Vector3(-0.5f, 0, -0.5f);
        for (int r = 0; r < Resolution; ++r)
        {
            for (int c = 0; c < Resolution; ++c)
            {
                int i = r * Resolution + c; // add 1px border around map?
                uvs[i] = new Vector2(c / fResolution, r / fResolution);

                float depth;
                if (c == 0 || c == Resolution - 1 ||
                    r == 0 || r == Resolution - 1)
                {
                    depth = 0; // do this in a separate loop?
                }
                else
                {
                    depth = 1 - Random.Range(0, surfaceVariability);
                }

                m_snowVertices[i] = offset + new Vector3(uvs[i].x, depth, uvs[i].y);

                // todo: make rim all 0s
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
        m_snowMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
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

    void CommitVertices()
    {
        m_snowMesh.SetVertices(m_snowVertices);
        m_snowMesh.UploadMeshData(false);
    }

    /// <summary>
    /// Deform the snow with the given pattern applying the specified force
    /// </summary>
    /// <param name="localPosition">where to place the center of the pattern, from 0 to 1</param>
    /// <param name="xSize">How large should the texture should be painted, 1 = the width of the mesh. y size is sized to keep the coorrect aspect</param>
    /// <param name="pattern">
    /// The pattern to paint.
    /// Only single channel textures should be used
    /// 0 = max extrude, 127 = none, 255 = max protrude.
    /// max = the full maximum height of the mesh (which is scaled by localScale.y)
    /// </param>
    /// <param name="pressureMpa">How much force to apply when painting the texture (in newtons), affects how deep the texture is pressed</param>
    /// <remarks>
    /// When creating pattern textures, set the following parameters in the texture import settings:
    /// Texture type = single channel
    /// Texture shape = 2D
    /// Channel = red (alpha also works depending on how texture is authored)
    /// Non-Power of 2 = None
    /// Read/Write = true
    /// Generate Mipmaps = false
    /// Format = R 8
    /// </remarks>
    // e.g. for foot steps
    public void Deform(Vector2 localPosition, float xSize, Texture2D pattern, float pressureMpa) // amount is in newtons?
    {
        // snow pattern should push snow up on edges?

        // todo: depth should be based on localScale.y (real height) then normalized

        // TODO: calculate depth to point at localPosition then carve to that depth
        float depthMeters = 0.8f;
        Carve(localPosition, xSize, pattern, depthMeters);
    }

    /// <summary>
    /// Carve the snow with the given pattern to the max depth of toDepth
    /// </summary>
    /// <param name="localPosition">where to place the center of the pattern, from 0 to 1</param>
    /// <param name="xSize">How large should the texture should be painted, 1 = the width of the mesh. y size is sized to keep the coorrect aspect</param>
    /// <param name="pattern">
    /// The pattern to paint.
    /// <seealso cref="Deform"/>
    /// <param name="toDepthMeters">The maximum depth to carve to (meters up from the bottom)</param>
    /// <seealso cref="Deform"/>
    // e.g. for a snowblower
    public void Carve(Vector2 localPosition, float xSize, Texture2D pattern, float toDepthMeters)
    {
        if (transform.localScale.y == 0)
            return;

        toDepthMeters /= transform.localScale.y;

        xSize *= Resolution;
        float ySize = xSize * ((float)pattern.height / pattern.width);

        float patternXScale = pattern.width / xSize;
        float patternYScale = pattern.height / ySize;

        localPosition.Scale(new Vector2(Resolution, Resolution));
        localPosition -= new Vector2(xSize / 2, ySize / 2);

        // use rects+intersection() for cleaner math?

        var pixels = pattern.GetPixelData<byte>(0); //todo: rgba
        // pattern sampling/filtering?
        for (float y = Mathf.Max(0, localPosition.y); y < Mathf.Min(Resolution, localPosition.y + ySize); ++y)
        {
            for (float x = Mathf.Max(0, localPosition.x); x < Mathf.Min(Resolution, localPosition.x + xSize); ++x)
            {
                int patternX = (int)((x - localPosition.x) * patternXScale);
                int patternY = (int)((y - localPosition.y) * patternYScale);

                int patternVal = pixels[patternX + patternY * pattern.width];

                const byte half = byte.MaxValue / 2;

                var vi = (int)x + (int)y * Resolution;
                var v = m_snowVertices[vi];
                if (patternVal < half)
                    v.y = Mathf.Min(v.y, (patternVal / (float)half) + toDepthMeters);
                else if (patternVal > half)
                    v.y += ((patternVal - half) / (float)half);

                m_snowVertices[vi] = v;
            }
        }

        CommitVertices();
    }

    void Start()
    {
        Reset();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position + new Vector3(0, transform.localScale.y / 2, 0), transform.localScale);
        Gizmos.DrawCube(transform.position, new Vector3(0.5f, 0.5f, 0.5f));
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
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reset();
            return;
        }

        var ray = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y));
        var plane = new Plane(transform.up, transform.position + new Vector3(0, transform.localScale.y, 0));
        if (!plane.Raycast(ray, out var dist))
            return;
        
        var pos = ray.GetPoint(dist);
        pos -= transform.position;

        var x = Vector3.Dot(pos, transform.right) / transform.localScale.x + 0.5f;
        var y = Vector3.Dot(pos, transform.forward) / transform.localScale.z + 0.5f;

        if (x < 0 || y < 0 || x >= 1 || y >= 1)
            return;

        if (Paintbrush == null)
            return;

        if ((Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButton(0)) ||
            Input.GetMouseButtonDown(0))
        {
            RecreateMeshVertices();
            Deform(new Vector2(x, y), BrushSizeFraction, Paintbrush, 400);
        }
    }
}
