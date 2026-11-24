using UnityEngine;

namespace ConsultorioSeguroNuevo
{
    /// <summary>
    /// Acompaña a un material suelto: polvo y sonido proporcionales al impacto, y regreso automático
    /// si sale de su sala o cae fuera del mundo, para que la práctica nunca quede bloqueada.
    /// </summary>
    [RequireComponent(typeof(ClinicItem))]
    public sealed class ItemPhysics : MonoBehaviour
    {
        [Tooltip("Velocidad relativa mínima, en m/s, para levantar polvo y sonar.")]
        public float impactThreshold = .9f;
        ClinicGame game;
        ClinicItem item;
        PracticeRoom room;
        float lastImpact;
        public int Impacts { get; private set; }

        public void Initialize(ClinicGame owner, ClinicItem target, PracticeRoom home)
        {
            game = owner;
            item = target;
            room = home;
        }

        void FixedUpdate()
        {
            if (!item || !item.Loose) return;
            Vector3 position = item.Body ? item.Body.worldCenterOfMass : transform.position;
            bool outside = position.y < -3 || (room && !room.Contains(position) && !room.Contains(position + Vector3.up * .5f));
            if (!outside) return;
            item.ReturnToPlace();
            if (game) game.ShowMessage("El material salió de la sala", item.displayName
                + " volvió a su lugar. Los residuos se clasifican dentro de su práctica.");
        }

        void OnCollisionEnter(Collision collision)
        {
            if (!item || !item.Loose) return;
            float speed = collision.relativeVelocity.magnitude;
            if (speed < impactThreshold || Time.time - lastImpact < .12f) return;
            lastImpact = Time.time;
            Impacts++;
            var contact = collision.GetContact(0);
            SimulationFx.Dust(contact.point, contact.normal, speed);
            SimulationFx.Play("Golpe", contact.point, Mathf.Clamp01(speed / 6f));
        }
    }
}
