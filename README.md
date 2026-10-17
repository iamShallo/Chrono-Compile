# Chrono-Compile: The Evolution of Programming Paradigms and Cognitive Load

[![Unity](https://img.shields.io/badge/Unity-6000.0+-black?style=for-the-badge&logo=unity&logoColor=white)](https://unity.com/)
[![C#](https://img.shields.io/badge/C%23-Code-239120?style=for-the-badge&logo=c-sharp&logoColor=white)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

This repository contains the source code and assets for **Chrono-Compile**, an educational, time-traveling puzzle game developed as a Master's thesis project in Computer Science at the University of Camerino. 

The software investigates the historical evolution of software development paradigms—ranging from early procedural systems to modern AI-assisted workflows—and evaluates their impact on learners' cognitive load.

---

## Background and Theoretical Framework

### Game-Based Learning in Computer Science Education
Game-Based Learning (GBL) integrates pedagogical frameworks into interactive environments to transform passive instruction into active problem-solving. In computer science education, foundational concepts such as syntax rules, control structures, and semantic constraints often impose significant barriers to entry. Chrono-Compile addresses this by embedding historical computational models into a chronological narrative, lowering initial friction while maintaining rigorous conceptual accuracy.

### Cognitive Load Theory (Leppink's Model)
The design of the simulation is grounded in Leppink's cognitive load model, focusing on the trade-offs between mental efforts during software learning:
- **Extraneous Cognitive Load:** The cognitive overhead resulting from poor interface design, low-level instruction management, and rigid syntactic syntax (prominent in early paradigms).
- **Intrinsic Cognitive Load:** The inherent difficulty associated with the problem domain itself (e.g., algorithmic logic and logical constraints), which shifts to the forefront as developer tools evolve.

---

## Evaluation and Empirical Testing

To measure the educational efficacy of the application, an empirical testing phase is structured for high school curricula focused on Information Technology and Telecommunications:
- **Research Objective:** To analyze whether a gamified, multi-era learning progression yields higher academic performance and reduces mental fatigue compared to traditional lecture methodologies.
- **Methodology:** Administration of standardized cognitive load questionnaires based on Leppink's model, administered before and after student interaction with the software modules.

---

## System Architecture and Levels

The application flow is managed via modular C# controllers and structured across distinct scenes:

1. **Menu Scene (`Menu`):** The main entry point featuring dynamic background switching, level selection, options, and direct integration with the thesis data-collection form.
2. **LV1 & LV2 (The Early Era):** Procedural logic and low-level instruction execution, emphasizing strict structural and syntactic constraints (`Livello1Manager.cs`, `Livello2Manager.cs`).
3. **LV3 (The 1990s - Object-Oriented & Virtual Machines):** Implements visual structured blocks, the Java Virtual Machine architecture, and scoping mechanisms (`Livello3Manager.cs`).
4. **LV4 (The 2020s - AI & Prompt Engineering):** Replaces traditional code composition with a conversational prompt interface (`Livello4Manager.cs`), focusing on high-order analytical precision and edge-case handling via natural language.
5. **Final Scene (`Fine`):** The concluding victory screen (`FineManager.cs`) acknowledging the completion of the time-traveling journey and providing direct access to the thesis Google Form for empirical evaluation.

---

## Project Structure

```text
Chrono-Compile/
│
├── Assets/
│   ├── Scenes/
│   │   ├── Menu/
│   │   │   └── Script/
│   │   │       └── MenuManager.cs       # Main menu and navigation controller
│   │   ├── LV1/
│   │   │   └── Script/
│   │   │       └── Livello1Manager.cs   # Procedural logic controller
│   │   ├── LV2/
│   │   │   └── Script/
│   │   │       └── Livello2Manager.cs   # Intermediate execution controller
│   │   ├── LV3/
│   │   │   └── Script/
│   │   │       └── Livello3Manager.cs   # JVM and visual block evaluation engine
│   │   ├── LV4/
│   │   │   └── Script/
│   │   │       └── Livello4Manager.cs   # Natural language prompt validation engine
│   │   └── Fine/
│   │       └── Script/
│   │           └── FineManager.cs       # Conclusion and feedback form controller
│   │
│   ├── TextMeshPro/
│   └── Settings/
│
├── Packages/
└── ProjectSettings/
