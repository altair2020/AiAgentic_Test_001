# Example 003 - Unity Top-Down Driving (GTA 1 Style)

This sample project provides a simple top-down driving game for Unity 2D.

## Included Gameplay
- Player car with acceleration, reverse, steering, and drag
- AI traffic cars that loop through waypoints
- City-like cross-road layout with blocking buildings
- Cash pickups to increase score
- Lives and quick respawn on crashes

## Graphics Included
- `Assets/Resources/Sprites/grass_tile.png`
- `Assets/Resources/Sprites/road_tile.png`
- `Assets/Resources/Sprites/building_tile.png`
- `Assets/Resources/Sprites/player_car.png`
- `Assets/Resources/Sprites/traffic_car.png`
- `Assets/Resources/Sprites/pickup_cash.png`

## Folder Layout
- `Assets/Scripts/TopDownDrive/SceneBootstrapTopDown.cs`
- `Assets/Scripts/TopDownDrive/PlayerCarController.cs`
- `Assets/Scripts/TopDownDrive/TrafficCarController.cs`
- `Assets/Scripts/TopDownDrive/TopDownGameManager.cs`
- `Assets/Scripts/TopDownDrive/PickupItem.cs`
- `Assets/Scripts/TopDownDrive/CameraFollow2D.cs`

## Quick Start
1. Open `output/example_003` as a Unity project.
2. Create a new 2D scene named `TopDownDriveScene`.
3. Add an empty GameObject named `SceneBootstrapTopDown`.
4. Attach `SceneBootstrapTopDown.cs`.
5. Press Play.

Controls:
- Throttle: `W` or `UpArrow`
- Brake/Reverse: `S` or `DownArrow`
- Steer: `A/D` or `LeftArrow/RightArrow`
