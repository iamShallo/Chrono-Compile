using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.TextCore.LowLevel;

public class MenuManager : MonoBehaviour
{
    private const string ChiaveFontDislessia = "font_dislessia";
    private static TMP_FontAsset fontOpenDyslexic;
    private static TMP_FontAsset fontPredefinito;
    private static bool ascoltoSceneAttivo;
    private static readonly Dictionary<TextMeshProUGUI, float> dimensioniTestoOriginali = new Dictionary<TextMeshProUGUI, float>();

    [Header("Pannelli UI (Bottoni)")]
    public GameObject pannelloPrincipale;
    public GameObject pannelloLivelli;
    public GameObject pannelloAchievement;
    public TextMeshProUGUI testoAchievement;
    public Button bottoneChiudiAchievement;
    public float durataAnimazioneAchievement = 0.3f;

    [Header("Sfondi (Immagini)")]
    public GameObject sfondoConBottoni;
    public GameObject sfondoSenzaBottoni;

    [Header("Effetto Caricamento")]
    public Image pannelloFade; // L'immagine nera che copre lo schermo
    public float durataFade = 1.0f;
    private bool inCaricamento = false; // Impedisce di cliccare più bottoni insieme
    private CanvasGroup gruppoAchievement;
    private RectTransform rettangoloAchievement;
    private Coroutine animazioneAchievement;
    private const float ScalaInizialeAchievement = 0.85f;

    [Header("Accessibilità")]
    public Button bottoneFontDislessia;
    private TextMeshProUGUI avvisoFontDislessia;
    private Coroutine animazioneAvvisoFont;

    [Header("Impostazioni Scene")]
    public string nomeLivello1 = "LV1";
    public string nomeLivello2 = "LV2";
    public string nomeLivello3 = "LV3";
    public string nomeLivello4 = "LV4";

