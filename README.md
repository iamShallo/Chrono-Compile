# ⏳ Chrono-Compile: The Evolution of Programming Paradigms & Cognitive Load

[![Unity](https://img.shields.io/badge/Unity-6000.0+-black?style=for-the-badge&logo=unity&logoColor=white)](https://unity.com/)
[![C#](https://img.shields.io/badge/C%23-Code-239120?style=for-the-badge&logo=c-sharp&logoColor=white)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

An educational, time-traveling puzzle game developed as a Master's degree thesis project in **Computer Science at the University of Camerino**. 

**Chrono-Compile** explores the evolution of programming paradigms from the 1960s to the AI-driven 2020s, analyzing how the shift in tools impacts human **cognitive load** (based on Leppink's cognitive load model) throughout programming history.

---

## 🎮 Game Architecture & Levels

The game is structured into four distinct chronological milestones, each shifting the cognitive burden from mechanical syntax memorization to high-level abstract logic and prompt engineering:

*   **LV1 & LV2 (The Early Days):** Focuses on procedural logic and early sequential programming constraints.
*   **LV3 (The 90s - Object-Oriented & Virtual Machines):** Introduces visual structured blocks, the Java Virtual Machine (WORA paradigm), and scoping mechanisms managed via `Livello3Manager.cs`.
*   **LV4 (2020+ - AI & Prompt Engineering):** Replaces rigid syntax with a modern conversational interface mimicking AI assistants. Students learn to handle edge cases, ambiguity, and strict logical constraints using natural language via `Livello4Manager.cs`.

---

## 📂 Project Structure

```text
Chrono-Compile/
│
├── Assets/
│   ├── Scenes/
│   │   ├── LV1/
│   │   │   └── Script/
│   │   │       └── Livello1Manager.cs   #[cite: 3] Early procedural logic controller
│   │   ├── LV2/
│   │   │   └── Script/
│   │   │       └── Livello2Manager.cs   #[cite: 3] Intermediate mechanics controller
│   │   ├── LV3/
│   │   │   └── Script/
│   │   │       └── Livello3Manager.cs   #[cite: 3] 90s JVM & visual blocks engine
│   │   └── LV4/
│   │       └── Script/
│   │           └── Livello4Manager.cs   #[cite: 3] Modern AI chat & prompt validation engine
│   │
│   ├── TextMeshPro/
│   └── Settings/
│
├── Packages/
└── ProjectSettings/
