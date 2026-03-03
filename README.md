# Homeworld-Inspired Unity Prototype (Clean-Room)

A concise, clean-room RTS prototype in Unity C# inspired by high-level Homeworld gameplay patterns.
This project is not a port and does not reuse original HomeworldSDL code or assets.

## Goals

- Validate a modular RTS architecture in Unity.
- Deliver a playable command-select-move-combat loop.
- Keep simulation logic testable and separate from rendering/UI.

## Scope Snapshot

- Single-session skirmish prototype.
- Basic unit selection, movement, attack behavior, and wave pressure.
- Lightweight resource + production loop.
- Placeholder visuals/audio for rapid iteration.

## Controls

- `W/A/S/D` or `Middle Mouse Drag`: Pan camera.
- `Mouse Wheel`: Zoom in/out.
- `Q/E` (optional): Rotate camera.
- `Left Click`: Select unit.
- `Left Click + Drag`: Box select.
- `Right Click`: Move selected units.
- `Shift + Right Click`: Queue command.
- `A` then `Right Click`: Attack-move.
- `Esc`: Clear selection / close contextual UI.

## Architecture Overview

### Runtime Layers

- `Core`: Bootstrap, game loop, fixed simulation tick.
- `Domain`: Units, movement, combat, economy, commands.
- `AI`: Enemy director and behavior state transitions.
- `Input`: Selection, command issuing, camera controls.
- `UI`: HUD, selection panel, production controls.
- `Data`: JSON-driven unit and match configuration.

### Key Design Principles

- Clean-room implementation only (no direct reuse from HomeworldSDL internals).
- Deterministic-leaning fixed-step simulation for consistent behavior.
- Service-style systems for testability; MonoBehaviours mainly as adapters.
- Data-first balancing via external config.

## Limitations vs Original HomeworldSDL

- No feature parity with the original game or HomeworldSDL codebase.
- Simplified tactical space model (prototype-level movement/combat behaviors).
- No multiplayer lockstep, replay pipeline, or campaign progression.
- Reduced ship systems (no subsystem targeting, docking, hyperspace complexity).
- Minimal asset/tooling pipeline compared to legacy production workflow.
- Performance targets focused on small battles, not massive fleet-scale simulation.

## Project Layout

```text
docs/                        Design notes and agent progress
src/                         Core Python reference components
unity/                       Shared Unity workspace content
output/example_007/          Homeworld-inspired Unity prototype deliverable
```

## Run (Prototype)

1. Open `output/example_007/` in Unity (2022 LTS+ recommended).
2. Load the main scene.
3. Enter Play Mode and verify camera, selection, command, and combat loop.

## Status

Prototype-focused and intentionally narrow in scope. See `docs/agents/agent-progress.md` for agent execution history.