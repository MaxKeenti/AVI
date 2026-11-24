using UnityEngine;

namespace ConsultorioSeguroNuevo
{
    public sealed class ClinicItem : ClinicInteractable
    {
        public string displayName = "Material";
        [TextArea(2, 5)] public string description;
        public WasteDestination correctDestination;
        [Min(.005f)] public float mass = .08f;
        public bool Completed { get; private set; }
        public bool Held { get; private set; }
        // Suelto: se soltó o lanzó y la simulación física lo controla hasta tomarlo o devolverlo.
        public bool Loose { get; private set; }
        public bool IsSharp => correctDestination == WasteDestination.Sharps;
        public Rigidbody Body { get; private set; }
        Transform originalParent;
        Vector3 originalPosition, originalScale;
        Quaternion originalRotation;
        Collider[] colliders;
        bool[] originalColliderStates;
        BoxCollider grip;
        Vector3 gripCenter, gripSize, bodyCenter, bodySize;
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
            // La caja de interacción es mayor que la malla para apuntar con facilidad. Al caer se ajusta
            // a la forma visible para que el objeto descanse sobre la superficie y no flote.
            grip = GetComponent<BoxCollider>();
            if (grip)
            {
                gripCenter = grip.center; gripSize = grip.size;
                bodyCenter = gripCenter; bodySize = gripSize;
                var renderers = GetComponentsInChildren<Renderer>(true);
                if (renderers.Length > 0)
                {
                    Bounds local = LocalBounds(renderers);
                    bodyCenter = local.center;
                    bodySize = Vector3.Max(local.size, Vector3.one * .02f);
                }
            }
            Body = GetComponent<Rigidbody>();
            if (!Body) Body = gameObject.AddComponent<Rigidbody>();
            Body.isKinematic = true;
            Body.mass = mass;
            // Sin interpolación mientras es cinemático: si no, el cuerpo reescribe su pose anterior
            // sobre el transform y el objeto no sigue a la mano al estar emparentado.
            Body.interpolation = RigidbodyInterpolation.None;
            Body.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            Body.linearDamping = .05f;
            Body.angularDamping = .35f;
        }

        Bounds LocalBounds(Renderer[] renderers)
        {
            Bounds result = default;
            bool first = true;
            foreach (var renderer in renderers)
            {
                var filter = renderer.GetComponent<MeshFilter>();
                if (!filter || !filter.sharedMesh) continue;
                var mesh = filter.sharedMesh.bounds;
                for (int corner = 0; corner < 8; corner++)
                {
                    Vector3 point = mesh.center + Vector3.Scale(mesh.extents,
                        new Vector3((corner & 1) == 0 ? -1 : 1, (corner & 2) == 0 ? -1 : 1, (corner & 4) == 0 ? -1 : 1));
                    point = transform.InverseTransformPoint(renderer.transform.TransformPoint(point));
                    if (first) { result = new Bounds(point, Vector3.zero); first = false; }
                    else result.Encapsulate(point);
                }
            }
            return first ? new Bounds(gripCenter, gripSize) : result;
        }

        public override bool CanInteract(ClinicGame game) => game.Playing && !Completed && !Held
            && !game.HeldItem && game.CurrentRoom && game.CurrentRoom.roomId == roomId
            && game.CurrentRoom.StepsComplete && (!game.requireGloves || game.GlovesOn);
        public override string Prompt(ClinicGame game)
        {
            if (Completed) return "";
            if (game.HeldItem) return "R · Devolver el objeto que llevas";
            if (game.CurrentRoom && !game.CurrentRoom.StepsComplete) return "Completa primero los pasos de la práctica";
            if (game.requireGloves && !game.GlovesOn) return "Ponte guantes en el dispensador de la sala";
            if (Loose && IsSharp) return "E · Recoger con pinzas: " + displayName;
            return "E · Tomar: " + displayName;
        }
        public override void Interact(ClinicGame game)
        {
            if (CanInteract(game)) game.PickUp(this);
        }

        public void Hold(Transform point)
        {
            Initialize();
            Freeze();
            Held = true;
            foreach (var collider in colliders) if (collider) collider.enabled = false;
            transform.SetParent(point, true);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }

        // Suelta el objeto en el mundo con la velocidad indicada; la gravedad y las colisiones hacen el resto.
        public void Release(Vector3 velocity, Vector3 angularVelocity)
        {
            Initialize();
            Held = false;
            Loose = true;
            transform.SetParent(originalParent, true);
            for (int i = 0; i < colliders.Length; i++) if (colliders[i]) colliders[i].enabled = originalColliderStates[i];
            if (grip) { grip.center = bodyCenter; grip.size = bodySize; }
            Body.isKinematic = false;
            Body.interpolation = RigidbodyInterpolation.Interpolate;
            Body.linearVelocity = velocity;
            Body.angularVelocity = angularVelocity;
            Body.WakeUp();
        }

        public void ReturnToPlace()
        {
            Initialize();
            Freeze();
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

        void Freeze()
        {
            Loose = false;
            if (grip) { grip.center = gripCenter; grip.size = gripSize; }
            if (!Body) return;
            if (!Body.isKinematic)
            {
                Body.linearVelocity = Vector3.zero;
                Body.angularVelocity = Vector3.zero;
            }
            Body.isKinematic = true;
            Body.interpolation = RigidbodyInterpolation.None;
        }
    }
}
