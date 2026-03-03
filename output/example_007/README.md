# Example 007 - HomeworldSDL C# Scope (Single-Session Deliverable)

This document defines a realistic MVP scope for creating a C# version inspired by HomeworldSDL traits:
- Large legacy SDL/C codebase structure
- RTS gameplay systems (selection, movement, combat, basic economy)
- Data-driven unit and scenario definitions

The goal is not a full port. The goal is a credible vertical slice that proves architecture, core loop, and extensibility in one implementation session.

## 1) MVP Feature List

### Core Runtime
- Unity 6 (or 2022 LTS) project with C# 11 compatible scripts.
- Top-down 3D space arena with a simple starfield backdrop and camera controls.
- Deterministic-ish fixed-step simulation loop (`SimulationTickSystem`) decoupled from render framerate.

### RTS Interaction
- Single and box selection for player units.
- Right-click move command with formation-lite behavior:
  - Units move toward destination with arrival radius.
  - Minor local separation to reduce overlap.
- Attack-move command (modifier key + right-click) using nearest hostile target acquisition.

### Unit Model
- 3 to 4 unit archetypes loaded from JSON:
  - `Scout`, `Interceptor`, `Corvette`, `ResourceCollector`.
- Shared stat schema: HP, speed, turn rate, weapon range, DPS, cost.
- Basic weapon system:
  - Range check
  - Cooldown timer
  - Direct damage (no projectile simulation in MVP)

### AI and Opponent
- Scripted enemy wave spawner.
- Enemy behavior state machine: idle -> move -> attack nearest player unit.
- Win/lose conditions:
  - Win when all enemy waves destroyed.
  - Lose when player command ship is destroyed.

### Resource and Production Loop
- Single resource type (`RU`).
- Resource nodes that can be harvested by `ResourceCollector`.
- Minimal production panel to queue 1 unit at a time from command ship if RU is sufficient.

### UI and Feedback
- HUD with RU count, selected unit panel, and wave status.
- Selection ring and health bar for selected units.
- Combat hit flash + basic SFX hooks (stubbed if assets unavailable).

### Save/Load (Optional MVP+)
- One quick-save JSON snapshot for active match state (units, RU, elapsed time).
- If session is time-constrained, defer to roadmap Phase 2.

## 2) Non-Goals

- Full feature parity with Homeworld/HomeworldSDL.
- True 3D six-degrees-of-freedom tactics and vertical fleet planes.
- Multiplayer, lockstep networking, replay system.
- Complex formations, docking, hyperspace, research tree, campaign progression.
- Advanced ballistics, collision volumes, and ship subsystem damage.
- Asset pipeline parity with original game formats.
- Editor tooling beyond minimal ScriptableObject/JSON loading.
- Performance optimization for massive fleets (1000+ entities).

## 3) C# File/Folder Plan

Proposed structure for `output/example_007/` Unity project assets and runtime code:

```text
output/example_007/
  README.md
  Assets/
    Scenes/
      Main.unity
    Scripts/
      Core/
        GameBootstrap.cs
        SimulationTickSystem.cs
        TimeService.cs
      Domain/
        Units/
          UnitDefinition.cs
          UnitRuntime.cs
          UnitFactory.cs
        Combat/
          CombatSystem.cs
          TargetingSystem.cs
        Movement/
          MovementSystem.cs
          Steering.cs
        Economy/
          ResourceSystem.cs
          HarvestSystem.cs
          ProductionQueueSystem.cs
        Commands/
          CommandType.cs
          UnitCommand.cs
          CommandProcessor.cs
      AI/
        EnemyDirector.cs
        EnemyStateMachine.cs
      Input/
        SelectionController.cs
        CommandInputController.cs
        CameraController.cs
      UI/
        HudController.cs
        SelectionPanelController.cs
        ProductionPanelController.cs
      Data/
        JsonDataLoader.cs
        MatchConfig.cs
      Infrastructure/
        EventBus.cs
        ILogger.cs
        UnityLogger.cs
    Data/
      Units/
        scout.json
        interceptor.json
        corvette.json
        resource_collector.json
      Match/
        default_match.json
    Prefabs/
      Units/
      UI/
    Materials/
    Audio/
```

Implementation notes:
- Keep systems as plain C# services where possible; MonoBehaviours should adapt Unity lifecycle and view bindings.
- Preserve clear separation between simulation state (`Domain`) and presentation (`UI`, visual effects).
- JSON schema should be versioned with a `schema_version` field to support future migration.

## 4) Acceptance Criteria

A session deliverable is accepted if all criteria below are met:

### Build and Run
- Project opens without compile errors in target Unity version.
- Main scene starts and player can control camera and units.

### RTS Loop
- Player can select at least 5 controllable units (single + drag selection).
- Move and attack-move commands execute reliably.
- Units engage enemies automatically when in range and apply damage over time.

### Economy and Production
- At least one collector can harvest RU from nodes and increase player RU.
- Player can spend RU to produce at least one unit type from command ship.

### Progression and Outcome
- Enemy waves spawn at configured intervals.
- Match can end in either win or lose state with clear UI messaging.

### Code Quality Baseline
- Public methods include XML doc comments for non-trivial APIs.
- No hardcoded secrets or credentials.
- Core systems are testable outside scene context (at least 3 edit-mode tests):
  - Movement arrival behavior
  - Combat cooldown and damage application
  - RU spend/insufficient-funds path

## 5) Phased Roadmap

### Phase 0 - Session Setup (30-45 min)
- Initialize Unity project skeleton and folders.
- Add core scene, bootstrap, camera, and data loading.
- Create placeholder prefabs and unit visuals.

### Phase 1 - Combatable Sandbox (90-120 min)
- Implement unit runtime model, selection, movement, targeting, and combat.
- Add enemy wave spawning and state machine.
- Verify full command-to-combat loop.

### Phase 2 - Economy Vertical Slice (60-90 min)
- Implement RU nodes, collector behavior, and RU accounting.
- Implement minimal production queue and spawn logic.
- Connect HUD values and production UI buttons.

### Phase 3 - Stabilization and Tests (45-60 min)
- Add acceptance checks and small edit-mode test set.
- Tune stats for a 5 to 10 minute playable match.
- Document known limitations and next-phase priorities.

### Phase 4 - Post-Session Expansion (Deferred)
- Save/load robustness, richer formations, fleet AI roles.
- Modular scenario pipeline and campaign framework.
- Performance refactor (ECS/jobification) when scale demands it.

## Suggested Session Target
- Deliver Phases 0 to 3 in one focused implementation session.
- Treat anything in Phase 4 as explicit out-of-scope follow-up.
