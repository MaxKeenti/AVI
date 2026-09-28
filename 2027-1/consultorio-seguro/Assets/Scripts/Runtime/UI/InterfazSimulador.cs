using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ConsultorioSeguro
{
    // Muestra el HUD del área en la que trabaja el usuario, los mensajes de
    // retroalimentación y los paneles de menú, pausa y créditos.
    // Los botones llaman al gestor desde su evento OnClick en el Inspector.
    public class InterfazSimulador : MonoBehaviour
    {
        [SerializeField] GestorSimulacion gestor;
        [SerializeField] Interactor interactor;
        [SerializeField] CatalogoAtribuciones catalogo;

        [SerializeField, TextArea(3, 8)] string creditosEquipo;

        [Tooltip("Segundos entre el mensaje del último residuo y el de práctica completada.")]
        [SerializeField, Min(0)] float esperaResumen = 2.5f;

        [Header("Juego")]
        [SerializeField] GameObject hud;
        [SerializeField] GameObject marcador;
        [SerializeField] TMP_Text textoEscenario;
        [SerializeField] TMP_Text textoMarcador;
        [SerializeField] GameObject indicacion;
        [SerializeField] TMP_Text textoIndicacion;

        [Header("Mensajes")]
        [SerializeField] GameObject mensaje;
        [SerializeField] Image fondoMensaje;
        [SerializeField] TMP_Text textoTituloMensaje;
        [SerializeField] TMP_Text textoMensaje;
        [SerializeField] Color colorCorrecto = new(0.12f, 0.5f, 0.28f, 0.95f);
        [SerializeField] Color colorIncorrecto = new(0.68f, 0.14f, 0.14f, 0.95f);
        [SerializeField] Color colorInformativo = new(0.12f, 0.24f, 0.42f, 0.95f);

        [Header("Paneles")]
        [SerializeField] GameObject panelMenu;
        [SerializeField] GameObject panelPausa;
        [SerializeField] Button botonReiniciarPractica;
        [SerializeField] GameObject panelCreditos;
        [SerializeField] TMP_Text textoCreditos;

        Coroutine ocultarMensaje;

        void Awake()
        {
            mensaje.SetActive(false);
            indicacion.SetActive(false);
            marcador.SetActive(false);
            panelCreditos.SetActive(false);
        }

        void OnEnable()
        {
            gestor.EstadoCambiado += AlCambiarEstado;
            gestor.EnfoqueCambiado += AlCambiarEnfoque;
            gestor.ResiduoClasificado += AlClasificar;
            gestor.PasoCompletado += AlCompletarPaso;
            gestor.EscenarioActualizado += AlActualizarEscenario;
            gestor.EscenarioTerminado += AlTerminarEscenario;
            gestor.InformacionSolicitada += AlInformar;
            interactor.IndicacionCambiada += AlCambiarIndicacion;
        }

        void OnDisable()
        {
            gestor.EstadoCambiado -= AlCambiarEstado;
            gestor.EnfoqueCambiado -= AlCambiarEnfoque;
            gestor.ResiduoClasificado -= AlClasificar;
            gestor.PasoCompletado -= AlCompletarPaso;
            gestor.EscenarioActualizado -= AlActualizarEscenario;
            gestor.EscenarioTerminado -= AlTerminarEscenario;
            gestor.InformacionSolicitada -= AlInformar;
            interactor.IndicacionCambiada -= AlCambiarIndicacion;
        }

        void Start() => textoCreditos.text = TextoCreditos();

        public void AbrirCreditos()
        {
            panelMenu.SetActive(false);
            panelCreditos.SetActive(true);
        }

        public void CerrarCreditos()
        {
            panelCreditos.SetActive(false);
            panelMenu.SetActive(gestor.Estado == EstadoSimulacion.Menu);
        }

        void AlCambiarEstado(EstadoSimulacion estado)
        {
            panelMenu.SetActive(estado == EstadoSimulacion.Menu);
            panelCreditos.SetActive(false);
            panelPausa.SetActive(estado == EstadoSimulacion.Pausa);
            hud.SetActive(estado != EstadoSimulacion.Menu);

            if (estado == EstadoSimulacion.Pausa)
                botonReiniciarPractica.interactable = gestor.EscenarioEnFoco != null;
            if (estado == EstadoSimulacion.Menu)
                OcultarMensaje();
        }

        void AlCambiarEnfoque(Escenario escenario) => ActualizarMarcador();

        void AlActualizarEscenario(Escenario escenario)
        {
            if (escenario == gestor.EscenarioEnFoco)
                ActualizarMarcador();
        }

        void ActualizarMarcador()
        {
            Escenario escenario = gestor.EscenarioEnFoco;
            marcador.SetActive(escenario != null);
            if (escenario == null)
                return;

            string progreso;
            if (escenario.Completado)
                progreso = "Práctica completada";
            else if (escenario.PasosPendientes)
            {
                SecuenciaPasos secuencia = escenario.Secuencia;
                progreso = secuencia.Esperando
                    ? "Espera un momento..."
                    : $"Paso {secuencia.Indice + 1} de {secuencia.Total}: {secuencia.PasoActual.Accion}";
            }
            else
                progreso = $"Residuos por clasificar: {escenario.Restantes} de {escenario.TotalResiduos}";

            Puntuacion puntuacion = escenario.Puntuacion;
            textoEscenario.text = escenario.Datos.nombre;
            textoMarcador.text =
                $"{progreso}\nAciertos: {puntuacion.Aciertos}   Errores: {puntuacion.Errores}   Puntos: {puntuacion.Puntos}";
        }

        void AlClasificar(Residuo residuo, ResultadoClasificacion resultado) =>
            MostrarMensaje(resultado.Titulo, resultado.Mensaje, resultado.Correcto ? colorCorrecto : colorIncorrecto);

        void AlCompletarPaso(Paso paso)
        {
            if (!string.IsNullOrWhiteSpace(paso.Mensaje))
                MostrarMensaje(paso.Accion, paso.Mensaje, colorInformativo);
        }

        void AlTerminarEscenario(Escenario escenario)
        {
            ActualizarMarcador();
            StartCoroutine(MostrarResumen(escenario));
        }

        // Espera a que se lea el mensaje del último residuo antes de mostrar el resumen.
        IEnumerator MostrarResumen(Escenario escenario)
        {
            yield return new WaitForSeconds(esperaResumen);
            if (gestor.Estado != EstadoSimulacion.Menu && escenario.Completado)
                MostrarMensaje($"Práctica completada: {escenario.Datos.nombre}", TextoResumen(escenario), colorInformativo);
        }

        void AlInformar(string titulo, string texto) => MostrarMensaje(titulo, texto, colorInformativo);

        void AlCambiarIndicacion(string texto)
        {
            indicacion.SetActive(!string.IsNullOrEmpty(texto));
            textoIndicacion.text = texto;
        }

        void MostrarMensaje(string titulo, string texto, Color color)
        {
            fondoMensaje.color = color;
            textoTituloMensaje.text = titulo;
            textoMensaje.text = texto;
            mensaje.SetActive(true);

            if (ocultarMensaje != null)
                StopCoroutine(ocultarMensaje);
            ocultarMensaje = StartCoroutine(OcultarDespues(3f + texto.Length / 30f));
        }

        IEnumerator OcultarDespues(float segundos)
        {
            yield return new WaitForSeconds(segundos);
            OcultarMensaje();
        }

        void OcultarMensaje()
        {
            if (ocultarMensaje != null)
                StopCoroutine(ocultarMensaje);
            ocultarMensaje = null;
            mensaje.SetActive(false);
        }

        string TextoResumen(Escenario escenario)
        {
            Puntuacion puntuacion = escenario.Puntuacion;
            int mejor = GestorSimulacion.MejorPuntuacion(escenario.Datos.accion);
            string comentario = puntuacion.Errores == 0
                ? "¡Sin errores!"
                : "Repasa los mensajes de los errores y vuelve a intentarlo.";

            string texto =
                $"Aciertos: {puntuacion.Aciertos}   Errores: {puntuacion.Errores}\n" +
                $"Puntuación: {puntuacion.Puntos} de {puntuacion.PuntosMaximos(escenario.TotalResiduos)}   Mejor: {mejor}\n" +
                $"{comentario} Para repetirla, usa la hoja de práctica del área.";

            if (gestor.TodosCompletados)
                texto += "\n\n¡Completaste las cinco prácticas del consultorio!";
            return texto;
        }

        string TextoCreditos()
        {
            StringBuilder texto = new();
            texto.AppendLine(creditosEquipo);
            texto.AppendLine();
            texto.AppendLine("<b>Recursos de terceros</b>");

            if (catalogo == null || catalogo.atribuciones.Count == 0)
            {
                texto.AppendLine("Todavía no se han agregado recursos de terceros.");
                return texto.ToString();
            }

            foreach (Atribucion atribucion in catalogo.atribuciones)
            {
                if (atribucion == null)
                    continue;

                texto.AppendLine();
                texto.AppendLine($"<b>{atribucion.titulo}</b> — {atribucion.autor}");
                texto.AppendLine($"Licencia: {atribucion.licencia}");
                if (!string.IsNullOrWhiteSpace(atribucion.modificaciones))
                    texto.AppendLine($"Modificaciones: {atribucion.modificaciones}");
                texto.AppendLine($"<link=\"{atribucion.url}\"><color=#6CB4FF><u>{atribucion.url}</u></color></link>");
            }

            return texto.ToString();
        }
    }
}
