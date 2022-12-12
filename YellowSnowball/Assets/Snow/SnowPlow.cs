using UnityEngine;

public class SnowPlow : MonoBehaviour
{
    public Texture2D PlowPattern;
    public float PlowSizeMeters;

    public double? SaltDurationSeconds;

    public ParticleSystem JetStream;

    public float AverageMetersChangedLastPlow { get; private set; }

    private void Start()
    {
    }

    void Plow(SnowTerrain snow)
    {
        var maybeSurfacePos = snow.WorldToSurface(transform.position);
        if (maybeSurfacePos != null)
        {
            var sp = maybeSurfacePos.Value;

            double? saltExpirationTime = SaltDurationSeconds.HasValue ? Time.timeAsDouble + SaltDurationSeconds.Value : null;

            var carvedData = snow.Carve(new Vector2(sp.x, sp.y), PlowSizeMeters, PlowPattern, sp.z, saltExpirationTime, true);
            AverageMetersChangedLastPlow = carvedData.AverageMetersChanged;

            if (JetStream != null)
            {
                var emission = JetStream.emission;
                // the clamp max ends up being the rate over time value displayed in the inspector
                emission.rateOverTimeMultiplier = Mathf.Clamp(AverageMetersChangedLastPlow * 10000, 1, 100);
            }
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
