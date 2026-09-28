using UnityEngine;
using UnityEngine.InputSystem;

namespace ConsultorioSeguro
{
    // Caminar y mirar en primera persona con las acciones Player/Move, Player/Look
    // y Player/Sprint del asset InputSystem_Actions.
    [RequireComponent(typeof(CharacterController))]
    public class ControladorPrimeraPersona : MonoBehaviour
    {
        [SerializeField] Transform camara;
        [SerializeField, Min(0)] float velocidad = 2.2f;
        [SerializeField, Min(0)] float velocidadCorrer = 4f;

        [Tooltip("Grados por píxel de movimiento del ratón.")]
        [SerializeField, Min(0)] float sensibilidadRaton = 0.1f;

        [Tooltip("Grados por segundo con el stick del control.")]
        [SerializeField, Min(0)] float sensibilidadStick = 120f;

        [SerializeField, Range(10, 89)] float limiteVertical = 80f;

        CharacterController controlador;
        InputAction mover;
        InputAction mirar;
        InputAction correr;
        float inclinacion;
        float velocidadVertical;

        public bool Habilitado { get; set; }

        void Awake()
        {
            controlador = GetComponent<CharacterController>();
            mover = InputSystem.actions.FindAction("Player/Move", true);
            mirar = InputSystem.actions.FindAction("Player/Look", true);
            correr = InputSystem.actions.FindAction("Player/Sprint", true);
        }

        void Update()
        {
            if (!Habilitado)
                return;

            Mirar();
            Caminar();
        }

        void Mirar()
        {
            Vector2 delta = mirar.ReadValue<Vector2>();
            bool esPuntero = mirar.activeControl?.device is Pointer;
            delta *= esPuntero ? sensibilidadRaton : sensibilidadStick * Time.deltaTime;

            transform.Rotate(0, delta.x, 0);
            inclinacion = Mathf.Clamp(inclinacion - delta.y, -limiteVertical, limiteVertical);
            camara.localRotation = Quaternion.Euler(inclinacion, 0, 0);
        }

        void Caminar()
        {
            Vector2 entrada = mover.ReadValue<Vector2>();
            Vector3 direccion = transform.right * entrada.x + transform.forward * entrada.y;
            float rapidez = correr.IsPressed() ? velocidadCorrer : velocidad;

            velocidadVertical = controlador.isGrounded
                ? -1f
                : velocidadVertical + Physics.gravity.y * Time.deltaTime;

            controlador.Move((direccion * rapidez + Vector3.up * velocidadVertical) * Time.deltaTime);
        }

        public void Teletransportar(Transform destino)
        {
            controlador.enabled = false;
            transform.SetPositionAndRotation(destino.position, Quaternion.Euler(0, destino.eulerAngles.y, 0));
            controlador.enabled = true;

            inclinacion = 0;
            camara.localRotation = Quaternion.identity;
            velocidadVertical = 0;
        }
    }
}
