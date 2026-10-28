# Chrono-Compile

![Unity](https://img.shields.io/badge/Unity-6000.3.23f1-black?style=for-the-badge&logo=unity&logoColor=white)
![C#](https://img.shields.io/badge/C%23-Unity-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![Input System](https://img.shields.io/badge/Input%20System-1.20.1-blue?style=for-the-badge)

Chrono-Compile is an educational puzzle game developed as part of a Master's
thesis in Computer Science at the University of Camerino. The project examines
how the historical development of programming paradigms can be presented
through an interactive learning environment, with particular attention to
cognitive load during the acquisition of programming concepts.

The game presents four stages of computing history. Each stage uses a
different interaction model and focuses on a specific transition in the
relationship between programmers, programs, and machines.

## Game Structure

| Scene | Historical context | Main activity |
| --- | --- | --- |
| `LV1` | 1940s: Von Neumann architecture | Operating a simulated computer through power, input, processing, output, and execution controls |
| `LV2` | 1960s-1970s: Structured programming | Rewriting programs based on line numbers, `GOTO`, conditional execution, and loops |
| `LV3` | 1990s: Virtual machines and visual programming | Assembling code blocks, compiling a program, and selecting a compatible target platform |
| `LV4` | 2020s: Artificial intelligence and prompt engineering | Writing prompts that specify operations, error handling, and bounded iteration |
| `Fine` | Final assessment | Reviewing game statistics and achievements, with access to the thesis questionnaire |

Each level contains three progressively more demanding tasks. Contextual help,
explicit objectives, terminal output, error messages, and debriefing text are
used to support the learning process without removing the problem-solving
component.

## Game Structure and Progression

The game is structured as a chronological progression through four historical
paradigms. The menu provides access to a new session, individual levels, the
achievement panel, and the thesis questionnaire. During a new session,
`GameAnalytics` resets the session data and the player begins with `LV1`.

Each level follows the same broad pattern:

1. an introductory transition presents the historical period;
2. three tasks increase in difficulty and introduce a related programming
   concept;
3. the player receives feedback through the interface, terminal output, and
   contextual help;
4. a debriefing summarizes the transition to the next period;
5. the temporal progress bar and level statistics are updated.

The levels use different interaction models rather than presenting the same
exercise in different visual settings. This is intended to make changes in
programming practice tangible: direct machine operation becomes structured
code, then abstracted execution through a virtual machine, and finally
natural-language specification.

## Level Environments

The visual design of each level reflects its historical setting and
interaction model.

| LV1 — Early Computing | LV2 — Structured Programming |
| --- | --- |
| <img src="Assets/Scenes/LV1/Immage/LV1.jpeg" alt="LV1 background" width="420"> | <img src="Assets/Scenes/LV2/Immage/LV2.jpg" alt="LV2 background" width="420"> |

| LV3 — Virtual Machines and Visual Programming | LV4 — Artificial Intelligence and Prompt Engineering |
| --- | --- |
| <img src="Assets/Scenes/LV3/Immage/LV3.jpg" alt="LV3 background" width="420"> | <img src="Assets/Scenes/LV4/Immage/LV4.png" alt="LV4 background" width="420"> |

## Educational Framework

Chrono-Compile applies game-based learning to introductory programming
education. Its progression moves from direct machine control to increasingly
abstract forms of programming:

1. execution of operations through hardware-like controls;
2. control flow and the limitations of unstructured code;
3. abstraction, compilation, scope, and platform compatibility;
4. specification and error handling through natural-language prompts.

The design is informed by the cognitive load model described by Leppink. The
interface provides structured objectives and immediate feedback to limit
unnecessary cognitive demands, while the tasks retain the intrinsic difficulty
of sequencing, control flow, scope, platform constraints, and edge cases.
Visual objectives, execution states, and feedback also provide a form of
cognitive offloading by making processes that would otherwise remain invisible
more explicit to the player.

The final scene records the results of the session and provides a link to the
questionnaire used for the empirical component of the thesis.

## Achievements and Session Analytics

Achievements are integrated into the progression rather than being limited to
an external scoring system. They provide additional feedback on the player's
actions and encourage attention to both successful solutions and mistakes.
Achievements are grouped into the following categories:

- **Stage achievements:** one for each of the three tasks in every level.
- **Timed achievements:** awarded for completing a level within the defined
  time threshold.
- **Level-specific achievements:** tied to behaviours such as making an error,
  avoiding the help system, or completing a level after repeated failed
  attempts.
- **Global achievements:** awarded for events across the whole session, such
  as the first error, accumulating several errors, completing all levels
  without using help, or completing the game without errors.

`GameAnalytics.cs` records elapsed time, total errors, help requests, level
completion, and unlocked achievements. The final scene presents these data as
a session report. The statistics are session-based and are not intended to
replace the questionnaires or the learning-outcome measures used in the
research study.

## Thesis Study and Research Objectives

The educational objective is to connect the historical development of
programming tools with direct interaction. The game is designed to let
students experience how successive abstractions can reduce coding friction
while introducing new demands, such as control-flow reasoning, scope,
platform compatibility, error handling, and precise specification.

The planned study focuses on students following secondary-school computer
science curricula. It compares a game-based learning condition with a
traditional instructional condition and addresses three research questions:

- **RQ1 — Learning outcomes:** To what extent does the game-based prototype
  affect students' learning outcomes compared with traditional instruction?
- **RQ2 — Motivation and engagement:** How does the game-based condition
  affect intrinsic motivation and task engagement?
- **RQ3 — Cognitive load:** How does perceived cognitive load differ between
  the two learning conditions, and how does it relate to learning outcomes?

The evaluation uses learning-outcome measures together with standardized
engagement questionnaires and a cognitive-load instrument based on Leppink
et al. The study is intended to examine whether a playful learning approach
can reduce perceived mental fatigue without reducing the conceptual demands of
the activities.

The project is also motivated by a limitation identified in the related
literature: positive results from educational games do not necessarily explain
how individual mechanics create or reduce didactic friction and cognitive
overload. Chrono-Compile therefore makes the mechanics visible and records
in-game indicators such as errors, help requests, and completion time. These
indicators are complementary to, and not substitutes for, the formal
evaluation instruments.

## Use of AI-Generated Images

Gemini and ChatGPT were used to generate some of the visual assets included in
the project. This approach was adopted because of the limited development
time, the available budget, and the difficulty of finding suitable historical
assets online.

The generated images were selected, edited when necessary, and integrated into
the Unity scenes as part of the project development process. AI was used for
image generation only; it was not used to implement the game logic or to make
the design and evaluation decisions.

## Main Features

- Four chronological programming scenarios with different interaction models.
- Three difficulty stages in each level.
- Contextual help panels and objective prompts.
- Terminal-style output, typewriter text, generated code previews, and
  simulated AI chat.
- Scene transitions, period introductions, fade effects, button animations,
  and screen-shake feedback.
- Session statistics, including elapsed time, errors, and help usage.
- Per-stage, per-level, timed, and global achievements.
- Final report with a summary of statistics and unlocked achievements.
- Menu navigation for starting a new session, selecting levels, reviewing
  achievements, opening the thesis questionnaire, and exiting the application.

## Requirements

- Unity **6000.3.23f1** (Unity 6)
- Unity Hub
- Git
- An operating system supported by the selected Unity version

The project uses Unity Input System, TextMesh Pro, UGUI, Timeline, Visual
Scripting, Unity AI Assistant, and Unity AI Inference. Package versions are
specified in [`Packages/manifest.json`](Packages/manifest.json).

## Getting Started

Clone the repository and open it with the required Unity version:

```bash
git clone https://github.com/iamShallo/Chrono-Compile.git
cd Chrono-Compile
```

Open the project through Unity Hub, then open the `Menu` scene and enter Play
Mode. The scenes used by the game are listed in
[`ProjectSettings/EditorBuildSettings.asset`](ProjectSettings/EditorBuildSettings.asset):

```text
Menu -> LV1 -> LV2 -> LV3 -> LV4 -> Fine
```

To create a build, confirm that these scenes are included in the build
configuration and appear in the order shown above. The `Library/` directory is
generated by Unity and is not required when cloning or restoring the project.

## Repository Layout

```text
Chrono-Compile/
├── Assets/
│   ├── Scenes/
│   │   ├── Menù/
│   │   │   ├── Menu.unity
│   │   │   └── Script/
│   │   │       ├── AchievementMenu.cs
│   │   │       ├── BottoneAnimato.cs
│   │   │       └── MenuManager.cs
│   │   ├── LV1/
│   │   │   ├── LV1.unity
│   │   │   └── Script/Livello1Manager.cs
│   │   ├── LV2/
│   │   │   ├── LV2.unity
│   │   │   └── Script/Livello2Manager.cs
│   │   ├── LV3/
│   │   │   ├── LV3.unity
│   │   │   └── Script/Livello3Manager.cs
│   │   ├── LV4/
│   │   │   ├── LV4.unity
│   │   │   └── Script/Livello4Manager.cs
│   │   └── Fine/
│   │       ├── Fine.unity
│   │       ├── GameAnalytics.cs
│   │       └── Livello5Manager.cs
├── Packages/
├── ProjectSettings/
└── README.md
```

### Core Scripts

- `MenuManager` controls scene navigation, level selection, loading fades,
  achievements, and external links.
- `Livello1Manager` through `Livello4Manager` implement the level-specific
  tasks, user interface, help system, feedback, and transitions.
- `GameAnalytics` maintains session statistics, achievement state, and the
  final reports.
- `Livello5Manager` presents the final report and handles the return to the
  main menu.
- `AchievementNotification` displays achievements when they are unlocked
  during a level.

## Project Status

The repository contains a playable Unity prototype covering the complete
sequence from the main menu to the final scene. The questionnaire link is
included in the application for the thesis data-collection phase. Empirical
analysis of the collected responses is outside the runtime project.

## License

The repository does not currently include a license file. A license should be
added before distributing the project outside its thesis context.
