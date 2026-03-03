# Example 009 - HWR Clean-Room Reimplementation Program

This folder starts a broad clean-room Unity C# reimplementation program inspired by large RTS architecture.
It is not a direct translation and does not reuse original source code or assets.

## Delivered In This Session
- Fixed-tick simulation clock and runner
- World model with entity snapshots and spawning
- Command queue + processor (`Move`, `Attack`, `Stop`)
- Movement and combat simulation systems
- Basic AI system for enemy retargeting
- Mission runtime evaluator (`Running`, `Success`, `Failed`)
- Runtime entity view synchronization
- Scene bootstrap with fleet spawning
- RTS camera controls
- Runtime status logging overlay

## Folder Layout
- `Assets/Scripts/HWR/Core/`
- `Assets/Scripts/HWR/Simulation/`
- `Assets/Scripts/HWR/Commands/`
- `Assets/Scripts/HWR/Selection/`
- `Assets/Scripts/HWR/Formations/`
- `Assets/Scripts/HWR/AI/`
- `Assets/Scripts/HWR/Camera/`
- `Assets/Scripts/HWR/Missions/`
- `Assets/Scripts/HWR/UI/`

## Quick Start
1. Open `output/example_009` as a Unity project.
2. Create a 3D scene named `HwrScene`.
3. Add an empty object named `SceneBootstrapHwr`.
4. Attach `Assets/Scripts/HWR/Core/SceneBootstrapHwr.cs`.
5. Press Play.

## Controls (Current)
- `W/A/S/D`: Pan camera
- `Q/E`: Rotate camera
- `Mouse Wheel`: Zoom
- `RMB`: Issue move command for player fleet to clicked ground point

## Important Scope Note
A complete clean-room implementation of an entire legacy RTS codebase is a multi-session program, not a single-turn deliverable.
This session establishes a substantial architecture scaffold and runnable baseline for iterative expansion.

## Progress Files
- `output/example_009/IMPLEMENTATION_PROGRESS.md`
- `docs/unity/example_009-rts-api-surface.md`