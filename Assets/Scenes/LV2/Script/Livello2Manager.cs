using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement; 

public class Livello2Manager : MonoBehaviour
{
    public enum StatoTask { Facile, Media, Difficile }

    [Header("Stato Corrente")]
    public StatoTask taskCorrente = StatoTask.Facile;
    private bool inTransizione = false;
    private bool livelloCompletato = false; 

    [Header("Transizione")]
    public Image pannelloTransizione; 

    [Header("Schermo CRT")]
    public TextMeshProUGUI testoCodiceFisso; 
    public TextMeshProUGUI terminaleLog;

    [Header("Pulsante SUBMIT")]
    public Button bottoneSubmit;
    public Image immagineBottoneSubmit;
    private Color coloreOriginaleSubmit;
    private Coroutine lampeggioVerdeCoroutine;

    [Header("Bottoni Interattivi")]
    public GameObject bottoneFacile;
    public TextMeshProUGUI testoRigaFacile;

    public GameObject bottoneMedia;
    public TextMeshProUGUI testoRigaMedia;

    public GameObject bottoneDifficile;
    public TextMeshProUGUI testoRigaDifficile;

    [Header("Interfaccia e Obiettivi")]
    public TextMeshProUGUI testoObiettivo;
    public Image barraTemporaleFill;
    public GameObject contenitoreBarraTemporale; 
    public GameObject contenitoreObiettivo;

    [Header("Sistema Help")]
    public GameObject pannelloHelp;
    public TextMeshProUGUI testoHelp;

    void Start()
    {
        if (pannelloHelp != null) pannelloHelp.SetActive(false);
        if (barraTemporaleFill != null) barraTemporaleFill.fillAmount = 0f; 
        
        if (immagineBottoneSubmit != null) coloreOriginaleSubmit = immagineBottoneSubmit.color;
        
        if (pannelloTransizione != null) StartCoroutine(EffettoFadeIn());

        ImpostaFaseCorrente();
    }

    IEnumerator EffettoFadeIn()
    {
        inTransizione = true;
        pannelloTransizione.gameObject.SetActive(true);
        pannelloTransizione.raycastTarget = true;

        float durataFade = 2.5f; 
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
        inTransizione = false;
    }

    void ImpostaFaseCorrente()
    {
        if (bottoneFacile) bottoneFacile.SetActive(taskCorrente == StatoTask.Facile);
        if (bottoneMedia) bottoneMedia.SetActive(taskCorrente == StatoTask.Media);
        if (bottoneDifficile) bottoneDifficile.SetActive(taskCorrente == StatoTask.Difficile);

        switch (taskCorrente)
        {
            case StatoTask.Facile:
                if(testoObiettivo) testoObiettivo.text = "FASE 1: I NUMERI DI RIGA E IL GOTO\nNegli anni '60 l'esecuzione seguiva i numeri di riga. Clicca il GOTO ridondante alla fine per pulire il codice.";
                if(testoCodiceFisso) testoCodiceFisso.text = "10 LET X = 12\n20 LET Y = 5\n30 LET RES = X + Y\n40 PRINT RES";
                if(testoRigaFacile) testoRigaFacile.text = "50 GOTO 60   <color=#FFFF00>[ <size=80%>CLICCA PER ELIMINARE</size> ]</color>\n60 END";
                if(terminaleLog) terminaleLog.text = "> ANALISI: Il flusso salta a fine programma senza motivo (tic dell'epoca).\n> AZIONE RICHIESTA: Rimuovere il salto superfluo.";
                break;

            case StatoTask.Media:
                if(testoObiettivo) testoObiettivo.text = "FASE 2: LO 'SPAGHETTI CODE'\nSenza costrutti moderni, i programmatori usavano GOTO condizionali creando percorsi illeggibili. Cliccalo per modernizzarlo.";
                if(testoCodiceFisso) testoCodiceFisso.text = "10 LET RES = 10 - 15\n20 REM CONTROLLO SEGNO";
                if(testoRigaMedia) testoRigaMedia.text = "30 IF RES < 0 THEN GOTO 70   <color=#FFFF00>[ <size=80%>REFACTOR IN IF/ELSE</size> ]</color>\n40 PRINT \"POSITIVO\"\n50 GOTO 80\n70 PRINT \"NEGATIVO\"\n80 END";
                if(terminaleLog) terminaleLog.text = "> ANALISI: Seguire la logica richiede salti mentali su e giu' per il file.\n> AZIONE RICHIESTA: Imporre blocco logico IF/ELSE.";
                break;

            case StatoTask.Difficile:
                if(testoObiettivo) testoObiettivo.text = "FASE 3: IL CICLO FOR\nPer contare fino a 3 servono ben 6 righe incasinate. Clicca il blocco finale per compattare tutto nell'elegante ciclo FOR.";
                if(testoCodiceFisso) testoCodiceFisso.text = "10 LET I = 0\n20 IF I >= 3 THEN GOTO 60\n30 PRINT \"CICLO: \" + I";
                if(testoRigaDifficile) testoRigaDifficile.text = "40 LET I = I + 1\n50 GOTO 20  <color=#FFFF00>[ <size=80%>REFACTOR IN FOR LOOP</size> ]</color>\n60 END";
                if(terminaleLog) terminaleLog.text = "> ANALISI: Inizializzazione, condizione e incremento sono sparsi ovunque.\n> AZIONE RICHIESTA: Compattare nel costrutto moderno FOR.";
                break;
        }
    }

