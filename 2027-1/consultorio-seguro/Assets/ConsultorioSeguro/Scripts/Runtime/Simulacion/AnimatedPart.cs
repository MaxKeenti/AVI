using System.Linq;
using UnityEngine;

namespace ConsultorioSeguroNuevo
{
    /// <summary>
    /// Inserta un pivote con Animator sobre piezas existentes de la escena. El Animator anima el hijo «Eje»
    /// en coordenadas locales, de modo que un mismo clip sirve para cualquier tapa, panel o equipo.
    /// Los controladores se generan con Consultorio Seguro → Generar animaciones de simulación.
    /// </summary>
    public sealed class AnimatedPart : MonoBehaviour
    {
        public const string AxisName = "Eje";
        public Animator Animator { get; private set; }
        public Transform Axis { get; private set; }
        public string ControllerName { get; private set; }

        public static RuntimeAnimatorController LoadController(string controller) =>
            Resources.Load<RuntimeAnimatorController>(SimulationFx.ResourceFolder + controller);

        /// <summary>Envuelve <paramref name="parts"/> con un pivote en la posición y orientación de la bisagra.</summary>
        public static AnimatedPart Wrap(string label, string controller, Vector3 hinge, Quaternion orientation, params Transform[] parts)
        {
            parts = parts.Where(part => part).ToArray();
            if (parts.Length == 0) return null;
            var wrapper = new GameObject("Animación · " + label).transform;
            wrapper.SetParent(parts[0].parent, false);
            wrapper.SetPositionAndRotation(hinge, orientation);
            var axis = new GameObject(AxisName).transform;
            axis.SetParent(wrapper, false);
            foreach (var part in parts) part.SetParent(axis, true);
            var animated = wrapper.gameObject.AddComponent<AnimatedPart>();
            animated.Axis = axis;
            animated.Attach(controller);
            return animated;
        }

        /// <summary>Crea un objeto animado vacío (por ejemplo, una luz) bajo <paramref name="parent"/>.</summary>
        public static AnimatedPart Create(string label, string controller, Transform parent, Vector3 position, Quaternion orientation)
        {
            var wrapper = new GameObject("Animación · " + label).transform;
            wrapper.SetParent(parent, false);
            wrapper.SetPositionAndRotation(position, orientation);
            var axis = new GameObject(AxisName).transform;
            axis.SetParent(wrapper, false);
            var animated = wrapper.gameObject.AddComponent<AnimatedPart>();
            animated.Axis = axis;
            animated.Attach(controller);
            return animated;
        }

        /// <summary>
        /// Articulación anidada sin Animator propio: crea «<paramref name="name"/>/Eje» bajo <paramref name="parent"/>
        /// para que el Animator de un antecesor la anime con la ruta «…/<paramref name="name"/>/Eje».
        /// </summary>
        public static Transform Nest(Transform parent, string name, Vector3 hinge, Quaternion orientation, params Transform[] parts)
        {
            var joint = new GameObject(name).transform;
            joint.SetParent(parent, false);
            joint.SetPositionAndRotation(hinge, orientation);
            var axis = new GameObject(AxisName).transform;
            axis.SetParent(joint, false);
            foreach (var part in parts.Where(part => part)) part.SetParent(axis, true);
            return axis;
        }

        /// <summary>
        /// Envuelve un brazo articulado que gira sobre la vertical de <paramref name="pivot"/>. Lo estaciona de modo
        /// que el giro positivo del clip (<paramref name="clipDegrees"/>) lleve <paramref name="tip"/> hacia
        /// <paramref name="target"/>. Devuelve el giro en el mundo al final del clip (positivo o negativo).
        /// </summary>
        public static AnimatedPart SwingToward(string label, string controller, Transform pivot, Vector3 tip, Vector3 target,
            float clipDegrees, out float worldDegrees) =>
            SwingToward(label, controller, pivot.position, new[] { pivot }, tip, target, clipDegrees, out worldDegrees);

        /// <summary>Igual que la sobrecarga anterior, para varias piezas que giran sobre una bisagra vertical en <paramref name="origin"/>.</summary>
        public static AnimatedPart SwingToward(string label, string controller, Vector3 origin, Transform[] parts, Vector3 tip,
            Vector3 target, float clipDegrees, out float worldDegrees)
        {
            Vector3 reach = tip - origin, wanted = target - origin;
            reach.y = 0; wanted.y = 0;
            if (reach.sqrMagnitude < 1e-6f) reach = Vector3.forward;
            float angle = wanted.sqrMagnitude < 1e-6f ? clipDegrees : Vector3.SignedAngle(reach, wanted, Vector3.up);
            float sign = angle >= 0 ? 1 : -1;
            // Con el pivote invertido, el giro local positivo equivale a un giro negativo en el mundo.
            Quaternion orientation = Quaternion.LookRotation(reach.normalized, Vector3.up);
            if (sign < 0) orientation *= Quaternion.Euler(0, 0, 180);
            var part = Wrap(label, controller, origin, orientation, parts);
            part.transform.RotateAround(origin, Vector3.up, angle - sign * clipDegrees);
            worldDegrees = sign * clipDegrees;
            return part;
        }

        void Attach(string controller)
        {
            ControllerName = controller;
            var asset = LoadController(controller);
            if (!asset)
            {
                Debug.LogWarning("Consultorio Seguro: falta el controlador de animación " + controller
                    + ". Ejecuta Consultorio Seguro → Generar animaciones de simulación.");
                return;
            }
            Animator = gameObject.AddComponent<Animator>();
            Animator.runtimeAnimatorController = asset;
            Animator.applyRootMotion = false;
            Animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
        }

        public void SetBool(string parameter, bool value)
        {
            if (Animator && Animator.isActiveAndEnabled && Animator.GetBool(parameter) != value) Animator.SetBool(parameter, value);
        }

        public void Trigger(string parameter)
        {
            if (Animator && Animator.isActiveAndEnabled) Animator.SetTrigger(parameter);
        }

        /// <summary>Vuelve al estado inicial del controlador y a la pose de reposo del eje.</summary>
        public void ResetPose()
        {
            if (Animator && Animator.isActiveAndEnabled)
            {
                Animator.Rebind();
                Animator.Update(0);
            }
            else if (Axis)
            {
                Axis.localPosition = Vector3.zero;
                Axis.localRotation = Quaternion.identity;
            }
        }
    }
}
