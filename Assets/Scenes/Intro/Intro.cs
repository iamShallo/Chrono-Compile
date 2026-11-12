using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Intro : MonoBehaviour
{
    [SerializeField] private float durataFadeIn = 3f;
    [SerializeField] private float durataVisualizzazione = 4f;
    [SerializeField] private float durataFadeOut = 1.5f;
    [SerializeField] private float fattoreZoomIniziale = 1.5f;
    [SerializeField] private string nomeScenaMenu = "Menu";

    private Image immagine;
    private RectTransform rettTransform;
    private Color coloreOriginale;
    private Vector3 scalaOriginale;

    private void Awake()
    {
        immagine = GetComponent<Image>();
        rettTransform = GetComponent<RectTransform>();
        coloreOriginale = immagine.color;
        scalaOriginale = rettTransform.localScale;

        ImpostaAlpha(0f);
    }

    private IEnumerator Start()
    {
        yield return AnimaFadeInEZoom();

        if (durataVisualizzazione > 0f)
            yield return new WaitForSecondsRealtime(durataVisualizzazione);

        yield return AnimaFade(durataFadeOut, 1f, 0f);
        SceneManager.LoadScene(nomeScenaMenu);
    }

    private IEnumerator AnimaFadeInEZoom()
    {
        float durata = Mathf.Max(0f, durataFadeIn);
        Vector3 scalaIniziale = scalaOriginale / Mathf.Max(1f, fattoreZoomIniziale);

        if (durata <= 0f)
        {
            ImpostaAlpha(1f);
            rettTransform.localScale = scalaOriginale;
            yield break;
        }

        rettTransform.localScale = scalaIniziale;
        float trascorso = 0f;
        while (trascorso < durata)
        {
            trascorso += Time.unscaledDeltaTime;
            float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(trascorso / durata));
            ImpostaAlpha(t);
            rettTransform.localScale = Vector3.LerpUnclamped(scalaIniziale, scalaOriginale, t);
            yield return null;
        }

        ImpostaAlpha(1f);
        rettTransform.localScale = scalaOriginale;
    }

    private IEnumerator AnimaFade(float durata, float alphaIniziale, float alphaFinale)
    {
        durata = Mathf.Max(0f, durata);
        if (durata <= 0f)
        {
            ImpostaAlpha(alphaFinale);
            yield break;
        }

        float trascorso = 0f;
        while (trascorso < durata)
        {
            trascorso += Time.unscaledDeltaTime;
            float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(trascorso / durata));
            ImpostaAlpha(Mathf.Lerp(alphaIniziale, alphaFinale, t));
            yield return null;
        }

        ImpostaAlpha(alphaFinale);
    }

    private void ImpostaAlpha(float alpha)
    {
        Color colore = coloreOriginale;
        colore.a = alpha;
        immagine.color = colore;
    }
}
