using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Livello3Manager : MonoBehaviour
{
    public enum StatoTask { Facile, Media, Difficile }
    public enum TipoBlocco { Vuoto, InputA, InputB, Add, Output, BivioY, LoopAnello, Increment }

    [Header("Stato Livello")]
    public StatoTask taskCorrente = StatoTask.Facile;
    private int slotSelezionato = -1;
    private bool inTransizione = false;
    private bool giocoFinito = false; 

    [Header("UI - Finestra Principale")]
    public GameObject containerFacile;
    public GameObject containerMedia;
    public GameObject containerDifficile;
    public TextMeshProUGUI logConsole;
    public Image pannelloTransizione; 

    [Header("Cambio Scena Finale")]
    public string nomeScenaSuccessiva = "LV4";

    [Header("Anteprima Codice (Live)")]
    public TextMeshProUGUI testoCodiceAnteprima;

    [Header("Interfaccia e Obiettivi")]
    public TextMeshProUGUI testoObiettivo;
    public Image barraTemporaleFill;

    [Header("Sistema Help")]
    public GameObject pannelloHelp;
    public TextMeshProUGUI testoHelp;

    [Header("Slots FACILE (4 Blocchi)")]
    public Image[] slotsFacileImg = new Image[4];
    public TextMeshProUGUI[] slotsFacileTxt = new TextMeshProUGUI[4];
    private TipoBlocco[] blocchiFacile = new TipoBlocco[4];

    [Header("Slots MEDIA (3 Blocchi)")]
    public Image[] slotsMediaImg = new Image[3];
    public TextMeshProUGUI[] slotsMediaTxt = new TextMeshProUGUI[3];
    private TipoBlocco[] blocchiMedia = new TipoBlocco[3];

    [Header("Slots DIFFICILE (2 Blocchi)")]
    public Image[] slotsDifficileImg = new Image[2];
    public TextMeshProUGUI[] slotsDifficileTxt = new TextMeshProUGUI[2];
    private TipoBlocco[] blocchiDifficile = new TipoBlocco[2];

    [Header("Popup Scelta Blocchi")]
    public GameObject pannelloPopupBlocchi;

    [Header("Virtual Machine (Windows, Mac, Linux)")]
    public GameObject finestraWindows;
    public GameObject finestraMac;
    public GameObject finestraLinux;

    [Header("Tasto CHECK (Invio - per verificare)")]
    public Button bottoneCheck;
    public Image immagineBottoneCheck;
    private Color coloreOriginaleCheck;

    [Header("Tasto SUBMIT (Fine Gioco)")]
    public Button bottoneSubmit;
    public Image immagineBottoneSubmit;
    private Color coloreOriginaleSubmit;
    private Coroutine lampeggioVerdeCoroutine;
    

    void Start()
    {
        if (pannelloHelp != null) pannelloHelp.SetActive(false);
        if (pannelloPopupBlocchi != null) pannelloPopupBlocchi.SetActive(false);
        
        if (finestraWindows != null) finestraWindows.SetActive(false);
        if (finestraMac != null) finestraMac.SetActive(false);
        if (finestraLinux != null) finestraLinux.SetActive(false);

        if (barraTemporaleFill != null) barraTemporaleFill.fillAmount = 0f;

        if (immagineBottoneSubmit != null) coloreOriginaleSubmit = immagineBottoneSubmit.color;
        if (immagineBottoneCheck != null) coloreOriginaleCheck = immagineBottoneCheck.color;

        for (int i = 0; i < 4; i++) blocchiFacile[i] = TipoBlocco.Vuoto;
        for (int i = 0; i < 3; i++) blocchiMedia[i] = TipoBlocco.Vuoto;
        for (int i = 0; i < 2; i++) blocchiDifficile[i] = TipoBlocco.Vuoto;

        if (pannelloTransizione != null) StartCoroutine(EffettoFadeIn());

        ImpostaFase();
    }

    IEnumerator EffettoFadeIn()
    {
        inTransizione = true;
        
        if (pannelloTransizione != null)
        {
            pannelloTransizione.gameObject.SetActive(true);
            pannelloTransizione.raycastTarget = true;
            pannelloTransizione.color = new Color(1f, 1f, 1f, 1f); 

            float durataFade = 2.0f; 
            float tempoInizio = Time.time;

            while (Time.time < tempoInizio + durataFade)
            {
                float t = (Time.time - tempoInizio) / durataFade;
                pannelloTransizione.color = new Color(1f, 1f, 1f, 1f - t); 
                yield return null;
            }

            pannelloTransizione.color = new Color(1f, 1f, 1f, 0f);
            pannelloTransizione.raycastTarget = false; 
            pannelloTransizione.gameObject.SetActive(false);
        }
        
        inTransizione = false;
    }

    void ImpostaFase()
    {
        if (containerFacile != null) containerFacile.SetActive(taskCorrente == StatoTask.Facile);
        if (containerMedia != null) containerMedia.SetActive(taskCorrente == StatoTask.Media);
        if (containerDifficile != null) containerDifficile.SetActive(taskCorrente == StatoTask.Difficile);

        AggiornaPopupObiettivo();
        AggiornaAnteprimaCodice();

        if (logConsole != null)
        {
            if (taskCorrente == StatoTask.Facile)
                logConsole.text = "> JVM READY.\n> Assembla la sequenza base. Usa il tasto INVIO per compilare e testare.";
            else if (taskCorrente == StatoTask.Media)
                logConsole.text = "> JVM READY.\n> Gestisci l'eccezione con il BIVIO Y. Premi INVIO per il test multi-piattaforma.";
            else if (taskCorrente == StatoTask.Difficile)
                logConsole.text = "> JVM READY.\n> Chiudi la logica nello SCOPE del LOOP. Premi INVIO per il test finale.";
        }
    }

    public void CliccatoSlot(int indice)
    {
        if (inTransizione || giocoFinito) return; 

        if (taskCorrente == StatoTask.Facile && (indice < 0 || indice >= 4)) return;
        if (taskCorrente == StatoTask.Media && (indice < 0 || indice >= 3)) return;
        if (taskCorrente == StatoTask.Difficile && (indice < 0 || indice >= 2)) return;
        
        slotSelezionato = indice;
        EvidenziaSlotDiGiallo(indice);

        if (pannelloPopupBlocchi != null) 
            pannelloPopupBlocchi.SetActive(true);
    }

    public void ScegliBloccoDalMenu(int idBlocco)
    {
        if (slotSelezionato < 0) return;

        TipoBlocco bloccoScelto = (TipoBlocco)idBlocco;

        if (taskCorrente == StatoTask.Facile && slotSelezionato < 4)
        {
            blocchiFacile[slotSelezionato] = bloccoScelto;
            AggiornaGraficaSlot(slotsFacileImg[slotSelezionato], slotsFacileTxt[slotSelezionato], bloccoScelto);
        }
        else if (taskCorrente == StatoTask.Media && slotSelezionato < 3)
        {
            blocchiMedia[slotSelezionato] = bloccoScelto;
            AggiornaGraficaSlot(slotsMediaImg[slotSelezionato], slotsMediaTxt[slotSelezionato], bloccoScelto);
        }
        else if (taskCorrente == StatoTask.Difficile && slotSelezionato < 2)
        {
            blocchiDifficile[slotSelezionato] = bloccoScelto;
            AggiornaGraficaSlot(slotsDifficileImg[slotSelezionato], slotsDifficileTxt[slotSelezionato], bloccoScelto);
        }

        if (pannelloPopupBlocchi != null) pannelloPopupBlocchi.SetActive(false);
        PulisciEvidenziature();
        AggiornaAnteprimaCodice(); 
        slotSelezionato = -1;
    }

    void AggiornaGraficaSlot(Image img, TextMeshProUGUI txt, TipoBlocco blocco)
    {
        if (img == null || txt == null) return; 

        switch (blocco)
        {
            case TipoBlocco.Vuoto: img.color = Color.white; txt.text = ""; break;
            case TipoBlocco.InputA: img.color = new Color(0.2f, 0.6f, 1f); txt.text = "INPUT A"; break;
            case TipoBlocco.InputB: img.color = new Color(0.2f, 0.6f, 1f); txt.text = "INPUT B"; break;
            case TipoBlocco.Add: img.color = new Color(1f, 0.4f, 0.4f); txt.text = "ADD"; break;
            case TipoBlocco.Output: img.color = new Color(0.2f, 0.8f, 0.2f); txt.text = "OUTPUT"; break;
            case TipoBlocco.BivioY: img.color = new Color(0.6f, 0.2f, 0.8f); txt.text = "BIVIO Y"; break;
            case TipoBlocco.LoopAnello: img.color = new Color(1f, 0.6f, 0f); txt.text = "LOOP"; break;
            case TipoBlocco.Increment: img.color = new Color(1f, 0.8f, 0f); txt.text = "++"; break;
        }
    }

    void EvidenziaSlotDiGiallo(int indice)
    {
        PulisciEvidenziature();
        
        if (taskCorrente == StatoTask.Facile && indice < slotsFacileImg.Length && slotsFacileImg[indice] != null) 
            slotsFacileImg[indice].color = Color.yellow;
        else if (taskCorrente == StatoTask.Media && indice < slotsMediaImg.Length && slotsMediaImg[indice] != null) 
            slotsMediaImg[indice].color = Color.yellow;
        else if (taskCorrente == StatoTask.Difficile && indice < slotsDifficileImg.Length && slotsDifficileImg[indice] != null) 
            slotsDifficileImg[indice].color = Color.yellow;
    }

    void PulisciEvidenziature()
    {
        for (int i = 0; i < 4; i++) { if(slotsFacileImg[i] != null) AggiornaGraficaSlot(slotsFacileImg[i], slotsFacileTxt[i], blocchiFacile[i]); }
        for (int i = 0; i < 3; i++) { if(slotsMediaImg[i] != null) AggiornaGraficaSlot(slotsMediaImg[i], slotsMediaTxt[i], blocchiMedia[i]); }
        for (int i = 0; i < 2; i++) { if(slotsDifficileImg[i] != null) AggiornaGraficaSlot(slotsDifficileImg[i], slotsDifficileTxt[i], blocchiDifficile[i]); }
    }

void AggiornaAnteprimaCodice()
    {
        if (testoCodiceAnteprima == null) return;

        // Struttura base in bianco puro, perfetta per risaltare sul grigio
        string codiceGenerato = "<color=#FFFFFF>public class VirtualMachine {\n";
        codiceGenerato += "    public static void main(String[] args) {\n</color>";

        if (taskCorrente == StatoTask.Facile)
            for (int i = 0; i < 4; i++) { codiceGenerato += "        " + RitornaCodiceDaBlocco(blocchiFacile[i]) + "\n"; }
        else if (taskCorrente == StatoTask.Media)
            for (int i = 0; i < 3; i++) { codiceGenerato += "        " + RitornaCodiceDaBlocco(blocchiMedia[i]) + "\n"; }
        else if (taskCorrente == StatoTask.Difficile)
            for (int i = 0; i < 2; i++) { codiceGenerato += "        " + RitornaCodiceDaBlocco(blocchiDifficile[i]) + "\n"; }

        codiceGenerato += "<color=#FFFFFF>    }\n}</color>";
        testoCodiceAnteprima.text = codiceGenerato;
    }

string RitornaCodiceDaBlocco(TipoBlocco blocco)
    {
        switch (blocco)
        {
            case TipoBlocco.InputA: return "<color=#00aaff>int</color> a = Scanner.read();";
            case TipoBlocco.InputB: return "<color=#00aaff>int</color> b = Scanner.read();";
            case TipoBlocco.Add: return "<color=#ff5555>int</color> result = a + b;";
            case TipoBlocco.Output: return "<color=#55ff55>System.out.println</color>(result);";
            case TipoBlocco.BivioY: return "<color=#aa55ff>if</color> (result > 0) { ... } <color=#aa55ff>else</color> { ... }";
            case TipoBlocco.LoopAnello: return "<color=#ffaa00>while</color> (i < 3) {"; // Il loop apre la parentesi graffa dello scope
            case TipoBlocco.Increment: return "    i++;\n        }"; // L'incremento chiude lo scope
            case TipoBlocco.Vuoto: return "<color=#555555>// ...</color>";
            default: return "";
        }
    }

    public void CliccatoCheck()
    {
        if (inTransizione || giocoFinito) return;

        bool corretto = false;
        string logTesto = "";

        if (taskCorrente == StatoTask.Facile)
        {
            int blocchiVuoti = 0;
            for (int i = 0; i < 4; i++) { if (blocchiFacile[i] == TipoBlocco.Vuoto) blocchiVuoti++; }

            if (blocchiVuoti > 0) logTesto = $"> ERRORE DI SINTASSI [Javac].\n> Trovati {blocchiVuoti} blocchi non definiti.";
            else if (blocchiFacile[0] == TipoBlocco.InputA && blocchiFacile[1] == TipoBlocco.InputB && 
                     blocchiFacile[2] == TipoBlocco.Add && blocchiFacile[3] == TipoBlocco.Output) corretto = true;
            else if (blocchiFacile[3] != TipoBlocco.Output) logTesto = "> ERRORE LOGICO.\n> Il programma deve restituire un OUTPUT.";
            else if (blocchiFacile[0] != TipoBlocco.InputA && blocchiFacile[0] != TipoBlocco.InputB) logTesto = "> EXCEPTION.\n> Nessun dato caricato (Richiesto: INPUT).";
            else logTesto = "> ERRORE DI FLUSSO.\n> L'ordine corretto è: Input, Input, Calcolo, Stampa.";
        }
        else if (taskCorrente == StatoTask.Media)
        {
            int blocchiVuoti = 0;
            for (int i = 0; i < 3; i++) { if (blocchiMedia[i] == TipoBlocco.Vuoto) blocchiVuoti++; }

            if (blocchiVuoti > 0) logTesto = $"> ERRORE DI SINTASSI.\n> Hai lasciato {blocchiVuoti} slot vuoti.";
            else if (blocchiMedia[0] == TipoBlocco.Add && blocchiMedia[1] == TipoBlocco.BivioY && blocchiMedia[2] == TipoBlocco.Output) corretto = true;
            else logTesto = "> NULL POINTER EXCEPTION.\n> Calcola i dati (ADD), verificali (BIVIO Y), poi invia l'OUTPUT.";
        }
        else if (taskCorrente == StatoTask.Difficile)
        {
            int blocchiVuoti = 0;
            for (int i = 0; i < 2; i++) { if (blocchiDifficile[i] == TipoBlocco.Vuoto) blocchiVuoti++; }

            if (blocchiVuoti > 0) logTesto = $"> ERRORE DI SINTASSI.\n> Mancano dei blocchi nel costrutto iterativo.";
            // ORA LA FASE DIFFICILE RICHIEDE IL BLOCCO LOOP E IL BLOCCO INCREMENT
            else if (blocchiDifficile[0] == TipoBlocco.LoopAnello && blocchiDifficile[1] == TipoBlocco.Increment) corretto = true;
            else if (blocchiDifficile[0] == TipoBlocco.Increment) logTesto = "> ERRORE DI SCOPE.\n> Non puoi incrementare una variabile prima di aprire un LOOP.";
            else logTesto = "> ERRORE LOGICO.\n> Per iterare devi inserire il blocco LOOP seguito dall'INCREMENTO.";
        }
        
        if (corretto)
        {
            if (immagineBottoneCheck != null) immagineBottoneCheck.color = Color.green;
            StartCoroutine(RoutineTestMultiPiattaforma());
        }
        else
        {
            StartCoroutine(FlashRosso(immagineBottoneCheck, coloreOriginaleCheck));
            if (logConsole != null) logConsole.text = logTesto;
        }
    }

    IEnumerator RoutineTestMultiPiattaforma()
    {
        inTransizione = true;
        
        if (logConsole != null) logConsole.text = "> [JAVAC] Analisi sintassi in corso...\n> Generazione file Bytecode (.class) completata.";
        yield return new WaitForSeconds(2.0f);

        if (logConsole != null) logConsole.text = "> [JVM] Inizializzazione Ambiente Virtuale...\n> [TEST 1/3] Avvio esecuzione su WINDOWS 95.";
        if (finestraWindows != null) finestraWindows.SetActive(true);
        yield return new WaitForSeconds(1.8f);
        
        if (logConsole != null) logConsole.text = "> [TEST 1/3] Windows: OK.\n> [TEST 2/3] Porting su architettura MAC OS in corso...";
        if (finestraMac != null) finestraMac.SetActive(true);
        yield return new WaitForSeconds(1.8f);
        
        if (logConsole != null) logConsole.text = "> [TEST 2/3] Mac OS: OK.\n> [TEST 3/3] Esecuzione diretta su Kernel LINUX...";
        if (finestraLinux != null) finestraLinux.SetActive(true);
        yield return new WaitForSeconds(2.0f);

        if (logConsole != null) logConsole.text = "> BUILD SUCCESSFUL.\n> Il paradigma 'Write Once, Run Anywhere' è stato validato su tutti i sistemi.";
        yield return new WaitForSeconds(2.0f);

        if (taskCorrente == StatoTask.Facile)
        {
            yield return StartCoroutine(AvanzaBarra(0.33f));
        }
        else if (taskCorrente == StatoTask.Media)
        {
            yield return StartCoroutine(AvanzaBarra(0.66f));
        }
        else if (taskCorrente == StatoTask.Difficile)
        {
            yield return StartCoroutine(AvanzaBarra(1.0f));
        }
        
        if (immagineBottoneCheck != null) immagineBottoneCheck.color = coloreOriginaleCheck;
        StartCoroutine(PassaggioFaseSuccessiva());
    }

    IEnumerator PassaggioFaseSuccessiva()
    {
        yield return new WaitForSeconds(0.5f);

        if (finestraWindows != null) finestraWindows.SetActive(false);
        if (finestraMac != null) finestraMac.SetActive(false);
        if (finestraLinux != null) finestraLinux.SetActive(false);

        if (taskCorrente == StatoTask.Facile) 
        {
            taskCorrente = StatoTask.Media;
            ImpostaFase();
            inTransizione = false;
        }
        else if (taskCorrente == StatoTask.Media) 
        {
            taskCorrente = StatoTask.Difficile;
            ImpostaFase();
            inTransizione = false;
        }
        else if (taskCorrente == StatoTask.Difficile) 
        {
            giocoFinito = true;
            inTransizione = false;
            
            if (logConsole != null) logConsole.text = "> MODULO ANNI '90 COMPLETATO CON SUCCESSO.\n> <color=#00FF66>PREMI SUBMIT PER CONCLUDERE LA SIMULAZIONE.</color>";
            
            if (immagineBottoneCheck != null) immagineBottoneCheck.color = new Color(0.4f, 0.4f, 0.4f);
            lampeggioVerdeCoroutine = StartCoroutine(RespiroVerdeSubmit());
        }
    }

    IEnumerator AvanzaBarra(float targetValue)
    {
        if (barraTemporaleFill == null) yield break;
        float inizio = Time.time;
        float valIniziale = barraTemporaleFill.fillAmount;
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
        if (inTransizione) return;
        
        if (giocoFinito)
        {
            if (lampeggioVerdeCoroutine != null) StopCoroutine(lampeggioVerdeCoroutine);
            if (immagineBottoneSubmit != null) immagineBottoneSubmit.color = Color.green;
            StartCoroutine(EffettoViaggioNelTempoUscita());
        }
        else
        {
            StartCoroutine(FlashRosso(immagineBottoneSubmit, coloreOriginaleSubmit));
            if (logConsole != null) logConsole.text = "> ACCESSO NEGATO.\n> Devi completare e verificare (INVIO) tutte le fasi prima di procedere al salto temporale.";
        }
    }

    IEnumerator EffettoViaggioNelTempoUscita()
    {
        inTransizione = true;
        if (logConsole != null) logConsole.text = "> INIZIALIZZAZIONE SALTO TEMPORALE...\n> RITORNO AL PRESENTE IN CORSO.";

        if (bottoneSubmit != null) bottoneSubmit.gameObject.SetActive(false);
        if (bottoneCheck != null) bottoneCheck.gameObject.SetActive(false);
        if (containerFacile != null) containerFacile.SetActive(false);
        if (containerMedia != null) containerMedia.SetActive(false);
        if (containerDifficile != null) containerDifficile.SetActive(false);

        if (pannelloTransizione != null)
        {
            pannelloTransizione.gameObject.SetActive(true);
            pannelloTransizione.raycastTarget = true;
            pannelloTransizione.color = new Color(1f, 1f, 1f, 0f);
            
            float durataFade = 2.5f;
            float tempoInizio = Time.time;

            while (Time.time < tempoInizio + durataFade)
            {
                float t = (Time.time - tempoInizio) / durataFade;
                pannelloTransizione.color = new Color(1f, 1f, 1f, t); 
                yield return null;
            }
            pannelloTransizione.color = new Color(1f, 1f, 1f, 1f);
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
                testoHelp.text = "MANUALE ANNI '90 - FASE 1 (La Java Virtual Machine)\n\nGUIDA:\nClicca sugli slot vuoti e componi visivamente la sequenza. Il codice Java si aggiornerà in automatico!\n\nCENNI STORICI:\nNel 1995, Sun Microsystems lanciò Java. Il segreto del motto WORA (Write Once, Run Anywhere) era il compilatore: non traduceva in linguaggio macchina specifico, ma in 'Bytecode'. Una 'Virtual Machine' installata su ogni PC leggeva questo bytecode e lo eseguiva al volo, che fosse un PC Windows, un Mac o un terminale Linux!";
                break;
            case StatoTask.Media:
                testoHelp.text = "MANUALE ANNI '90 - FASE 2 (Visual Branching ed Eccezioni)\n\nGUIDA:\nScegli il blocco 'BIVIO Y' per far ramificare il codice in due percorsi.\n\nCENNI STORICI:\nCon la diffusione delle Interfacce Grafiche (GUI), il codice non scorreva più solo dall'alto al basso. I programmi dovevano reagire agli eventi dell'utente. Il branching diventa fondamentale per gestire le eccezioni (come i blocchi Try/Catch), catturando gli errori prima che il programma vada in crash.";
                break;
            case StatoTask.Difficile:
                testoHelp.text = "MANUALE ANNI '90 - FASE 3 (Visual Loop e Scope)\n\nGUIDA:\nScegli il blocco 'LOOP'. Assicurati di inserire l'operazione e l'incremento del contatore al suo interno.\n\nCENNI STORICI:\nL'approccio a blocchi visivi introduce fisicamente il concetto di 'Scope' (l'ambito di visibilità). Le parentesi graffe { } in Java, così come la cornice grafica del blocco Loop, creano un ambiente chiuso e protetto per le variabili. Un concetto che ispirerà profondamente gli ambienti educativi moderni.";
                break;
        }
    }
    
    public void ChiudiHelp() { if (pannelloHelp) pannelloHelp.SetActive(false); }

    void AggiornaPopupObiettivo()
    {
        if (testoObiettivo == null) return;

        switch (taskCorrente)
        {
            case StatoTask.Facile: 
                testoObiettivo.text = "OBIETTIVO 1: WORA\nIncastra i blocchi (A, B, Calcolo, Stampa). Controlla il codice Java autogenerato e premi INVIO per il test OS."; 
                break;
            case StatoTask.Media: 
                testoObiettivo.text = "OBIETTIVO 2: BIVIO LOGICO\nScegli il blocco a Y per indicare visivamente la ramificazione del programma. Premi INVIO."; 
                break;
            case StatoTask.Difficile: 
                testoObiettivo.text = "OBIETTIVO 3: LOOP & SCOPE\nUtilizza il blocco LOOP per racchiudere le operazioni iterative al suo interno. Premi INVIO."; 
                break;
        }
    }
}