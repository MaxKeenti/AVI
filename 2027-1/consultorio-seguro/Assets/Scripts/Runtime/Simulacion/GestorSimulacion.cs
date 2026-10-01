using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ConsultorioSeguro
{
    public enum EstadoSimulacion
    {
        Menu,
        Jugando,
        Pausa,
    }

    // Controla el flujo entre menú, recorrido y pausa, sabe en qué área está
    // trabajando el usuario y guarda la mejor puntuación de cada acción.
    public class GestorSimulacion : MonoBehaviour
    {
        const string ClaveMejorPuntuacion = "mejor_puntuacion_";

        [SerializeField] List<Escenario> escenarios = new();
        [SerializeField] ControladorPrimeraPersona jugador;
        [SerializeField] Interactor interactor;

        InputAction accionPausa;
        readonly HashSet<Escenario> instruccionesMostradas = new();

        public static GestorSimulacion Instancia { get; private set; }
        public IReadOnlyList<Escenario> Escenarios => escenarios;
        public EstadoSimulacion Estado { get; private set; }
        public bool TodosCompletados => escenarios.All(e => e.Completado);

        // Área de la que se muestra el marcador: la última en la que entró o trabajó el usuario.
        public Escenario EscenarioEnFoco { get; private set; }

        public event Action<EstadoSimulacion> EstadoCambiado;
        public event Action<Escenario> EnfoqueCambiado;
        public event Action<Residuo, ResultadoClasificacion> ResiduoClasificado;
        public event Action<Paso> PasoCompletado;
        public event Action<Escenario> EscenarioActualizado;
        public event Action<Escenario> EscenarioTerminado;
        public event Action<string, string> InformacionSolicitada;

        void Awake()
        {
            Instancia = this;
            accionPausa = InputSystem.actions.FindAction("UI/Cancel", true);

            foreach (Escenario escenario in escenarios)
            {
                escenario.ResiduoClasificado += (residuo, resultado) =>
                {
                    Enfocar(escenario);
                    ResiduoClasificado?.Invoke(residuo, resultado);
                };
                escenario.PasoCompletado += paso =>
                {
                    Enfocar(escenario);
                    PasoCompletado?.Invoke(paso);
                };
                escenario.Actualizado += () => EscenarioActualizado?.Invoke(escenario);
                escenario.Terminado += () => AlTerminar(escenario);
            }
        }

        void Start() => CambiarEstado(EstadoSimulacion.Menu);

        void OnDestroy()
        {
            if (Instancia == this)
                Instancia = null;
            Time.timeScale = 1;
        }

        void Update()
        {
            if (!accionPausa.WasPressedThisFrame())
                return;

            var interfaz = FindFirstObjectByType<InterfazSimulador>();
            if (interfaz != null && interfaz.PanelAuxiliarAbierto)
            {
                interfaz.CerrarPanelAuxiliar();
                return;
            }
            if (Estado == EstadoSimulacion.Jugando)
                Pausar();
            else if (Estado == EstadoSimulacion.Pausa)
                Reanudar();
        }

        public void Entrar() => CambiarEstado(EstadoSimulacion.Jugando);

        public void Pausar() => CambiarEstado(EstadoSimulacion.Pausa);

        public void Reanudar() => CambiarEstado(EstadoSimulacion.Jugando);

        public void IrAlMenu() => CambiarEstado(EstadoSimulacion.Menu);

        public void Salir()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        // Llamado por ZonaEscenario cuando el usuario entra a un área.
        public void EntrarZona(Escenario escenario)
        {
            Enfocar(escenario);

            // Las instrucciones se muestran la primera vez, no cada vez que se cruza el borde del área.
            if (!escenario.Iniciado && !escenario.Completado && instruccionesMostradas.Add(escenario))
                Informar(escenario.Datos.nombre, escenario.Datos.instrucciones);
        }

        public void Enfocar(Escenario escenario)
        {
            if (escenario == EscenarioEnFoco)
                return;

            EscenarioEnFoco = escenario;
            EnfoqueCambiado?.Invoke(escenario);
        }

        public void Reiniciar(Escenario escenario)
        {
            if (interactor.Sostenido != null && interactor.Sostenido.Escenario == escenario)
                interactor.Liberar();

            escenario.Iniciar();
            Enfocar(escenario);
            Informar(escenario.Datos.nombre, escenario.Datos.instrucciones);
        }

        // Botón del menú de pausa.
        public void ReiniciarEnFoco()
        {
            if (EscenarioEnFoco == null)
                return;

            Reiniciar(EscenarioEnFoco);
            Reanudar();
        }

        public void Informar(string titulo, string texto) => InformacionSolicitada?.Invoke(titulo, texto);

        public static int MejorPuntuacion(AccionSimulacion accion) =>
            PlayerPrefs.GetInt(ClaveMejorPuntuacion + accion, 0);

        void AlTerminar(Escenario escenario)
        {
            AccionSimulacion accion = escenario.Datos.accion;
            if (escenario.Puntuacion.Puntos > MejorPuntuacion(accion))
            {
                PlayerPrefs.SetInt(ClaveMejorPuntuacion + accion, escenario.Puntuacion.Puntos);
                PlayerPrefs.Save();
            }

            EscenarioTerminado?.Invoke(escenario);
        }

        void CambiarEstado(EstadoSimulacion estado)
        {
            Estado = estado;
            bool jugando = estado == EstadoSimulacion.Jugando;

            jugador.Habilitado = jugando;
            interactor.enabled = jugando;
            Cursor.lockState = jugando ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !jugando;
            Time.timeScale = estado == EstadoSimulacion.Pausa ? 0 : 1;

            EstadoCambiado?.Invoke(estado);
        }
    }
}
