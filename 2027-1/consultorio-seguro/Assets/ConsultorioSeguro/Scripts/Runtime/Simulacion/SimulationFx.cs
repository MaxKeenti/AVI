using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace ConsultorioSeguroNuevo
{
    /// <summary>
    /// Partículas y sonidos de la simulación. Los sistemas se configuran por código para que la escena
    /// guardada no cambie; el material y la textura proceden de Resources/Simulacion.
    /// </summary>
    public static class SimulationFx
    {
        public const string ResourceFolder = "Simulacion/";
        static Material particleMaterial;
        static readonly Dictionary<string, AudioClip> clips = new Dictionary<string, AudioClip>();

        public static Material ParticleMaterial
        {
            get
            {
                if (particleMaterial) return particleMaterial;
                particleMaterial = Resources.Load<Material>(ResourceFolder + "Particula");
                if (particleMaterial) return particleMaterial;
                // Respaldo si no se generaron los recursos: partícula transparente sin textura.
                var shader = Shader.Find("Universal Render Pipeline/Particles/Unlit") ?? Shader.Find("Sprites/Default");
                particleMaterial = new Material(shader) { name = "Partícula de respaldo", hideFlags = HideFlags.DontSave };
                ConfigureTransparentParticle(particleMaterial);
                return particleMaterial;
            }
        }

        public static void ConfigureTransparentParticle(Material material)
        {
            material.SetFloat("_Surface", 1);
            material.SetFloat("_Blend", 0);
            material.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha);
            material.SetFloat("_DstBlend", (float)BlendMode.OneMinusSrcAlpha);
            material.SetFloat("_ZWrite", 0);
            material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            material.renderQueue = (int)RenderQueue.Transparent;
        }

        // --- Fábrica ---

        static ParticleSystem Create(string name, Transform parent, Vector3 position, Quaternion rotation,
            Action<ParticleSystem> configure)
        {
            // Se crea inactivo para configurar la duración antes de que el sistema empiece a emitir.
            var go = new GameObject(name);
            go.SetActive(false);
            go.transform.SetParent(parent, true);
            go.transform.SetPositionAndRotation(position, rotation);
            var system = go.AddComponent<ParticleSystem>();
            var main = system.main;
            main.playOnAwake = false;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.scalingMode = ParticleSystemScalingMode.Hierarchy;
            var renderer = go.GetComponent<ParticleSystemRenderer>();
            renderer.sharedMaterial = ParticleMaterial;
            renderer.shadowCastingMode = ShadowCastingMode.Off;
            renderer.receiveShadows = false;
            renderer.sortingFudge = -2;
            configure(system);
            go.SetActive(true);
            return system;
        }

        static ParticleSystem Burst(string name, Vector3 position, Quaternion rotation, Action<ParticleSystem> configure)
        {
            var system = Create(name, null, position, rotation, ps =>
            {
                var main = ps.main;
                main.loop = false;
                main.stopAction = ParticleSystemStopAction.Destroy;
                var emission = ps.emission;
                emission.rateOverTime = 0;
                configure(ps);
            });
            system.Play();
            return system;
        }

        static AnimationCurve Rise(float start, float end) => AnimationCurve.EaseInOut(0, start, 1, end);
        static Gradient Fade(Color color, float startAlpha, float peak = .15f)
        {
            var gradient = new Gradient();
            gradient.SetKeys(new[] { new GradientColorKey(color, 0), new GradientColorKey(color, 1) },
                new[] { new GradientAlphaKey(startAlpha, 0), new GradientAlphaKey(color.a, peak), new GradientAlphaKey(0, 1) });
            return gradient;
        }

        // --- Emisores continuos de los equipos ---

        /// <summary>Vapor que asciende del autoclave durante el ciclo simulado.</summary>
        public static ParticleSystem Steam(Transform parent, Vector3 position)
        {
            return Create("Partículas · Vapor del autoclave", parent, position, Quaternion.Euler(-90, 0, 0), ps =>
            {
                var main = ps.main;
                main.loop = true;
                main.duration = 2;
                main.startLifetime = new ParticleSystem.MinMaxCurve(1.6f, 2.6f);
                main.startSpeed = new ParticleSystem.MinMaxCurve(.25f, .55f);
                main.startSize = new ParticleSystem.MinMaxCurve(.05f, .11f);
                main.startRotation = new ParticleSystem.MinMaxCurve(0, Mathf.PI * 2);
                main.startColor = new Color(.96f, .97f, .98f, .42f);
                main.gravityModifier = -.04f;
                main.maxParticles = 160;
                var emission = ps.emission;
                emission.rateOverTime = 28;
                var shape = ps.shape;
                shape.shapeType = ParticleSystemShapeType.Cone;
                shape.angle = 14;
                shape.radius = .04f;
                var size = ps.sizeOverLifetime;
                size.enabled = true;
                size.size = new ParticleSystem.MinMaxCurve(1, Rise(1, 5.5f));
                var color = ps.colorOverLifetime;
                color.enabled = true;
                color.color = Fade(new Color(1, 1, 1, .45f), 0, .12f);
                var noise = ps.noise;
                noise.enabled = true;
                noise.strength = .18f;
                noise.frequency = .6f;
                var rotation = ps.rotationOverLifetime;
                rotation.enabled = true;
                rotation.z = new ParticleSystem.MinMaxCurve(-.6f, .6f);
            });
        }

        /// <summary>Chorro de agua que cae del grifo y salpica al tocar la tarja.</summary>
        public static ParticleSystem Water(Transform parent, Vector3 position)
        {
            return Create("Partículas · Agua del grifo", parent, position, Quaternion.Euler(90, 0, 0), ps =>
            {
                var main = ps.main;
                main.loop = true;
                main.duration = 1;
                main.startLifetime = new ParticleSystem.MinMaxCurve(.45f, .6f);
                main.startSpeed = new ParticleSystem.MinMaxCurve(.2f, .35f);
                main.startSize = new ParticleSystem.MinMaxCurve(.012f, .022f);
                main.startColor = new Color(.78f, .9f, 1f, .75f);
                main.gravityModifier = 1;
                main.maxParticles = 240;
                var emission = ps.emission;
                emission.rateOverTime = 150;
                var shape = ps.shape;
                shape.shapeType = ParticleSystemShapeType.Cone;
                shape.angle = 2;
                shape.radius = .006f;
                var collision = ps.collision;
                collision.enabled = true;
                collision.type = ParticleSystemCollisionType.World;
                collision.mode = ParticleSystemCollisionMode.Collision3D;
                collision.quality = ParticleSystemCollisionQuality.Medium;
                collision.bounce = .25f;
                collision.dampen = .6f;
                collision.lifetimeLoss = .55f;
                collision.collidesWith = Physics.DefaultRaycastLayers;
                var color = ps.colorOverLifetime;
                color.enabled = true;
                color.color = Fade(new Color(.8f, .92f, 1f, .7f), .7f, .5f);
            });
        }

        /// <summary>Nube de vapor que sale por la puerta al abrir el autoclave.</summary>
        public static ParticleSystem SteamBurst(Vector3 position, Vector3 outward) =>
            Burst("Partículas · Vapor al abrir", position, Quaternion.LookRotation(outward.normalized + Vector3.up * .35f), ps =>
            {
                var main = ps.main;
                main.duration = 1.5f;
                main.startLifetime = new ParticleSystem.MinMaxCurve(1.4f, 2.4f);
                main.startSpeed = new ParticleSystem.MinMaxCurve(.35f, .8f);
                main.startSize = new ParticleSystem.MinMaxCurve(.08f, .16f);
                main.startRotation = new ParticleSystem.MinMaxCurve(0, Mathf.PI * 2);
                main.gravityModifier = -.06f;
                ps.emission.SetBursts(new[] { new ParticleSystem.Burst(0, 40), new ParticleSystem.Burst(.25f, 25) });
                var shape = ps.shape;
                shape.shapeType = ParticleSystemShapeType.Cone;
                shape.angle = 30;
                shape.radius = .14f;
                var size = ps.sizeOverLifetime;
                size.enabled = true;
                size.size = new ParticleSystem.MinMaxCurve(1, Rise(1, 4.5f));
                var color = ps.colorOverLifetime;
                color.enabled = true;
                color.color = Fade(new Color(1, 1, 1, .5f), 0, .1f);
                var noise = ps.noise;
                noise.enabled = true;
                noise.strength = .2f;
                noise.frequency = .5f;
                var drag = ps.limitVelocityOverLifetime;
                drag.enabled = true;
                drag.drag = 1.2f;
            });

        // --- Ráfagas de retroalimentación ---

        /// <summary>Destellos verdes y blancos al acertar una clasificación.</summary>
        public static ParticleSystem Success(Vector3 position) => Burst("Partículas · Acierto", position, Quaternion.Euler(-90, 0, 0), ps =>
        {
            var main = ps.main;
            main.duration = 1;
            main.startLifetime = new ParticleSystem.MinMaxCurve(.55f, .9f);
            main.startSpeed = new ParticleSystem.MinMaxCurve(.7f, 1.6f);
            main.startSize = new ParticleSystem.MinMaxCurve(.018f, .04f);
            var colors = new Gradient();
            colors.SetKeys(new[] { new GradientColorKey(new Color(.42f, .86f, .52f), 0), new GradientColorKey(new Color(.95f, 1f, .88f), 1) },
                new[] { new GradientAlphaKey(1, 0), new GradientAlphaKey(1, 1) });
            main.startColor = new ParticleSystem.MinMaxGradient(colors) { mode = ParticleSystemGradientMode.RandomColor };
            main.gravityModifier = .35f;
            ps.emission.SetBursts(new[] { new ParticleSystem.Burst(0, 34) });
            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.angle = 38;
            shape.radius = .12f;
            var color = ps.colorOverLifetime;
            color.enabled = true;
            color.color = Fade(Color.white, 1, .6f);
        });

        /// <summary>Nube roja breve cuando el recipiente rechaza una clasificación incorrecta.</summary>
        public static ParticleSystem Rejection(Vector3 position) => Burst("Partículas · Clasificación revisada", position, Quaternion.identity, ps =>
        {
            var main = ps.main;
            main.duration = 1;
            main.startLifetime = new ParticleSystem.MinMaxCurve(.45f, .7f);
            main.startSpeed = new ParticleSystem.MinMaxCurve(.25f, .7f);
            main.startSize = new ParticleSystem.MinMaxCurve(.05f, .1f);
            main.startColor = new Color(.86f, .26f, .2f, .55f);
            ps.emission.SetBursts(new[] { new ParticleSystem.Burst(0, 20) });
            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = .1f;
            var size = ps.sizeOverLifetime;
            size.enabled = true;
            size.size = new ParticleSystem.MinMaxCurve(1, Rise(1, 2.4f));
            var color = ps.colorOverLifetime;
            color.enabled = true;
            color.color = Fade(new Color(.86f, .26f, .2f, .55f), .55f, .2f);
        });

        /// <summary>Polvo levantado cuando un material suelto golpea una superficie.</summary>
        public static ParticleSystem Dust(Vector3 position, Vector3 normal, float strength)
        {
            Quaternion rotation = normal.sqrMagnitude > .01f ? Quaternion.LookRotation(normal) : Quaternion.Euler(-90, 0, 0);
            return Burst("Partículas · Impacto", position, rotation, ps =>
            {
                var main = ps.main;
                main.duration = .5f;
                main.startLifetime = new ParticleSystem.MinMaxCurve(.35f, .6f);
                main.startSpeed = new ParticleSystem.MinMaxCurve(.15f, .35f + strength * .2f);
                main.startSize = new ParticleSystem.MinMaxCurve(.025f, .06f);
                main.startColor = new Color(.82f, .79f, .72f, .5f);
                main.gravityModifier = .1f;
                ps.emission.SetBursts(new[] { new ParticleSystem.Burst(0, (short)Mathf.Clamp(6 + strength * 4, 6, 18)) });
                var shape = ps.shape;
                shape.shapeType = ParticleSystemShapeType.Hemisphere;
                shape.radius = .03f;
                var size = ps.sizeOverLifetime;
                size.enabled = true;
                size.size = new ParticleSystem.MinMaxCurve(1, Rise(.8f, 2f));
                var color = ps.colorOverLifetime;
                color.enabled = true;
                color.color = Fade(new Color(.82f, .79f, .72f, .5f), .5f, .2f);
            });
        }

        /// <summary>Destello suave en el panel cuando se completa un paso.</summary>
        public static ParticleSystem StepDone(Vector3 position, Quaternion facing) => Burst("Partículas · Paso completado", position, facing, ps =>
        {
            var main = ps.main;
            main.duration = .8f;
            main.startLifetime = new ParticleSystem.MinMaxCurve(.4f, .7f);
            main.startSpeed = new ParticleSystem.MinMaxCurve(.12f, .35f);
            main.startSize = new ParticleSystem.MinMaxCurve(.015f, .03f);
            main.startColor = new Color(.55f, .9f, .62f, .9f);
            ps.emission.SetBursts(new[] { new ParticleSystem.Burst(0, 16) });
            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Box;
            shape.scale = new Vector3(.5f, .22f, .02f);
            var color = ps.colorOverLifetime;
            color.enabled = true;
            color.color = Fade(new Color(.55f, .9f, .62f, .9f), .9f, .5f);
        });

        /// <summary>Confeti al completar una práctica.</summary>
        public static ParticleSystem Confetti(Vector3 position) => Burst("Partículas · Práctica completada", position, Quaternion.Euler(-90, 0, 0), ps =>
        {
            var main = ps.main;
            main.duration = 2.5f;
            main.startLifetime = new ParticleSystem.MinMaxCurve(1.8f, 2.6f);
            main.startSpeed = new ParticleSystem.MinMaxCurve(2.2f, 3.6f);
            main.startSize = new ParticleSystem.MinMaxCurve(.025f, .05f);
            main.startRotation = new ParticleSystem.MinMaxCurve(0, Mathf.PI * 2);
            var palette = new Gradient();
            palette.SetKeys(new[]
            {
                new GradientColorKey(new Color(.36f, .62f, .5f), 0), new GradientColorKey(new Color(.98f, .76f, .3f), .35f),
                new GradientColorKey(new Color(.95f, .95f, .9f), .65f), new GradientColorKey(new Color(.42f, .66f, .88f), 1)
            }, new[] { new GradientAlphaKey(1, 0), new GradientAlphaKey(1, 1) });
            main.startColor = new ParticleSystem.MinMaxGradient(palette) { mode = ParticleSystemGradientMode.RandomColor };
            main.gravityModifier = .8f;
            main.maxParticles = 200;
            ps.emission.SetBursts(new[] { new ParticleSystem.Burst(0, 90), new ParticleSystem.Burst(.15f, 40) });
            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.angle = 32;
            shape.radius = .2f;
            var rotation = ps.rotationOverLifetime;
            rotation.enabled = true;
            rotation.z = new ParticleSystem.MinMaxCurve(-6, 6);
            var drag = ps.limitVelocityOverLifetime;
            drag.enabled = true;
            drag.drag = 1.4f;
            var color = ps.colorOverLifetime;
            color.enabled = true;
            color.color = Fade(Color.white, 1, .8f);
        });

        // --- Sonido procedural: evita distribuir audio de terceros ---

        public static AudioClip Clip(string name)
        {
            if (clips.TryGetValue(name, out var clip) && clip) return clip;
            const int rate = 22050;
            float length = name switch { "Agua" => 1f, "Vapor" => 1f, "Acierto" => .42f, "Error" => .3f, "Tapa" => .12f, _ => .16f };
            int samples = Mathf.CeilToInt(rate * length);
            var data = new float[samples];
            var random = new System.Random(name.GetHashCode());
            float filtered = 0;
            for (int i = 0; i < samples; i++)
            {
                float t = i / (float)rate;
                float noise = (float)random.NextDouble() * 2 - 1;
                data[i] = name switch
                {
                    // Golpe sordo: seno grave con caída rápida y algo de ruido.
                    "Golpe" => (Mathf.Sin(2 * Mathf.PI * 95 * t) * .8f + noise * .25f) * Mathf.Exp(-t * 32),
                    // Tapa: chasquido corto de plástico.
                    "Tapa" => (Mathf.Sin(2 * Mathf.PI * 420 * t) * .5f + noise * .5f) * Mathf.Exp(-t * 55),
                    "Acierto" => Mathf.Sin(2 * Mathf.PI * (t < .14f ? 660 : 880) * t) * .45f * Mathf.Exp(-(t < .14f ? t : t - .14f) * 9),
                    "Error" => Mathf.Sign(Mathf.Sin(2 * Mathf.PI * 140 * t)) * .22f * Mathf.Exp(-t * 7),
                    // Agua y vapor: ruido filtrado en bucle; el primero más grave y continuo.
                    "Agua" => (filtered = Mathf.Lerp(filtered, noise, .35f)) * .35f,
                    "Vapor" => (filtered = Mathf.Lerp(filtered, noise, .8f)) * .18f,
                    _ => noise * Mathf.Exp(-t * 40) * .4f
                };
            }
            if (name == "Agua" || name == "Vapor")
            {
                // Funde los extremos para que el bucle no chasquee.
                int fade = rate / 20;
                for (int i = 0; i < fade; i++)
                {
                    float w = i / (float)fade;
                    data[i] *= w;
                    data[samples - 1 - i] *= w;
                }
            }
            clip = AudioClip.Create("Sonido · " + name, samples, 1, rate, false);
            clip.SetData(data, 0);
            clips[name] = clip;
            return clip;
        }

        public static void Play(string name, Vector3 position, float volume = .6f)
        {
            if (volume <= .01f) return;
            var go = new GameObject("Sonido · " + name);
            go.transform.position = position;
            var source = go.AddComponent<AudioSource>();
            source.clip = Clip(name);
            source.volume = Mathf.Clamp01(volume);
            source.spatialBlend = 1;
            source.minDistance = .6f;
            source.maxDistance = 14;
            source.pitch = UnityEngine.Random.Range(.94f, 1.06f);
            source.Play();
            UnityEngine.Object.Destroy(go, source.clip.length + .1f);
        }

        public static AudioSource Loop(string name, Transform parent, float volume)
        {
            var source = parent.gameObject.AddComponent<AudioSource>();
            source.clip = Clip(name);
            source.loop = true;
            source.playOnAwake = false;
            source.volume = volume;
            source.spatialBlend = 1;
            source.minDistance = .5f;
            source.maxDistance = 10;
            return source;
        }
    }
}
