using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ConsultorioSeguro
{
    // Pasos que deben hacerse en orden antes de clasificar los residuos de un escenario.
    public class SecuenciaPasos : MonoBehaviour
    {
        [SerializeField] List<Paso> pasos = new();
        [SerializeField] UnityEvent alTerminar;
        [SerializeField] UnityEvent alReiniciar;

        int indice;
        Coroutine esperaActual;

        public bool Activa { get; private set; }
        public bool Esperando { get; private set; }
        public int Indice => indice;
        public int Total => pasos.Count;
        public bool Terminada => indice >= pasos.Count;
        public Paso PasoActual => Terminada ? null : pasos[indice];
        public UnityEvent AlTerminar => alTerminar;
        public UnityEvent AlReiniciar => alReiniciar;

        public event Action<Paso> PasoCompletado;
        public event Action EsperaCambiada;
        public event Action Terminado;

        void Awake()
        {
            foreach (Paso paso in pasos)
                paso.Secuencia = this;
        }

        public bool EsPasoActual(Paso paso) => Activa && !Esperando && PasoActual == paso;

        public void Iniciar()
        {
            Restablecer();
            Activa = true;
        }

        void Restablecer()
        {
            if (esperaActual != null)
                StopCoroutine(esperaActual);

            esperaActual = null;
            Esperando = false;
            Activa = false;
            indice = 0;

            // En orden inverso para deshacer los efectos como se aplicaron.
            for (int i = pasos.Count - 1; i >= 0; i--)
                pasos[i].Reiniciar();
            alReiniciar.Invoke();
        }

        public void Completar(Paso paso)
        {
            if (!EsPasoActual(paso))
                return;

            paso.MarcarCompletado();
            indice++;
            PasoCompletado?.Invoke(paso);

            if (paso.Espera > 0)
                esperaActual = StartCoroutine(Esperar(paso.Espera));
            else
                ComprobarFin();
        }

        IEnumerator Esperar(float segundos)
        {
            Esperando = true;
            EsperaCambiada?.Invoke();
            yield return new WaitForSeconds(segundos);
            Esperando = false;
            esperaActual = null;
            EsperaCambiada?.Invoke();
            ComprobarFin();
        }

        void ComprobarFin()
        {
            if (!Terminada)
                return;

            alTerminar.Invoke();
            Terminado?.Invoke();
        }
    }
}
