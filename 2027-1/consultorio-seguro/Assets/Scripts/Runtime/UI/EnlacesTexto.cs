using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ConsultorioSeguro
{
    // Abre en el navegador los <link="url"> de un texto de TextMeshPro al hacer clic.
    [RequireComponent(typeof(TMP_Text))]
    public class EnlacesTexto : MonoBehaviour, IPointerClickHandler
    {
        TMP_Text texto;

        void Awake() => texto = GetComponent<TMP_Text>();

        public void OnPointerClick(PointerEventData evento)
        {
            Camera camara = texto.canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : evento.pressEventCamera;
            int indice = TMP_TextUtilities.FindIntersectingLink(texto, evento.position, camara);
            if (indice < 0)
                return;

            string url = texto.textInfo.linkInfo[indice].GetLinkID();
            if (!string.IsNullOrWhiteSpace(url))
                Application.OpenURL(url);
        }
    }
}
