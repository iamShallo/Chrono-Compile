using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Livello4Manager : MonoBehaviour
{
    public enum StatoTask { Facile, Media, Difficile }
    
    [Header("Stato Livello")]
    public StatoTask taskCorrente = StatoTask.Facile;
    private bool inTransizione = false;
    private bool giocoFinito = false;

    [Header("Interfaccia Chat IA")]
    public TMP_InputField campoPrompt;
    public TextMeshProUGUI testoChat;
    public ScrollRect scrollChat; 
    public GameObject coperturaLogoIA; 

    [Header("Terminale di Esecuzione")]
    public TextMeshProUGUI testoCodiceGenerato;
    public TextMeshProUGUI testoTerminale;

    [Header("Meta-Gioco (UI)")]
    public Image pannelloTransizione;
    public Image barraTemporaleFill;
    public string nomeScenaSuccessiva = "Fine";

    [Header("Interfaccia Obiettivi")]
    public TextMeshProUGUI testoObiettivo;
    
    [Header("Sistema Help")]
    public GameObject pannelloHelp;
    public TextMeshProUGUI testoHelp;

    [Header("Tasto SUBMIT (Fine Gioco)")]
    public Button bottoneSubmit;
    public Image immagineBottoneSubmit;
    private Color coloreOriginaleSubmit;
    private Coroutine lampeggioVerdeCoroutine;

    private bool attesaIA = false;
    private bool primoMessaggioInviato = false;

    void Start()
    {
        if (pannelloHelp != null) pannelloHelp.SetActive(false);
        if (coperturaLogoIA != null) coperturaLogoIA.SetActive(false);
        if (barraTemporaleFill != null) barraTemporaleFill.fillAmount = 0f;
        
        if (immagineBottoneSubmit != null) coloreOriginaleSubmit = immagineBottoneSubmit.color;

        testoChat.text = "";
        testoCodiceGenerato.text = "<color=#555555>// In attesa del prompt per la generazione...</color>";
        testoTerminale.text = "> STANDBY";

        if (pannelloTransizione != null) StartCoroutine(EffettoFadeIn());

        AggiornaInterfacciaMetaGioco();
    }

IEnumerator EffettoFadeIn()
    {
        inTransizione = true;
        if (pannelloTransizione != null)
        {
            pannelloTransizione.gameObject.SetActive(true);
            pannelloTransizione.raycastTarget = true; 
            
            // Inizia completamente NERO
            pannelloTransizione.color = new Color(0f, 0f, 0f, 1f); 

            float durataFade = 2.0f;
            float tempoInizio = Time.time;

            while (Time.time < tempoInizio + durataFade)
            {
                float t = (Time.time - tempoInizio) / durataFade;
                // L'opacità scende gradualmente: 1f - t
                pannelloTransizione.color = new Color(0f, 0f, 0f, 1f - t); 
                yield return null;
            }

            // Finisce trasparente
            pannelloTransizione.color = new Color(0f, 0f, 0f, 0f);
            pannelloTransizione.raycastTarget = false;
            pannelloTransizione.gameObject.SetActive(false);
        }
        inTransizione = false;
    }

    void AggiornaInterfacciaMetaGioco()
    {
        if (testoObiettivo == null) return;

        if (taskCorrente == StatoTask.Facile)
        {
            testoObiettivo.text = "OBIETTIVO 1: LA SINTASSI SCOMPARE\nUsa il linguaggio naturale per chiedere all'IA di fare la SOMMA di DUE numeri.";
        }
        else if (taskCorrente == StatoTask.Media)
        {
            testoObiettivo.text = "OBIETTIVO 2: GLI EDGE CASES\nChiedi una DIVISIONE. Attenzione: se non specifichi di gestire l'ERRORE (zero), l'IA crasherà.";
        }
        else if (taskCorrente == StatoTask.Difficile)
        {
            testoObiettivo.text = "OBIETTIVO 3: L'AMBIGUITÀ\nChiedi un CICLO FOR. Specifica un limite quantitativo (es. 10) o finirai in un loop infinito.";
        }
    }

    // ==========================================
    // LOGICA FINTA CHAT IA
    // ==========================================

    public void InviaPrompt()
    {
        if (attesaIA || inTransizione || giocoFinito) return;
        if (campoPrompt == null || string.IsNullOrWhiteSpace(campoPrompt.text)) return;

        string promptUtente = campoPrompt.text;
        campoPrompt.text = ""; 

        if (!primoMessaggioInviato)
        {
            primoMessaggioInviato = true;
            if (coperturaLogoIA != null) coperturaLogoIA.SetActive(true); 
        }

        AggiungiMessaggioChat("Utente", promptUtente, "#FFFFFF");
        StartCoroutine(ElaboraRispostaIA(promptUtente.ToLower()));
    }

    void AggiungiMessaggioChat(string mittente, string messaggio, string coloreHex)
    {
        testoChat.text += $"\n<color={coloreHex}><b>{mittente}:</b></color>\n{messaggio}\n";
        StartCoroutine(ScrollaInBasso());
    }

    // Il fix del "doppio frame" per far funzionare lo scroll
    IEnumerator ScrollaInBasso()
    {
        yield return new WaitForEndOfFrame(); 
        Canvas.ForceUpdateCanvases(); 
        yield return new WaitForEndOfFrame(); 
        
        if (scrollChat != null) 
        {
            scrollChat.verticalNormalizedPosition = 0f;
            scrollChat.velocity = Vector2.zero;
        }
    }

    IEnumerator ElaboraRispostaIA(string prompt)
    {
        attesaIA = true;
        string tempMsg = "\n<color=#55aaff><b>Gemini:</b></color>\n<i>Sto elaborando...</i>\n";
        testoChat.text += tempMsg;
        StartCoroutine(ScrollaInBasso());

        yield return new WaitForSeconds(1.5f); 

        testoChat.text = testoChat.text.Replace(tempMsg, "");

        // --- FASE 1: FACILE ---
        if (taskCorrente == StatoTask.Facile)
        {
            if (prompt.Contains("somma") || prompt.Contains("addizione") || prompt.Contains("sommare"))
            {
                if (prompt.Contains("due") || prompt.Contains("2"))
                {
                    AggiungiMessaggioChat("Gemini", "Certamente, ecco il codice Python per eseguire un'addizione di due variabili:", "#55aaff");
                    GeneraCodice("def somma(a, b):\n    return a + b\n\nprint(somma(5, 3))");
                    yield return StartCoroutine(EseguiTerminale("> RUN: script.py\n> OUTPUT: 8\n> TEST COMPLETATO CON SUCCESSO.", true));
                }
                else
                {
                    AggiungiMessaggioChat("Gemini", "Hai chiesto una somma, ma il tuo prompt è ambiguo. Quanti numeri devo sommare? Specifica 'due numeri'.", "#55aaff");
                }
            }
            else
            {
                AggiungiMessaggioChat("Gemini", "Non posso aiutarti su questo ora. Il tuo obiettivo attuale è generare una funzione di **somma**.", "#55aaff");
            }
        }
        // --- FASE 2: MEDIA ---
        else if (taskCorrente == StatoTask.Media)
        {
            if (prompt.Contains("dividi") || prompt.Contains("divisione") || prompt.Contains("dividere"))
            {
                if (prompt.Contains("zero") || prompt.Contains("errore") || prompt.Contains("eccezione") || prompt.Contains("0"))
                {
                    AggiungiMessaggioChat("Gemini", "Ottima richiesta. Ho generato il codice includendo il blocco Try/Except per gestire la divisione per zero in sicurezza.", "#55aaff");
                    GeneraCodice("def dividi(a, b):\n    try:\n        return a / b\n    except ZeroDivisionError:\n        return 'Errore: div per zero'\n\nprint(dividi(10, 0))");
                    yield return StartCoroutine(EseguiTerminale("> RUN: script.py\n> OUTPUT: Errore: div per zero\n> ECCEZIONE GESTITA. TEST SUPERATO.", true));
                }
                else
                {
                    AggiungiMessaggioChat("Gemini", "Ecco una funzione base per la divisione:", "#55aaff");
                    GeneraCodice("def dividi(a, b):\n    return a / b\n\nprint(dividi(10, 0))");
                    yield return StartCoroutine(EseguiTerminale("> RUN: script.py\n> <color=#FF3333>ZeroDivisionError: division by zero</color>\n> CRASH DI SISTEMA. Hai omesso le istruzioni sull'errore nel prompt.", false));
                }
            }
            else
            {
                AggiungiMessaggioChat("Gemini", "Restiamo concentrati. Per favore, chiedimi di eseguire una **divisione**.", "#55aaff");
            }
        }
        // --- FASE 3: DIFFICILE ---
        else if (taskCorrente == StatoTask.Difficile)
        {
            if (prompt.Contains("ciclo") || prompt.Contains("loop") || prompt.Contains("for") || prompt.Contains("while"))
            {
                if (prompt.Contains("10") || prompt.Contains("100") || prompt.Contains("array") || prompt.Contains("lista") || prompt.Contains("limite"))
                {
                    AggiungiMessaggioChat("Gemini", "Perfetto. Ho creato un ciclo con un limite quantitativo preciso, in modo da evitare sovraccarichi.", "#55aaff");
                    GeneraCodice("for i in range(10):\n    print('Elaborazione:', i)\n\nprint('Ciclo terminato correttamente.')");
                    yield return StartCoroutine(EseguiTerminale("> RUN: script.py\n> OUTPUT: Ciclo 0... 9\n> TEST SUPERATO IN SICUREZZA.", true));
                }
                else
                {
                    AggiungiMessaggioChat("Gemini", "Nessun limite specificato. Eseguo un ciclo indefinito come da tue istruzioni:", "#55aaff");
                    GeneraCodice("while True:\n    print('Ciclo in esecuzione...')\n");
                    yield return StartCoroutine(EseguiTerminale("> RUN: script.py\n> <color=#FF3333>FATAL ERROR: OUT OF MEMORY (Infinite Loop Detected)</color>\n> L'IA richiede parametri quantitativi esatti.", false));
                }
            }
            else
            {
                AggiungiMessaggioChat("Gemini", "Ho bisogno che tu specifichi chiaramente la generazione di un **ciclo** o un loop.", "#55aaff");
            }
        }

        attesaIA = false;
    }

    void GeneraCodice(string codice)
    {
        testoCodiceGenerato.text = $"<color=#00FF66>{codice}</color>";
    }

    IEnumerator EseguiTerminale(string output, bool successo)
    {
        testoTerminale.text = "> Compilazione JIT in corso...\n";
        yield return new WaitForSeconds(1.0f);
        testoTerminale.text += output;

        if (successo)
        {
            yield return new WaitForSeconds(3.5f);
            
            if (taskCorrente == StatoTask.Facile)
            {
                yield return StartCoroutine(AvanzaBarra(0.33f));
                taskCorrente = StatoTask.Media;
            }
            else if (taskCorrente == StatoTask.Media)
            {
                yield return StartCoroutine(AvanzaBarra(0.66f));
                taskCorrente = StatoTask.Difficile;
            }
            else if (taskCorrente == StatoTask.Difficile)
            {
                yield return StartCoroutine(AvanzaBarra(1.0f));
                giocoFinito = true;
                testoChat.text += "\n<color=#00FF66><b>SISTEMA:</b>\nSimulazione completata. La programmazione è passata dalla scrittura del codice al Prompt Engineering.\nPremi SUBMIT per terminare.</color>\n";
                StartCoroutine(ScrollaInBasso());
                lampeggioVerdeCoroutine = StartCoroutine(RespiroVerdeSubmit());
            }

            AggiornaInterfacciaMetaGioco();
            
            if (!giocoFinito)
            {
                testoCodiceGenerato.text = "<color=#555555>// Pronti per la prossima richiesta...</color>";
                testoTerminale.text = "> STANDBY";
            }
        }
        else
        {
            yield return new WaitForSeconds(2.0f); 
        }
    }

    IEnumerator AvanzaBarra(float targetValue)
    {
        if (barraTemporaleFill == null) yield break;
        float inizio = Time.time, valIniziale = barraTemporaleFill.fillAmount;
        float durataAnimazione = 1.5f; 

        while (Time.time < inizio + durataAnimazione)
        {
            float t = (Time.time - inizio) / durataAnimazione;
            t = t * t * (3f - 2f * t); 
            barraTemporaleFill.fillAmount = Mathf.Lerp(valIniziale, targetValue, t);
            yield return null;
        }
        barraTemporaleFill.fillAmount = targetValue;
    }

    public void CliccatoSubmit()
    {
        if (inTransizione || attesaIA) return;
        
        if (giocoFinito)
        {
            if (lampeggioVerdeCoroutine != null) StopCoroutine(lampeggioVerdeCoroutine);
            if (immagineBottoneSubmit != null) immagineBottoneSubmit.color = Color.green;
            StartCoroutine(EffettoViaggioNelTempoUscita());
        }
        else
        {
            StartCoroutine(FlashRosso(immagineBottoneSubmit, coloreOriginaleSubmit));
            testoTerminale.text = "> <color=#FF3333>ACCESSO NEGATO. Completa tutti i prompt prima di procedere.</color>";
        }
    }

   IEnumerator EffettoViaggioNelTempoUscita()
    {
        inTransizione = true;
        testoTerminale.text = "> SALVATAGGIO PROGRESSI...\n> USCITA DALLA SIMULAZIONE IN CORSO.";

        if (pannelloTransizione != null)
        {
            pannelloTransizione.gameObject.SetActive(true);
            pannelloTransizione.raycastTarget = true;
            
            // 1. Inizia NERO TRASPARENTE (alpha = 0)
            pannelloTransizione.color = new Color(0f, 0f, 0f, 0f);
            
            float durataFade = 2.5f;
            float tempoInizio = Time.time;

            while (Time.time < tempoInizio + durataFade)
            {
                float t = (Time.time - tempoInizio) / durataFade;
                // 2. L'alpha sale gradualmente con 't', diventando sempre più opaco
                pannelloTransizione.color = new Color(0f, 0f, 0f, t); 
                yield return null;
            }
            
            // 3. Finisce NERO OPACO al 100% (alpha = 1)
            pannelloTransizione.color = new Color(0f, 0f, 0f, 1f);
        }

        yield return new WaitForSeconds(1.0f);
        
        if (!string.IsNullOrEmpty(nomeScenaSuccessiva))
        {
            SceneManager.LoadScene(nomeScenaSuccessiva);
        }
    }

    IEnumerator FlashRosso(Image bottoneImg, Color coloreOriginale)
    {
        if (bottoneImg == null) yield break;
        bottoneImg.color = Color.red;
        yield return new WaitForSeconds(0.15f);
        bottoneImg.color = coloreOriginale;
        yield return new WaitForSeconds(0.15f);
        bottoneImg.color = Color.red;
        yield return new WaitForSeconds(0.15f);
        bottoneImg.color = coloreOriginale;
    }

    IEnumerator RespiroVerdeSubmit()
    {
        if (immagineBottoneSubmit == null) yield break;
        while (giocoFinito)
        {
            float t = (Mathf.Sin(Time.time * 4f) + 1f) / 2f; 
            immagineBottoneSubmit.color = Color.Lerp(coloreOriginaleSubmit, Color.green, t);
            yield return null;
        }
    }

    public void ApriHelp()
    {
        if (inTransizione) return; 
        if (pannelloHelp == null || testoHelp == null) return;
        
        pannelloHelp.SetActive(true); 

        switch (taskCorrente)
        {
            case StatoTask.Facile:
                testoHelp.text = "MANUALE 2020+ - FASE 1 (La fine della sintassi)\n\nGUIDA:\nUsa il linguaggio naturale per chiedere la risoluzione del problema. Non preoccuparti del codice.\n\nANALISI COGNITIVA:\nIn questa era, i Large Language Models (LLM) come ChatGPT o Gemini hanno azzerato il 'Carico Cognitivo Estraneo' (ricordarsi punti e virgola o parole chiave di sistema). L'attenzione si sposta interamente sul problema logico.";
                break;
            case StatoTask.Media:
                testoHelp.text = "MANUALE 2020+ - FASE 2 (L'invisibilità dell'Edge Case)\n\nGUIDA:\nChiedi all'IA di dividere, ma ricordale espressamente cosa fare in caso di errore.\n\nANALISI COGNITIVA:\nL'IA è condiscendente. Se le chiedi una funzione base, te la dà, omettendo la sicurezza del codice. Il 'Carico Cognitivo Intrinseco' (la difficoltà reale del problema) ora consiste nel prevedere gli errori e comunicarli alla macchina in anticipo.";
                break;
            case StatoTask.Difficile:
                testoHelp.text = "MANUALE 2020+ - FASE 3 (Il Prompt Engineering)\n\nGUIDA:\nDefinisci vincoli esatti per il ciclo iterativo, inserendo numeri o limiti.\n\nANALISI COGNITIVA:\nIl linguaggio naturale è ambiguo, mentre il codice era deterministico. La sfida suprema del programmatore moderno (Prompt Engineer) è usare l'italiano o l'inglese con la stessa precisione chirurgica che si usava per il C++ o il Java, per non far deragliare l'AI.";
                break;
        }
    }
    
// ==========================================
    // RITORNO AL MENU PRINCIPALE CON ANIMAZIONE
    // ==========================================
    public void TornaAlMenu()
    {
        // Se c'è già un'animazione in corso o il gioco è finito, ignora il clic
        if (inTransizione) return; 
        
        StartCoroutine(EffettoFadeMenu());
    }

IEnumerator EffettoFadeMenu()
    {
        inTransizione = true;

        if (pannelloTransizione != null)
        {
            pannelloTransizione.gameObject.SetActive(true);
            pannelloTransizione.raycastTarget = true; // Blocca i clic sullo schermo
            
            // 1. Inizia NERO TRASPARENTE (alpha = 0)
            pannelloTransizione.color = new Color(0f, 0f, 0f, 0f); 
            
            float durataFade = 1.0f; // Durata del caricamento (1 secondo)
            float tempoInizio = Time.time;

            while (Time.time < tempoInizio + durataFade)
            {
                float t = (Time.time - tempoInizio) / durataFade;
                // 2. L'alpha sale gradualmente con 't'
                pannelloTransizione.color = new Color(0f, 0f, 0f, t); 
                yield return null;
            }
            
            // 3. Finisce NERO OPACO (alpha = 1)
            pannelloTransizione.color = new Color(0f, 0f, 0f, 1f);
        }

        yield return new WaitForSeconds(0.2f); // Piccola pausa scenica prima di caricare
        SceneManager.LoadScene("Menu"); 
    }

    public void ChiudiHelp() { if (pannelloHelp) pannelloHelp.SetActive(false); }
}