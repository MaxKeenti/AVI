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
