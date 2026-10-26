using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Los menús se crean al jugar con el mismo estilo que Consultorio Seguro: tarjeta clara,
// botones verde salvia y un velo verde oscuro sobre la fotografía del consultorio.
public class MainMenu : MonoBehaviour
{
    public const string CreditsText =
        "<b>PROYECTO</b>\nConsultorio Seguro RA: mobiliario y equipo de un consultorio sobre marcadores impresos, con realidad aumentada de Vuforia Engine.\n\n" +
        "<b>CURSO</b>\nSimuladores Virtuales · 7NM78\nEquipo 7 · Los Capitos 3D\nProfesora: Bustamante Tranquilino Rocío\n\n" +
        "<b>INTEGRANTES</b>\n• 2023601658 · Alatorre Fuentes Eduardo\n• 2022601088 · Cruz Cruz Guillermo\n• 2022602815 · Flores Roa Jorge Alejandro\n• 2021601769 · Gonzalez Calzada Maximiliano\n• 2024600004 · Neri Mondragón Mónica\n• 2024601122 · Soto Soto Héctor";

    Font font;
    RectTransform root;
    GameObject mainPanel, creditsPanel;
    int layoutWidth, layoutHeight;
    readonly Color ink = new Color(.15f, .21f, .19f);
    readonly Color sage = new Color(.25f, .39f, .33f);
    readonly Color paper = new Color(.96f, .96f, .92f, .98f);

    void Awake()
    {
        font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        var canvasObject = new GameObject("Interfaz de Consultorio Seguro RA", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvasObject.transform.SetParent(transform, false);
        root = canvasObject.GetComponent<RectTransform>();
        canvasObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1600, 1000);
        scaler.matchWidthOrHeight = .5f;
        if (!FindFirstObjectByType<EventSystem>())
            new GameObject("Navegación de menús", typeof(EventSystem), typeof(InputSystemUIInputModule)).transform.SetParent(transform, false);
        BuildBackdrop();
        var shade = Panel("Fondo de menús", root, new Color(.06f, .12f, .10f, .78f));
        Stretch(shade);
        mainPanel = BuildMain(shade).gameObject;
        creditsPanel = BuildReadingPanel(shade, "Créditos", CreditsText).gameObject;
        OpenMainMenuPanel();
    }

    public void OpenOptionsPanel()
    {
        mainPanel.SetActive(false);
        creditsPanel.SetActive(true);
        EventSystem.current?.SetSelectedGameObject(null);
    }

    public void OpenMainMenuPanel()
    {
        creditsPanel.SetActive(false);
        mainPanel.SetActive(true);
        EventSystem.current?.SetSelectedGameObject(null);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        SceneManager.LoadScene("ARCamera");
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
        // Esc o el botón Atrás de Android regresan desde los créditos.
        if (creditsPanel.activeSelf && Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            OpenMainMenuPanel();
    }

    void BuildBackdrop()
    {
        var photo = Resources.Load<Texture2D>("fondonew");
        if (!photo) return;
        var image = new GameObject("Fotografía del consultorio", typeof(RectTransform), typeof(RawImage), typeof(AspectRatioFitter));
        image.transform.SetParent(root, false);
        image.GetComponent<RawImage>().texture = photo;
        image.GetComponent<RawImage>().raycastTarget = false;
        var fitter = image.GetComponent<AspectRatioFitter>();
        fitter.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
        fitter.aspectRatio = (float)photo.width / photo.height;
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
        var label = MakeText(card, "Etiqueta", "ESCANEAR · EXPLORAR · APRENDER", 16, sage, TextAnchor.MiddleCenter);
        Position(label.rectTransform, new Vector2(.5f, 1), new Vector2(0, -55), new Vector2(740, 35));
        var title = MakeText(card, "Título", "Consultorio Seguro RA", 48, ink, TextAnchor.MiddleCenter);
        Position(title.rectTransform, new Vector2(.5f, 1), new Vector2(0, -114), new Vector2(760, 80));
        var subtitle = MakeText(card, "Introducción", "Mobiliario y equipo de un consultorio\nsobre marcadores impresos, en realidad aumentada.", 22, ink, TextAnchor.MiddleCenter);
        Position(subtitle.rectTransform, new Vector2(.5f, 1), new Vector2(0, -199), new Vector2(730, 78));
        MakeButton(card, "Iniciar cámara", new Vector2(0, 42), StartGame);
        MakeButton(card, "Créditos", new Vector2(0, -31), OpenOptionsPanel);
        MakeButton(card, "Salir", new Vector2(0, -104), QuitGame);
        var footer = MakeText(card, "Nota", "Apunta la cámara a un marcador para ver su modelo.\n\nSimuladores Virtuales · 7NM78 · Equipo 7 · Los Capitos 3D", 18, ink, TextAnchor.MiddleCenter);
        Position(footer.rectTransform, new Vector2(.5f, 0), new Vector2(0, 104), new Vector2(730, 120));
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
        // Una superficie gráfica permite recibir la rueda o el arrastre sobre toda el área de lectura.
        viewport.GetComponent<Image>().color = Color.clear;
        var content = new GameObject("Contenido", typeof(RectTransform)).GetComponent<RectTransform>();
        content.SetParent(viewport, false);
        content.anchorMin = new Vector2(0, 1); content.anchorMax = Vector2.one;
        content.pivot = new Vector2(.5f, 1); content.offsetMin = content.offsetMax = Vector2.zero;
        var text = MakeText(content, "Texto", body, 22, ink, TextAnchor.UpperLeft);
        text.verticalOverflow = VerticalWrapMode.Overflow;
        float width = 1120 * .89f - 18;
        var settings = text.GetGenerationSettings(new Vector2(width, 0));
        float height = text.cachedTextGeneratorForLayout.GetPreferredHeight(body, settings) / text.pixelsPerUnit + 8;
        text.rectTransform.anchorMin = new Vector2(0, 1);
        text.rectTransform.anchorMax = Vector2.one;
        text.rectTransform.pivot = new Vector2(.5f, 1);
        text.rectTransform.sizeDelta = new Vector2(-18, height);
        text.rectTransform.anchoredPosition = new Vector2(-9, -8);
        content.sizeDelta = new Vector2(0, Mathf.Max(620, height + 28));
        var view = scroll.GetComponent<ScrollRect>();
        view.content = content; view.viewport = viewport; view.horizontal = false; view.vertical = true;
        view.movementType = ScrollRect.MovementType.Clamped;
        view.scrollSensitivity = 36;
        view.verticalNormalizedPosition = 1;
        MakeButton(panel, "← Volver", new Vector2(0, -379), OpenMainMenuPanel, 400);
        var note = MakeText(panel, "Desplazamiento", "Desliza para desplazarte · Esc para volver", 16, ink, TextAnchor.MiddleCenter);
        Position(note.rectTransform, new Vector2(.5f, 0), new Vector2(0, 22), new Vector2(990, 30));
        return panel;
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
