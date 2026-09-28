using System.Collections;
using UnityEngine;

namespace ConsultorioSeguro
{
    // Mueve un objeto a otra pose local, por ejemplo, el brazo del equipo de rayos X.
    // Pensado para llamarse desde los eventos de un Paso.
    public class TransicionTransform : MonoBehaviour
    {
        [SerializeField] Vector3 posicionFinal;
        [SerializeField] Vector3 rotacionFinal;
        [SerializeField, Min(0.01f)] float duracion = 1f;

        Vector3 posicionInicial;
        Quaternion rotacionInicial;
        bool capturada;
        Coroutine movimiento;

        void Awake() => Capturar();

        void Capturar()
        {
            if (capturada)
                return;

            posicionInicial = transform.localPosition;
            rotacionInicial = transform.localRotation;
            capturada = true;
        }

        public void Aplicar()
        {
            Capturar();
            if (movimiento != null)
                StopCoroutine(movimiento);
            movimiento = StartCoroutine(Mover(posicionFinal, Quaternion.Euler(rotacionFinal)));
        }

        public void Revertir()
        {
            Capturar();
            if (movimiento != null)
                StopCoroutine(movimiento);
            movimiento = null;
            transform.localPosition = posicionInicial;
            transform.localRotation = rotacionInicial;
        }

        IEnumerator Mover(Vector3 posicion, Quaternion rotacion)
        {
            Vector3 posicionOrigen = transform.localPosition;
            Quaternion rotacionOrigen = transform.localRotation;

            for (float t = 0; t < 1; t += Time.deltaTime / duracion)
            {
                float suave = Mathf.SmoothStep(0, 1, t);
                transform.localPosition = Vector3.Lerp(posicionOrigen, posicion, suave);
                transform.localRotation = Quaternion.Slerp(rotacionOrigen, rotacion, suave);
                yield return null;
            }

            transform.localPosition = posicion;
            transform.localRotation = rotacion;
            movimiento = null;
        }
    }
}
