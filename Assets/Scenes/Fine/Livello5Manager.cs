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
        if (testoStatistiche != null) testoStatistiche.text = GameAnalytics.OttieniReportFinale();
        if (testoAchievement != null && testoAchievement != testoStatistiche)
            testoAchievement.text = GameAnalytics.OttieniReportAchievementFinale();
        if (pannelloAchievement != null) pannelloAchievement.SetActive(true);
        if (bottoneTornaMenu != null) bottoneTornaMenu.onClick.AddListener(TornaAlMenu);
        // Avvia il fade-in morbido da schermo nero appena entri nel LV5
        if (pannelloTransizione != null) StartCoroutine(EffettoFadeIn());
    }

    // ==========================================
    // AZIONI DEI BOTTONI
    // ==========================================

    // Mantiene disponibile l'azione per eventuali collegamenti futuri.
    public void ApriFormTesi()
    {
        Application.OpenURL("https://forms.gle/xjPnizdEX5kb1teL9");
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