    // --- MECCANICA TASTO SUBMIT ---
    public void CliccatoSubmit()
    {
        if (inTransizione && !livelloCompletato) return;

        if (!livelloCompletato)
        {
            StartCoroutine(FlashErroreRosso());
        }
        else
        {
            if (lampeggioVerdeCoroutine != null) StopCoroutine(lampeggioVerdeCoroutine);
            if (immagineBottoneSubmit != null) immagineBottoneSubmit.color = Color.green;
            
            StartCoroutine(EffettoViaggioNelTempoUscita());
        }
    }

    IEnumerator FlashErroreRosso()
    {
        if (immagineBottoneSubmit == null) yield break;
        immagineBottoneSubmit.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        immagineBottoneSubmit.color = coloreOriginaleSubmit;
        yield return new WaitForSeconds(0.1f);
        
        immagineBottoneSubmit.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        immagineBottoneSubmit.color = coloreOriginaleSubmit;
    }

    IEnumerator RespiroVerdeContinuo()
    {
        if (immagineBottoneSubmit == null) yield break;
        while (livelloCompletato)
        {
            float t = (Mathf.Sin(Time.time * 4f) + 1f) / 2f; 
            immagineBottoneSubmit.color = Color.Lerp(coloreOriginaleSubmit, Color.green, t);
            yield return null;
        }
    }

    IEnumerator EffettoViaggioNelTempoUscita()
    {
        inTransizione = true;
        if(terminaleLog) terminaleLog.text = "> COMANDO ACCETTATO.\n> INIZIALIZZAZIONE SALTO TEMPORALE IN CORSO...";

        if (bottoneSubmit != null) bottoneSubmit.gameObject.SetActive(false);
        if (bottoneFacile != null) bottoneFacile.SetActive(false);
        if (bottoneMedia != null) bottoneMedia.SetActive(false);
        if (bottoneDifficile != null) bottoneDifficile.SetActive(false);
        
        if (contenitoreBarraTemporale != null) contenitoreBarraTemporale.SetActive(false);
        if (contenitoreObiettivo != null) contenitoreObiettivo.SetActive(false);

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
        
        // CARICAMENTO EFFETTIVO DELLA SCENA DEL LIVELLO 3
        SceneManager.LoadScene("LV3");
    }

    // --- FASE 1: FACILE ---
    public void CliccatoRigaFacile()
    {
        if (inTransizione || taskCorrente != StatoTask.Facile) return;
        StartCoroutine(RisolviFacileCoroutine());
    }

