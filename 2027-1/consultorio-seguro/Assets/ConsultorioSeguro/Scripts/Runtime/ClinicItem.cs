using UnityEngine;

namespace ConsultorioSeguroNuevo
{
    public sealed class ClinicItem : ClinicInteractable
    {
        public string displayName = "Material";
        [TextArea(2, 5)] public string description;
        public WasteDestination correctDestination;
        public bool Completed { get; private set; }
        public bool Held { get; private set; }
        Transform originalParent;
        Vector3 originalPosition, originalScale;
        Quaternion originalRotation;
        Collider[] colliders;
        bool[] originalColliderStates;
        bool initialized;

        public void Initialize()
        {
            if (initialized) return;
            initialized = true;
            originalParent = transform.parent;
            originalPosition = transform.localPosition;
            originalRotation = transform.localRotation;
            originalScale = transform.localScale;
            colliders = GetComponentsInChildren<Collider>(true);
            originalColliderStates = new bool[colliders.Length];
            for (int i = 0; i < colliders.Length; i++) originalColliderStates[i] = colliders[i].enabled;
        }

        public override bool CanInteract(ClinicGame game) => game.Playing && !Completed && !Held
            && !game.HeldItem && game.CurrentRoom && game.CurrentRoom.roomId == roomId
            && game.CurrentRoom.StepsComplete;
        public override string Prompt(ClinicGame game)
        {
            if (Completed) return "";
            if (game.HeldItem) return "R · Devolver el objeto que llevas";
            if (game.CurrentRoom && !game.CurrentRoom.StepsComplete) return "Completa primero los pasos de la práctica";
            return "E · Tomar: " + displayName;
        }
        public override void Interact(ClinicGame game)
        {
            if (CanInteract(game)) game.PickUp(this);
        }

        public void Hold(Transform point)
        {
            Initialize();
            Held = true;
            foreach (var collider in colliders) if (collider) collider.enabled = false;
            transform.SetParent(point, true);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
        public void ReturnToPlace()
        {
            Initialize();
            Held = false;
            transform.SetParent(originalParent, false);
            transform.localPosition = originalPosition;
            transform.localRotation = originalRotation;
            transform.localScale = originalScale;
            for (int i = 0; i < colliders.Length; i++) if (colliders[i]) colliders[i].enabled = originalColliderStates[i];
        }
        public void Complete()
        {
            ReturnToPlace();
            Completed = true;
            gameObject.SetActive(false);
        }
        public void ResetItem()
        {
            ReturnToPlace();
            Completed = false;
            gameObject.SetActive(true);
        }
    }
}
