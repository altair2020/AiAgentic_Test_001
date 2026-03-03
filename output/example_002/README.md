# Example 002 - Unity 3D Pong

This sample project provides a basic 3D Pong implementation for Unity using C# scripts and runtime-generated 3D meshes.

## Included Gameplay
- Two 3D paddles (left and right)
- 3D ball movement on the X/Z plane
- Goal detection and score tracking
- Score reset after a player wins

## Folder Layout
- `Assets/Scripts/Pong3D/GameManager3D.cs`
- `Assets/Scripts/Pong3D/PaddleController3D.cs`
- `Assets/Scripts/Pong3D/BallController3D.cs`
- `Assets/Scripts/Pong3D/GoalZone3D.cs`
- `Assets/Scripts/Pong3D/SceneBootstrap3D.cs`

## Quick Start
1. Create a new 3D scene named `Pong3DScene`.
2. Add an empty object named `SceneBootstrap3D`.
3. Attach `SceneBootstrap3D.cs`.
4. Press Play.

`SceneBootstrap3D` creates the floor, paddles, ball, walls, goal triggers, camera, and lighting.

## Controls
- Left paddle: `W` / `S`
- Right paddle: `UpArrow` / `DownArrow`

## Win Rule
First player to reach `winningScore` wins. Scores reset automatically after win.
