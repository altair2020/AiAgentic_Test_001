# Example 008 - Homeworld-Lite C# Prototype

This example is a clean-room Unity C# prototype inspired by Homeworld-style RTS systems.
It is not a line-by-line port of HomeworldSDL source.

## Included Gameplay Systems
- RTS camera pan/rotate/zoom + focus hotkey
- Click and drag-box unit selection
- Command layer: right-click move or attack
- Formation slots (`Line` and `Sphere`)
- Fixed simulation tick driver
- Basic enemy AI retargeting loop
- Simple combat and unit destruction

## Folder Layout
- `Assets/Scripts/HomeworldLite/SceneBootstrapHomeworldLite.cs`
- `Assets/Scripts/HomeworldLite/ShipUnit.cs`
- `Assets/Scripts/HomeworldLite/SelectionManager3D.cs`
- `Assets/Scripts/HomeworldLite/CommandLayerLite.cs`
- `Assets/Scripts/HomeworldLite/FormationServiceLite.cs`
- `Assets/Scripts/HomeworldLite/RtsCameraController.cs`
- `Assets/Scripts/HomeworldLite/SimpleAiController.cs`
- `Assets/Scripts/HomeworldLite/SimulationTickDriver.cs`
- `Assets/Scripts/HomeworldLite/CameraFocusHotkey.cs`
- `Assets/Scripts/HomeworldLite/GameHudLite.cs`

## Quick Start
1. Open `output/example_008` as a Unity project.
2. Create a new 3D scene named `HomeworldLiteScene`.
3. Add an empty GameObject named `SceneBootstrapHomeworldLite`.
4. Attach `SceneBootstrapHomeworldLite.cs`.
5. Press Play.

## Controls
- `LMB`: Select single unit
- `LMB Drag`: Box-select
- `RMB`: Move (ground) or attack (enemy ship)
- `1`: Line formation
- `2`: Sphere formation
- `F`: Focus camera on selection
- `W/A/S/D`: Pan camera
- `Q/E`: Rotate camera
- `Mouse Wheel`: Zoom

## Scope Notes
- Uses primitive placeholder geometry and simple materials.
- Designed as a functional C# architecture baseline for a larger RTS rewrite.
- Does not include campaign scripting, resource economy, docking, or original asset pipeline.