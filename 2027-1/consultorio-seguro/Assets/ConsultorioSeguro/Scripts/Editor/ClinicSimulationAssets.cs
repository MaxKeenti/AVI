using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace ConsultorioSeguroNuevo.Editor
{
    /// <summary>
    /// Genera los recursos de las acciones de simulación: clips (.anim) y controladores (.controller)
    /// editables en las ventanas Animation y Animator, más los materiales de partículas e indicadores.
    /// Regenerar conserva los GUID de los recursos existentes.
    /// </summary>
    public static class ClinicSimulationAssets
    {
        public const string ResourcesFolder = "Assets/ConsultorioSeguro/Resources/Simulacion";
        public const string ClipsFolder = "Assets/ConsultorioSeguro/Animaciones";
        const string Axis = AnimatedPart.AxisName;

        sealed class Track
        {
            public string Path, Property;
            public Type Type;
            public Keyframe[] Keys;
        }

        [MenuItem("Consultorio Seguro/Generar animaciones de simulación")]
        public static void Generate()
        {
            Directory.CreateDirectory(ResourcesFolder);
            Directory.CreateDirectory(ClipsFolder);
            AssetDatabase.Refresh();
            BuildLid();
            BuildArm();
            BuildChair();
            BuildLamp();
            BuildPanel();
            BuildMaterials();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("CONSULTORIO_SIMULACION · Animaciones y materiales de simulación generados en " + ResourcesFolder);
        }

        // --- Controladores ---

        static void BuildLid()
        {
            // Rotación positiva en X levanta el frente de la tapa sobre la bisagra posterior.
            var closed = Clip("Tapa_Cerrada", Rot("x", (0, 0)));
            var open = Clip("Tapa_Abrir", Rot("x", (0, 0), (.2f, 84), (.3f, 75), (.36f, 78)));
            var close = Clip("Tapa_Cerrar", Rot("x", (0, 78), (.22f, 0), (.27f, 5), (.32f, 0)));
            Toggle("Tapa", "Abierta", ("Cerrada", closed), ("Abriendo", open), ("Cerrando", close));
        }

        static void BuildChair()
        {
            // Reposo: sillón bajo con el respaldo erguido. Trabajo: elevado y reclinado como en la escena guardada.
            string back = Axis + "/" + ClinicSimulation.BackrestJoint + "/" + Axis;
            const float upright = 25;
            var low = Clip("Sillon_Bajo", Pos("y", (0, 0)), Curve(back, typeof(Transform), "localEulerAnglesRaw.x", (0, upright)));
            var rise = Clip("Sillon_Elevar", Pos("y", (0, 0), (1.4f, .14f)),
                Curve(back, typeof(Transform), "localEulerAnglesRaw.x", (0, upright), (.2f, upright), (1.8f, 0)));
            var lower = Clip("Sillon_Bajar", Pos("y", (0, .14f), (1.4f, 0)),
                Curve(back, typeof(Transform), "localEulerAnglesRaw.x", (0, 0), (1.8f, upright)));
            Toggle("Sillon", "Elevado", ("Bajo", low), ("Elevando", rise), ("Bajando", lower));
        }

        static void BuildLamp()
        {
            string light = Axis + "/Luz";
            float swing = ClinicSimulation.LampSwing;
            var off = Clip("Lampara_Apagada", Rot("y", (0, 0)), Curve(light, typeof(Light), "m_Intensity", (0, 0)));
            // El brazo se coloca sobre el paciente y la luz parpadea brevemente antes de la intensidad de trabajo.
            var on = Clip("Lampara_Encender", Rot("y", (0, 0), (1.2f, swing)),
                Curve(light, typeof(Light), "m_Intensity", (0, 0), (.9f, 0), (.96f, 1.4f), (1.02f, .2f), (1.1f, 2f), (1.4f, 2.8f)));
            var fade = Clip("Lampara_Apagar", Rot("y", (0, swing), (.3f, swing), (1.5f, 0)),
                Curve(light, typeof(Light), "m_Intensity", (0, 2.8f), (.3f, 0)));
            Toggle("Lampara", "Encendida", ("Apagada", off), ("Encendiendo", on), ("Apagando", fade));
        }

        static void BuildArm()
        {
            float swing = ClinicSimulation.ArmSwing;
            var rest = Clip("Brazo_Reposo", Rot("y", (0, 0)));
            var shot = Clip("Brazo_Toma", Rot("y", (0, 0), (.7f, swing), (1.6f, swing), (2.2f, 0)));
            OneShot("Brazo", "Toma", ("Reposo", rest), ("Toma", shot));
        }

        static void BuildPanel()
        {
            var rest = Clip("Panel_Reposo", Rot("y", (0, 0)), Pos("z", (0, 0)));
            // Se hunde al pulsarlo y gira una vuelta completa para confirmar el paso.
            var done = Clip("Panel_Completar", Rot("y", (0, 0), (.15f, 0), (.85f, 360)),
                Pos("z", (0, 0), (.08f, .025f), (.18f, 0)));
            OneShot("Panel", "Completar", ("Reposo", rest), ("Completar", done));
        }

        static AnimatorController Controller(string name)
        {
            string path = ResourcesFolder + "/" + name + ".controller";
            var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(path);
            if (!controller) controller = AnimatorController.CreateAnimatorControllerAtPath(path);
            foreach (var parameter in controller.parameters.ToArray()) controller.RemoveParameter(parameter);
            var machine = controller.layers[0].stateMachine;
            foreach (var state in machine.states.ToArray()) machine.RemoveState(state.state);
            foreach (var transition in machine.anyStateTransitions.ToArray()) machine.RemoveAnyStateTransition(transition);
            return controller;
        }

        /// <summary>Estado de reposo, entrada y salida controlados por un parámetro booleano.</summary>
        static void Toggle(string name, string parameter, (string name, AnimationClip clip) rest,
            (string name, AnimationClip clip) entering, (string name, AnimationClip clip) leaving)
        {
            var controller = Controller(name);
            controller.AddParameter(parameter, AnimatorControllerParameterType.Bool);
            var machine = controller.layers[0].stateMachine;
            var idle = State(machine, rest, new Vector3(260, 0));
            var into = State(machine, entering, new Vector3(520, -80));
            var outof = State(machine, leaving, new Vector3(520, 80));
            machine.defaultState = idle;
            Link(idle, into, AnimatorConditionMode.If, parameter);
            Link(outof, into, AnimatorConditionMode.If, parameter);
            Link(into, outof, AnimatorConditionMode.IfNot, parameter);
            EditorUtility.SetDirty(controller);
        }

        /// <summary>Animación de ida y vuelta disparada por un trigger; regresa sola al reposo.</summary>
        static void OneShot(string name, string trigger, (string name, AnimationClip clip) rest, (string name, AnimationClip clip) action)
        {
            var controller = Controller(name);
            controller.AddParameter(trigger, AnimatorControllerParameterType.Trigger);
            var machine = controller.layers[0].stateMachine;
            var idle = State(machine, rest, new Vector3(260, 0));
            var play = State(machine, action, new Vector3(520, 0));
            machine.defaultState = idle;
            Link(idle, play, AnimatorConditionMode.If, trigger);
            var back = play.AddTransition(idle);
            back.hasExitTime = true;
            back.exitTime = 1;
            back.hasFixedDuration = true;
            back.duration = 0;
            EditorUtility.SetDirty(controller);
        }

        static AnimatorState State(AnimatorStateMachine machine, (string name, AnimationClip clip) entry, Vector3 position)
        {
            var state = machine.AddState(entry.name, position);
            state.motion = entry.clip;
            state.writeDefaultValues = true;
            return state;
        }

        static void Link(AnimatorState from, AnimatorState to, AnimatorConditionMode mode, string parameter)
        {
            var transition = from.AddTransition(to);
            transition.hasExitTime = false;
            transition.hasFixedDuration = true;
            transition.duration = .05f;
            transition.canTransitionToSelf = false;
            transition.AddCondition(mode, 0, parameter);
        }

        // --- Clips ---

        static Track Curve(string path, Type type, string property, params (float time, float value)[] keys) => new Track
        {
            Path = path, Type = type, Property = property,
            // Tangentes planas: cada tramo acelera y frena suavemente.
            Keys = keys.Select(k => new Keyframe(k.time, k.value, 0, 0)).ToArray()
        };
        static Track Rot(string axis, params (float, float)[] keys) => Curve(Axis, typeof(Transform), "localEulerAnglesRaw." + axis, keys);
        static Track Pos(string axis, params (float, float)[] keys) => Curve(Axis, typeof(Transform), "m_LocalPosition." + axis, keys);

        static AnimationClip Clip(string name, params Track[] tracks)
        {
            var all = new List<Track>(tracks);
            float length = Mathf.Max(1 / 60f, tracks.Max(track => track.Keys.Last().time));
            // Unity necesita los tres componentes de rotación y de posición para cada objeto animado.
            foreach (var group in new[] { "localEulerAnglesRaw.", "m_LocalPosition." })
            {
                foreach (var path in tracks.Where(t => t.Property.StartsWith(group)).Select(t => t.Path).Distinct().ToArray())
                    foreach (var axis in new[] { "x", "y", "z" })
                        if (!all.Any(t => t.Path == path && t.Property == group + axis))
                            all.Add(Curve(path, typeof(Transform), group + axis, (0, 0), (length, 0)));
            }
            string assetPath = ClipsFolder + "/" + name + ".anim";
            var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(assetPath);
            bool created = !clip;
            if (created) clip = new AnimationClip { name = name };
            clip.ClearCurves();
            clip.frameRate = 60;
            foreach (var track in all)
            {
                var keys = track.Keys.Length == 1 ? new[] { track.Keys[0], new Keyframe(length, track.Keys[0].value, 0, 0) } : track.Keys;
                AnimationUtility.SetEditorCurve(clip, EditorCurveBinding.FloatCurve(track.Path, track.Type, track.Property), new AnimationCurve(keys));
            }
            var settings = AnimationUtility.GetAnimationClipSettings(clip);
            settings.loopTime = false;
            AnimationUtility.SetAnimationClipSettings(clip, settings);
            if (created) AssetDatabase.CreateAsset(clip, assetPath);
            else EditorUtility.SetDirty(clip);
            return clip;
        }

        // --- Materiales ---

        static void BuildMaterials()
        {
            string texturePath = ResourcesFolder + "/Particula.png";
            if (!File.Exists(texturePath))
            {
                const int size = 64;
                var texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
                for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                {
                    float d = Vector2.Distance(new Vector2(x + .5f, y + .5f), new Vector2(size / 2f, size / 2f)) / (size / 2f);
                    float alpha = Mathf.Clamp01(1 - d);
                    texture.SetPixel(x, y, new Color(1, 1, 1, alpha * alpha * (3 - 2 * alpha)));
                }
                File.WriteAllBytes(texturePath, texture.EncodeToPNG());
                UnityEngine.Object.DestroyImmediate(texture);
                AssetDatabase.ImportAsset(texturePath);
            }
            var importer = (TextureImporter)AssetImporter.GetAtPath(texturePath);
            if (importer && (!importer.alphaIsTransparency || importer.wrapMode != TextureWrapMode.Clamp || importer.mipmapEnabled))
            {
                importer.alphaIsTransparency = true;
                importer.wrapMode = TextureWrapMode.Clamp;
                importer.mipmapEnabled = false;
                importer.SaveAndReimport();
            }
            var particle = MaterialAsset("Particula", "Universal Render Pipeline/Particles/Unlit");
            particle.SetTexture("_BaseMap", AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath));
            particle.SetColor("_BaseColor", Color.white);
            SimulationFx.ConfigureTransparentParticle(particle);
            EditorUtility.SetDirty(particle);
            var indicator = MaterialAsset("Indicador", "Universal Render Pipeline/Unlit");
            indicator.SetColor("_BaseColor", new Color(.55f, .57f, .56f));
            EditorUtility.SetDirty(indicator);
        }

        static Material MaterialAsset(string name, string shaderName)
        {
            string path = ResourcesFolder + "/" + name + ".mat";
            var material = AssetDatabase.LoadAssetAtPath<Material>(path);
            var shader = Shader.Find(shaderName);
            if (!shader) throw new Exception("No se encontró el shader " + shaderName);
            if (!material)
            {
                material = new Material(shader) { name = name };
                AssetDatabase.CreateAsset(material, path);
            }
            else material.shader = shader;
            return material;
        }
    }
}
