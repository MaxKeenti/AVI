using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ConsultorioSeguroNuevo
{
    /// <summary>
    /// Efectos de manipulación de un recipiente: la tapa se abre al apuntarle con un material en mano,
    /// el material describe una trayectoria balística hasta la boca, el recipiente se sacude al recibirlo
    /// y rechaza visiblemente una clasificación incorrecta. La lógica de puntuación no cambia.
    /// </summary>
    public sealed class BinFeedback : MonoBehaviour
    {
        public const string LidController = "Tapa", ModelName = "Modelo de recipiente";
        public const float FlightTime = .42f;
        ClinicGame game;
        ClinicDestination destination;
        public AnimatedPart Lid { get; private set; }
        /// <summary>Los contenedores de punzocortantes tienen una abertura fija: no se abren.</summary>
        public bool FixedOpening { get; private set; }
        public bool LidOpen { get; private set; }
        /// <summary>Eje del pedal; se hunde mientras la tapa está abierta.</summary>
        public Transform Pedal { get; private set; }
        float pedalAngle;
        float holdOpenUntil;
        Coroutine shaking;
        Quaternion restRotation;

        public void Initialize(ClinicGame owner, ClinicDestination target)
        {
            game = owner;
            destination = target;
            restRotation = transform.localRotation;
            var slot = transform.Cast<Transform>().FirstOrDefault(child => child.name == "Abertura segura");
            FixedOpening = slot;
            if (FixedOpening) return;
            var model = transform.Find(ModelName);
            if (model)
            {
                // Bote de pedal real: la tapa gira sobre su propio nodo de bisagra y el pedal se hunde al abrir.
                var lidNode = model.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "lid");
                if (lidNode) Lid = AnimatedPart.Wrap("Tapa de " + name, LidController, lidNode.position, transform.rotation, lidNode);
                var pedal = model.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "pedal-waste-bin_0");
                if (pedal)
                {
                    Bounds bounds = LocalBounds(pedal);
                    Vector3 back = transform.TransformPoint(new Vector3(bounds.center.x, bounds.center.y, bounds.max.z));
                    Pedal = AnimatedPart.Nest(transform, "Pedal articulado", back, transform.rotation, pedal);
                }
                return;
            }
            var lid = transform.Cast<Transform>().FirstOrDefault(child => child.name == "Tapa");
            if (!lid) return;
            // Bisagra en el borde posterior superior de la tapa (el rótulo marca el frente, en -Z).
            Bounds local = LocalBounds(lid);
            Vector3 hinge = transform.TransformPoint(new Vector3(local.center.x, local.max.y, local.max.z));
            Lid = AnimatedPart.Wrap("Tapa de " + name, LidController, hinge, transform.rotation, lid);
        }

        Bounds LocalBounds(Transform part)
        {
            var filters = part.GetComponentsInChildren<MeshFilter>();
            Bounds result = new Bounds(transform.InverseTransformPoint(part.position), Vector3.zero);
            foreach (var filter in filters)
            {
                if (!filter.sharedMesh) continue;
                var mesh = filter.sharedMesh.bounds;
                for (int corner = 0; corner < 8; corner++)
                {
                    Vector3 point = mesh.center + Vector3.Scale(mesh.extents,
                        new Vector3((corner & 1) == 0 ? -1 : 1, (corner & 2) == 0 ? -1 : 1, (corner & 4) == 0 ? -1 : 1));
                    result.Encapsulate(transform.InverseTransformPoint(filter.transform.TransformPoint(point)));
                }
            }
            return result;
        }

        /// <summary>Punto de la boca del recipiente en coordenadas del mundo.</summary>
        public Vector3 Mouth
        {
            get
            {
                var renderers = GetComponentsInChildren<Renderer>().Where(r => r.enabled && !(r is ParticleSystemRenderer)).ToArray();
                if (renderers.Length == 0) return transform.position + Vector3.up * .7f;
                Bounds bounds = renderers[0].bounds;
                foreach (var renderer in renderers.Skip(1)) bounds.Encapsulate(renderer.bounds);
                // Con la tapa abierta, sus límites crecen hacia arriba: la boca es la parte superior del cuerpo.
                float top = Lid && Lid.Axis ? Mathf.Min(bounds.max.y, Lid.transform.position.y) : bounds.max.y;
                return new Vector3(bounds.center.x, top + .02f, bounds.center.z);
            }
        }

        void Update()
        {
            if (!game) return;
            if (Pedal)
            {
                // El pedal se pisa antes de que la tapa termine de subir (frente en -Z: giro negativo).
                pedalAngle = Mathf.MoveTowards(pedalAngle, LidOpen ? -14 : 0, 140 * Time.deltaTime);
                Pedal.localRotation = Quaternion.Euler(pedalAngle, 0, 0);
            }
            bool aimed = game.Playing && game.HeldItem && game.Target == destination;
            bool open = aimed || Time.time < holdOpenUntil;
            if (open != LidOpen)
            {
                LidOpen = open;
                if (Lid)
                {
                    Lid.SetBool("Abierta", open);
                    SimulationFx.Play("Tapa", transform.position + Vector3.up * .6f, .35f);
                }
            }
        }

        public void OnDeposited(ClinicItem item, bool correct, Vector3 handPosition, Quaternion handRotation)
        {
            Vector3 mouth = Mouth;
            if (correct)
            {
                holdOpenUntil = Time.time + FlightTime + .45f;
                var proxy = DepositProxy.Launch(item, handPosition, handRotation, mouth, FlightTime, true);
                StartCoroutine(AfterArrival(proxy, mouth, true));
            }
            else
            {
                // El recipiente permanece cerrado y devuelve el material: rebote y nube roja.
                holdOpenUntil = 0;
                var proxy = DepositProxy.Launch(item, handPosition, handRotation, mouth + Vector3.up * .08f, FlightTime * .8f, false);
                StartCoroutine(HideOriginalWhileBouncing(item, FlightTime * .8f + .45f));
                StartCoroutine(AfterArrival(proxy, mouth, false));
            }
        }

        IEnumerator AfterArrival(DepositProxy proxy, Vector3 mouth, bool correct)
        {
            yield return new WaitForSeconds(correct ? FlightTime : FlightTime * .8f);
            if (correct)
            {
                SimulationFx.Success(mouth);
                SimulationFx.Play("Acierto", mouth, .55f);
            }
            else
            {
                SimulationFx.Rejection(mouth + Vector3.up * .06f);
                SimulationFx.Play("Error", mouth, .5f);
            }
            SimulationFx.Play("Golpe", mouth, .5f);
            Shake(correct ? 4f : 7f);
        }

        // Oculta el original devuelto a su lugar mientras la copia rebota, para no ver dos objetos.
        IEnumerator HideOriginalWhileBouncing(ClinicItem item, float seconds)
        {
            var hidden = new List<Renderer>();
            foreach (var renderer in item.GetComponentsInChildren<Renderer>())
                if (renderer.enabled && renderer.gameObject.name != "Silueta de interacción") { renderer.enabled = false; hidden.Add(renderer); }
            float until = Time.time + seconds;
            while (Time.time < until && item && !item.Held) yield return null;
            foreach (var renderer in hidden) if (renderer) renderer.enabled = true;
        }

        public void Shake(float degrees)
        {
            if (shaking != null) StopCoroutine(shaking);
            shaking = StartCoroutine(ShakeRoutine(degrees));
        }

        IEnumerator ShakeRoutine(float degrees)
        {
            const float duration = .45f;
            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                float damping = 1 - t / duration;
                float wobble = Mathf.Sin(t * 48) * degrees * damping * damping;
                transform.localRotation = restRotation * Quaternion.Euler(wobble * .35f, 0, wobble);
                yield return null;
            }
            transform.localRotation = restRotation;
            shaking = null;
        }

        public void ResetFeedback()
        {
            holdOpenUntil = 0;
            LidOpen = false;
            pedalAngle = 0;
            if (Pedal) Pedal.localRotation = Quaternion.identity;
            if (Lid) Lid.ResetPose();
            if (shaking != null) StopCoroutine(shaking);
            shaking = null;
            transform.localRotation = restRotation;
        }

        void OnDisable() => ResetFeedback();
    }

    /// <summary>
    /// Copia visual del material que viaja de la mano al recipiente bajo la gravedad de PhysX.
    /// No tiene colisionador ni componentes de juego, así que no altera la puntuación.
    /// </summary>
    public sealed class DepositProxy : MonoBehaviour
    {
        Rigidbody body;
        float arrival, born;
        bool accepted, landed;
        Vector3 target, startScale;

        public static DepositProxy Launch(ClinicItem item, Vector3 from, Quaternion rotation, Vector3 to, float flightTime, bool accepted)
        {
            var root = new GameObject("Trayectoria · " + item.displayName);
            root.transform.SetPositionAndRotation(from, rotation);
            root.transform.localScale = item.transform.lossyScale;
            foreach (var source in item.GetComponentsInChildren<MeshRenderer>(true))
            {
                var filter = source.GetComponent<MeshFilter>();
                if (!filter || !filter.sharedMesh || source.gameObject.name == "Silueta de interacción") continue;
                var copy = new GameObject(source.name, typeof(MeshFilter), typeof(MeshRenderer));
                copy.transform.SetParent(root.transform, false);
                copy.transform.localPosition = item.transform.InverseTransformPoint(source.transform.position);
                copy.transform.localRotation = Quaternion.Inverse(item.transform.rotation) * source.transform.rotation;
                Vector3 itemScale = item.transform.lossyScale, sourceScale = source.transform.lossyScale;
                copy.transform.localScale = new Vector3(sourceScale.x / Mathf.Max(itemScale.x, 1e-5f),
                    sourceScale.y / Mathf.Max(itemScale.y, 1e-5f), sourceScale.z / Mathf.Max(itemScale.z, 1e-5f));
                copy.GetComponent<MeshFilter>().sharedMesh = filter.sharedMesh;
                copy.GetComponent<MeshRenderer>().sharedMaterials = source.sharedMaterials;
            }
            var proxy = root.AddComponent<DepositProxy>();
            proxy.body = root.AddComponent<Rigidbody>();
            proxy.body.useGravity = true;
            proxy.body.interpolation = RigidbodyInterpolation.Interpolate;
            // Velocidad inicial de un tiro parabólico: p(T) = p0 + v·T + ½·g·T² llega a la boca en T.
            proxy.body.linearVelocity = (to - from) / flightTime - .5f * Physics.gravity * flightTime;
            proxy.body.angularVelocity = new Vector3(Random.Range(-4f, 4f), Random.Range(-2f, 2f), Random.Range(-4f, 4f));
            proxy.target = to;
            proxy.accepted = accepted;
            proxy.born = Time.time;
            proxy.arrival = Time.time + flightTime;
            proxy.startScale = root.transform.localScale;
            return proxy;
        }

        void FixedUpdate()
        {
            if (landed || Time.time < arrival) return;
            landed = true;
            if (!accepted)
            {
                // Rebote: invierte la componente vertical y lo aleja de la boca.
                Vector3 velocity = body.linearVelocity;
                Vector3 away = transform.position - target;
                away.y = 0;
                body.linearVelocity = new Vector3(0, Mathf.Abs(velocity.y) * .45f + 1f, 0) + away.normalized * .6f;
            }
        }

        void Update()
        {
            if (!landed) return;
            float t = Time.time - arrival;
            // Al entrar se reduce dentro de la boca; si se rechaza, se desvanece tras el rebote.
            float fade = accepted ? .14f : .42f;
            transform.localScale = startScale * Mathf.Clamp01(1 - t / fade);
            if (t >= fade || Time.time - born > 3) Destroy(gameObject);
        }
    }
}