    void Start()
    {
        PlayerPrefs.SetInt(ChiaveFontDislessia, 0);
        PlayerPrefs.Save();
        InizializzaFontPredefinito();
        AttivaAggiornamentoFontGlobale();
        if (pannelloPrincipale != null) pannelloPrincipale.SetActive(true);
        if (pannelloLivelli != null) pannelloLivelli.SetActive(false);

        if (sfondoConBottoni != null) sfondoConBottoni.SetActive(true);
        if (sfondoSenzaBottoni != null) sfondoSenzaBottoni.SetActive(false);
        ConfiguraBottoneFontDislessia();
        CreaAvvisoFontNelCanvas();
        ConfiguraTestoAccessibilitaCompatto(testoAchievement);

        if (pannelloAchievement != null)
        {
            rettangoloAchievement = pannelloAchievement.GetComponent<RectTransform>();
            gruppoAchievement = pannelloAchievement.GetComponent<CanvasGroup>();
            if (gruppoAchievement == null)
                gruppoAchievement = pannelloAchievement.AddComponent<CanvasGroup>();

            pannelloAchievement.SetActive(false);
            if (bottoneChiudiAchievement != null)
            {
                bottoneChiudiAchievement.onClick.RemoveListener(ChiudiAchievement);
                bottoneChiudiAchievement.onClick.AddListener(ChiudiAchievement);
            }

        }

        // Assicuriamoci che il fade sia spento all'avvio del menu
        if (pannelloFade != null)
        {
            pannelloFade.gameObject.SetActive(false);
            pannelloFade.color = new Color(0, 0, 0, 0); // Trasparente
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void AttivaAggiornamentoFontGlobale()
    {
        if (ascoltoSceneAttivo)
            return;

        ascoltoSceneAttivo = true;
        SceneManager.sceneLoaded += QuandoCaricaScena;
    }

    private static void QuandoCaricaScena(Scene scena, LoadSceneMode modalita)
    {
        ApplicaFontAccessibilita();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InizializzaFontPredefinito()
    {
        if (fontPredefinito == null)
        {
            fontPredefinito = Resources.Load<TMP_FontAsset>(
                "Fonts & Materials/LiberationSans SDF");
            if (fontPredefinito == null)
                fontPredefinito = TMP_Settings.defaultFontAsset;
        }
    }

    public static bool FontDislessiaAttivo
    {
        get { return PlayerPrefs.GetInt(ChiaveFontDislessia, 0) == 1; }
    }

    public static TMP_FontAsset OttieniFontAccessibilita()
    {
        if (fontOpenDyslexic != null)
            return fontOpenDyslexic;

        Font font = Resources.Load<Font>("OpenDyslexic-Regular");
        if (font == null)
        {
            Debug.LogError("Font OpenDyslexic non trovato in Assets/Resources.");
            return null;
        }

        fontOpenDyslexic = TMP_FontAsset.CreateFontAsset(
            font, 90, 9, GlyphRenderMode.SDFAA, 1024, 1024,
            AtlasPopulationMode.Dynamic, true);
        return fontOpenDyslexic;
    }

    public static void ApplicaFontAccessibilita()
    {
        InizializzaFontPredefinito();

        TMP_FontAsset font = FontDislessiaAttivo
            ? OttieniFontAccessibilita()
            : fontPredefinito;

        if (font == null)
            return;

        TMP_Settings.defaultFontAsset = font;
        TextMeshProUGUI[] testi = FindObjectsByType<TextMeshProUGUI>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None);
        foreach (TextMeshProUGUI testo in testi)
        {
            testo.font = font;
            testo.fontSharedMaterial = font.material;
            testo.SetVerticesDirty();
            testo.SetLayoutDirty();
            ApplicaDimensioneTesto(testo);
        }
    }

    private static void ApplicaDimensioneTesto(TextMeshProUGUI testo)
    {
        if (!dimensioniTestoOriginali.ContainsKey(testo))
            dimensioniTestoOriginali[testo] = testo.fontSize;

        testo.fontSize = FontDislessiaAttivo
            ? dimensioniTestoOriginali[testo] * 0.68f
            : dimensioniTestoOriginali[testo];
    }

    public static void ConfiguraTestoDinamico(TextMeshProUGUI testo)
    {
        if (testo == null)
            return;

        TMP_FontAsset font = FontDislessiaAttivo
            ? OttieniFontAccessibilita()
            : fontPredefinito != null ? fontPredefinito : TMP_Settings.defaultFontAsset;
        if (font != null)
            testo.font = font;
        ApplicaDimensioneTesto(testo);
    }

    public static void ConfiguraTestoAccessibilitaCompatto(TextMeshProUGUI testo)
    {
        if (testo == null)
            return;

        ConfiguraTestoDinamico(testo);
        if (FontDislessiaAttivo && dimensioniTestoOriginali.ContainsKey(testo))
            testo.fontSize = dimensioniTestoOriginali[testo] * 0.55f;
    }

    public static void ConfiguraPannelloHelpResponsive(GameObject pannello, TextMeshProUGUI testo)
    {
        if (pannello == null || testo == null)
            return;

        RectTransform rettangoloPannello = pannello.GetComponent<RectTransform>();
        if (rettangoloPannello == null)
            return;

        RectTransform contenitore = rettangoloPannello.parent as RectTransform;
        float larghezzaContenitore = contenitore != null && contenitore.rect.width > 0f
            ? contenitore.rect.width
            : 1920f;
        float altezzaContenitore = contenitore != null && contenitore.rect.height > 0f
            ? contenitore.rect.height
            : 1080f;

        float larghezzaPannello = Mathf.Min(560f, larghezzaContenitore * 0.34f);
        rettangoloPannello.anchorMin = new Vector2(0f, 0.5f);
        rettangoloPannello.anchorMax = new Vector2(0f, 0.5f);
        rettangoloPannello.pivot = new Vector2(0f, 0.5f);
        rettangoloPannello.anchoredPosition = new Vector2(24f, 0f);
        rettangoloPannello.sizeDelta = new Vector2(
            larghezzaPannello,
            Mathf.Min(900f, altezzaContenitore * 0.88f));

        Image immaginePannello = pannello.GetComponent<Image>();
        if (immaginePannello != null)
        {
            immaginePannello.color = new Color(0.005f, 0.01f, 0.025f, 0.97f);
            immaginePannello.raycastTarget = true;
        }

        Outline contorno = pannello.GetComponent<Outline>();
        if (contorno == null)
            contorno = pannello.AddComponent<Outline>();
        contorno.effectColor = new Color(0.15f, 0.85f, 1f, 0.9f);
        contorno.effectDistance = new Vector2(2f, 2f);

        RectTransform rettangoloTesto = testo.rectTransform;
        rettangoloTesto.SetParent(pannello.transform, false);
        rettangoloTesto.anchorMin = Vector2.zero;
        rettangoloTesto.anchorMax = Vector2.one;
        rettangoloTesto.pivot = new Vector2(0.5f, 0.5f);
        rettangoloTesto.anchoredPosition = Vector2.zero;
        rettangoloTesto.offsetMin = new Vector2(32f, 28f);
        rettangoloTesto.offsetMax = new Vector2(-32f, -84f);

        ContentSizeFitter adattatoreTesto = testo.GetComponent<ContentSizeFitter>();
        if (adattatoreTesto != null)
            Object.Destroy(adattatoreTesto);
        ScrollRect scorrimentoPrecedente = pannello.GetComponent<ScrollRect>();
        if (scorrimentoPrecedente != null)
            Object.Destroy(scorrimentoPrecedente);

        RectTransform areaTestoPrecedente = pannello.transform.Find("HelpViewport") as RectTransform;
        if (areaTestoPrecedente != null)
            Object.Destroy(areaTestoPrecedente.gameObject);

        testo.alignment = TextAlignmentOptions.Left;
        testo.verticalAlignment = VerticalAlignmentOptions.Top;
        testo.fontSize = Mathf.Clamp(larghezzaPannello / 18f, 20f, 34f);
        testo.fontSizeMin = Mathf.Min(18f, testo.fontSize);
        testo.fontSizeMax = testo.fontSize;
        testo.enableAutoSizing = true;
        testo.textWrappingMode = TextWrappingModes.Normal;
        testo.overflowMode = TextOverflowModes.Truncate;
        testo.raycastTarget = false;

        foreach (Button bottone in pannello.GetComponentsInChildren<Button>(true))
        {
            RectTransform rettangoloBottone = bottone.GetComponent<RectTransform>();
            rettangoloBottone.anchorMin = Vector2.one;
            rettangoloBottone.anchorMax = Vector2.one;
            rettangoloBottone.pivot = Vector2.one;
            rettangoloBottone.anchoredPosition = new Vector2(-20f, -18f);
            rettangoloBottone.sizeDelta = new Vector2(64f, 48f);
            bottone.transform.SetAsLastSibling();
        }

        CanvasGroup gruppoPannello = pannello.GetComponent<CanvasGroup>();
        if (gruppoPannello == null)
            gruppoPannello = pannello.AddComponent<CanvasGroup>();
        gruppoPannello.interactable = true;
        gruppoPannello.blocksRaycasts = true;
    }

    public static IEnumerator AnimaPannelloHelp(GameObject pannello, bool apertura, float durata = 0.3f)
    {
        if (pannello == null)
            yield break;

        RectTransform rettangolo = pannello.GetComponent<RectTransform>();
        if (rettangolo == null)
            yield break;

        CanvasGroup gruppo = pannello.GetComponent<CanvasGroup>();
        if (gruppo == null)
            gruppo = pannello.AddComponent<CanvasGroup>();

        Vector2 posizioneVisibile = rettangolo.anchoredPosition;
        Vector2 posizioneFuoriSchermo = posizioneVisibile + Vector2.down * (rettangolo.rect.height + 32f);
        float tempoInizio = Time.unscaledTime;
        float durataAnimazione = Mathf.Max(0.01f, durata);

        if (apertura)
        {
            pannello.SetActive(true);
            rettangolo.anchoredPosition = posizioneFuoriSchermo;
            gruppo.alpha = 0f;
            gruppo.interactable = false;
            gruppo.blocksRaycasts = false;
        }

        Vector2 posizioneIniziale = apertura ? posizioneFuoriSchermo : rettangolo.anchoredPosition;
        Vector2 posizioneFinale = apertura ? posizioneVisibile : posizioneFuoriSchermo;
        float alphaIniziale = apertura ? 0f : gruppo.alpha;
        float alphaFinale = apertura ? 1f : 0f;

        while (Time.unscaledTime < tempoInizio + durataAnimazione)
        {
            float progresso = Mathf.Clamp01((Time.unscaledTime - tempoInizio) / durataAnimazione);
            progresso = 1f - Mathf.Pow(1f - progresso, 3f);
            rettangolo.anchoredPosition = Vector2.LerpUnclamped(posizioneIniziale, posizioneFinale, progresso);
            gruppo.alpha = Mathf.Lerp(alphaIniziale, alphaFinale, progresso);
            yield return null;
        }

        rettangolo.anchoredPosition = apertura ? posizioneVisibile : posizioneFuoriSchermo;
        gruppo.alpha = alphaFinale;
        gruppo.interactable = apertura;
        gruppo.blocksRaycasts = apertura;

        if (!apertura)
        {
            pannello.SetActive(false);
            rettangolo.anchoredPosition = posizioneVisibile;
            gruppo.alpha = 1f;
        }
    }

    private void ConfiguraBottoneFontDislessia()
    {
        if (bottoneFontDislessia == null)
            bottoneFontDislessia = TrovaBottoneFontDislessiaNelCanvas();

        if (bottoneFontDislessia == null)
            return;

        bottoneFontDislessia.onClick.RemoveListener(InvertiFontDislessia);
        bottoneFontDislessia.onClick.AddListener(InvertiFontDislessia);
        AggiornaContornoFontDislessia(FontDislessiaAttivo);
        ApplicaFontAccessibilita();
    }

    public void InvertiFontDislessia()
    {
        bool attivo = !FontDislessiaAttivo;
        PlayerPrefs.SetInt(ChiaveFontDislessia, attivo ? 1 : 0);
        PlayerPrefs.Save();
        AggiornaContornoFontDislessia(attivo);
        ApplicaFontAccessibilita();
        ConfiguraTestoAccessibilitaCompatto(testoAchievement);
        RicaricaCanvasAchievement();
        MostraAvvisoFontDislessia(attivo);
    }

    private void RicaricaCanvasAchievement()
    {
        if (testoAchievement != null)
            testoAchievement.ForceMeshUpdate(true, true);

        Canvas.ForceUpdateCanvases();
        if (rettangoloAchievement != null)
            LayoutRebuilder.ForceRebuildLayoutImmediate(rettangoloAchievement);

        if (pannelloAchievement != null && pannelloAchievement.activeSelf)
        {
            pannelloAchievement.SetActive(false);
            pannelloAchievement.SetActive(true);
        }

        Canvas.ForceUpdateCanvases();
    }

    private void MostraAvvisoFontDislessia(bool attivo)
    {
        if (avvisoFontDislessia == null)
            return;

        avvisoFontDislessia.text = attivo
            ? "OPEN DYSLEXIC ATTIVATO\nTutti i testi del viaggio sono stati aggiornati."
            : "OPEN DYSLEXIC DISATTIVATO\nÈ stato ripristinato il carattere standard.";
        avvisoFontDislessia.color = attivo
            ? new Color(0.2f, 1f, 0.35f, 1f)
            : new Color(1f, 0.25f, 0.25f, 1f);

        CanvasGroup gruppo = avvisoFontDislessia.GetComponent<CanvasGroup>();
        gruppo.alpha = 1f;
        avvisoFontDislessia.gameObject.SetActive(true);
        if (animazioneAvvisoFont != null)
            StopCoroutine(animazioneAvvisoFont);
        animazioneAvvisoFont = StartCoroutine(NascondiAvvisoFont(gruppo));
    }

    private IEnumerator NascondiAvvisoFont(CanvasGroup gruppo)
    {
        yield return new WaitForSecondsRealtime(3.5f);
        float tempoInizio = Time.unscaledTime;
        while (Time.unscaledTime < tempoInizio + 0.8f)
        {
            gruppo.alpha = 1f - Mathf.Clamp01((Time.unscaledTime - tempoInizio) / 0.8f);
            yield return null;
        }
        gruppo.alpha = 0f;
        avvisoFontDislessia.gameObject.SetActive(false);
        animazioneAvvisoFont = null;
    }

    private void AggiornaContornoFontDislessia(bool attivo)
    {
        if (bottoneFontDislessia == null)
            return;

        Outline contorno = bottoneFontDislessia.GetComponent<Outline>();
        if (contorno == null)
            contorno = bottoneFontDislessia.gameObject.AddComponent<Outline>();

        contorno.effectColor = attivo
            ? new Color(0.1f, 1f, 0.25f, 1f)
            : new Color(1f, 0.1f, 0.1f, 1f);
        contorno.effectDistance = new Vector2(4f, 4f);
        contorno.useGraphicAlpha = false;
    }

    private Button TrovaBottoneFontDislessiaNelCanvas()
    {
        TextMeshProUGUI etichetta = null;
        TextMeshProUGUI[] testi = FindObjectsByType<TextMeshProUGUI>(
            FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (TextMeshProUGUI testo in testi)
        {
            if (testo.text.Trim() == "FONT DISLESSIA")
            {
                etichetta = testo;
                break;
            }
        }

        if (etichetta == null)
            return null;

        Button bottone = etichetta.GetComponentInParent<Button>();
        if (bottone == null)
            return null;

        return bottone;
    }

    private void CreaAvvisoFontNelCanvas()
    {
        Canvas canvas = FindAnyObjectByType<Canvas>();
        if (canvas == null)
            return;
        GameObject avvisoOggetto = new GameObject("AvvisoFontDislessia", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI), typeof(CanvasGroup));
        avvisoOggetto.transform.SetParent(canvas.transform, false);
        RectTransform rettangoloAvviso = avvisoOggetto.GetComponent<RectTransform>();
        rettangoloAvviso.anchorMin = new Vector2(0.5f, 0f);
        rettangoloAvviso.anchorMax = new Vector2(0.5f, 0f);
        rettangoloAvviso.pivot = new Vector2(0.5f, 0f);
        rettangoloAvviso.anchoredPosition = new Vector2(0f, 42f);
        rettangoloAvviso.sizeDelta = new Vector2(700f, 86f);
        avvisoFontDislessia = avvisoOggetto.GetComponent<TextMeshProUGUI>();
        avvisoFontDislessia.alignment = TextAlignmentOptions.Center;
        avvisoFontDislessia.fontSize = 23f;
        avvisoFontDislessia.fontStyle = FontStyles.Bold;
        avvisoFontDislessia.raycastTarget = false;
        ConfiguraTestoDinamico(avvisoFontDislessia);
        avvisoOggetto.GetComponent<CanvasGroup>().alpha = 0f;
        avvisoOggetto.SetActive(false);
    }

    // ==========================================
    // FUNZIONI CARICAMENTO E TRANSIZIONE
    // ==========================================

    public void IniziaDaCapo()
    {
        GameAnalytics.ResetStatistiche();
        AvviaTransizione(nomeLivello1);
    }
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
                pannelloFade.color = new Color(0f, 0f, 0f, Mathf.Clamp01(t));
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
        ChiudiAchievement();
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

    // Collega questo metodo al bottone Achievement creato nel Canvas.
    public void ApriAchievement()
    {
        if (inCaricamento || pannelloAchievement == null || testoAchievement == null) return;

        if (animazioneAchievement != null)
            StopCoroutine(animazioneAchievement);

        rettangoloAchievement.localScale = Vector3.one * ScalaInizialeAchievement;
        gruppoAchievement.alpha = 0f;
        pannelloAchievement.SetActive(true);
        testoAchievement.text = GameAnalytics.OttieniReportAchievement();
        animazioneAchievement = StartCoroutine(AnimaAchievement(ScalaInizialeAchievement, 1f, 0f, 1f));
    }

    public void ChiudiAchievement()
    {
        if (pannelloAchievement == null || !pannelloAchievement.activeSelf)
            return;

        if (animazioneAchievement != null)
            StopCoroutine(animazioneAchievement);
        animazioneAchievement = StartCoroutine(ChiudiAchievementAnimato());
    }

    private IEnumerator ChiudiAchievementAnimato()
    {
        yield return StartCoroutine(AnimaAchievement(rettangoloAchievement.localScale.x, ScalaInizialeAchievement, gruppoAchievement.alpha, 0f));
        pannelloAchievement.SetActive(false);
        animazioneAchievement = null;
    }

    private IEnumerator AnimaAchievement(float scalaIniziale, float scalaFinale, float alphaIniziale, float alphaFinale)
    {
        float durata = Mathf.Max(0.01f, durataAnimazioneAchievement);
        float tempoInizio = Time.unscaledTime;

        while (Time.unscaledTime < tempoInizio + durata)
        {
            float progresso = Mathf.Clamp01((Time.unscaledTime - tempoInizio) / durata);
            progresso = progresso * progresso * (3f - 2f * progresso);
            rettangoloAchievement.localScale = Vector3.one * Mathf.Lerp(scalaIniziale, scalaFinale, progresso);
            gruppoAchievement.alpha = Mathf.Lerp(alphaIniziale, alphaFinale, progresso);
            yield return null;
        }

        rettangoloAchievement.localScale = Vector3.one * scalaFinale;
        gruppoAchievement.alpha = alphaFinale;
        animazioneAchievement = null;
    }

    // ==========================================
    // LINK ESTERNI E USCITA
    // ==========================================

    // NUOVA FUNZIONE PER IL GOOGLE FORM
    public void ApriForm()
    {
        if (inCaricamento) return;
        Application.OpenURL("https://docs.google.com/forms/d/e/1FAIpQLSfDlH9NSJmjMWG__ETkYMKI3LbaTiKRsv11I60BZ_ZuhfiX_A/viewform?usp=sharing&ouid=109398919092185628014");
    }

    public void EsciDalGioco()
    {
        if (inCaricamento) return;
        Debug.Log("Uscita dal gioco (funziona solo nella Build finale)");
        Application.Quit();
    }
}