    IEnumerator RisolviFacileCoroutine()
    {
        inTransizione = true;
        if(terminaleLog) terminaleLog.text = "> ELIMINAZIONE IN CORSO...\n> La macchina ora sa scorrere il codice verso il basso naturalmente.";
        
        if(testoRigaFacile) testoRigaFacile.text = "<color=#FF3333><s>50 GOTO 60</s></color>\n60 END";
        yield return new WaitForSeconds(3.0f); 

        if(testoCodiceFisso) testoCodiceFisso.text = "let X = 12;\nlet Y = 5;\nlet RES = X + Y;\nprint(RES);";
        if(testoRigaFacile) testoRigaFacile.text = "end;";
        
        if(terminaleLog) terminaleLog.text = "> SUCCESSO: Flusso lineare ripristinato. Numeri di riga obsoleti rimossi.";
        
        yield return StartCoroutine(AvanzaBarra(0.33f));
        yield return new WaitForSeconds(4.0f);
        
        taskCorrente = StatoTask.Media;
        inTransizione = false;
        ImpostaFaseCorrente();
    }

    // --- FASE 2: MEDIA ---
    public void CliccatoRigaMedia()
    {
        if (inTransizione || taskCorrente != StatoTask.Media) return;
        StartCoroutine(RisolviMediaCoroutine());
    }

    IEnumerator RisolviMediaCoroutine()
    {
        inTransizione = true;
        
        if(testoRigaMedia) testoRigaMedia.text = "<color=#FF3333><s>30 IF RES < 0 THEN GOTO 70\n...[SPAGHETTI CODE]...</s></color>";
        if(terminaleLog) terminaleLog.text = "> APPLICAZIONE 'PROGRAMMAZIONE STRUTTURATA' (Edsger Dijkstra, 1968)...";
        yield return new WaitForSeconds(3.0f);

        if(testoCodiceFisso) testoCodiceFisso.text = "int RES = 10 - 15;\n// CONTROLLO SEGNO\n";
        
        string codiceDaScrivere = "<color=#00FF66>if (RES >= 0) {\n    print(\"POSITIVO\");\n}\nelse {\n    print(\"NEGATIVO\");\n}</color>";
        
        yield return StartCoroutine(TypewriterEffect(testoRigaMedia, codiceDaScrivere, 0.05f));

        if(terminaleLog) terminaleLog.text = "> SUCCESSO: Logica incapsulata in blocchi chiusi { }. Il codice ora e' leggibile.";
        
        yield return StartCoroutine(AvanzaBarra(0.66f));
        yield return new WaitForSeconds(5.0f);
        
        taskCorrente = StatoTask.Difficile;
        inTransizione = false;
        ImpostaFaseCorrente();
    }

    // --- FASE 3: DIFFICILE ---
    public void CliccatoRigaDifficile()
    {
        if (inTransizione || taskCorrente != StatoTask.Difficile) return;
        StartCoroutine(RisolviDifficileCoroutine());
    }

    IEnumerator RisolviDifficileCoroutine()
    {
        inTransizione = true;
        
        if(testoRigaDifficile) testoRigaDifficile.text = "<color=#FF3333><s>40 LET I = I + 1\n50 GOTO 20\n60 END</s></color>";
        if(terminaleLog) terminaleLog.text = "> RACCOLTA VARIABILI: Incapsulamento di Inizializzazione, Condizione e Incremento...";
        yield return new WaitForSeconds(3.5f);

        if(testoCodiceFisso) testoCodiceFisso.text = "// CICLO OTTIMIZZATO\n";
        
        string codiceDaScrivere = "<color=#00FF66>for (int i = 0; i < 3; i++) {\n    print(\"CICLO: \" + i);\n}</color>";
        
        yield return StartCoroutine(TypewriterEffect(testoRigaDifficile, codiceDaScrivere, 0.05f));
        yield return new WaitForSeconds(1.5f);

        if(terminaleLog) terminaleLog.text = "> RIVOLUZIONE COMPLETATA: 6 righe di Spaghetti Code ridotte a 3 righe eleganti.\n> L'era del GOTO e' finita.";
        
        yield return StartCoroutine(AvanzaBarra(1.0f));

        livelloCompletato = true;
        if(terminaleLog) terminaleLog.text = "> OTTIMIZZAZIONE 100%. SISTEMA STABILE.\n> <color=#00FF66>PREMI SUBMIT PER AVVIARE IL SALTO TEMPORALE.</color>";
        
        lampeggioVerdeCoroutine = StartCoroutine(RespiroVerdeContinuo());
    }

