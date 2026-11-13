using NUnit.Framework;

public class PromptClassifierLivello4Tests
{
    [TestCase(Livello4Manager.StatoTask.Facile, "Somma due numeri", Livello4Manager.EsitoPromptLivello4.Valido)]
    [TestCase(Livello4Manager.StatoTask.Facile, "Aggiungi 2 valori", Livello4Manager.EsitoPromptLivello4.Valido)]
    [TestCase(Livello4Manager.StatoTask.Facile, "Somma numeri", Livello4Manager.EsitoPromptLivello4.RichiestaAmbigua)]
    [TestCase(Livello4Manager.StatoTask.Facile, "La sommaia di due numeri", Livello4Manager.EsitoPromptLivello4.NonRiconosciuto)]
    [TestCase(Livello4Manager.StatoTask.Media, "Dividi per zero e gestisci l'errore", Livello4Manager.EsitoPromptLivello4.Valido)]
    [TestCase(Livello4Manager.StatoTask.Media, "Esegui una divisione", Livello4Manager.EsitoPromptLivello4.GestioneErroreMancante)]
    [TestCase(Livello4Manager.StatoTask.Media, "Una divisibilità sicura", Livello4Manager.EsitoPromptLivello4.NonRiconosciuto)]
    [TestCase(Livello4Manager.StatoTask.Difficile, "Crea un ciclo for con limite 10", Livello4Manager.EsitoPromptLivello4.Valido)]
    [TestCase(Livello4Manager.StatoTask.Difficile, "Crea un ciclo for con un limite", Livello4Manager.EsitoPromptLivello4.LimiteQuantitativoMancante)]
    [TestCase(Livello4Manager.StatoTask.Difficile, "Crea una forma geometrica", Livello4Manager.EsitoPromptLivello4.NonRiconosciuto)]
    [TestCase(Livello4Manager.StatoTask.Difficile, "", Livello4Manager.EsitoPromptLivello4.NonRiconosciuto)]
    public void Valuta_RiconoscePromptValidiEAmbigui(
        Livello4Manager.StatoTask task,
        string prompt,
        Livello4Manager.EsitoPromptLivello4 esitoAtteso)
    {
        Assert.That(Livello4Manager.PromptClassifierLivello4.Valuta(task, prompt), Is.EqualTo(esitoAtteso));
    }
}
