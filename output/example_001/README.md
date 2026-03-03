# Example 001 - Unity Pong

This sample project provides a basic 2D Pong implementation for Unity using C# scripts.

## Included Gameplay
- Two paddles (left and right)
- Ball launch and bounce physics behavior
- Goal detection and score tracking
- Score reset after a player wins

## Folder Layout
- `Assets/Scripts/Pong/GameManager.cs`
- `Assets/Scripts/Pong/PaddleController.cs`
- `Assets/Scripts/Pong/BallController.cs`
- `Assets/Scripts/Pong/GoalZone.cs`
- `Assets/Scripts/Pong/SceneBootstrap.cs`

## Quick Start (Auto-Bootstrap)
1. Create a new 2D scene named `PongScene`.
2. Add an empty object named `SceneBootstrap`.
3. Attach `SceneBootstrap.cs`.
4. Press Play.

`SceneBootstrap` creates paddles, ball, walls, goals, camera setup, and wires references automatically.

## Scene Setup (Unity)
1. Create a new 2D scene named `PongScene`.
2. Add an empty object named `GameManager` and attach `GameManager.cs`.
3. Create two paddle objects (Sprite + BoxCollider2D + Rigidbody2D set to Kinematic).
4. Attach `PaddleController.cs` to each paddle.
5. Set left paddle controls to `W/S`; set right paddle controls to `UpArrow/DownArrow`.
6. Create a ball object (Circle Sprite + CircleCollider2D + Rigidbody2D Dynamic, Gravity Scale 0).
7. Attach `BallController.cs` to the ball and assign `GameManager`.
8. Create top and bottom wall colliders so the ball bounces.
9. Create left and right goal trigger objects (BoxCollider2D with `Is Trigger` enabled).
10. Attach `GoalZone.cs` to each goal and set `scoringPlayer`:
    - Left goal trigger -> `Right`
    - Right goal trigger -> `Left`
11. Optional: add your own UI layer (for example TextMeshPro) if you want on-screen score labels.

## Win Rule
First player to reach `winningScore` wins. Scores reset automatically after win.
