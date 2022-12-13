using Unity.VisualScripting;
using UnityEngine;

public class SnowPlow : MonoBehaviour
{
    public Texture2D PlowPattern;
    public float PlowSizeMeters;

    public double? SaltDurationSeconds;

    public ParticleSystem JetStream;

    public float AverageMetersChangedLastPlow { get; private set; }

    private float m_jetEmissionRate;

    private void Start()
    {
        if (JetStream != null)
            m_jetEmissionRate = JetStream.emission.rateOverTimeMultiplier; // this is the value that's displayed in the 'rate over time' (at least when it's a single value)
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
                // * 10k is a 'good enough' value
                emission.rateOverTimeMultiplier = Mathf.Clamp(AverageMetersChangedLastPlow * 10000, 0, m_jetEmissionRate);
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
