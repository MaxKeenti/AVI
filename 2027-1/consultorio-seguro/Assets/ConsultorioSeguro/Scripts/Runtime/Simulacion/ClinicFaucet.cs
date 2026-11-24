using UnityEngine;

namespace ConsultorioSeguroNuevo
{
    /// <summary>Grifo que el visitante abre o cierra con E; el chorro se cierra solo para no olvidarlo abierto.</summary>
    public sealed class ClinicFaucet : ClinicInteractable
    {
        [Min(1)] public float autoClose = 8f;
        public ParticleSystem Stream { get; private set; }
        public bool Running { get; private set; }
        AudioSource sound;
        float closeAt;

        public void Initialize(Vector3 spout)
        {
            Stream = SimulationFx.Water(transform, spout);
            sound = SimulationFx.Loop("Agua", Stream.transform, .35f);
        }

        public override bool CanOutline(ClinicGame game) => false;
        public override bool CanInteract(ClinicGame game) => game.Playing && !game.HeldItem;
        public override string Prompt(ClinicGame game) => game.HeldItem ? "" : Running ? "E · Cerrar el grifo" : "E · Abrir el grifo";
        public override void Interact(ClinicGame game)
        {
            if (CanInteract(game)) SetRunning(!Running);
        }

        /// <summary>Abre el chorro durante <paramref name="seconds"/> segundos o, si es cero, hasta el cierre automático.</summary>
        public void SetRunning(bool running, float seconds = 0)
        {
            Running = running;
            closeAt = Time.time + (seconds > 0 ? seconds : autoClose);
            if (!Stream) return;
            if (running)
            {
                Stream.Play();
                if (sound) sound.Play();
            }
            else
            {
                Stream.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                if (sound) sound.Stop();
            }
        }

        void Update()
        {
            if (Running && Time.time > closeAt) SetRunning(false);
        }
    }
}
