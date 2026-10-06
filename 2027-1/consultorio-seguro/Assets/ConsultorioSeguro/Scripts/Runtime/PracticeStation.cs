using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace ConsultorioSeguroNuevo
{
    public sealed class PracticeStation : ClinicInteractable
    {
        public int stepIndex;
        public string actionText = "Realizar el paso indicado";
        [TextArea(2, 6)] public string completionText;
        [Min(0)] public float duration;
        public UnityEvent onComplete = new UnityEvent();
        public UnityEvent onReset = new UnityEvent();
        public bool Completed { get; private set; }
        public bool Working { get; private set; }

        public override bool CanInteract(ClinicGame game) => game.Playing && !Completed && !Working
            && !game.HeldItem && game.CurrentRoom && game.CurrentRoom.roomId == roomId
            && game.CurrentRoom.CanDo(this);
        public override string Prompt(ClinicGame game)
        {
            if (Completed) return actionText + " · Completado";
            if (Working) return "Proceso simulado en curso…";
            return CanInteract(game) ? "E · " + actionText : "Sigue el paso indicado en pantalla";
        }
        public override void Interact(ClinicGame game)
        {
            if (CanInteract(game)) StartCoroutine(Perform(game, game.CurrentRoom));
        }
        IEnumerator Perform(ClinicGame game, PracticeRoom room)
        {
            Working = true;
            room.BeginStep();
            if (duration > 0)
            {
                game.ShowMessage(actionText, "Proceso representado de forma abreviada. En la clínica se siguen los tiempos y controles validados del fabricante.");
                yield return new WaitForSeconds(duration);
            }
            Completed = true;
            Working = false;
            room.FinishStep(this);
            onComplete.Invoke();
            game.ShowMessage("Paso completado", completionText);
        }
        public void ResetStation()
        {
            StopAllCoroutines();
            Completed = Working = false;
            onReset.Invoke();
        }
    }
}
