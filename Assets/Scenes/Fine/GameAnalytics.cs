using UnityEngine;

public static class GameAnalytics
{
    public static float tempoInizioPartita;
    public static int erroriTotali = 0;
    public static int aiutiUsati = 0;

    // Chiama questo quando premi "Inizia Partita" nel Menu
    public static void ResetStatistiche()
    {
        tempoInizioPartita = Time.time;
        erroriTotali = 0;
        aiutiUsati = 0;
    }

    public static void RegistraErrore() { erroriTotali++; }
    public static void RegistraAiuto() { aiutiUsati++; }

    // Calcola e formatta il tempo finale per la scena "Fine"
    public static string OttieniReportFinale()
    {
        float tempoTotale = Time.time - tempoInizioPartita;
        int minuti = Mathf.FloorToInt(tempoTotale / 60F);
        int secondi = Mathf.FloorToInt(tempoTotale % 60F);

        return $"<b>REPORT TEMPORALE DI MISSIONE:</b>\n" +
               $"• Tempo totale: <color=#00FF66>{minuti}m {secondi}s</color>\n" +
               $"• Anomalie/Errori generati: <color=#FF5555>{erroriTotali}</color>\n" +
               $"• Consultazioni Manuale (Help): <color=#55AAFF>{aiutiUsati}</color>\n\n" +
               $"<size=80%><i>(Ricopia questi tre dati nelle prime domande del Google Form!)</i></size>";
    }
}