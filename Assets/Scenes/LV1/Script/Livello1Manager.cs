using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Livello1Manager : MonoBehaviour
{
    public enum StatoTask { Facile, Media, Difficile }
    
    [Header("Impostazioni Livello")]
    public StatoTask taskCorrente = StatoTask.Facile;
    public TextMeshProUGUI schermoTerminale;
    public TextMeshProUGUI testoObiettivo; 

    [Header("Barra e Transizione")]
    public Image barraTemporaleFill; 
    public GameObject contenitoreBarraTemporale; 
    public GameObject contenitoreObiettivo;       
    public Image pannelloTransizione; 

    [Header("Pulsante SUBMIT")]
    public Button bottoneSubmit;
    public Image immagineBottoneSubmit;
    private Color coloreOriginaleSubmit;
    private Coroutine lampeggioVerdeCoroutine;
    private bool livelloCompletato = false;

    [Header("Leve di Comando")]
    public Toggle levaStatus; 
    public Toggle levaPower;  
    public Toggle levaInputA; 
    public Toggle levaInputB; 
    public Toggle levaProcess;
    public Toggle levaOutput; 
    public Toggle levaLoopExecute; 

    [Header("Sistema Help")]
    public GameObject pannelloHelp;
    public TextMeshProUGUI testoHelp;

    private bool inTransizione = false; 

    void Start()
    {
        if (pannelloHelp != null) 
            pannelloHelp.SetActive(false);

        if (barraTemporaleFill != null) 
            barraTemporaleFill.fillAmount = 0f;

        if (immagineBottoneSubmit != null) 
            coloreOriginaleSubmit = immagineBottoneSubmit.color;

        if (levaStatus != null)
        {
            levaStatus.interactable = true;
            levaStatus.SetIsOnWithoutNotify(false);
        }

        SpegniTutteLeLeveSenzaNotifica();
        AggiornaPopupObiettivo();

        if (schermoTerminale != null) 
            schermoTerminale.text = ""; 

        // Avvia direttamente il Fade-In NERO all'inizio
        if (pannelloTransizione != null) StartCoroutine(EffettoFadeIn());   
    }

    public void ValutaInterruttori()
    {
        if (schermoTerminale == null || levaStatus == null) return;

        if (inTransizione) 
        {
            if (!levaStatus.isOn) 
                levaStatus.SetIsOnWithoutNotify(true);
            return;
        }

        if (!levaStatus.isOn)
        {
            schermoTerminale.text = "";
            return;
        }

        switch (taskCorrente)
        {
            case StatoTask.Facile: GestisciTaskFacile(); break;
            case StatoTask.Media: GestisciTaskMedia(); break;
            case StatoTask.Difficile: GestisciTaskDifficile(); break;
        }
    }

    void GestisciTaskFacile()
    {
        if (levaOutput.isOn && !levaProcess.isOn) { StartCoroutine(MostraErrore("ERRORE HARDWARE:\nImpossibile stampare dati non elaborati. Aziona prima PROCESS.")); return; }
        if (levaProcess.isOn && (!levaInputA.isOn || !levaInputB.isOn)) { StartCoroutine(MostraErrore("ERRORE ALU:\nRegistri vuoti. L'Unita' Aritmetica non puo' calcolare senza operandi.")); return; }
        if ((levaInputA.isOn || levaInputB.isOn) && !levaPower.isOn) { StartCoroutine(MostraErrore("ERRORE DI SISTEMA:\nCircuiti non alimentati. Accendi il POWER.")); return; }
        if (levaLoopExecute.isOn) { StartCoroutine(MostraErrore("ERRORE:\nIl comando EXECUTE non e' richiesto in questa fase sequenziale.")); return; }

        if (levaPower.isOn && levaInputA.isOn && levaInputB.isOn && levaProcess.isOn && levaOutput.isOn)
        {
            schermoTerminale.text = ">>> CICLO DI VON NEUMANN COMPLETATO <<<\n[POWER: ON] -> DATI CARICATI [X=12, Y=5]\n[ALU]: ADD(12, 5) = 17\n[OUTPUT]: STAMPATO SU CARTA PERFORATA [17]\n\nSUCCESS! LETTURA RISULTATO IN CORSO...";
            StartCoroutine(AvanzaBarraTemporale(0.33f)); 
            StartCoroutine(PassaggioFase(StatoTask.Media, 9f));
        }
        else if (levaPower.isOn && levaInputA.isOn && levaInputB.isOn && levaProcess.isOn) { 
            schermoTerminale.text = "[SISTEMA ATTIVO]\nRegistri A e B letti.\nALU ha calcolato: 12 + 5 = 17.\n\n> AZIONE: Tira la leva OUTPUT per stampare il risultato."; 
        }
        else if (levaPower.isOn && levaInputA.isOn && levaInputB.isOn) { 
            schermoTerminale.text = "[SISTEMA ATTIVO]\nDati immessi nei registri di memoria.\n\n> AZIONE: Tira la leva PROCESS per far eseguire il calcolo all'ALU."; 
        }
        else if (levaPower.isOn && (levaInputA.isOn || levaInputB.isOn)) { 
            schermoTerminale.text = "[SISTEMA ATTIVO]\nIn attesa del caricamento completo di entrambi i registri (A e B)..."; 
        }
        else if (levaPower.isOn) { 
            schermoTerminale.text = "[SISTEMA ATTIVO]\nEnergia principale stabilita.\n\n> AZIONE: Carica i dati nei registri INPUT A e INPUT B."; 
        }
        else { 
            schermoTerminale.text = "[DISPLAY: ONLINE]\nATTENZIONE: Macchina spenta.\n\n> AZIONE: Alimenta il calcolatore azionando POWER."; 
        }
    }

    void GestisciTaskMedia()
    {
        if (levaOutput.isOn) { StartCoroutine(MostraErrore("ERRORE CRITICO:\nHai eseguito la stampa!\nLa specifica imponeva di SALTARE la stampa se il risultato era negativo.")); return; }
        if (levaProcess.isOn && (!levaInputA.isOn || !levaInputB.isOn)) { StartCoroutine(MostraErrore("ERRORE:\nInserisci i dati prima di avviare il calcolo.")); return; }
        if ((levaInputA.isOn || levaInputB.isOn) && !levaPower.isOn) { StartCoroutine(MostraErrore("ERRORE:\nManca l'energia principale (POWER).")); return; }
        if (levaLoopExecute.isOn && !levaProcess.isOn) { StartCoroutine(MostraErrore("ERRORE:\nCalcola il risultato prima di confermare il salto condizionato.")); return; }

        if (levaPower.isOn && levaInputA.isOn && levaInputB.isOn && levaProcess.isOn && levaLoopExecute.isOn)
        {
            schermoTerminale.text = ">>> SALTO CONDIZIONATO ESEGUITO <<<\n[ALU]: 10 + (-15) = -5\n[FLAG NEGATIVO ATTIVO]: Rilevato numero minore di zero.\n[EXECUTE]: Salto logico attivato -> STAMPA SALTATA CON SUCCESSO!\n\nSUCCESS! ANALISI DEL SALTO IN CORSO...";
            StartCoroutine(AvanzaBarraTemporale(0.66f)); 
            StartCoroutine(PassaggioFase(StatoTask.Difficile, 9f));
        }
        else if (levaPower.isOn && levaInputA.isOn && levaInputB.isOn && levaProcess.isOn) { 
            schermoTerminale.text = "[CALCOLO ESEGUITO]: Risultato = -5 (NEGATIVO).\nLa regola vieta di stampare i numeri negativi.\n\n> AZIONE: Tira la leva EXECUTE per confermare il salto logico e bypassare la stampa."; 
        }
        else if (levaPower.isOn && levaInputA.isOn && levaInputB.isOn) { 
            schermoTerminale.text = "[MEMORIA CARICATA]\nRegistri impostati su X=10 e Y=-15.\n\n> AZIONE: Attiva PROCESS per valutare il segno algebrico."; 
        }
        else if (levaPower.isOn) { 
            schermoTerminale.text = "[FASE 2: IL CONTROLLO DI SEGNALE]\nCarica i nuovi dati nei registri per testare la ramificazione logica."; 
        }
        else { 
            schermoTerminale.text = "[DISPLAY: ONLINE]\nAttiva il POWER per iniziare la sessione di diramazione logica."; 
        }
    }

    void GestisciTaskDifficile()
    {
        if (levaLoopExecute.isOn)
        {
            if (levaPower.isOn && levaInputA.isOn && levaInputB.isOn && levaProcess.isOn && levaOutput.isOn) 
            {
                StartCoroutine(EseguiLoopAutomatico());
            } 
            else 
            {
                StartCoroutine(MostraErrore("ERRORE:\nConfigura l'intera sequenza di calcolo prima di delegare l'esecuzione automatica a EXECUTE."));
                levaLoopExecute.SetIsOnWithoutNotify(false);
            }
            return;
        }

        if (levaOutput.isOn && !levaProcess.isOn) { StartCoroutine(MostraErrore("ERRORE: Impossibile stampare senza elaborazione.")); return; }
        if (levaProcess.isOn && (!levaInputA.isOn || !levaInputB.isOn)) { StartCoroutine(MostraErrore("ERRORE: Dati di input mancanti.")); return; }
        if ((levaInputA.isOn || levaInputB.isOn) && !levaPower.isOn) { StartCoroutine(MostraErrore("ERRORE: Sistema non alimentato.")); return; }
        
        if (levaPower.isOn && levaInputA.isOn && levaInputB.isOn && levaProcess.isOn && levaOutput.isOn) {
            schermoTerminale.text = "ROUTINE COMPLETA DEFINITA:\n1. Carica incremento (5)\n2. Somma all'Accumulatore\n3. Stampa parziale\n\n> AZIONE: Tira la leva EXECUTE per avviare il ciclo automatico hardware!";
        }
        else if (levaPower.isOn) { 
            schermoTerminale.text = "[FASE 3: AUTOMAZIONE CICLICA]\nImposta tutti gli interruttori (A, B, Process, Output) per definire la singola unità di ripetizione del ciclo."; 
        }
        else {
            schermoTerminale.text = "[DISPLAY: ONLINE]\nAttiva il POWER per preparare il circuito iterativo.";
        }
    }

    IEnumerator EseguiLoopAutomatico()
    {
        inTransizione = true; 
        schermoTerminale.text = "[HARDWARE OVERRIDE]\nAVVIO CICLO AUTOMATICO DELL'ACCUMULATORE...";
        
        yield return new WaitForSeconds(3.0f); 
        levaInputA.SetIsOnWithoutNotify(false);
        levaInputB.SetIsOnWithoutNotify(false);
        levaProcess.SetIsOnWithoutNotify(false);
        levaOutput.SetIsOnWithoutNotify(false);
        yield return new WaitForSeconds(1f);

        int accumulatore = 0;
        int incremento = 5;

        for (int i = 1; i <= 3; i++)
        {
            if (!levaStatus.isOn) { schermoTerminale.text = ""; inTransizione = false; yield break; }

            schermoTerminale.text = $"[ITERAZIONE {i} DI 3]\nCaricamento incremento X = {incremento}...";
            yield return new WaitForSeconds(1.8f); 
            levaInputA.isOn = true;
            levaInputB.isOn = true;
            
            schermoTerminale.text = $"[ITERAZIONE {i} DI 3]\n[ACCUMULATORE]: {accumulatore} + {incremento}";
            yield return new WaitForSeconds(2.0f); 
            levaProcess.isOn = true;
            
            accumulatore += incremento; 
            
            schermoTerminale.text = $"[ITERAZIONE {i} DI 3]\nTOTALE PARZIALE IN MEMORIA: {accumulatore}\nTrasmissione a stampante...";
            yield return new WaitForSeconds(2.0f);
            levaOutput.isOn = true;
            
            yield return new WaitForSeconds(3.0f); 
            
            if (i < 3) 
            {
                levaInputA.SetIsOnWithoutNotify(false);
                levaInputB.SetIsOnWithoutNotify(false);
                levaProcess.SetIsOnWithoutNotify(false);
                levaOutput.SetIsOnWithoutNotify(false);
                yield return new WaitForSeconds(0.8f);
            }
        }

        yield return StartCoroutine(AvanzaBarraTemporale(1f)); 
        
        livelloCompletato = true;
        schermoTerminale.text = $"[SEQUENZA HARDWARE COMPLETATA]\nVALORE FINALE ACCUMULATORE: {accumulatore}\n\n>>> ERA ANNI '40-'50 COMPLETATA.\n<color=#00FF66>PREMI SUBMIT PER AVVIARE IL SALTO ALL'ERA SUCCESSIVA.</color>";
        
        if (immagineBottoneSubmit != null)
        {
            lampeggioVerdeCoroutine = StartCoroutine(RespiroVerdeContinuo());
        }
    }

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
            
            StartCoroutine(EffettoViaggioNelTempo());
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

    // Animazione di Vittoria e cambio livello
    IEnumerator EffettoViaggioNelTempo()
    {
        inTransizione = true;
        schermoTerminale.text = "[TIME JUMP SEQUENCE INITIATED]\nTRASFERIMENTO DATI NELLA MEMORIA MAGNETICA...";

        if (bottoneSubmit != null) bottoneSubmit.gameObject.SetActive(false);
        if (contenitoreBarraTemporale != null) contenitoreBarraTemporale.SetActive(false);
        if (contenitoreObiettivo != null) contenitoreObiettivo.SetActive(false);

        if (pannelloTransizione != null)
        {
            pannelloTransizione.gameObject.SetActive(true);
            pannelloTransizione.raycastTarget = true; 
            pannelloTransizione.color = new Color(0f, 0f, 0f, 0f);

            float durataFade = 2.5f;
            float tempoInizio = Time.time;

            while (Time.time < tempoInizio + durataFade)
            {
                float t = (Time.time - tempoInizio) / durataFade;
                pannelloTransizione.color = new Color(0f, 0f, 0f, 0f - t); // Questo fade a bianco rimane per il viaggio nel tempo
                yield return null;
            }
            pannelloTransizione.color = new Color(0f, 0f, 0f, 1f);
        }

        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("LV2"); 
    }

    // Fade-in nero iniziale 
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
    
    IEnumerator AvanzaBarraTemporale(float targetValue)
    {
        if (barraTemporaleFill == null) yield break;
        float tempoInizio = Time.time;
        float valoreIniziale = barraTemporaleFill.fillAmount;
        float durataAnimazione = 1.5f;

        while (Time.time < tempoInizio + durataAnimazione) {
            float t = (Time.time - tempoInizio) / durataAnimazione;
            barraTemporaleFill.fillAmount = Mathf.Lerp(valoreIniziale, targetValue, t);
            yield return null;
        }
        barraTemporaleFill.fillAmount = targetValue;
    }

    IEnumerator MostraErrore(string messaggioErrore)
    {
        inTransizione = true;
        SpegniTutteLeLeveSenzaNotifica(); 
        schermoTerminale.text = messaggioErrore + "\n\n[RESETTING CIRCUITI]";
        yield return new WaitForSeconds(4f);
        schermoTerminale.text = (levaStatus != null && levaStatus.isOn) ? "[DISPLAY: ONLINE]\nPOWER: OFFLINE\nMEM: NULL\nALU: SLEEP\n\nAWAITING POWER..." : "";
        inTransizione = false;
    }

    IEnumerator PassaggioFase(StatoTask nuovaFase, float tempoAttesa)
    {
        inTransizione = true;
        yield return new WaitForSeconds(tempoAttesa); 
        taskCorrente = nuovaFase;
        AggiornaPopupObiettivo();
        SpegniTutteLeLeveSenzaNotifica();
        schermoTerminale.text = (levaStatus != null && levaStatus.isOn) ? "[DISPLAY: ONLINE]\nPOWER: OFFLINE\nMEM: NULL\nALU: SLEEP\n\nAWAITING POWER..." : "";
        inTransizione = false;
    }

    void SpegniTutteLeLeveSenzaNotifica()
    {
        if (levaPower != null) levaPower.SetIsOnWithoutNotify(false);
        if (levaInputA != null) levaInputA.SetIsOnWithoutNotify(false); 
        if (levaInputB != null) levaInputB.SetIsOnWithoutNotify(false); 
        if (levaProcess != null) levaProcess.SetIsOnWithoutNotify(false); 
        if (levaOutput != null) levaOutput.SetIsOnWithoutNotify(false); 
        if (levaLoopExecute != null) levaLoopExecute.SetIsOnWithoutNotify(false);
    }

    public void ApriHelp()
    {
        if (inTransizione && livelloCompletato) return; 
        
        if (pannelloHelp == null || testoHelp == null) return;

        pannelloHelp.SetActive(true);
        switch (taskCorrente)
        {
            case StatoTask.Facile:
                testoHelp.text = "MANUALE OPERATIVO - FASE 1 (Sequenza)\n\nGUIDA:\nImpara il ciclo macchina di Von Neumann: alimenta il sistema, inserisci i dati nei registri di input, esegui il calcolo tramite l'Unita' Aritmetico-Logica (ALU) e finalizza con la stampa.\n\nCENNI STORICI:\nNei primi computer a valvole degli anni '40, non esistevano monitor o tastiere interattive. Le programmatrici riconfiguravano fisicamente cavi e interruttori elettrici per ogni singola operazione matematica.";
                break;
            case StatoTask.Media:
                testoHelp.text = "MANUALE OPERATIVO - FASE 2 (Il Salto Condizionato)\n\nGUIDA:\nIl computer deve prendere decisioni autonome in base ai dati calcolati. Se il risultato e' negativo, sfrutta la leva EXECUTE per deviare il flusso ed evitare la stampa.\n\nCENNI STORICI:\nL'introduzione del 'Branching' (salto condizionato) ha permesso ai computer di smettere di essere semplici calcolatrici e diventare macchine logiche in grado di eseguire scelte basate sullo stato interno.";
                break;
            case StatoTask.Difficile:
                testoHelp.text = "MANUALE OPERATIVO - FASE 3 (L'Accumulatore)\n\nGUIDA:\nConfigura i parametri di incremento e lascia che l'hardware esegua iterazioni multiple accumulando i risultati parziali in memoria.\n\nCENNI STORICI:\nPrima dei linguaggi evoluti, i cicli ripetitivi venivano gestiti salvando temporaneamente i totali parziali in registri dedicati chiamati appunto 'Accumulatori'.";
                break;
        }
    }
    
    // Ritorno al menu sfumando a NERO
    public void TornaAlMenu()
    {
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

    public void ChiudiHelp()
    {
        if (pannelloHelp != null) pannelloHelp.SetActive(false);
    }

    void AggiornaPopupObiettivo()
    {
        if (testoObiettivo == null) return;

        switch (taskCorrente)
        {
            case StatoTask.Facile: 
                testoObiettivo.text = "OBIETTIVO 1: ARCHITETTURA DI BASE\nAccendi STATUS e POWER. Carica i registri A e B, processa i dati con l'ALU e stampa il risultato."; 
                break;
            case StatoTask.Media: 
                testoObiettivo.text = "OBIETTIVO 2: SALTO CONDIZIONATO\nElabora i dati negativi. Ometti l'OUTPUT e aziona EXECUTE per applicare il controllo di flusso."; 
                break;
            case StatoTask.Difficile: 
                testoObiettivo.text = "OBIETTIVO 3: CICLO HARDWARE\nImposta la routine di incremento e tira EXECUTE per avviare il calcolo iterativo automatico."; 
                break;
        }
    }
}