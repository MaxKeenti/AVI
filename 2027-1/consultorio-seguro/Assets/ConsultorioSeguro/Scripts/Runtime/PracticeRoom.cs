using System;
using System.Linq;
using UnityEngine;

namespace ConsultorioSeguroNuevo
{
    public sealed class PracticeRoom : MonoBehaviour
    {
        public int roomId;
        public string title = "Sala de práctica";
        [TextArea(3, 8)] public string instructions;
        public BoxCollider roomVolume;
        public PracticeStation[] steps = Array.Empty<PracticeStation>();
        public ClinicItem[] Items { get; private set; } = Array.Empty<ClinicItem>();
        public int NextStep { get; private set; }
        public int Correct { get; private set; }
        public int Mistakes { get; private set; }
        public bool Busy { get; private set; }
        public bool StepsComplete => NextStep >= steps.Length && !Busy;
        public int Remaining => Items.Count(item => item && !item.Completed);
        public bool Completed => StepsComplete && Items.Length > 0 && Remaining == 0;
        public int Score => Mathf.Max(0, Correct * 10 - Mistakes * 5);
        public PracticeStation CurrentStep => NextStep < steps.Length ? steps[NextStep] : null;

        public void Initialize(ClinicItem[] items)
        {
            Items = items.Where(item => item.roomId == roomId).ToArray();
            if (steps == null || steps.Length == 0)
                steps = FindObjectsByType<PracticeStation>(FindObjectsSortMode.None)
                    .Where(step => step.roomId == roomId).OrderBy(step => step.stepIndex).ToArray();
            else steps = steps.Where(step => step).OrderBy(step => step.stepIndex).ToArray();
            ResetPractice();
        }

        public bool Contains(Vector3 worldPoint)
        {
            if (!roomVolume || !roomVolume.enabled || !roomVolume.gameObject.activeInHierarchy) return false;
            Vector3 point = roomVolume.transform.InverseTransformPoint(worldPoint) - roomVolume.center;
            Vector3 half = roomVolume.size * .5f;
            return Mathf.Abs(point.x) < half.x && Mathf.Abs(point.y) < half.y && Mathf.Abs(point.z) < half.z;
        }

        public bool CanDo(PracticeStation station) => !Busy && CurrentStep == station && !station.Completed;
        public void BeginStep() => Busy = true;
        public void FinishStep(PracticeStation station)
        {
            if (CurrentStep != station) return;
            Busy = false;
            NextStep++;
        }
        public void RecordAnswer(bool correct)
        {
            if (correct) Correct++;
            else Mistakes++;
        }

        public void ResetPractice()
        {
            NextStep = Correct = Mistakes = 0;
            Busy = false;
            foreach (var step in steps) if (step) step.ResetStation();
            foreach (var item in Items) if (item) item.ResetItem();
        }
    }
}
