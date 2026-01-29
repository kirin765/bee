<!-- Purpose: concise, repo-specific guidance for AI coding agents -->
# Copilot / AI agent instructions — bee (Unity)

Purpose: Help an AI agent become productive quickly in this Unity project by summarizing architecture, workflows, conventions, and important integration points.

- **Big picture:** This is a single Unity game project (Unity Editor 6000.3.2f1). Gameplay code lives under Assets/Scripts; data/config lives as ScriptableObjects under Assets/ScriptableObjects; scenes, prefabs and art are in the parallel top-level folders (Assets/Scenes, Assets/Prefabs, Assets/Art). The compiled project file is [Assembly-CSharp.csproj](Assembly-CSharp.csproj).

- **Core systems & files:**
  - Gameplay: [Assets/Scripts/Bee.cs](Assets/Scripts/Bee.cs), [Assets/Scripts/Bow.cs](Assets/Scripts/Bow.cs), [Assets/Scripts/Arrow.cs](Assets/Scripts/Arrow.cs), [Assets/Scripts/Spawner.cs](Assets/Scripts/Spawner.cs).
  - UI / state: [Assets/Scripts/GameOver.cs](Assets/Scripts/GameOver.cs), [Assets/Scripts/Hearts.cs](Assets/Scripts/Hearts.cs), [Assets/Scripts/Xp.cs](Assets/Scripts/Xp.cs).
  - ScriptableObjects (data-driven tuning): [Assets/ScriptableObjects/EnemyTypeSO.cs](Assets/ScriptableObjects/EnemyTypeSO.cs), [Assets/ScriptableObjects/PhaseSO.cs](Assets/ScriptableObjects/PhaseSO.cs).

- **Why it’s structured this way:** Gameplay logic is kept in small MonoBehaviour scripts attached to GameObjects; tunable parameters and collections live in ScriptableObjects so designers can iterate in the Editor without code changes. Spawning is centralized in `Spawner.cs` and uses Prefabs under `Assets/Prefabs`.

- **Packages & engine version:** Check current packages and editor version to avoid breaking changes:
  - Packages: [Packages/manifest.json](Packages/manifest.json) — includes `com.unity.inputsystem`, `render-pipelines.universal`, `com.unity.test-framework`, `com.unity.visualscripting`, `TextMesh Pro`, etc.
  - Editor: [ProjectSettings/ProjectVersion.txt](ProjectSettings/ProjectVersion.txt) shows `6000.3.2f1`.

- **Developer workflows (how to build, run, debug):**
  - Primary development happens inside the Unity Editor (open the project with Unity Hub using Editor 6000.3.2f1).
  - From command line (Windows) a basic unattended build is possible with Unity CLI:

    ```powershell
    "C:\Program Files\Unity\Hub\Editor\6000.3.2f1\Editor\Unity.exe" -projectPath "C:\Users\kwan7\bee" -quit -batchmode -buildWindows64Player "C:\builds\bee.exe"
    ```

  - Run automated tests through Unity Test Runner or CLI (Test Framework present):

    ```powershell
    "...\Editor\Unity.exe" -projectPath "C:\Users\kwan7\bee" -runTests -testPlatform PlayMode -quit -batchmode -logFile test-results.log
    ```

  - Debugging: open the solution produced by Unity (Assembly-CSharp.csproj) in Visual Studio / VS Code and attach the debugger to the Unity Editor process. The project already references the Visual Studio/VSCode Unity analyzers.

- **Project-specific conventions & patterns** (discoverable by reading the code):
  - Scripts are named to match their MonoBehaviour (e.g., `Bow.cs` expected on the Bow GameObject).
  - Use ScriptableObjects for tuning (see `EnemyTypeSO.cs`, `PhaseSO.cs`) rather than hard-coded constants.
  - Input uses the new Input System asset `InputSystem_Actions.inputactions` — prefer Input System API over legacy `Input` in changes.
  - Rendering uses URP (Universal Render Pipeline asset present) — shader and lighting changes must respect URP materials.

- **Integration points & external dependencies:**
  - Text rendering: TextMesh Pro (Assets/TextMesh Pro folder).
  - Input: `com.unity.inputsystem` + `InputSystem_Actions.inputactions` (Assets root).
  - Rendering: Universal Render Pipeline (see `Assets/UniversalRenderPipelineGlobalSettings.asset`).
  - Visual Scripting and Test Framework packages are present and may be used by designers/tests.

- **Where to make safe changes:**
  - Modify gameplay logic in `Assets/Scripts/*` and parameters in `Assets/ScriptableObjects/*`.
  - Avoid editing `Library/` and auto-generated files (`Assembly-CSharp.csproj` is generated; prefer editing source `.cs` files).

- **Examples to refer to when making edits:**
  - To change enemy behavior, update `EnemyTypeSO.cs` and how `Spawner.cs` uses it.
  - To adjust shooting mechanics, inspect `Bow.cs` and `Arrow.cs` together (arrow lifetime/velocity logic in `Arrow.cs`).

- **Tests & CI notes:**
  - Test Framework is installed but no tests are present in the repo root; if you add tests, follow Unity Test Runner conventions and run via the Editor or CLI.
  - CI should run Unity in batchmode using the Editor version in `ProjectVersion.txt` to avoid mismatch errors.

If anything is unclear or you'd like more detail in any section (e.g., add exact Editor CLI scripts, example unit tests, or a smaller design doc), tell me which area to expand.
