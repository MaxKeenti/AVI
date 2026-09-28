using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ConsultorioSeguro
{
    // Una acción de simulación en un área del consultorio. Los Residuo hijos de
    // este objeto son los que el usuario debe clasificar; si hay una SecuenciaPasos,
    // aparecen al terminarla. Todas las áreas están disponibles desde el inicio.
    public class Escenario : MonoBehaviour
    {
        [SerializeField] DatosEscenario datos;

        [Tooltip("Opcional. Pasos que se hacen antes de clasificar los residuos.")]
        [SerializeField] SecuenciaPasos secuencia;

        readonly List<Residuo> residuos = new();

        public DatosEscenario Datos => datos;
        public SecuenciaPasos Secuencia => secuencia;
        public Puntuacion Puntuacion { get; private set; }
        public bool Completado { get; private set; }
        public int TotalResiduos => residuos.Count;
        public int Restantes => residuos.Count(r => !r.Clasificado);
        public bool PasosPendientes => secuencia != null && !secuencia.Terminada;

        // Si el usuario ya empezó a trabajar en esta área desde el último reinicio.
        public bool Iniciado =>
            Puntuacion.Aciertos + Puntuacion.Errores > 0 || (secuencia != null && secuencia.Indice > 0);

        public event Action<Residuo, ResultadoClasificacion> ResiduoClasificado;
        public event Action<Paso> PasoCompletado;
        public event Action Actualizado;
        public event Action Terminado;

        void Awake()
        {
            if (datos == null)
                Debug.LogError($"El escenario {name} no tiene DatosEscenario asignados.", this);

            Puntuacion = datos != null
                ? new Puntuacion(datos.puntosPorAcierto, datos.penalizacionPorError)
                : new Puntuacion(10, 5);

            GetComponentsInChildren(true, residuos);
            foreach (Residuo residuo in residuos)
                residuo.Inicializar(this);

            if (secuencia == null)
                return;

            secuencia.PasoCompletado += paso =>
            {
                PasoCompletado?.Invoke(paso);
                Actualizado?.Invoke();
            };
            secuencia.EsperaCambiada += () => Actualizado?.Invoke();
            secuencia.Terminado += AlTerminarPasos;
        }

        void Start() => Iniciar();

        // Deja el área como al principio: residuos en su lugar, pasos sin hacer y marcador en cero.
        public void Iniciar()
        {
            Completado = false;
            Puntuacion.Reiniciar();

            foreach (Residuo residuo in residuos)
                residuo.Restablecer(secuencia == null);

            if (secuencia != null)
                secuencia.Iniciar();

            Actualizado?.Invoke();
        }

        public void Clasificar(Residuo residuo, Destino destino)
        {
            if (Completado)
            {
                residuo.Devolver();
                return;
            }

            ResultadoClasificacion resultado = Clasificador.Evaluar(residuo.Datos, destino);
            Puntuacion.Registrar(resultado.Correcto);

            if (resultado.Correcto)
                residuo.MarcarClasificado();
            else
                residuo.Devolver();

            ResiduoClasificado?.Invoke(residuo, resultado);
            Actualizado?.Invoke();
            ComprobarFin();
        }

        void AlTerminarPasos()
        {
            foreach (Residuo residuo in residuos)
                residuo.Mostrar();

            Actualizado?.Invoke();
            ComprobarFin();
        }

        void ComprobarFin()
        {
            if (Completado || PasosPendientes || Restantes > 0)
                return;

            Completado = true;
            Terminado?.Invoke();
        }
    }
}
