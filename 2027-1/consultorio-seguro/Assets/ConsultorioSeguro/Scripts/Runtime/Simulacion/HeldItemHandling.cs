using UnityEngine;
using UnityEngine.InputSystem;

namespace ConsultorioSeguroNuevo
{
    /// <summary>
    /// Manipulación del objeto en mano sobre el punto de sujeción: viaje desde la mesa hasta la mano,
    /// inercia al girar la cabeza y al caminar, e inspección con la rueda o arrastrando con clic derecho.
    /// El objeto permanece en la posición local cero del punto; solo se desplaza el punto.
    /// </summary>
    public sealed class HeldItemHandling : MonoBehaviour
    {
        [Tooltip("Rigidez del resorte que devuelve el objeto a la mano.")]
        public float stiffness = 140f;
        [Tooltip("Amortiguación del resorte; con 2·√rigidez el regreso no oscila.")]
        public float damping = 17f;
        public float swayPerDegree = .0035f;
        public float maxSway = .06f;
        public float inspectSensitivity = .25f;
        public float wheelDegrees = 18f;
        ClinicGame game;
        ClinicPlayer player;
        Vector3 restPosition, anchor;
        Vector3 inspectPosition = new Vector3(0, -.05f, .42f);
        Vector3 offset, offsetVelocity;
        Quaternion inspectRotation = Quaternion.identity;
        Quaternion lastEyeRotation;
        float tilt, tiltVelocity;
        public bool Inspecting { get; private set; }
        public Quaternion InspectRotation => inspectRotation;

        public void Initialize(ClinicGame owner, ClinicPlayer visitor)
        {
            game = owner;
            player = visitor;
            restPosition = anchor = transform.localPosition;
            lastEyeRotation = player.eye.transform.rotation;
            game.ItemPickedUp += OnPickedUp;
        }

        void OnDestroy()
        {
            if (game) game.ItemPickedUp -= OnPickedUp;
        }

        void OnPickedUp(ClinicItem item, Vector3 fromPosition, Quaternion fromRotation)
        {
            // El objeto parte de donde estaba y el resorte lo trae hasta la mano.
            Vector3 local = transform.parent.InverseTransformPoint(fromPosition);
            anchor = restPosition;
            offset = Vector3.ClampMagnitude(local - restPosition, 1.6f);
            offsetVelocity = Vector3.zero;
            inspectRotation = Quaternion.identity;
            transform.localPosition = anchor + offset;
        }

        /// <summary>Gira el objeto inspeccionado; también se usa en pruebas sin ratón.</summary>
        public void Rotate(float yawDegrees, float pitchDegrees)
        {
            inspectRotation = Quaternion.AngleAxis(-yawDegrees, Vector3.up) * Quaternion.AngleAxis(pitchDegrees, Vector3.right) * inspectRotation;
        }

        void LateUpdate()
        {
            if (!game || !player) return;
            bool holding = game.HeldItem && game.Playing;
            var mouse = Mouse.current;
            Inspecting = holding && mouse != null && mouse.rightButton.isPressed;
            player.LookSuspended = Inspecting;
            if (holding && mouse != null)
            {
                float wheel = mouse.scroll.ReadValue().y;
                if (Mathf.Abs(wheel) > .01f) Rotate(Mathf.Sign(wheel) * wheelDegrees, 0);
                if (Inspecting)
                {
                    Vector2 drag = mouse.delta.ReadValue() * inspectSensitivity;
                    Rotate(drag.x, drag.y);
                }
            }
            if (!holding) inspectRotation = Quaternion.Slerp(inspectRotation, Quaternion.identity, 1 - Mathf.Exp(-12 * Time.unscaledDeltaTime));

            float dt = Time.deltaTime;
            if (dt <= 0) return;
            // Inercia: el giro de la cabeza deja el objeto ligeramente atrás.
            Quaternion eye = player.eye.transform.rotation;
            Quaternion delta = Quaternion.Inverse(lastEyeRotation) * eye;
            lastEyeRotation = eye;
            delta.ToAngleAxis(out float angle, out Vector3 axis);
            if (angle > 180) angle -= 360;
            Vector3 angular = axis * angle;
            if (float.IsNaN(angular.x)) angular = Vector3.zero;
            Vector3 lag = new Vector3(-angular.y, angular.x, 0) * swayPerDegree;
            offset += Vector3.ClampMagnitude(lag, maxSway);
            // El paso agrega una pequeña oscilación lateral.
            float walk = player.PlanarVelocity.magnitude;
            float targetTilt = Mathf.Clamp(-Vector3.Dot(player.PlanarVelocity, player.transform.right) * 2.5f, -6, 6);
            tilt = Mathf.SmoothDamp(tilt, targetTilt, ref tiltVelocity, .12f);
            // Al inspeccionar, el objeto se acerca al centro de la vista.
            anchor = Vector3.Lerp(anchor, Inspecting ? inspectPosition : restPosition, 1 - Mathf.Exp(-14 * dt));
            Vector3 bob = new Vector3(0, Mathf.Sin(Time.time * 9) * .004f * Mathf.Clamp01(walk / 2), 0);
            // Resorte amortiguado semiimplícito, estable con pasos variables.
            offsetVelocity += (-stiffness * offset - damping * offsetVelocity) * dt;
            offset += offsetVelocity * dt;
            transform.localPosition = anchor + offset + bob;
            transform.localRotation = Quaternion.Euler(0, 0, tilt) * inspectRotation;
        }
    }
}
