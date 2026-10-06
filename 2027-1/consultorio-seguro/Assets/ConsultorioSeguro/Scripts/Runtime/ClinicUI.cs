using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace ConsultorioSeguroNuevo
{
    // Los menús se crean al jugar; el consultorio y sus rótulos permanecen editables en la escena.
    public sealed class ClinicUI : MonoBehaviour
    {
        public const string ManualText =
            "<b>RECORRIDO</b>\nW, A, S, D o flechas: caminar.\nRatón: mirar alrededor. Mayús: caminar más rápido.\nEsc: pausar; vuelve a pulsarlo para reanudar.\nM: abrir el manual de controles.\n\n" +
            "<b>INTERACCIÓN</b>\nAcércate a menos de 2,6 metros y apunta con el punto central. Pulsa E cuando aparezca la indicación. Los instrumentos cercanos de la sala actual muestran una silueta blanca; el objeto apuntado cambia a dorado. El color solo indica dónde puedes interactuar.\n\n" +
            "<b>LAS CINCO PRÁCTICAS</b>\n1. Clasificación de residuos.\n2. Reprocesamiento de instrumentos.\n3. Manejo de materiales.\n4. Procedimiento odontológico.\n5. Radiografía digital.\nPuedes visitarlas en cualquier orden. En cada sala, lee el objetivo de la esquina superior izquierda y realiza sus pasos. Después clasifica sus residuos.\n\n" +
            "<b>TOMAR, CLASIFICAR Y DEVOLVER</b>\nApunta al material y pulsa E para tomarlo. Lee las condiciones del caso. Apunta al recipiente elegido y pulsa E para depositarlo. Pulsa R en cualquier momento del recorrido para devolver el objeto a su lugar. La ayuda de contorno se oculta mientras llevas un objeto.\n\n" +
            "<b>RESULTADOS Y REPETICIÓN</b>\nCada acierto suma 10 puntos y cada error resta 5, sin puntuaciones negativas. Si te equivocas, lee la explicación: el objeto vuelve a su lugar para intentarlo de nuevo. Al terminar, puedes repetir con Esc → Reiniciar esta práctica.\n\n" +
            "<b>MANUAL SIEMPRE DISPONIBLE</b>\nPulsa M para abrir esta guía durante el recorrido. También está disponible desde el inicio, desde la pausa o apuntando al panel de controles de cada sala y pulsando E. Usa Volver para regresar y Reanudar para seguir explorando.\n\n" +
            "<b>ALCANCE EDUCATIVO</b>\nLos casos describen condiciones concretas. Este recorrido no sustituye los procedimientos institucionales ni representa una certificación clínica. Los tiempos de equipo se abrevian en la simulación; sigue los ciclos validados y las instrucciones del fabricante en la práctica real.";

        ClinicGame game;
        Font font;
        RectTransform root;
        GameObject hud, overlay, mainPanel, pausePanel, manualPanel, creditsPanel, toast;
        Text roomTitle, objective, prompt, holding, toastTitle, toastBody, progress;
        Button resetButton;
        float messageUntil;
        int layoutWidth, layoutHeight;
        readonly Color ink = new Color(.15f, .21f, .19f);
        readonly Color sage = new Color(.25f, .39f, .33f);
        readonly Color paper = new Color(.96f, .96f, .92f, .98f);

        public void Initialize(ClinicGame owner)
        {
            game = owner;
            font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            var canvasObject = new GameObject("Interfaz de Consultorio Seguro", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvasObject.transform.SetParent(transform, false);
            root = canvasObject.GetComponent<RectTransform>();
            canvasObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1600, 1000);
            scaler.matchWidthOrHeight = .5f;
            if (!FindFirstObjectByType<EventSystem>())
                new GameObject("Navegación de menús", typeof(EventSystem), typeof(InputSystemUIInputModule)).transform.SetParent(transform, false);
            BuildHud();
            var shade = Panel("Fondo de menús", root, new Color(.06f, .12f, .10f, .78f));
            Stretch(shade);
            overlay = shade.gameObject;
            mainPanel = BuildMain(shade).gameObject;
            pausePanel = BuildPause(shade).gameObject;
            manualPanel = BuildReadingPanel(shade, "Manual de controles", ManualText).gameObject;
            var sourceCredits = Resources.Load<TextAsset>("Creditos");
            string credits = sourceCredits ? sourceCredits.text : game.credits;
            if (string.IsNullOrWhiteSpace(credits)) credits = "Consultorio Seguro\nSimulador educativo. Consulta el registro de fuentes incluido con el proyecto.";
            creditsPanel = BuildReadingPanel(shade, "Créditos y fuentes", credits).gameObject;
        }

        RectTransform Card(Transform parent, string name, Vector2 size)
        {
            var panel = Panel(name, parent, paper);
            Position(panel, new Vector2(.5f, .5f), Vector2.zero, size);
            return panel;
        }

        RectTransform BuildMain(Transform parent)
        {
            var card = Card(parent, "Inicio", new Vector2(820, 710));
            var label = MakeText(card, "Etiqueta", "APRENDER · EXPLORAR · CUIDAR", 16, sage, TextAnchor.MiddleCenter);
            Position(label.rectTransform, new Vector2(.5f, 1), new Vector2(0, -55), new Vector2(740, 35));
            var title = MakeText(card, "Título", "Consultorio Seguro", 48, ink, TextAnchor.MiddleCenter);
            Position(title.rectTransform, new Vector2(.5f, 1), new Vector2(0, -114), new Vector2(760, 80));
            var subtitle = MakeText(card, "Introducción", "Un consultorio para practicar el manejo de residuos\nbiológico-infecciosos en cinco actividades.", 22, ink, TextAnchor.MiddleCenter);
            Position(subtitle.rectTransform, new Vector2(.5f, 1), new Vector2(0, -199), new Vector2(730, 78));
            MakeButton(card, "Entrar al consultorio", new Vector2(0, 42), game.StartVisit);
            MakeButton(card, "Manual de controles", new Vector2(0, -31), game.OpenManual);
            MakeButton(card, "Créditos y fuentes", new Vector2(0, -104), game.OpenCredits);
            var footer = MakeText(card, "Nota educativa", "Recorre la clínica con WASD y el ratón.\nE para interactuar · Esc para pausar\n\nSimulación educativa · Casos con condiciones específicas", 18, ink, TextAnchor.MiddleCenter);
            Position(footer.rectTransform, new Vector2(.5f, 0), new Vector2(0, 104), new Vector2(730, 120));
            return card;
        }

        RectTransform BuildPause(Transform parent)
        {
            var card = Card(parent, "Pausa", new Vector2(800, 700));
            var title = MakeText(card, "Título", "Recorrido en pausa", 38, ink, TextAnchor.MiddleCenter);
            Position(title.rectTransform, new Vector2(.5f, 1), new Vector2(0, -73), new Vector2(740, 65));
            progress = MakeText(card, "Avance general", "", 21, ink, TextAnchor.MiddleCenter);
            Position(progress.rectTransform, new Vector2(.5f, 1), new Vector2(0, -143), new Vector2(700, 58));
            MakeButton(card, "Reanudar", new Vector2(0, 83), game.Resume);
            MakeButton(card, "Manual de controles", new Vector2(0, 13), game.OpenManual);
            resetButton = MakeButton(card, "Reiniciar esta práctica", new Vector2(0, -57), game.RestartCurrentRoom);
            MakeButton(card, "Créditos y fuentes", new Vector2(0, -127), game.OpenCredits);
            MakeButton(card, "Volver al inicio", new Vector2(0, -197), game.MainMenu);
            var help = MakeText(card, "Ayuda", "Esc · Reanudar", 17, ink, TextAnchor.MiddleCenter);
            Position(help.rectTransform, new Vector2(.5f, 0), new Vector2(0, 55), new Vector2(660, 35));
            return card;
        }

        RectTransform BuildReadingPanel(Transform parent, string title, string body)
        {
            var panel = Card(parent, title, new Vector2(1120, 900));
            var heading = MakeText(panel, "Título", title, 36, ink, TextAnchor.MiddleLeft);
            Position(heading.rectTransform, new Vector2(0, 1), new Vector2(540, -67), new Vector2(990, 64));
            var scroll = new GameObject("Lectura desplazable", typeof(RectTransform), typeof(ScrollRect)).GetComponent<RectTransform>();
            scroll.SetParent(panel, false);
            scroll.anchorMin = new Vector2(.055f, .165f); scroll.anchorMax = new Vector2(.945f, .84f);
            scroll.offsetMin = scroll.offsetMax = Vector2.zero;
            var viewport = new GameObject("Ventana", typeof(RectTransform), typeof(Image), typeof(RectMask2D)).GetComponent<RectTransform>();
            viewport.SetParent(scroll, false); Stretch(viewport);
            // Una superficie gráfica permite recibir la rueda sobre toda el área de lectura.
            viewport.GetComponent<Image>().color = Color.clear;
            var content = new GameObject("Contenido", typeof(RectTransform)).GetComponent<RectTransform>();
            content.SetParent(viewport, false);
            content.anchorMin = new Vector2(0, 1); content.anchorMax = Vector2.one;
            content.pivot = new Vector2(.5f, 1); content.offsetMin = content.offsetMax = Vector2.zero;
            // Divide las fuentes extensas para respetar el límite de vértices de cada texto UI.
            // Los fragmentos conservan todo el contenido original dentro del mismo desplazamiento.
            float width = 1120 * .89f - 18;
            float offset = 8;
            int section = 0;
            foreach (string value in ReadingSections(body))
            {
                var text = MakeText(content, "Texto " + (++section), value, 22, ink, TextAnchor.UpperLeft);
                text.horizontalOverflow = HorizontalWrapMode.Wrap;
                text.verticalOverflow = VerticalWrapMode.Overflow;
                var settings = text.GetGenerationSettings(new Vector2(width, 0));
                float height = text.cachedTextGeneratorForLayout.GetPreferredHeight(value, settings) / text.pixelsPerUnit + 8;
                text.rectTransform.anchorMin = new Vector2(0, 1);
                text.rectTransform.anchorMax = Vector2.one;
                text.rectTransform.pivot = new Vector2(.5f, 1);
                text.rectTransform.sizeDelta = new Vector2(-18, height);
                text.rectTransform.anchoredPosition = new Vector2(-9, -offset);
                offset += height;
            }
            content.sizeDelta = new Vector2(0, Mathf.Max(620, offset + 20));
            var view = scroll.GetComponent<ScrollRect>();
            view.content = content; view.viewport = viewport; view.horizontal = false; view.vertical = true;
            view.movementType = ScrollRect.MovementType.Clamped;
            view.scrollSensitivity = 36;
            view.verticalNormalizedPosition = 1;
            MakeButton(panel, "← Volver", new Vector2(-246, -379), game.Back, 400);
            var resume = MakeButton(panel, "Reanudar", new Vector2(246, -379), game.Resume, 400);
            resume.gameObject.name = "Reanudar desde ayuda";
            var note = MakeText(panel, "Desplazamiento", "Rueda del ratón para desplazarte · Esc para volver", 16, ink, TextAnchor.MiddleCenter);
            Position(note.rectTransform, new Vector2(.5f, 0), new Vector2(0, 22), new Vector2(990, 30));
            return panel;
        }

        static IEnumerable<string> ReadingSections(string body)
        {
            body ??= string.Empty;
            const int limit = 6000;
            int start = 0;
            while (start < body.Length)
            {
                int length = Mathf.Min(limit, body.Length - start);
                if (start + length < body.Length)
                {
                    int newline = body.LastIndexOf('\n', start + length - 1, length);
                    if (newline >= start) length = newline - start + 1;
                }
                yield return body.Substring(start, length);
                start += length;
            }
        }

        void BuildHud()
        {
            var hudRoot = new GameObject("Guía de recorrido", typeof(RectTransform)).GetComponent<RectTransform>();
            hudRoot.SetParent(root, false); Stretch(hudRoot); hud = hudRoot.gameObject;
            var status = Panel("Práctica actual", hudRoot, new Color(.06f, .14f, .11f, .88f));
            Position(status, new Vector2(0, 1), new Vector2(268, -112), new Vector2(488, 178));
            roomTitle = MakeText(status, "Sala", "Bienvenido al consultorio", 23, Color.white, TextAnchor.UpperLeft);
            Position(roomTitle.rectTransform, new Vector2(0, 1), new Vector2(245, -40), new Vector2(450, 53));
            objective = MakeText(status, "Objetivo", "", 19, Color.white, TextAnchor.UpperLeft);
            Position(objective.rectTransform, new Vector2(0, 1), new Vector2(245, -115), new Vector2(450, 94));
            var dot = MakeText(hudRoot, "Punto de mira", "·", 46, Color.white, TextAnchor.MiddleCenter);
            Position(dot.rectTransform, new Vector2(.5f, .5f), Vector2.zero, new Vector2(32, 48));
            prompt = MakeText(hudRoot, "Interacción", "", 25, Color.white, TextAnchor.MiddleCenter);
            Position(prompt.rectTransform, new Vector2(.5f, 0), new Vector2(0, 111), new Vector2(1280, 84));
            AddShadow(prompt);
            holding = MakeText(hudRoot, "Objeto en mano", "", 18, Color.white, TextAnchor.MiddleCenter);
            Position(holding.rectTransform, new Vector2(.5f, 0), new Vector2(0, 49), new Vector2(1220, 38));
            AddShadow(holding);
            var toastPanel = Panel("Explicación", hudRoot, paper);
            Position(toastPanel, new Vector2(1, 1), new Vector2(-282, -213), new Vector2(516, 378));
            toast = toastPanel.gameObject;
            toastTitle = MakeText(toastPanel, "Título", "", 23, sage, TextAnchor.UpperLeft);
            Position(toastTitle.rectTransform, new Vector2(.5f, 1), new Vector2(0, -46), new Vector2(470, 58));
            toastBody = MakeText(toastPanel, "Explicación", "", 19, ink, TextAnchor.UpperLeft);
            Position(toastBody.rectTransform, new Vector2(.5f, 1), new Vector2(0, -222), new Vector2(470, 270));
            toastBody.resizeTextForBestFit = true; toastBody.resizeTextMinSize = 16; toastBody.resizeTextMaxSize = 19;
            toast.SetActive(false);
        }

        public void ShowScreen(ClinicScreen screen)
        {
            bool playing = screen == ClinicScreen.Playing;
            hud.SetActive(playing); overlay.SetActive(!playing);
            mainPanel.SetActive(screen == ClinicScreen.Menu);
            pausePanel.SetActive(screen == ClinicScreen.Pause);
            manualPanel.SetActive(screen == ClinicScreen.Manual);
            creditsPanel.SetActive(screen == ClinicScreen.Credits);
            resetButton.interactable = game.CurrentRoom;
            progress.text = game.CompletedRooms + " de " + game.rooms.Length + " prácticas completadas";
            if (screen == ClinicScreen.Menu) toast.SetActive(false);
            // Desde el inicio, Volver es la única salida; Reanudar aparece al estar dentro del recorrido.
            foreach (var panel in new[] { manualPanel, creditsPanel })
            {
                var resume = panel.transform.Find("Reanudar desde ayuda");
                if (resume) resume.gameObject.SetActive(game.HasStartedVisit);
            }
            EventSystem.current?.SetSelectedGameObject(null);
        }

        public void ShowMessage(string title, string body)
        {
            body ??= string.Empty;
            toastTitle.text = title ?? string.Empty;
            toastBody.text = body;
            toast.SetActive(true);
            messageUntil = Time.time + Mathf.Clamp(6 + body.Length / 28f, 9, 26);
        }

        void Update()
        {
            // Regenera el atlas del texto cuando CanvasScaler ya conoce la resolución real.
            if (root && (layoutWidth != Screen.width || layoutHeight != Screen.height))
            {
                layoutWidth = Screen.width; layoutHeight = Screen.height;
                Canvas.ForceUpdateCanvases();
                foreach (var label in root.GetComponentsInChildren<Text>(true)) label.SetAllDirty();
                Canvas.ForceUpdateCanvases();
            }
            if (!game || !game.Playing) return;
            var room = game.CurrentRoom;
            roomTitle.text = room ? room.title : "Consultorio Seguro";
            if (!room) objective.text = "Visita recepción y explora las cinco salas.\nEsc · Pausa y manual de controles";
            else if (room.Completed) objective.text = "Práctica completada · " + room.Score + " puntos\nEsc · Reiniciar esta práctica";
            else if (room.Busy) objective.text = "Proceso simulado en curso…\nAciertos: " + room.Correct + " · Errores: " + room.Mistakes;
            else if (!room.StepsComplete) objective.text = "Paso " + (room.NextStep + 1) + " de " + room.steps.Length + ": " + room.CurrentStep.actionText;
            else objective.text = "Materiales por clasificar: " + room.Remaining + "\nAciertos: " + room.Correct + " · Errores: " + room.Mistakes;
            prompt.text = game.Prompt;
            holding.text = game.HeldItem ? "En mano: " + game.HeldItem.displayName + " · R para devolver" : "E · Interactuar     M · Manual     Esc · Pausa";
            if (toast.activeSelf && Time.time > messageUntil) toast.SetActive(false);
        }

        RectTransform Panel(string name, Transform parent, Color color)
        {
            var panel = new GameObject(name, typeof(RectTransform), typeof(Image)).GetComponent<RectTransform>();
            panel.SetParent(parent, false); panel.GetComponent<Image>().color = color;
            return panel;
        }
        Text MakeText(Transform parent, string name, string value, int size, Color color, TextAnchor alignment)
        {
            var text = new GameObject(name, typeof(RectTransform), typeof(Text)).GetComponent<Text>();
            text.transform.SetParent(parent, false);
            text.font = font; text.fontSize = size; text.color = color; text.text = value;
            text.alignment = alignment; text.supportRichText = true; text.raycastTarget = false;
            text.horizontalOverflow = HorizontalWrapMode.Wrap; text.verticalOverflow = VerticalWrapMode.Truncate;
            return text;
        }
        Button MakeButton(Transform parent, string label, Vector2 position, UnityEngine.Events.UnityAction action, float width = 650)
        {
            var panel = Panel(label, parent, sage);
            Position(panel, new Vector2(.5f, .5f), position, new Vector2(width, 55));
            var button = panel.gameObject.AddComponent<Button>();
            var colors = button.colors;
            colors.highlightedColor = new Color(.78f, .87f, .72f);
            colors.pressedColor = new Color(.68f, .79f, .6f);
            colors.disabledColor = new Color(.45f, .45f, .45f, .45f);
            button.colors = colors;
            button.onClick.AddListener(action);
            var text = MakeText(panel, "Texto del botón", label, 23, Color.white, TextAnchor.MiddleCenter);
            Stretch(text.rectTransform);
            return button;
        }
        static void AddShadow(Text text)
        {
            var shadow = text.gameObject.AddComponent<Shadow>();
            shadow.effectColor = new Color(0, 0, 0, .9f); shadow.effectDistance = new Vector2(1.5f, -1.5f);
        }
        static void Position(RectTransform rect, Vector2 anchor, Vector2 position, Vector2 size)
        {
            rect.anchorMin = rect.anchorMax = anchor; rect.pivot = new Vector2(.5f, .5f);
            rect.anchoredPosition = position; rect.sizeDelta = size;
        }
        static void Stretch(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one;
            rect.offsetMin = rect.offsetMax = Vector2.zero;
        }
    }
}
