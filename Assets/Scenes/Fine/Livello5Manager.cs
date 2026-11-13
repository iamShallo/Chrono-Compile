using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class Livello5Manager : MonoBehaviour
{
    [Header("Transizione")]
    public Image pannelloTransizione;
    public TextMeshProUGUI testoStatistiche;
    public GameObject pannelloAchievement;
    public TextMeshProUGUI testoAchievement;
    public Button bottoneTornaMenu;

    void Start()
    {
        Livello5Manager[] managerSulLivello = GetComponents<Livello5Manager>();
        if (managerSulLivello.Length > 0 && managerSulLivello[0] != this) return;

        MenuManager.ConfiguraTestoAccessibilitaCompatto(testoStatistiche);
        MenuManager.ConfiguraTestoAccessibilitaCompatto(testoAchievement);
        if (testoStatistiche != null) testoStatistiche.text = "";
        if (testoAchievement != null && testoAchievement != testoStatistiche) testoAchievement.text = "";
        if (pannelloAchievement != null)
            pannelloAchievement.SetActive(testoAchievement == testoStatistiche);
        if (bottoneTornaMenu != null) bottoneTornaMenu.onClick.AddListener(TornaAlMenu);
        StartCoroutine(MostraRisultatiAnimati());
        // Avvia il fade-in morbido da schermo nero appena entri nel LV5
        if (pannelloTransizione != null) StartCoroutine(EffettoFadeIn());
    }

    IEnumerator MostraRisultatiAnimati()
    {
        yield return StartCoroutine(MostraTestoProgressivamente(testoStatistiche, GameAnalytics.OttieniReportFinale()));

        if (pannelloAchievement != null)
        {
            pannelloAchievement.SetActive(true);
            pannelloAchievement.transform.localScale = Vector3.one * 0.82f;
            yield return StartCoroutine(EffettoCelebrazioneDiscreta(pannelloAchievement.transform));
        }

        if (testoAchievement != null && testoAchievement != testoStatistiche)
            yield return StartCoroutine(MostraTestoProgressivamente(testoAchievement, GameAnalytics.OttieniReportAchievementFinale()));
    }

    IEnumerator MostraTestoProgressivamente(TextMeshProUGUI testo, string contenuto)
    {
        if (testo == null) yield break;

        testo.text = contenuto;
        testo.maxVisibleCharacters = 0;
        testo.ForceMeshUpdate();
        int caratteriTotali = testo.textInfo.characterCount;
        for (int carattere = 1; carattere <= caratteriTotali; carattere++)
        {
            testo.maxVisibleCharacters = carattere;
            yield return new WaitForSeconds(0.018f);
        }

        testo.maxVisibleCharacters = int.MaxValue;
    }

    IEnumerator EffettoCelebrazioneDiscreta(Transform pannello)
    {
        if (pannello == null) yield break;

        Vector3 scalaFinale = Vector3.one;
        float tempo = 0f;
        const float durata = 0.42f;
        while (tempo < durata)
        {
            tempo += Time.deltaTime;
            float t = Mathf.Clamp01(tempo / durata);
            float scala = t < 0.7f
                ? Mathf.Lerp(0.82f, 1.04f, t / 0.7f)
                : Mathf.Lerp(1.04f, 1f, (t - 0.7f) / 0.3f);
            pannello.localScale = scalaFinale * scala;
            yield return null;
        }

        pannello.localScale = scalaFinale;
    }

    // ==========================================
    // AZIONI DEI BOTTONI
    // ==========================================

    // Mantiene disponibile l'azione per eventuali collegamenti futuri.
    public void ApriFormTesi()
    {
        Application.OpenURL("https://docs.google.com/forms/d/e/1FAIpQLSfDlH9NSJmjMWG__ETkYMKI3LbaTiKRsv11I60BZ_ZuhfiX_A/viewform?usp=sharing&ouid=109398919092185628014");
    }

    // Collega questo al bottone "TORNA AL MENU"
    public void TornaAlMenu()
    {
        StartCoroutine(EffettoFadeMenu());
    }

    // ==========================================
    // EFFETTI VISIVI (FADE)
    // ==========================================

    IEnumerator EffettoFadeIn()
    {
        if (pannelloTransizione != null)
        {
            pannelloTransizione.gameObject.SetActive(true);
            pannelloTransizione.raycastTarget = true;
            pannelloTransizione.color = new Color(0f, 0f, 0f, 1f); // Parte tutto nero

            float durataFade = 2.0f;
            float tempoInizio = Time.time;

            while (Time.time < tempoInizio + durataFade)
            {
                float t = (Time.time - tempoInizio) / durataFade;
                pannelloTransizione.color = new Color(0f, 0f, 0f, 1f - Mathf.Clamp01(t));
                yield return null;
            }

            pannelloTransizione.color = new Color(0f, 0f, 0f, 0f);
            pannelloTransizione.raycastTarget = false;
            pannelloTransizione.gameObject.SetActive(false);
        }
    }

    IEnumerator EffettoFadeMenu()
    {
        if (pannelloTransizione != null)
        {
            pannelloTransizione.gameObject.SetActive(true);
            pannelloTransizione.raycastTarget = true;
            pannelloTransizione.color = new Color(0f, 0f, 0f, 0f);

            float durataFade = 1.5f;
            float tempoInizio = Time.time;

            while (Time.time < tempoInizio + durataFade)
            {
                float t = (Time.time - tempoInizio) / durataFade;
                pannelloTransizione.color = new Color(0f, 0f, 0f, Mathf.Clamp01(t));
                yield return null;
            }
            pannelloTransizione.color = new Color(0f, 0f, 0f, 1f); // Torna tutto nero
        }

        yield return new WaitForSeconds(0.2f);
        SceneManager.LoadScene("Menu"); // Torna alla scrivania iniziale
    }
}