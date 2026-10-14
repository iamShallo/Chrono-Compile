using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuManager : MonoBehaviour
{
    [Header("Pannelli UI (Bottoni)")]
    public GameObject pannelloPrincipale; 
    public GameObject pannelloLivelli;    

    [Header("Sfondi (Immagini)")]
    public GameObject sfondoConBottoni;   
    public GameObject sfondoSenzaBottoni; 

    [Header("Effetto Caricamento")]
    public Image pannelloFade; // L'immagine nera che copre lo schermo
    public float durataFade = 1.0f;
    private bool inCaricamento = false; // Impedisce di cliccare più bottoni insieme

    [Header("Impostazioni Scene")]
    public string nomeLivello1 = "LV1";
    public string nomeLivello2 = "LV2";
    public string nomeLivello3 = "LV3";
    public string nomeLivello4 = "LV4";

    void Start()
    {
        if (pannelloPrincipale != null) pannelloPrincipale.SetActive(true);
        if (pannelloLivelli != null) pannelloLivelli.SetActive(false);
        
        if (sfondoConBottoni != null) sfondoConBottoni.SetActive(true);
        if (sfondoSenzaBottoni != null) sfondoSenzaBottoni.SetActive(false);

        // Assicuriamoci che il fade sia spento all'avvio del menu
        if (pannelloFade != null)
        {
            pannelloFade.gameObject.SetActive(false);
            pannelloFade.color = new Color(0, 0, 0, 0); // Trasparente
        }
    }

    // ==========================================
    // FUNZIONI CARICAMENTO E TRANSIZIONE
    // ==========================================

    public void IniziaDaCapo() { AvviaTransizione(nomeLivello1); }
    public void VaiALivello1() { AvviaTransizione(nomeLivello1); }
    public void VaiALivello2() { AvviaTransizione(nomeLivello2); }
    public void VaiALivello3() { AvviaTransizione(nomeLivello3); }
    public void VaiALivello4() { AvviaTransizione(nomeLivello4); }

    private void AvviaTransizione(string nomeScena)
    {
        if (inCaricamento) return; 
        StartCoroutine(EffettoFadeELoad(nomeScena));
    }

    IEnumerator EffettoFadeELoad(string nomeScena)
    {
        inCaricamento = true;

        if (pannelloFade != null)
        {
            pannelloFade.gameObject.SetActive(true);
            pannelloFade.raycastTarget = true; 

            float tempoInizio = Time.time;
            while (Time.time < tempoInizio + durataFade)
            {
                float t = (Time.time - tempoInizio) / durataFade;
                pannelloFade.color = new Color(0f, 0f, 0f, t);
                yield return null;
            }
            pannelloFade.color = new Color(0f, 0f, 0f, 1f);
        }

        yield return new WaitForSeconds(0.2f); 
        SceneManager.LoadScene(nomeScena);
    }

    // ==========================================
    // FUNZIONI NAVIGAZIONE INTERNA AL MENU
    // ==========================================

    public void ApriPannelloLivelli()
    {
        if (inCaricamento) return;
        if (pannelloPrincipale != null) pannelloPrincipale.SetActive(false);
        if (pannelloLivelli != null) pannelloLivelli.SetActive(true);

        if (sfondoConBottoni != null) sfondoConBottoni.SetActive(false);
        if (sfondoSenzaBottoni != null) sfondoSenzaBottoni.SetActive(true);
    }

    public void ChiudiPannelloLivelli()
    {
        if (inCaricamento) return;
        if (pannelloLivelli != null) pannelloLivelli.SetActive(false);
        if (pannelloPrincipale != null) pannelloPrincipale.SetActive(true);

        if (sfondoSenzaBottoni != null) sfondoSenzaBottoni.SetActive(false);
        if (sfondoConBottoni != null) sfondoConBottoni.SetActive(true);
    }

    // ==========================================
    // LINK ESTERNI E USCITA
    // ==========================================

    // NUOVA FUNZIONE PER IL GOOGLE FORM
    public void ApriForm()
    {
        if (inCaricamento) return;
        Application.OpenURL("https://forms.gle/xjPnizdEX5kb1teL9");
    }

    public void EsciDalGioco()
    {
        if (inCaricamento) return;
        Debug.Log("Uscita dal gioco (funziona solo nella Build finale)");
        Application.Quit();
    }
}