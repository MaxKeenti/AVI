using TMPro;
using UnityEngine;

namespace ConsultorioSeguro
{
    // Hoja de práctica colgada en cada área: muestra el avance y el marcador de
    // esa acción. Al interactuar muestra las instrucciones o, si ya se terminó,
    // reinicia la práctica para repetirla.
    public class TableroPractica : MonoBehaviour, IInteractuable
    {
        [SerializeField] Escenario escenario;
        [SerializeField] TMP_Text texto;
        [SerializeField] TMP_Text textoReverso;

        void OnEnable()
        {
            escenario.Actualizado += Actualizar;
            escenario.Terminado += Actualizar;
        }

        void OnDisable()
        {
            escenario.Actualizado -= Actualizar;
            escenario.Terminado -= Actualizar;
        }

        void Start() => Actualizar();

        public bool PuedeInteractuar(Interactor interactor) => interactor.Sostenido == null;

        public string Indicacion(Interactor interactor)
        {
            string nombre = escenario.Datos.nombre;
            if (!PuedeInteractuar(interactor))
                return nombre;
            return escenario.Completado ? $"Repetir práctica: {nombre}" : $"Ver instrucciones: {nombre}";
        }

        public void Interactuar(Interactor interactor)
        {
            GestorSimulacion gestor = GestorSimulacion.Instancia;
            if (gestor == null)
                return;

            if (escenario.Completado)
                gestor.Reiniciar(escenario);
            else
                gestor.Informar(escenario.Datos.nombre, escenario.Datos.instrucciones);
        }

        void Actualizar()
        {
            Puntuacion puntuacion = escenario.Puntuacion;

            string estado;
            if (escenario.Completado)
                estado = "<color=#215B39>Práctica completada</color>";
            else if (escenario.PasosPendientes)
                estado = escenario.Secuencia.Esperando
                    ? "Espera un momento..."
                    : $"Paso {escenario.Secuencia.Indice + 1} de {escenario.Secuencia.Total}: {escenario.Secuencia.PasoActual.Accion}";
            else
                estado = $"Residuos por clasificar: {escenario.Restantes} de {escenario.TotalResiduos}";

            // El gestor guarda la mejor puntuación en el mismo evento; se toma la actual por si aún no lo hizo.
            int mejor = GestorSimulacion.MejorPuntuacion(escenario.Datos.accion);
            if (escenario.Completado)
                mejor = Mathf.Max(mejor, puntuacion.Puntos);

            string accion = escenario.Completado ? "E · Repetir práctica" : "E · Ver instrucciones";

            texto.text =
                $"<b>{escenario.Datos.nombre}</b>\n" +
                $"<size=75%>{estado}\n" +
                $"Aciertos: {puntuacion.Aciertos}   Errores: {puntuacion.Errores}   Puntos: {puntuacion.Puntos}\n" +
                $"Mejor puntuación: {mejor}</size>\n" +
                $"<size=75%><i>{accion}</i></size>";
            if (textoReverso != null) textoReverso.text = texto.text;
        }
    }
}
