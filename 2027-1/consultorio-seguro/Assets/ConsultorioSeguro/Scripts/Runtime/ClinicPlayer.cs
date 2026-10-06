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
        CharacterController controller;
        float pitch, verticalVelocity;
        public Transform HoldPoint { get; private set; }

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
            HoldPoint = new GameObject("Objeto en mano").transform;
            HoldPoint.SetParent(eye.transform, false);
            HoldPoint.localPosition = new Vector3(.23f, -.20f, .55f);
        }

        void Update()
        {
            if (!game || !game.Playing) return;
            var keyboard = Keyboard.current;
            if (Mouse.current != null && Cursor.lockState == CursorLockMode.Locked)
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
            float speed = walkSpeed * (keyboard != null && keyboard.leftShiftKey.isPressed ? 1.45f : 1);
            verticalVelocity = controller.isGrounded ? -1.5f : verticalVelocity + Physics.gravity.y * Time.deltaTime;
            controller.Move(((transform.right * input.x + transform.forward * input.y) * speed
                + Vector3.up * verticalVelocity) * Time.deltaTime);
        }

        public void Teleport(Vector3 position, float yaw = 0)
        {
            controller.enabled = false;
            transform.SetPositionAndRotation(position, Quaternion.Euler(0, yaw, 0));
            controller.enabled = true;
            verticalVelocity = pitch = 0;
            eye.transform.localRotation = Quaternion.identity;
        }
    }
}
