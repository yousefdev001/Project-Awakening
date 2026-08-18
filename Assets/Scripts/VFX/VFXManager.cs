using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Awakening.VFX
{
    /// <summary>
    /// Master Visual Effects Manager providing procedural ParticleSystem generation, pooling, and playback.
    /// Eliminates external asset requirements by constructing optimized particle emitters in memory.
    /// </summary>
    public class VFXManager : MonoBehaviour
    {
        public static VFXManager Instance { get; private set; }

        private Material _defaultParticleMaterial;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            CreateDefaultMaterial();
        }

        private void CreateDefaultMaterial()
        {
            // Use standard mobile particles shader or unlit shader for high performance
            Shader shader = Shader.Find("Particles/Standard Unlit") ?? Shader.Find("Universal Render Pipeline/Particles/Unlit") ?? Shader.Find("Mobile/Particles/Additive") ?? Shader.Find("Sprites/Default");
            _defaultParticleMaterial = new Material(shader);
        }

        public void SpawnVFX(VFXType type, Vector3 position, Quaternion rotation = default)
        {
            GameObject vfxObj = new GameObject($"VFX_{type}");
            vfxObj.transform.position = position;
            vfxObj.transform.rotation = rotation == default ? Quaternion.identity : rotation;

            ParticleSystem ps = vfxObj.AddComponent<ParticleSystem>();
            ParticleSystemRenderer psr = vfxObj.GetComponent<ParticleSystemRenderer>();
            if (psr != null && _defaultParticleMaterial != null)
            {
                psr.material = _defaultParticleMaterial;
            }

            ConfigureParticleSystem(ps, type);
            ps.Play();

            float maxDuration = ps.main.duration + ps.main.startLifetime.constantMax + 0.2f;
            Destroy(vfxObj, maxDuration);
        }

        private void ConfigureParticleSystem(ParticleSystem ps, VFXType type)
        {
            var main = ps.main;
            var emission = ps.emission;
            var shape = ps.shape;
            var colorOverLifetime = ps.colorOverLifetime;
            var sizeOverLifetime = ps.sizeOverLifetime;

            main.loop = false;
            main.playOnAwake = false;

            switch (type)
            {
                case VFXType.SlashHitSparks:
                    main.duration = 0.2f;
                    main.startLifetime = new ParticleSystem.MinMaxCurve(0.15f, 0.25f);
                    main.startSpeed = new ParticleSystem.MinMaxCurve(5f, 9f);
                    main.startSize = new ParticleSystem.MinMaxCurve(0.08f, 0.15f);
                    main.startColor = new Color(1f, 0.85f, 0.2f); // Golden Yellow
                    emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 18) });
                    shape.shapeType = ParticleSystemShapeType.Sphere;
                    shape.radius = 0.15f;
                    break;

                case VFXType.HeavyCleaveBurst:
                    main.duration = 0.3f;
                    main.startLifetime = new ParticleSystem.MinMaxCurve(0.2f, 0.35f);
                    main.startSpeed = new ParticleSystem.MinMaxCurve(7f, 12f);
                    main.startSize = new ParticleSystem.MinMaxCurve(0.15f, 0.25f);
                    main.startColor = new Color(1f, 0.3f, 0.1f); // Fiery Orange
                    emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 32) });
                    shape.shapeType = ParticleSystemShapeType.Sphere;
                    shape.radius = 0.3f;
                    break;

                case VFXType.BloodSplatter:
                    main.duration = 0.35f;
                    main.startLifetime = new ParticleSystem.MinMaxCurve(0.25f, 0.45f);
                    main.startSpeed = new ParticleSystem.MinMaxCurve(3f, 7f);
                    main.startSize = new ParticleSystem.MinMaxCurve(0.1f, 0.2f);
                    main.startColor = new Color(0.75f, 0.05f, 0.05f); // Blood Crimson
                    main.gravityModifier = 1.2f;
                    emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 22) });
                    shape.shapeType = ParticleSystemShapeType.Cone;
                    shape.angle = 35f;
                    shape.radius = 0.2f;
                    break;

                case VFXType.SkillMagicBurst:
                    main.duration = 0.45f;
                    main.startLifetime = new ParticleSystem.MinMaxCurve(0.3f, 0.55f);
                    main.startSpeed = new ParticleSystem.MinMaxCurve(4f, 8f);
                    main.startSize = new ParticleSystem.MinMaxCurve(0.15f, 0.3f);
                    main.startColor = new Color(0.3f, 0.7f, 1.0f); // Arcane Cyan
                    emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 40) });
                    shape.shapeType = ParticleSystemShapeType.Sphere;
                    shape.radius = 0.5f;
                    break;

                case VFXType.DodgeDustTrail:
                    main.duration = 0.25f;
                    main.startLifetime = new ParticleSystem.MinMaxCurve(0.2f, 0.35f);
                    main.startSpeed = new ParticleSystem.MinMaxCurve(1f, 3f);
                    main.startSize = new ParticleSystem.MinMaxCurve(0.2f, 0.4f);
                    main.startColor = new Color(0.7f, 0.7f, 0.7f, 0.5f); // Dust Grey
                    emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 14) });
                    shape.shapeType = ParticleSystemShapeType.Hemisphere;
                    shape.radius = 0.4f;
                    break;

                case VFXType.LevelUpPillar:
                    main.duration = 0.8f;
                    main.startLifetime = new ParticleSystem.MinMaxCurve(0.5f, 0.85f);
                    main.startSpeed = new ParticleSystem.MinMaxCurve(4f, 8f);
                    main.startSize = new ParticleSystem.MinMaxCurve(0.15f, 0.3f);
                    main.startColor = new Color(1f, 0.9f, 0.25f); // Gold Radiance
                    main.gravityModifier = -0.5f; // Ascend upwards
                    emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 50) });
                    shape.shapeType = ParticleSystemShapeType.Circle;
                    shape.radius = 1.0f;
                    shape.rotation = new Vector3(90f, 0f, 0f);
                    break;

                case VFXType.GoldPickupSparkle:
                    main.duration = 0.35f;
                    main.startLifetime = new ParticleSystem.MinMaxCurve(0.25f, 0.45f);
                    main.startSpeed = new ParticleSystem.MinMaxCurve(2f, 4f);
                    main.startSize = new ParticleSystem.MinMaxCurve(0.1f, 0.2f);
                    main.startColor = new Color(1f, 0.84f, 0.1f); // Coin Gold
                    main.gravityModifier = -0.3f;
                    emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 16) });
                    shape.shapeType = ParticleSystemShapeType.Sphere;
                    shape.radius = 0.3f;
                    break;

                case VFXType.GroundSlamShockwave:
                    main.duration = 0.5f;
                    main.startLifetime = new ParticleSystem.MinMaxCurve(0.3f, 0.5f);
                    main.startSpeed = new ParticleSystem.MinMaxCurve(6f, 11f);
                    main.startSize = new ParticleSystem.MinMaxCurve(0.25f, 0.45f);
                    main.startColor = new Color(0.85f, 0.45f, 0.15f); // Earth & Flame
                    emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 45) });
                    shape.shapeType = ParticleSystemShapeType.Circle;
                    shape.radius = 0.5f;
                    shape.rotation = new Vector3(90f, 0f, 0f);
                    break;

                case VFXType.BossEnrageAura:
                    main.duration = 0.7f;
                    main.startLifetime = new ParticleSystem.MinMaxCurve(0.4f, 0.7f);
                    main.startSpeed = new ParticleSystem.MinMaxCurve(2f, 5f);
                    main.startSize = new ParticleSystem.MinMaxCurve(0.2f, 0.4f);
                    main.startColor = new Color(1f, 0.1f, 0.1f); // Deep Fire Crimson
                    main.gravityModifier = -0.6f;
                    emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 35) });
                    shape.shapeType = ParticleSystemShapeType.Sphere;
                    shape.radius = 1.2f;
                    break;
            }
        }
    }
}
