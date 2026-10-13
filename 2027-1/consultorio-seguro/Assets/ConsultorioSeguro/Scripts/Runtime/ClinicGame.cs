using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ConsultorioSeguroNuevo
{
    public enum ClinicScreen { Menu, Playing, Pause, Manual, Credits }

    public sealed class ClinicGame : MonoBehaviour
    {
        public ClinicPlayer player;
        public PracticeRoom[] rooms = Array.Empty<PracticeRoom>();
        [TextArea(5, 30)] public string credits;
        public Shader outlineShader;
        [Min(.5f)] public float interactionRange = 2.6f;
        [Min(.5f)] public float outlineRange = 3.5f;
        public ClinicScreen Screen { get; private set; } = ClinicScreen.Menu;
        public bool Playing => Screen == ClinicScreen.Playing;
        public bool HasStartedVisit { get; private set; }
        public ClinicItem HeldItem { get; private set; }
        public PracticeRoom CurrentRoom { get; private set; }
        public ClinicInteractable Target { get; private set; }
        public int CompletedRooms => rooms.Count(room => room && room.Completed);
        public string Prompt { get; private set; } = "";
        public event Action<ClinicScreen> ScreenChanged;
        ClinicScreen previousScreen;
        ClinicUI ui;
        ClinicOutline[] outlines;

        void Start()
        {
            if (!player) player = FindFirstObjectByType<ClinicPlayer>();
            player.game = this;
            if (rooms == null || rooms.Length == 0)
                rooms = FindObjectsByType<PracticeRoom>(FindObjectsSortMode.None).OrderBy(room => room.roomId).ToArray();
            var items = FindObjectsByType<ClinicItem>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var item in items) item.Initialize();
            foreach (var room in rooms) room.Initialize(items);
            if (!outlineShader) outlineShader = Shader.Find("ConsultorioSeguro/Silueta");
            var interactions = FindObjectsByType<ClinicInteractable>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            outlines = interactions.Where(item => item.showOutline && (item is ClinicItem || item is PracticeStation))
                .Select(item =>
                {
                    var outline = item.GetComponent<ClinicOutline>();
                    if (!outline) outline = item.gameObject.AddComponent<ClinicOutline>();
                    outline.Initialize(this, item, outlineShader);
                    return outline;
                }).ToArray();
            ui = gameObject.AddComponent<ClinicUI>();
            ui.Initialize(this);
            SetScreen(ClinicScreen.Menu);
        }

        void Update()
        {
            var keyboard = Keyboard.current;
            if (keyboard != null && keyboard.escapeKey.wasPressedThisFrame)
            {
                if (Screen == ClinicScreen.Manual || Screen == ClinicScreen.Credits) Back();
                else if (Playing) Pause();
                else if (Screen == ClinicScreen.Pause) Resume();
                return;
            }
            if (keyboard != null && keyboard.mKey.wasPressedThisFrame)
            {
                if (Screen == ClinicScreen.Manual) Back();
                else OpenManual();
                return;
            }
            if (!Playing) return;
            RefreshRoom();
            RefreshTarget();
            if (keyboard == null) return;
            if (keyboard.rKey.wasPressedThisFrame) ReturnHeld();
            if (keyboard.eKey.wasPressedThisFrame && Target && Target.CanInteract(this)) Target.Interact(this);
        }

        void LateUpdate()
        {
            if (outlines != null) foreach (var outline in outlines) if (outline) outline.Refresh();
        }

        public void RefreshRoom()
        {
            PracticeRoom room = rooms.FirstOrDefault(candidate => candidate && candidate.Contains(player.eye.transform.position));
            if (room == CurrentRoom) return;
            CurrentRoom = room;
            if (room) ShowMessage(room.title, room.instructions);
        }

        public void RefreshTarget()
        {
            Target = null;
            if (Playing && Physics.Raycast(player.eye.transform.position, player.eye.transform.forward,
                    out RaycastHit hit, interactionRange, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
                Target = hit.collider.GetComponentInParent<ClinicInteractable>();
            if (Target && Target.roomId != 0 && (!CurrentRoom || Target.roomId != CurrentRoom.roomId)) Target = null;
            Prompt = Target ? Target.Prompt(this) : HeldItem ? "R · Devolver el objeto a su lugar" : "";
        }

        public bool ShouldOutline(ClinicInteractable target)
        {
            if (!Playing || HeldItem || !target || !target.isActiveAndEnabled || !CurrentRoom || !player) return false;
            if (target.roomId != CurrentRoom.roomId || !CurrentRoom.Contains(player.eye.transform.position)) return false;
            return (target.transform.position - player.eye.transform.position).sqrMagnitude <= outlineRange * outlineRange
                && target.CanOutline(this);
        }

        public void PickUp(ClinicItem item)
        {
            if (!item || !item.CanInteract(this)) return;
            HeldItem = item;
            item.Hold(player.HoldPoint);
            Target = null;
            ShowMessage(item.displayName, item.description + "\n\nElige el destino. Pulsa R para devolverlo a su lugar.");
        }

        public void ReturnHeld()
        {
            if (HeldItem) HeldItem.ReturnToPlace();
            HeldItem = null;
        }

        public void Deposit(ClinicDestination destination)
        {
            if (!Playing || !HeldItem || !destination || !destination.CanInteract(this)) return;
            if (destination.roomId != HeldItem.roomId)
            {
                ShowMessage("Mantén el material en su área", "Regresa a la sala de esta práctica o pulsa R para devolver el objeto a su lugar.");
                return;
            }
            var item = HeldItem;
            bool correct = item.correctDestination == destination.destination;
            CurrentRoom.RecordAnswer(correct);
            HeldItem = null;
            if (correct) item.Complete();
            else item.ReturnToPlace();
            string feedback = WasteInformation.Explanation(item.correctDestination);
            if (correct)
            {
                if (CurrentRoom.Completed)
                    feedback += "\n\nPráctica completada. " + CurrentRoom.Score + " puntos. "
                        + CompletedRooms + " de " + rooms.Length + " prácticas terminadas.";
                ShowMessage("Clasificación correcta", item.displayName + "\n" + feedback);
            }
            else ShowMessage("Revisa la clasificación", "Destino elegido: " + WasteInformation.Label(destination.destination)
                + ".\n" + feedback + "\nEl objeto volvió a su lugar. Puedes intentarlo de nuevo.");
        }

        public void StartVisit() { HasStartedVisit = true; Resume(); }
        public void Pause() => SetScreen(ClinicScreen.Pause);
        public void Resume() { HasStartedVisit = true; SetScreen(ClinicScreen.Playing); }
        public void MainMenu()
        {
            ReturnHeld();
            SetScreen(ClinicScreen.Menu);
        }
        public void OpenManual()
        {
            previousScreen = Screen == ClinicScreen.Menu ? ClinicScreen.Menu : ClinicScreen.Pause;
            SetScreen(ClinicScreen.Manual);
        }
        public void OpenCredits()
        {
            previousScreen = Screen == ClinicScreen.Menu ? ClinicScreen.Menu : ClinicScreen.Pause;
            SetScreen(ClinicScreen.Credits);
        }
        public void Back() => SetScreen(previousScreen);
        public void RestartCurrentRoom()
        {
            if (!CurrentRoom) return;
            if (HeldItem && HeldItem.roomId == CurrentRoom.roomId) ReturnHeld();
            CurrentRoom.ResetPractice();
            Resume();
            ShowMessage(CurrentRoom.title, "Práctica reiniciada.\n" + CurrentRoom.instructions);
        }
        public void ShowMessage(string title, string body)
        {
            if (ui) ui.ShowMessage(title, body);
        }
        void SetScreen(ClinicScreen screen)
        {
            Screen = screen;
            Target = null;
            Prompt = "";
            Time.timeScale = Playing ? 1 : 0;
            Cursor.lockState = Playing ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !Playing;
            ScreenChanged?.Invoke(screen);
            if (ui) ui.ShowScreen(screen);
            if (outlines != null) foreach (var outline in outlines) if (outline) outline.Refresh();
        }
        void OnDestroy()
        {
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
