using Unity.VisualScripting;
using UnityEngine;

public class SnowPlow : MonoBehaviour
{
    public Texture2D PlowPattern;
    public float PlowSizeMeters;

    Vector3 lastPosition;

    private void Start()
    {
        lastPosition = transform.position;
    }

    void Plow(SnowTerrain snow)
    {
        var maybeSurfacePos = snow.WorldToSurface(transform.position);
        if (maybeSurfacePos != null)
        {
            var sp = maybeSurfacePos.Value;
            snow.Carve(new Vector2(sp.x, sp.y), PlowSizeMeters, PlowPattern, 0.0f /* TODO: get height at pos? */, true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<SnowTerrain>(out var snow))
            Plow(snow);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<SnowTerrain>(out var snow))
            Plow(snow);
    }
}
