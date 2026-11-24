using UnityEngine;
using UnityEngine.InputSystem;

namespace ConsultorioSeguroNuevo
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class ClinicPlayer : MonoBehaviour
    {
        public Camera eye;
        public float walkSpeed = 2.2f;
        public float mouseSensitivity = .095f;
        public ClinicGame game;
        [Header("Movimiento")]
        [Tooltip("Aceleración horizontal en m/s². Da inercia al arrancar y al detenerse.")]
        [Min(1)] public float acceleration = 10f;
        [Min(1)] public float deceleration = 14f;
        [Tooltip("Altura de los ojos al agacharse con C.")]
        [Min(.4f)] public float crouchEyeHeight = 1.08f;
        [Range(0, 1)] public float crouchSpeedFactor = .5f;
        [Tooltip("Amplitud vertical del balanceo de la cabeza al caminar.")]
        [Range(0, .06f)] public float headBob = .022f;
        [Tooltip("Velocidad máxima que la cápsula transmite al empujar materiales sueltos.")]
        [Min(0)] public float pushSpeed = 1.2f;
        /// <summary>Permite solicitar agacharse sin teclado (pruebas, controles táctiles).</summary>
        public bool crouchRequested;
        CharacterController controller;
        float pitch, verticalVelocity, eyeHeight, bobPhase, bobWeight;
        Vector3 eyeBase;
        public Transform HoldPoint { get; private set; }
        public Vector3 PlanarVelocity { get; private set; }
        public bool Crouching { get; private set; }
        public float StandingEyeHeight => eyeBase.y;
        // Mientras se inspecciona un objeto, el ratón lo gira en lugar de mover la mirada.
        public bool LookSuspended { get; set; }

        void Reset()
        {
            var capsule = GetComponent<CharacterController>();
            capsule.height = 1.8f;
            capsule.radius = .27f;
            capsule.center = new Vector3(0, .9f, 0);
            capsule.stepOffset = .2f;
            capsule.skinWidth = .025f;
        }

        void Awake()
        {
            controller = GetComponent<CharacterController>();
            // Conserva las colisiones físicas y evita apuntar a la propia cápsula al mirar hacia abajo.
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            if (!eye) eye = GetComponentInChildren<Camera>();
            if (!eye)
            {
                eye = new GameObject("Cámara del visitante", typeof(Camera), typeof(AudioListener)).GetComponent<Camera>();
                eye.transform.SetParent(transform, false);
                eye.transform.localPosition = Vector3.up * 1.65f;
                eye.tag = "MainCamera";
            }
            eyeBase = eye.transform.localPosition;
            eyeHeight = eyeBase.y;
            HoldPoint = new GameObject("Objeto en mano").transform;
            HoldPoint.SetParent(eye.transform, false);
            HoldPoint.localPosition = new Vector3(.23f, -.20f, .55f);
        }

        void Update()
        {
            if (!game || !game.Playing) return;
            var keyboard = Keyboard.current;
            if (Mouse.current != null && Cursor.lockState == CursorLockMode.Locked && !LookSuspended)
            {
                Vector2 look = Mouse.current.delta.ReadValue() * mouseSensitivity;
                transform.Rotate(0, look.x, 0);
                pitch = Mathf.Clamp(pitch - look.y, -80, 80);
                eye.transform.localRotation = Quaternion.Euler(pitch, 0, 0);
            }
            Vector2 input = Vector2.zero;
            if (keyboard != null)
            {
                input.x = (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed ? 1 : 0)
                    - (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed ? 1 : 0);
                input.y = (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed ? 1 : 0)
                    - (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed ? 1 : 0);
            }
            input = Vector2.ClampMagnitude(input, 1);
            Crouching = crouchRequested || (keyboard != null && keyboard.cKey.isPressed);
            float speed = walkSpeed * (keyboard != null && keyboard.leftShiftKey.isPressed && !Crouching ? 1.45f : 1)
                * (Crouching ? crouchSpeedFactor : 1);
            // Inercia: la velocidad se aproxima al objetivo en lugar de cambiar de golpe.
            Vector3 desired = (transform.right * input.x + transform.forward * input.y) * speed;
            float rate = desired.sqrMagnitude > PlanarVelocity.sqrMagnitude ? acceleration : deceleration;
            PlanarVelocity = Vector3.MoveTowards(PlanarVelocity, desired, rate * Time.deltaTime);
            verticalVelocity = controller.isGrounded ? -1.5f : verticalVelocity + Physics.gravity.y * Time.deltaTime;
            controller.Move((PlanarVelocity + Vector3.up * verticalVelocity) * Time.deltaTime);
            UpdateEye(input.sqrMagnitude > .01f && controller.isGrounded);
        }

        void UpdateEye(bool walking)
        {
            eyeHeight = Mathf.MoveTowards(eyeHeight, Crouching ? crouchEyeHeight : eyeBase.y, 2.6f * Time.deltaTime);
            float stride = PlanarVelocity.magnitude;
            // El balanceo solo responde a la marcha con teclado; las rutas automáticas mantienen la cámara estable.
            bobWeight = Mathf.MoveTowards(bobWeight, walking ? Mathf.Clamp01(stride / walkSpeed) : 0, 4 * Time.deltaTime);
            if (bobWeight > 0) bobPhase += Time.deltaTime * (4.2f + stride * 2.4f);
            else bobPhase = 0;
            float amplitude = headBob * bobWeight;
            eye.transform.localPosition = new Vector3(eyeBase.x + Mathf.Cos(bobPhase) * amplitude * .6f,
                eyeHeight + Mathf.Sin(bobPhase * 2) * amplitude, eyeBase.z);
        }

        // Empuja materiales sueltos al caminar contra ellos; los objetos colocados son cinemáticos y no se mueven.
        void OnControllerColliderHit(ControllerColliderHit hit)
        {
            var body = hit.collider.attachedRigidbody;
            if (!body || body.isKinematic || hit.moveDirection.y < -.3f) return;
            Vector3 push = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z) * Mathf.Max(PlanarVelocity.magnitude, .4f);
            push = Vector3.ClampMagnitude(push, pushSpeed);
            Vector3 current = body.linearVelocity;
            body.linearVelocity = new Vector3(push.x, current.y, push.z);
        }

        public void Teleport(Vector3 position, float yaw = 0)
        {
            controller.enabled = false;
            transform.SetPositionAndRotation(position, Quaternion.Euler(0, yaw, 0));
            controller.enabled = true;
            verticalVelocity = pitch = 0;
            PlanarVelocity = Vector3.zero;
            bobWeight = bobPhase = 0;
            eye.transform.localRotation = Quaternion.identity;
            eye.transform.localPosition = new Vector3(eyeBase.x, eyeHeight, eyeBase.z);
        }
    }
}
