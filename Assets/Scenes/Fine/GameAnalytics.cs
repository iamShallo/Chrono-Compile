using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public static class GameAnalytics
{
    private const int NumeroLivelli = 4;
    private const int FasiPerLivello = 3;
    private const int AchievementExtraPerLivello = 1;
    private const int AchievementSpecialiPerLivello = 1;
    private const int AchievementSpeciali = 4;
    private static readonly bool[,] achievementOttenuti = new bool[NumeroLivelli, FasiPerLivello + AchievementExtraPerLivello + AchievementSpecialiPerLivello];
    private static readonly bool[] achievementSpecialiOttenuti = new bool[AchievementSpeciali];
    private static readonly float[] inizioLivello = new float[NumeroLivelli];
    private static readonly float[] tempiLivelli = new float[NumeroLivelli];
    private static readonly bool[] livelloCompletato = new bool[NumeroLivelli];
    public static event Action<string> AchievementSbloccato;

    private static readonly string[,] nomiAchievement =
    {
        { "LV1 - Ciclo di Von Neumann", "LV1 - Salto condizionato", "LV1 - Hardware completato" },
        { "LV2 - GOTO eliminato", "LV2 - Codice strutturato", "LV2 - Ciclo FOR" },
        { "LV3 - Pipeline compilata", "LV3 - Bivio multi-piattaforma", "LV3 - Loop nello scope" },
        { "LV4 - Somma richiesta", "LV4 - Errore gestito", "LV4 - Loop specificato" }
    };

    private static readonly string[,] nomiAchievementExtra =
    {
        { "LV1 - Il fulmine ha paura" },
        { "LV2 - GOTO supersonico" },
        { "LV3 - Compilatore indemoniato" },
        { "LV4 - Prompt telepatico" }
    };

    private static readonly string[] nomiAchievementSpecialiLivello =
    {
        "LV1 - Panico da interruttore",
        "LV2 - GOTO? Mai visto",
        "LV3 - Loop mentale",
        "LV4 - Zero assoluto"
    };

    private static readonly string[] descrizioniAchievementSpecialiLivello =
    {
        "Completa il livello dopo aver commesso almeno un errore con le leve.",
        "Completa il livello senza usare il manuale: il GOTO non sapra' mai che esisti.",
        "Completa il livello dopo aver commesso almeno 3 errori: il loop e' nella tua testa.",
        "Completa il livello dopo aver provocato almeno un errore di divisione per zero."
    };

    private static readonly string[] nomiAchievementSpeciali =
    {
        "Errore 404: pazienza non trovata",
        "Bug magnete",
        "Manuale? Mai aperto",
        "Compilatore zen"
    };

    private static readonly string[] descrizioniAchievementSpeciali =
    {
        "Commetti il tuo primo errore: anche il computer ha bisogno di presentazioni.",
        "Accumula 5 errori tipici tra leve, codice e prompt. Il bug ormai ti riconosce.",
        "Completa tutti i livelli senza aprire neanche una volta il manuale Help.",
        "Completa tutti i livelli senza commettere errori. La calma e' una feature."
    };

    private static readonly string[,] descrizioniAchievement =
    {
        { "Attiva correttamente status, power, input, processo e output.", "Imposta le leve nell'ordine corretto per eseguire il salto condizionato.", "Completa tutte le fasi hardware del livello 1." },
        { "Riscrivi il programma senza usare istruzioni GOTO.", "Completa il programma con una struttura chiara e leggibile.", "Usa un ciclo FOR funzionante per risolvere la fase." },
        { "Completa la fase della pipeline senza lasciare istruzioni incomplete.", "Scegli il percorso compatibile con la piattaforma richiesta.", "Risolvi il loop mantenendo tutte le istruzioni nel loro scope." },
        { "Inserisci il valore richiesto e completa la fase di somma.", "Gestisci correttamente l'errore di divisione per zero.", "Specifica e completa il loop richiesto dal prompt." }
    };

    public static float tempoInizioPartita;
    public static int erroriTotali = 0;
    public static int aiutiUsati = 0;

    // Chiama questo quando premi "Inizia Partita" nel Menu
    public static void ResetStatistiche()
    {
        tempoInizioPartita = Time.time;
        erroriTotali = 0;
        aiutiUsati = 0;
        for (int livello = 0; livello < NumeroLivelli; livello++)
        {
            for (int fase = 0; fase < FasiPerLivello; fase++)
            {
                for (int achievement = 0; achievement < FasiPerLivello + AchievementExtraPerLivello + AchievementSpecialiPerLivello; achievement++)
                    achievementOttenuti[livello, achievement] = false;
                inizioLivello[livello] = 0f;
                tempiLivelli[livello] = 0f;
                livelloCompletato[livello] = false;
            }
        }
        for (int achievement = 0; achievement < AchievementSpeciali; achievement++)
            achievementSpecialiOttenuti[achievement] = false;
    }

    public static void RegistraErrore()
    {
        erroriTotali++;
        if (erroriTotali == 1)
            SbloccaAchievementSpeciale(0);
        if (erroriTotali == 5)
            SbloccaAchievementSpeciale(1);
    }

    public static void RegistraAiuto()
    {
        aiutiUsati++;
    }

    public static void SbloccaAchievement(int livello, int fase)
    {
        if (livello < 1 || livello > NumeroLivelli || fase < 1 || fase > FasiPerLivello)
            return;

        if (achievementOttenuti[livello - 1, fase - 1])
            return;

        achievementOttenuti[livello - 1, fase - 1] = true;
        AchievementSbloccato?.Invoke(OttieniNomeAchievement(livello - 1, fase - 1));
    }

    public static void IniziaLivello(int livello)
    {
        if (livello < 1 || livello > NumeroLivelli || livelloCompletato[livello - 1])
            return;

        if (inizioLivello[livello - 1] <= 0f)
            inizioLivello[livello - 1] = Time.time;
    }

    public static void CompletaLivello(int livello)
    {
        if (livello < 1 || livello > NumeroLivelli || livelloCompletato[livello - 1])
            return;

        IniziaLivello(livello);
        tempiLivelli[livello - 1] = Mathf.Max(0f, Time.time - inizioLivello[livello - 1]);
        livelloCompletato[livello - 1] = true;

        // Achievement volutamente eccentrici: rapidita' estrema o completamento.
        if (tempiLivelli[livello - 1] <= 90f)
            SbloccaAchievementTemporale(livello - 1, 0);
        if (livello == 1 && erroriTotali >= 1)
            SbloccaAchievementSpecialeLivello(0);
        if (livello == 2 && aiutiUsati == 0)
            SbloccaAchievementSpecialeLivello(1);
        if (livello == 3 && erroriTotali >= 3)
            SbloccaAchievementSpecialeLivello(2);
        if (livello == 4 && erroriTotali >= 1)
            SbloccaAchievementSpecialeLivello(3);
        if (livello == NumeroLivelli && aiutiUsati == 0)
            SbloccaAchievementSpeciale(2);
        if (livello == NumeroLivelli && erroriTotali == 0)
            SbloccaAchievementSpeciale(3);
    }

    private static void SbloccaAchievementTemporale(int livello, int indice)
    {
        int indiceAchievement = FasiPerLivello + indice;
        if (achievementOttenuti[livello, indiceAchievement])
            return;

        achievementOttenuti[livello, indiceAchievement] = true;
        AchievementSbloccato?.Invoke(nomiAchievementExtra[livello, indice]);
    }

    private static void SbloccaAchievementSpecialeLivello(int livello)
    {
        int indiceAchievement = FasiPerLivello + AchievementExtraPerLivello;
        if (livello < 0 || livello >= NumeroLivelli || achievementOttenuti[livello, indiceAchievement])
            return;

        achievementOttenuti[livello, indiceAchievement] = true;
        AchievementSbloccato?.Invoke(nomiAchievementSpecialiLivello[livello]);
    }

    private static string OttieniNomeAchievement(int livello, int fase)
    {
        return nomiAchievement[livello, fase];
    }

    private static void SbloccaAchievementSpeciale(int indice)
    {
        if (indice < 0 || indice >= AchievementSpeciali || achievementSpecialiOttenuti[indice])
            return;

        achievementSpecialiOttenuti[indice] = true;
        AchievementSbloccato?.Invoke(nomiAchievementSpeciali[indice]);
    }

    private static string FormattaTempo(float tempo)
    {
        int minuti = Mathf.FloorToInt(tempo / 60f);
        int secondi = Mathf.FloorToInt(tempo % 60f);
        return $"{minuti}m {secondi:00}s";
    }

    public static string OttieniReportAchievement()
    {
        int ottenuti = 0;
        int totale = NumeroLivelli * (FasiPerLivello + AchievementExtraPerLivello + AchievementSpecialiPerLivello) + AchievementSpeciali;
        System.Text.StringBuilder report = new System.Text.StringBuilder("<b>ACHIEVEMENT CHRONO::COMPILE :</b>\n");

        for (int livello = 0; livello < NumeroLivelli; livello++)
        {
            report.AppendLine($"\n<color=#55AAFF>Livello {livello + 1}</color> <size=80%>({FormattaTempo(tempiLivelli[livello])})</size>");
            for (int fase = 0; fase < FasiPerLivello; fase++)
            {
                bool ottenuto = achievementOttenuti[livello, fase];
                if (ottenuto) ottenuti++;
                report.AppendLine(ottenuto
                    ? $"<color=#00FF66>[X] {nomiAchievement[livello, fase]} - {descrizioniAchievement[livello, fase]}</color>"
                    : $"<color=#777777>[ ] {nomiAchievement[livello, fase]} (mancante) - {descrizioniAchievement[livello, fase]}</color>");
            }
            for (int achievement = 0; achievement < AchievementExtraPerLivello; achievement++)
            {
                bool ottenuto = achievementOttenuti[livello, FasiPerLivello + achievement];
                if (ottenuto) ottenuti++;
                report.AppendLine(ottenuto
                    ? $"<color=#00FF66>[X] {nomiAchievementExtra[livello, achievement]} - {OttieniDescrizioneExtra(achievement)}</color>"
                    : $"<color=#777777>[ ] {nomiAchievementExtra[livello, achievement]} (mancante) - {OttieniDescrizioneExtra(achievement)}</color>");
            }
            bool specialeLivelloOttenuto = achievementOttenuti[livello, FasiPerLivello + AchievementExtraPerLivello];
            if (specialeLivelloOttenuto) ottenuti++;
            report.AppendLine(specialeLivelloOttenuto
                ? $"<color=#00FF66>[X] {nomiAchievementSpecialiLivello[livello]} - {descrizioniAchievementSpecialiLivello[livello]}</color>"
                : $"<color=#777777>[ ] {nomiAchievementSpecialiLivello[livello]} (mancante) - {descrizioniAchievementSpecialiLivello[livello]}</color>");
        }

        report.AppendLine("\n<color=#55AAFF>Achievement speciali</color>");
        for (int achievement = 0; achievement < AchievementSpeciali; achievement++)
        {
            bool ottenuto = achievementSpecialiOttenuti[achievement];
            if (ottenuto)
                ottenuti++;

            report.AppendLine(ottenuto
                ? $"<color=#00FF66>[X] {nomiAchievementSpeciali[achievement]} - {descrizioniAchievementSpeciali[achievement]}</color>"
                : $"<color=#777777>[ ] {nomiAchievementSpeciali[achievement]} (mancante) - {descrizioniAchievementSpeciali[achievement]}</color>");
        }

        report.Insert(0, $"<size=85%><color=#00FF66>{ottenuti}/{totale} ottenuti</color>\n\n");
        report.Append("</size>");
        return report.ToString();
    }

    public static string OttieniReportAchievementFinale()
    {
        int ottenuti = 0;
        int totale = NumeroLivelli * (FasiPerLivello + AchievementExtraPerLivello + AchievementSpecialiPerLivello) + AchievementSpeciali;
        System.Text.StringBuilder report = new System.Text.StringBuilder("<b>ACHIEVEMENT CHRONO::COMPILE :</b>\n");

        for (int livello = 0; livello < NumeroLivelli; livello++)
        {
            report.AppendLine($"\n<color=#55AAFF>Livello {livello + 1}</color>");
            for (int fase = 0; fase < FasiPerLivello; fase++)
            {
                bool ottenuto = achievementOttenuti[livello, fase];
                if (ottenuto) ottenuti++;
                report.AppendLine(ottenuto
                    ? $"<color=#00FF66>[X] {nomiAchievement[livello, fase]}</color>"
                    : $"<color=#777777>[ ] {nomiAchievement[livello, fase]} (mancante)</color>");
            }

            for (int achievement = 0; achievement < AchievementExtraPerLivello; achievement++)
            {
                bool ottenuto = achievementOttenuti[livello, FasiPerLivello + achievement];
                if (ottenuto) ottenuti++;
                report.AppendLine(ottenuto
                    ? $"<color=#00FF66>[X] {nomiAchievementExtra[livello, achievement]}</color>"
                    : $"<color=#777777>[ ] {nomiAchievementExtra[livello, achievement]} (mancante)</color>");
            }
            bool specialeLivelloOttenuto = achievementOttenuti[livello, FasiPerLivello + AchievementExtraPerLivello];
            if (specialeLivelloOttenuto) ottenuti++;
            report.AppendLine(specialeLivelloOttenuto
                ? $"<color=#00FF66>[X] {nomiAchievementSpecialiLivello[livello]}</color>"
                : $"<color=#777777>[ ] {nomiAchievementSpecialiLivello[livello]} (mancante)</color>");
        }

        report.AppendLine("\n<color=#55AAFF>Achievement speciali</color>");
        for (int achievement = 0; achievement < AchievementSpeciali; achievement++)
        {
            bool ottenuto = achievementSpecialiOttenuti[achievement];
            if (ottenuto) ottenuti++;
            report.AppendLine(ottenuto
                ? $"<color=#00FF66>[X] {nomiAchievementSpeciali[achievement]}</color>"
                : $"<color=#777777>[ ] {nomiAchievementSpeciali[achievement]} (mancante)</color>");
        }

        report.Insert(0, $"<size=85%><color=#00FF66>{ottenuti}/{totale} ottenuti</color>\n\n");
        report.Append("</size>");
        return report.ToString();
    }

    private static string OttieniDescrizioneExtra(int indice)
    {
        return indice == 0
            ? "Completa il livello in massimo 90 secondi."
            : "Completa il livello: questo achievement e' sempre ottenibile.";
    }

    public static string OttieniDescrizioniAchievement()
    {
        System.Text.StringBuilder descrizioni = new System.Text.StringBuilder();
        descrizioni.AppendLine("<b>COME OTTENERLI</b>\n");

        for (int livello = 0; livello < NumeroLivelli; livello++)
        {
            descrizioni.AppendLine($"<color=#55AAFF>Livello {livello + 1}</color>");
            for (int fase = 0; fase < FasiPerLivello; fase++)
                descrizioni.AppendLine($"<color=#FFFFFF>{nomiAchievement[livello, fase]}</color> - {descrizioniAchievement[livello, fase]}");
            descrizioni.AppendLine($"<color=#FFFFFF>{nomiAchievementExtra[livello, 0]}</color> - {OttieniDescrizioneExtra(0)}");
            descrizioni.AppendLine($"<color=#FFFFFF>{nomiAchievementSpecialiLivello[livello]}</color> - {descrizioniAchievementSpecialiLivello[livello]}");
            descrizioni.AppendLine();
        }

        descrizioni.AppendLine("<color=#55AAFF>Speciali e un po' assurdi</color>");
        for (int achievement = 0; achievement < AchievementSpeciali; achievement++)
            descrizioni.AppendLine($"<color=#FFFFFF>{nomiAchievementSpeciali[achievement]}</color> - {descrizioniAchievementSpeciali[achievement]}");

        return descrizioni.ToString();
    }

    // Calcola e formatta il tempo finale per la scena "Fine"
    public static string OttieniReportFinale()
    {
        float tempoTotale = Time.time - tempoInizioPartita;
        int minuti = Mathf.FloorToInt(tempoTotale / 60F);
        int secondi = Mathf.FloorToInt(tempoTotale % 60F);

        return $"<b>REPORT TEMPORALE DI MISSIONE:</b>\n" +
               $"• Tempo totale: <color=#00FF66>{minuti}m {secondi}s</color>\n" +
               $"• Anomalie/Errori generati: <color=#FF5555>{erroriTotali}</color>\n" +
               $"• Consultazioni Manuale (Help): <color=#55AAFF>{aiutiUsati}</color>";
    }

}

