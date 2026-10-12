# 🚀 Dalla Scheda Perforata all'IA: L'Evoluzione dei Paradigmi di Programmazione e del Carico Cognitivo

[![Unity](https://img.shields.io/badge/Unity-6000.0+-black?style=for-the-badge&logo=unity&logoColor=white)](https://unity.com/)
[![C#](https://img.shields.io/badge/C%23-Code-239120?style=for-the-badge&logo=c-sharp&logoColor=white)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

Progetto di tesi magistrale in **Computer Science (Università di Camerino)** incentrato sull'analisi del carico cognitivo (basato sul modello di Leppink) applicato all'apprendimento della programmazione informatica attraverso un'esperienza videoludica educativa a 4 livelli storici.

---

## 🎮 Panoramica del Gioco

Il videogioco guida lo studente attraverso un viaggio temporale nell'evoluzione dei linguaggi e degli strumenti di sviluppo, dimostrando come il **carico cognitivo estraneo** si sia ridotto nel tempo, spostando la sfida mentale (*carico intrinseco*) verso la logica e la precisione formale:

1. **Livello 1 & 2 (Anni '60-'70):** Programmazione sequenziale di base (GOTO / logiche procedurali primordiali).
2. **Livello 3 (Anni '90):** Introduzione della programmazione a blocchi strutturati, della Java Virtual Machine (WORA) e dei concetti di Scope ed Eccezioni.
3. **Livello 4 (2020+):** L'era dell'Intelligenza Artificiale e del **Prompt Engineering**. La sintassi scompare per lasciare spazio al linguaggio naturale, evidenziando i rischi dell'ambiguità e la necessità di gestire i limiti e gli *edge cases*.

---

## 🛠️ Architettura e Stack Tecnologico

- **Engine:** Unity 6 (UI Toolkit / Canvas Scaler con layout adattivi).
- **Linguaggio di Scripting:** C# (Gestione delle macchine a stati, coroutine avanzate per i feedback visivi ed elaborazione testuale dei prompt tramite keyword matching).
- **UI/UX:** Design in stile "Modern Dark Mode" con integrazione di fitte interfacce di chat (ScrollRect dinamiche con autoscroll) ed emulatori di terminale.

---

## 📂 Struttura del Progetto

```text
Assets/
│
├── Scenes/
│   ├── LV1/ & LV2/          # Moduli storici iniziali
│   ├── LV3/                 # Modulo Anni '90 (JVM & Visual Blocks)
│   │   └── Script/          # Livello3Manager.cs
│   └── LV4/                 # Modulo 2020+ (AI & Prompt Engineering)
│       └── Script/          # Livello4Manager.cs
│
└── Sprites & UI/            # Asset grafici, sfondi custom e layout UI