    // ==========================================
    // EFFETTI SPECIALI & UTILS
    // ==========================================

    IEnumerator TypewriterEffect(TextMeshProUGUI casellaTesto, string testoCompleto, float ritardoLettera)
    {
        if(casellaTesto == null) yield break;
        casellaTesto.text = ""; 
        bool isRichTextTag = false; 

        foreach (char lettera in testoCompleto.ToCharArray())
        {
            if (lettera == '<') isRichTextTag = true; 
            casellaTesto.text += lettera;
            if (lettera == '>') isRichTextTag = false; 
            
            if (!isRichTextTag) 
                yield return new WaitForSeconds(ritardoLettera);
        }
    }

    IEnumerator AvanzaBarra(float targetValue)
    {
        if (barraTemporaleFill == null) yield break;
        float inizio = Time.time, valIniziale = barraTemporaleFill.fillAmount;
        float durataAnimazione = 2.5f; 

        while (Time.time < inizio + durataAnimazione)
        {
            float t = (Time.time - inizio) / durataAnimazione;
            t = t * t * (3f - 2f * t); 
            barraTemporaleFill.fillAmount = Mathf.Lerp(valIniziale, targetValue, t);
            yield return null;
        }
        barraTemporaleFill.fillAmount = targetValue;
    }

    // === FIX HELP LIVELLO 2 ===
    public void ApriHelp() 
    { 
        if (inTransizione && livelloCompletato) return; 
        if (pannelloHelp == null || testoHelp == null) return;
        
        pannelloHelp.SetActive(true); 

        switch (taskCorrente)
        {
            case StatoTask.Facile:
                testoHelp.text = "MANUALE ANNI '60-'70 - FASE 1 (I Numeri di Riga)\n\nGUIDA:\nL'istruzione GOTO muoveva l'esecuzione a una riga specifica. Spesso veniva usata inutilmente. Cliccala per modernizzare il codice.\n\nCENNI STORICI:\nPerché il codice inizia con 10, 20, 30? All'epoca non esistevano editor moderni e il codice si scriveva riga per riga su terminali neri. Si andava di 10 in 10 per lasciarsi lo 'spazio' per inserire nuove istruzioni in mezzo in caso di errore (es. la riga 15). Usare la parola LET era obbligatorio per spiegare al computer che stavi assegnando un valore.";
                break;
            case StatoTask.Media:
                testoHelp.text = "MANUALE ANNI '60-'70 - FASE 2 (If/Else)\n\nGUIDA:\nInvece di saltare con un GOTO a un'etichetta lontana, la logica moderna racchiude il codice dentro due rami: IF (se vero) ed ELSE (se falso).\n\nCENNI STORICI:\nNel 1968, Edsger Dijkstra pubblico' 'Go To Statement Considered Harmful'. Spiego' che quando i programmi saltano continuamente da un punto all'altro (usando i numeri di riga come bersaglio) diventano impossibili da comprendere, creando il caotico 'Spaghetti Code'.";
                break;
            case StatoTask.Difficile:
                testoHelp.text = "MANUALE ANNI '60-'70 - FASE 3 (Il Ciclo FOR)\n\nGUIDA:\nUn ciclo con un contatore non deve essere gestito con salti manuali e incrementi sparsi. Il costrutto FOR compatta tutto in una sola riga.\n\nCENNI STORICI:\nIl teorema di Bohm-Jacopini dimostro' che ogni algoritmo puo' essere scritto con Sequenza, Selezione e Iterazione. L'incapsulamento dei cicli in costrutti come FOR e WHILE ha reso il codice compatto, seppellendo per sempre l'era dei salti di riga.";
                break;
        }
    }
    
    public void ChiudiHelp() 
    { 
        if (pannelloHelp) pannelloHelp.SetActive(false); 
    }
}