public class AchievementNotification : MonoBehaviour
{
    private readonly Queue<string> codaAchievement = new Queue<string>();
    [SerializeField] private GameObject pannelloAchievement;
    [SerializeField] private TextMeshProUGUI testoAchievement;
    private RectTransform pannello;
    private CanvasGroup gruppoPannello;
    private TextMeshProUGUI testo;
    private Coroutine animazione;
    private const float LarghezzaPannello = 460f;
    private bool registrato;
    private Vector2 posizioneVisibile;

    private void Awake()
    {
        if (pannelloAchievement != null && testoAchievement != null)
            Configura(pannelloAchievement, testoAchievement);
    }

    private void OnDestroy()
    {
        if (registrato)
            GameAnalytics.AchievementSbloccato -= MostraAchievement;
    }

    public void Configura(GameObject pannelloAssociato, TextMeshProUGUI testoAssociato)
    {
        pannelloAchievement = pannelloAssociato;
        testoAchievement = testoAssociato;
        pannello = pannelloAchievement != null ? pannelloAchievement.GetComponent<RectTransform>() : null;
        testo = testoAchievement;
        gruppoPannello = pannelloAchievement != null
            ? pannelloAchievement.GetComponent<CanvasGroup>()
            : null;

        if (pannello == null || testo == null)
        {
            Debug.LogWarning("AchievementNotification richiede un pannello e un testo associati nella scena.", this);
            enabled = false;
            return;
        }

        if (!registrato)
        {
            GameAnalytics.AchievementSbloccato += MostraAchievement;
            registrato = true;
        }

        posizioneVisibile = pannello.anchoredPosition;
        if (gruppoPannello == null)
            gruppoPannello = pannelloAchievement.AddComponent<CanvasGroup>();
        gruppoPannello.alpha = 0f;
        pannelloAchievement.SetActive(true);
    }

