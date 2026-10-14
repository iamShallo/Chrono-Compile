using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class BottoneAnimato : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    private Vector3 scalaOriginale;
    private Vector3 targetScale;
    private bool isHovering = false;

    [Header("Impostazioni Animazione")]
    public float moltiplicatoreHover = 1.05f;  // Ingrandisce al 105% quando ci passi sopra
    public float moltiplicatorePremuto = 0.9f; // Rimpicciolisce al 90% quando clicchi
    public float velocitaAnimazione = 15f;     // Velocità dell'effetto elastico

    void Start()
    {
        // Salva la dimensione originale all'avvio
        scalaOriginale = transform.localScale;
        targetScale = scalaOriginale;
    }

    void Update()
    {
        // Questo calcola costantemente la fluidità dell'animazione in ogni frame
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * velocitaAnimazione);
    }

    // Quando il mouse ENTRA sul bottone
    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        targetScale = scalaOriginale * moltiplicatoreHover;
    }

    // Quando il mouse ESCE dal bottone
    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        targetScale = scalaOriginale;
    }

    // Quando PREMI il clic del mouse
    public void OnPointerDown(PointerEventData eventData)
    {
        targetScale = scalaOriginale * moltiplicatorePremuto;
    }

    // Quando RILASCI il clic del mouse
    public void OnPointerUp(PointerEventData eventData)
    {
        // Se il mouse è ancora sopra il bottone, torna ingrandito. Altrimenti torna normale.
        targetScale = isHovering ? (scalaOriginale * moltiplicatoreHover) : scalaOriginale;
    }
}