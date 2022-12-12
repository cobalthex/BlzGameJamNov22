using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class SnowTerrain : MonoBehaviour
{
    /// <summary>
    /// Extra data for each snow vertex (does not include height, which is encoded into the mesh vertices)
    /// </summary>
    public struct SnowVertexData
    {
        public double saltExpirationTime;
    }

    //public Texture2D InitialTexture;

    public int Resolution = 128;

    /// <summary>
    /// Maximum height of the snow (in meters). Min is 0
    /// </summary>
    public float MaxHeightMeters = 5;

    [Header("Debugging")]
    public Texture2D RemoveBrush;
    public Texture2D AddBrush;
    public float BrushSizeMeters = 10f;
    public float BrushCarveDepthMeters = 0.5f;
    public Transform TestObj;

    // todo: density map

    Mesh m_snowMesh;
    NativeArray<Vector3> m_snowVertices;
    SnowVertexData[] m_snowVertexExtras;

    public float RemainingSnow { get; private set; }

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
        m_snowVertexExtras = new SnowVertexData[Resolution * Resolution];

        // todo: trianglular grid?

        RemainingSnow = 0;

        var fResolution = (float)Resolution;
        var offset = new Vector3(-0.5f, 0, -0.5f);
        for (int r = 0; r < Resolution; ++r)
        {
            for (int c = 0; c < Resolution; ++c)
            {
                int i = r * Resolution + c; // add 1px border around map?
                uvs[i] = new Vector2(c / fResolution, r / fResolution);

                float depth;
                // make the edge of the mesh 0
                //if (c < 1 || c >= Resolution - 1 ||
                //    r < 1 || r >= Resolution - 1)
                //{
                //    depth = 0; // do this in a separate loop?
                //}
                //else
                {
                    depth = 1 - Random.Range(0, surfaceVariability);
                }

                m_snowVertices[i] = offset + new Vector3(uvs[i].x, depth, uvs[i].y);
                RemainingSnow += depth;
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

    /// <summary>
    /// Submit changes to the GPU
    /// </summary>
    public void CommitVertices()
    {
        m_snowMesh.SetVertices(m_snowVertices);
        m_snowMesh.UploadMeshData(false);
    }

    /// <summary>
    /// Get the depth from the base at a local position on the terrain
    /// </summary>
    /// <param name="localPosition">The local position, in meters</param>
    /// <returns>The depth, in meters, from the base at the specified position, or null if the position is out of bounds</returns>
    public (float, SnowVertexData)? SnowAtPoint(Vector2 localPosition)
    {
        localPosition.x = localPosition.x / transform.localScale.x * Resolution;
        localPosition.y = localPosition.y / transform.localScale.z * Resolution;
        if (localPosition.x < 0 || localPosition.y < 0 || localPosition.x >= Resolution || localPosition.y >= Resolution)
            return null;

        RecreateMeshVertices();
        int index = (int)localPosition.x + (int)localPosition.y * Resolution;
        return (m_snowVertices[index].y * transform.localScale.y, m_snowVertexExtras[index]);
    }

    /// <summary>
    /// Get the position on the surface relative to a screen point
    /// </summary>
    /// <param name="screenPosition">Where on the screen to start</param>
    /// <returns>The relative surface position, in meters, or null if the mouse isn't inside the surface</returns>
    public Vector2? ScreenToSurface(Vector2 screenPosition)
    {
        var ray = Camera.main.ScreenPointToRay(new Vector3(screenPosition.x, screenPosition.y, 0));
        var plane = new Plane(transform.up, transform.position + new Vector3(0, transform.localScale.y, 0));
        if (!plane.Raycast(ray, out var planeDist))
            return null;

        var pos = ray.GetPoint(planeDist);
        pos -= transform.position;

       var x = Vector3.Dot(pos, transform.right) + (transform.localScale.x / 2);
       var y = Vector3.Dot(pos, transform.forward) + (transform.localScale.z / 2);

        if (x < 0 || y < 0 || x >= transform.localScale.x || y >= transform.localScale.z)
            return null;

        return new Vector2(x, y);
    }

    /// <summary>
    /// Get the surface position for a point in the world
    /// </summary>
    /// <param name="worldPosition">A world relative position</param>
    /// <returns>The position if in the surface, or null if not. The Z coordinate is the signed height off the base</returns>
    public Vector3? WorldToSurface(Vector3 worldPosition)
    {
        var plane = new Plane(transform.up, transform.position);
        var closest = plane.ClosestPointOnPlane(worldPosition - transform.position);

        var surfaceRelative = new Vector2(closest.x + transform.localScale.x / 2, closest.z + transform.localScale.z / 2);
        if (surfaceRelative.x < 0 || surfaceRelative.y < 0 ||
            surfaceRelative.x >= transform.localScale.x || surfaceRelative.y >= transform.localScale.z)
            return null;

        return new Vector3(surfaceRelative.x, surfaceRelative.y, worldPosition.y - closest.y);
    }
    public struct SnowMutation
    {
        public float totalMetersChanged;
        public int verticesModified;

        public float AverageMetersChanged => (verticesModified > 0 ? totalMetersChanged / verticesModified : 0);
    }

    /// <summary>
    /// Deform the snow with the given pattern applying the specified force
    /// </summary>
    /// <param name="relativePosition">where to place the center of the pattern, from 0 to 1</param>
    /// <param name="xSize">How large should the texture should be painted, 1 = the width of the mesh. y size is sized to keep the coorrect aspect</param>
    /// <param name="pattern">
    /// The pattern to paint.
    /// Only single channel textures should be used
    /// 0 = remove 1m, 127 = none, 255 = add 1m. values scaled by patternScale
    /// </param>
    /// <param name="patternScaleMeters">How much to scale the pattern by, values in meters</param>
    /// <param name="commit">Submit changes to the GPU (use false if calling multiple times in a frame)</param>
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
    /// <returns>The amount of snow deformed in sq meters</returns>
    // e.g. for foot steps
    public SnowMutation Deform(Vector2 relativePosition, float xSize, Texture2D pattern, float patternScaleMeters, double? addSaltExpirationTime, bool commit = true)
    {
        // todo: to support forces, would need density bitmap

        patternScaleMeters /= transform.localScale.y;

        float DeformFn(float curLevel, float delta)
        {
            return curLevel + (delta * patternScaleMeters);
        }
        
        return MutateSnow(relativePosition, xSize, pattern, addSaltExpirationTime, DeformFn, commit);
    }

    /// <summary>
    /// Carve the snow with the given pattern to the max depth of toDepth
    /// </summary>
    /// <param name="relativePosition">where to place the center of the pattern, from 0 to localScale.x/y</param>
    /// <param name="xSize">How large should the texture should be painted, 1 = 1m. y size is sized to keep the coorrect aspect</param>
    /// <param name="pattern">
    /// The pattern to paint
    /// <seealso cref="Deform"/>
    /// </param>
    /// <param name="toDepthMeters">The maximum depth to carve to (meters up from the bottom)</param>
    /// <param name="commit">Submit changes to the GPU (use false if calling multiple times in a frame)</param>
    /// <seealso cref="Deform"/>
    /// <returns>The amount of snow mutated in sq meters</returns>
    // e.g. for a snowblower
    public SnowMutation Carve(Vector2 relativePosition, float size, Texture2D pattern, float toDepthMeters, double? addSaltExpirationTime, bool commit = true)
    {
        toDepthMeters /= transform.localScale.y;

        float CarveFn(float curLevel, float delta)
        {
            return Mathf.Max(Mathf.Min(toDepthMeters, curLevel), curLevel + delta);
        }

        // todo: support additive textures?

        return MutateSnow(relativePosition, size, pattern, addSaltExpirationTime, CarveFn, commit);
    }

    public delegate float MutateSnowFn(float currentSnowLevel, float patternValMeters);

    /// <summary>
    /// Mutate the snow level
    /// </summary>
    /// <param name="relativePosition">where to place the center of the pattern, from 0 to localScale.x/y</param>
    /// <param name="xSize">How large should the texture should be painted, 1 = 1m. y size is sized to keep the coorrect aspect</param>
    /// <param name="pattern">
    /// The pattern to paint
    /// <seealso cref="Deform"/>
    /// </param>
    /// <param name="mutator">A function that mutates snow, returning the new height of the snow</param>
    /// <remarks>B/c this takes in a mutator fn, this can get expensive to run</remarks>
    /// <returns>The amount of snow mutated in sq meters</returns>
    public SnowMutation MutateSnow(Vector2 relativePosition, float size, Texture2D pattern, double? addSaltExpirationTime, MutateSnowFn mutator, bool commit)
    {
        if (transform.localScale.y == 0)
            return new SnowMutation();

        var now = Time.timeAsDouble;

        // the order of this math can probably be optimized; process is normalize from meters then multiply by the resolution

        float xSize = size / transform.localScale.x * Resolution;

        float ySize = size / transform.localScale.z * Resolution;

        float patternXScale = pattern.width / xSize;
        float patternYScale = pattern.height / ySize;

        relativePosition.x = (relativePosition.x / transform.localScale.x) * Resolution - (xSize / 2);
        relativePosition.y = (relativePosition.y / transform.localScale.z) * Resolution - (ySize / 2);

        RecreateMeshVertices();

        float deltaValues = 0;

        int n = 0;

        var pixels = pattern.GetPixelData<byte>(0); //todo: rgba?
        // pattern sampling/filtering?
        for (float y = Mathf.Max(0, relativePosition.y);
            y < Mathf.Min(Resolution - 0, relativePosition.y + ySize);
            ++y)
        {
            for (float x = Mathf.Max(0, relativePosition.x);
                x < Mathf.Min(Resolution - 0, relativePosition.x + xSize);
                ++x)
            {
                int patternX = (int)((x - relativePosition.x) * patternXScale);
                int patternY = (int)((y - relativePosition.y) * patternYScale);

                const byte half = byte.MaxValue / 2;

                byte patternVal = pixels[patternX + patternY * pattern.width];
                float scaledPatternVal = ((patternVal - half) / (float)patternVal) / transform.localScale.y;

                var vi = (int)x + (int)y * Resolution;
                var v = m_snowVertices[vi];
                var prevDepth = v.y;

                ref var extraData = ref m_snowVertexExtras[vi];

                var saltExpiration = extraData.saltExpirationTime;
                if (addSaltExpirationTime.HasValue)
                    extraData.saltExpirationTime = addSaltExpirationTime.Value;

                v.y = Mathf.Clamp(mutator(prevDepth, scaledPatternVal), 0, MaxHeightMeters);
                if (saltExpiration > now) // can only remove snow with salt (ideally it would remove salt if plowing below depth, but that's more work)
                    v.y = Mathf.Min(v.y, prevDepth);

                m_snowVertices[vi] = v;
                deltaValues += (prevDepth - v.y);
                ++n;
            }
        }

        RemainingSnow -= deltaValues;
        if (commit)
            CommitVertices();

        return new SnowMutation { totalMetersChanged = deltaValues * transform.localScale.y, verticesModified = n };
    }

    /// <summary>
    /// Mutate a single point of the snow (getting the closest vertex). <see cref="CommitVertices"/> must be called manually
    /// </summary>
    /// <param name="relativePosition">Where in the snow mesh (meters)</param>
    /// <param name="mutator">How to change the snow (takes in the current snow level)</param>
    /// <returns>True if the snow was mutated</returns>
    public bool MutateSnowNoCommit(Vector2 relativePosition, System.Func<float, float> mutator)
    {
        // average value to nearby vertices?

        relativePosition.x = Mathf.Round((relativePosition.x / transform.localScale.x) * Resolution);
        relativePosition.y = Mathf.Round((relativePosition.y / transform.localScale.z) * Resolution);

        RecreateMeshVertices();

        if (relativePosition.x < 0 || relativePosition.y < 0 || relativePosition.x >= Resolution || relativePosition.y >= Resolution)
            return false;

        var vi = (int)relativePosition.x + (int)relativePosition.y * Resolution;
        var v = m_snowVertices[vi];
        var lastPos = v.y;
        v.y = Mathf.Max(0, mutator(lastPos));
        m_snowVertices[vi] = v;

        return lastPos != v.y;
    }

    void Start()
    {
        Reset();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position + new Vector3(0, transform.localScale.y / 2, 0), transform.localScale);
        var corner = transform.position;
        corner.x -= transform.localScale.x / 2;
        corner.z -= transform.localScale.z / 2;
        Gizmos.DrawCube(corner, new Vector3(0.5f, 0.5f, 0.5f));
    }

    private void OnGUI()
    {
        string depth = "(Out of bounds)";
        var mousePos = ScreenToSurface(Input.mousePosition);
        if (mousePos.HasValue)
            depth = SnowAtPoint(mousePos.Value).ToString();

        Vector3? surfacePos = TestObj == null ? null : WorldToSurface(TestObj.position);

        GUI.color = Color.black;
        GUI.Label(new Rect(21, 21, 200, 20), "Depth: " + depth);
        GUI.Label(new Rect(21, 41, 200, 20), $"Snow remaining: {RemainingSnow}m");
        GUI.Label(new Rect(20, 61, 200, 20), $"test obj pos: {surfacePos}");

        GUI.color = new Color(144, 24, 0);
        GUI.Label(new Rect(20, 20, 200, 20), "Depth: " + depth);
        GUI.Label(new Rect(20, 40, 200, 20), $"Snow remaining: {RemainingSnow}m");
        GUI.Label(new Rect(20, 60, 200, 20), $"test obj pos: {surfacePos}");
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
    Vector2? lastRelPos;
    private void Update()
    {
#if DEBUG
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reset();
            return;
        }
#endif

        var snowBrushInput = System.MathF.Sign(Input.GetAxis("SnowBrush")); // unity mathf does not return 0 -> 0
        if (snowBrushInput == 0)
        {
            lastRelPos = null;
            return;
        }

        var brush = snowBrushInput > 0 ? RemoveBrush : AddBrush;
        if (brush == null)
            return;

        var surfacePos = ScreenToSurface(Input.mousePosition);
        if (surfacePos == null)
            return;

        if (Input.GetKey(KeyCode.LeftShift) || lastRelPos == null)
        {
            var start = lastRelPos.GetValueOrDefault(surfacePos.Value);

            var dir = (surfacePos.Value - start);
            var dist = Mathf.Max(1, dir.magnitude);
            dir /= dist;

            for (float i = 0; i <= dist; i += (transform.localScale.x / Resolution))
            {
                Deform(start + dir * i, BrushSizeMeters, brush, 0.1f, null, commit: false);
            }
            CommitVertices();
         
            lastRelPos = surfacePos.Value;
        }
    }
}