    private void MostraAchievement(string nomeAchievement)
    {
        codaAchievement.Enqueue(nomeAchievement);
        if (animazione == null)
            animazione = StartCoroutine(AnimazioniAchievement());
    }

    private IEnumerator AnimazioniAchievement()
    {
        while (codaAchievement.Count > 0)
        {
            testo.text = "<color=#FFD700><b>ACHIEVEMENT SBLOCCATO</b></color>\n" +
                         $"<color=#FFFFFF>{codaAchievement.Dequeue()}</color>";
            pannelloAchievement.SetActive(true);
            gruppoPannello.alpha = 0f;
            float ingresso = posizioneVisibile.x + LarghezzaPannello;
            float uscita = posizioneVisibile.x + LarghezzaPannello;
            yield return StartCoroutine(Scorri(
                new Vector2(ingresso, posizioneVisibile.y),
                posizioneVisibile,
                0f,
                1f,
                0.6f));
            yield return new WaitForSecondsRealtime(2.2f);
            yield return StartCoroutine(Scorri(posizioneVisibile, new Vector2(uscita, posizioneVisibile.y), 1f, 0f, 0.6f));
            gruppoPannello.alpha = 0f;
        }

        animazione = null;
    }

    private IEnumerator Scorri(Vector2 posizioneIniziale, Vector2 posizioneFinale, float alphaIniziale, float alphaFinale, float durata)
    {
        float tempoInizio = Time.unscaledTime;
        while (Time.unscaledTime < tempoInizio + durata)
        {
            float progresso = Mathf.Clamp01((Time.unscaledTime - tempoInizio) / durata);
            progresso = progresso * progresso * (3f - 2f * progresso);
            pannello.anchoredPosition = Vector2.Lerp(posizioneIniziale, posizioneFinale, progresso);
            gruppoPannello.alpha = Mathf.Lerp(alphaIniziale, alphaFinale, progresso);
            yield return null;
        }

        pannello.anchoredPosition = posizioneFinale;
        gruppoPannello.alpha = alphaFinale;
    }
}