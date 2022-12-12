using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class SnowStorm : MonoBehaviour
{
    [Range(0.001f, 0.1f)]
    public float SnowflakeHeightMeters = 0.01f;

    public Texture2D SnowflakeDeformPattern;

    /// <remarks>Ideally this would be the same as <see cref="SnowflakeHeightMeters"/> but b/c theres no texture filtering this needs to be much larger</remarks>
    public float SnowflakePatternSizeMeters = 0.5f;

    ParticleSystem m_particleSystem;
    List<ParticleSystem.Particle> m_particlesEntered = new List<ParticleSystem.Particle>();

    private void OnEnable()
    {
        m_particleSystem = GetComponent<ParticleSystem>();
        var main = m_particleSystem.main;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        var triggerMod = m_particleSystem.trigger;
        triggerMod.enabled = true;
        triggerMod.colliderQueryMode = ParticleSystemColliderQueryMode.One;
        triggerMod.enter = ParticleSystemOverlapAction.Callback;
        triggerMod.inside = ParticleSystemOverlapAction.Kill;

        var terrains = FindObjectsOfType<SnowTerrain>();
        for (int i = 0; i < terrains.Length; ++i)
        {
            if (terrains[i].TryGetComponent<Collider>(out var collider))
                triggerMod.SetCollider(i, collider);
        }
    }

    // TODO: make this a singleton?
    void OnParticleTrigger()
    {
        int numEntered = m_particleSystem.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, m_particlesEntered, out var colliders);

        var terrains = HashSetPool<SnowTerrain>.Get();
        for (int i = 0; i < numEntered; ++i)
        {
            ParticleSystem.Particle p = m_particlesEntered[i];
            p.remainingLifetime = 0;
            if (colliders.GetCollider(i, 0).TryGetComponent<SnowTerrain>(out var terrain))
            {
                var relPos = terrain.WorldToSurface(p.position);
                if (relPos == null)
                    continue;

                //if (!terrain.MutateSnowNoCommit(new Vector2(relPos.Value.x, relPos.Value.y), (cur) => cur + SnowflakeHeightMeters))
                //    continue;

                if (terrain.Deform(new Vector2(relPos.Value.x, relPos.Value.y), SnowflakePatternSizeMeters, SnowflakeDeformPattern, SnowflakeHeightMeters, null, false) == 0)
                    continue;

                terrains.Add(terrain);
            }
        }

        foreach (var terrain in terrains)
            terrain.CommitVertices();
        HashSetPool<SnowTerrain>.Release(terrains);

        // re-assign the modified particles back into the particle system
        m_particleSystem.SetTriggerParticles(ParticleSystemTriggerEventType.Enter, m_particlesEntered);
    }
}
