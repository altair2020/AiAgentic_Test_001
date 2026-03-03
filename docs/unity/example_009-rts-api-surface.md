# Example 009 Unity Clean-Room RTS API Surface (C#)

This document defines an implementation-ready API surface for a Homeworld-inspired, clean-room RTS prototype in Unity.

## Design Goals
- Keep simulation deterministic by using a fixed tick and pure data state transitions.
- Split real-time simulation from presentation (camera, audio, VFX).
- Allow AI and mission scripts to issue the same commands as players.
- Make save/load independent from concrete file formats or persistence backends.

## Suggested Namespace Layout
```csharp
namespace Example009.RTS.Core {}
namespace Example009.RTS.World {}
namespace Example009.RTS.Commands {}
namespace Example009.RTS.Selection {}
namespace Example009.RTS.Formations {}
namespace Example009.RTS.Camera {}
namespace Example009.RTS.AI {}
namespace Example009.RTS.Missions {}
namespace Example009.RTS.Persistence {}
namespace Example009.RTS.Audio {}
namespace Example009.RTS.Events {}
```

## Common Types
```csharp
public readonly record struct TeamId(int Value);
public readonly record struct EntityId(int Value);
public readonly record struct Tick(long Value);

public readonly record struct Float3(float X, float Y, float Z);
public readonly record struct YawPitch(float YawDeg, float PitchDeg);

public enum SimulationPhase
{
    PreTick,
    CommandApply,
    AI,
    Movement,
    Combat,
    PostTick
}
```

## Core Loop
```csharp
public interface IGameRuntime
{
    void Initialize(GameBootstrapConfig config);
    void StartSession(SessionStartRequest request);
    void StopSession();
    bool IsRunning { get; }
}

public interface ISimulationClock
{
    float TickDeltaSeconds { get; }
    Tick CurrentTick { get; }
    int MaxCatchUpTicksPerFrame { get; }
    int ConsumeTicks(float unscaledDeltaTime);
}

public interface IGameLoopController
{
    void AdvanceFrame(float unscaledDeltaTime);
    void AdvanceTick();
}

public sealed record GameBootstrapConfig(
    ISimulationClock Clock,
    IWorldModel World,
    ICommandProcessor CommandProcessor,
    IAISystem AiSystem,
    IMissionRuntime MissionRuntime,
    IEventBus EventBus,
    IAudioEventRouter AudioRouter);

public sealed record SessionStartRequest(
    string MissionId,
    TeamId LocalPlayerTeam,
    int RandomSeed,
    bool EnableFogOfWar,
    bool EnableAi);
```

## World Model
```csharp
public interface IWorldModel
{
    Tick CurrentTick { get; }
    IReadOnlyCollection<EntityId> Entities { get; }
    bool Exists(EntityId entityId);
    EntitySnapshot GetEntity(EntityId entityId);
    IReadOnlyList<EntityId> Query(QuerySpec query);

    EntityId Spawn(SpawnSpec spec);
    bool Despawn(EntityId entityId);
    void SetTransform(EntityId entityId, Float3 position, YawPitch rotation);
    void SetVelocity(EntityId entityId, Float3 linearVelocity);
    void SetHealth(EntityId entityId, float health);
}

public sealed record EntitySnapshot(
    EntityId Id,
    TeamId Team,
    string Archetype,
    Float3 Position,
    YawPitch Rotation,
    Float3 Velocity,
    float Health,
    float MaxHealth);

public sealed record QuerySpec(
    TeamId? Team,
    Float3? Center,
    float Radius,
    string[]? Tags,
    bool IncludeHidden);

public sealed record SpawnSpec(
    TeamId Team,
    string Archetype,
    Float3 Position,
    YawPitch Rotation,
    IReadOnlyDictionary<string, float>? Tunables);
```

## Command Layer
```csharp
public interface ICommand
{
    Guid CommandId { get; }
    TeamId IssuerTeam { get; }
    Tick IssuedAtTick { get; }
    bool RequiresAuthorityCheck { get; }
}

public interface ICommandValidator
{
    CommandValidationResult Validate(ICommand command, IWorldModel world);
}

public interface ICommandQueue
{
    void Enqueue(ICommand command);
    IReadOnlyList<ICommand> DrainForTick(Tick tick);
}

public interface ICommandProcessor
{
    CommandApplyReport ApplyBatch(Tick tick, IReadOnlyList<ICommand> commands);
}

public sealed record CommandValidationResult(bool IsValid, string? Reason);
public sealed record CommandApplyReport(int AppliedCount, int RejectedCount, IReadOnlyList<string> Errors);

public sealed record MoveCommand(
    Guid CommandId,
    TeamId IssuerTeam,
    Tick IssuedAtTick,
    IReadOnlyList<EntityId> Units,
    Float3 Destination,
    bool QueueAfterCurrentOrder) : ICommand
{
    public bool RequiresAuthorityCheck => true;
}

public sealed record AttackCommand(
    Guid CommandId,
    TeamId IssuerTeam,
    Tick IssuedAtTick,
    IReadOnlyList<EntityId> Units,
    EntityId Target,
    bool QueueAfterCurrentOrder) : ICommand
{
    public bool RequiresAuthorityCheck => true;
}

public sealed record AbilityCommand(
    Guid CommandId,
    TeamId IssuerTeam,
    Tick IssuedAtTick,
    IReadOnlyList<EntityId> Units,
    string AbilityId,
    Float3 TargetPoint) : ICommand
{
    public bool RequiresAuthorityCheck => true;
}
```

## Selection
```csharp
public interface ISelectionService
{
    TeamId LocalTeam { get; }
    IReadOnlyList<EntityId> CurrentSelection { get; }

    void ReplaceSelection(IReadOnlyList<EntityId> entities);
    void AddToSelection(IReadOnlyList<EntityId> entities);
    void RemoveFromSelection(IReadOnlyList<EntityId> entities);
    void ClearSelection();

    IReadOnlyList<EntityId> SelectByScreenRect(ScreenRect rect, SelectionFilter filter);
    EntityId? SelectSingle(ScreenPoint point, SelectionFilter filter);
}

public sealed record SelectionFilter(
    bool OwnUnitsOnly,
    bool CombatUnitsOnly,
    bool IncludeStructures,
    bool IncludeWorkers);

public readonly record struct ScreenPoint(float X, float Y);
public readonly record struct ScreenRect(float XMin, float YMin, float XMax, float YMax);
```

## Formations
```csharp
public enum FormationType
{
    Sphere,
    Wall,
    Wedge,
    Column,
    Custom
}

public interface IFormationPlanner
{
    FormationPlan BuildPlan(
        IReadOnlyList<EntityId> units,
        Float3 anchor,
        Float3 forward,
        FormationType type,
        float spacing);
}

public interface IFormationService
{
    FormationAssignment Assign(IReadOnlyList<EntityId> units, FormationPlan plan);
    bool UpdateAnchor(Guid formationId, Float3 anchor, Float3 forward);
    bool Dissolve(Guid formationId);
}

public sealed record FormationSlot(int Index, Float3 LocalOffset, EntityId? AssignedUnit);

public sealed record FormationPlan(
    Guid FormationId,
    FormationType Type,
    Float3 Anchor,
    Float3 Forward,
    float Spacing,
    IReadOnlyList<FormationSlot> Slots);

public sealed record FormationAssignment(
    Guid FormationId,
    IReadOnlyDictionary<EntityId, Float3> UnitTargets);
```

## Camera
```csharp
public interface IRtsCameraController
{
    Float3 Position { get; }
    YawPitch Orbit { get; }
    float Zoom { get; }

    void TickCamera(float deltaTime, CameraInputState input);
    void FocusOn(Float3 point, float radius);
    void Follow(EntityId entityId);
    void ClearFollow();
    void SetBounds(CameraBounds bounds);
}

public sealed record CameraInputState(
    float PanX,
    float PanY,
    float ZoomDelta,
    float OrbitDeltaYaw,
    float OrbitDeltaPitch,
    bool FastMove,
    bool EdgePan);

public sealed record CameraBounds(Float3 Min, Float3 Max);
```

## AI
```csharp
public interface IAISystem
{
    void RegisterBrain(TeamId team, ITeamBrain brain);
    void UnregisterBrain(TeamId team);
    void TickAI(Tick tick, IWorldModel world, ICommandQueue commandQueue);
}

public interface ITeamBrain
{
    TeamId Team { get; }
    void Evaluate(Tick tick, IWorldModel world, IBlackboard blackboard, ICommandQueue commandQueue);
}

public interface IBlackboard
{
    T Get<T>(string key, T fallback = default!);
    void Set<T>(string key, T value);
    bool Remove(string key);
    bool Contains(string key);
}
```

## Mission Scripting Hooks
```csharp
public interface IMissionRuntime
{
    string MissionId { get; }
    void Initialize(MissionContext context);
    void Tick(Tick tick);
    void OnEvent(GameEvent gameEvent);
    MissionStatus GetStatus();
}

public interface IMissionScript
{
    void OnStart(MissionApi api);
    void OnTick(Tick tick, MissionApi api);
    void OnEvent(GameEvent gameEvent, MissionApi api);
}

public sealed record MissionContext(
    TeamId PlayerTeam,
    IWorldModel World,
    ICommandQueue CommandQueue,
    IEventBus Events,
    IMissionObjectiveTracker Objectives);

public interface IMissionObjectiveTracker
{
    void SetObjectiveState(string objectiveId, ObjectiveState state);
    ObjectiveState GetObjectiveState(string objectiveId);
    IReadOnlyDictionary<string, ObjectiveState> Snapshot();
}

public enum ObjectiveState
{
    Hidden,
    Active,
    Completed,
    Failed
}

public sealed record MissionStatus(bool IsComplete, bool IsFailed, string[] CompletedObjectives);

public sealed class MissionApi
{
    public TeamId PlayerTeam { get; init; }
    public IWorldModel World { get; init; } = default!;
    public ICommandQueue CommandQueue { get; init; } = default!;
    public IEventBus Events { get; init; } = default!;
    public IMissionObjectiveTracker Objectives { get; init; } = default!;

    public EntityId Spawn(SpawnSpec spec) => throw new NotImplementedException();
    public void Emit(GameEvent gameEvent) => throw new NotImplementedException();
}
```

## Save / Load Abstraction
```csharp
public interface ISaveLoadService
{
    SaveMetadata QuickSave(SessionSnapshot snapshot);
    SessionSnapshot QuickLoad();
    SaveMetadata SaveAs(string slotId, SessionSnapshot snapshot);
    SessionSnapshot Load(string slotId);
    IReadOnlyList<SaveMetadata> ListSaves();
    bool Delete(string slotId);
}

public interface ISnapshotSerializer
{
    byte[] Serialize(SessionSnapshot snapshot);
    SessionSnapshot Deserialize(byte[] payload);
    int Version { get; }
}

public interface ISaveStorage
{
    void Write(string key, byte[] payload);
    byte[] Read(string key);
    bool Exists(string key);
    IReadOnlyList<string> Keys();
    bool Delete(string key);
}

public sealed record SessionSnapshot(
    string MissionId,
    Tick Tick,
    int RandomSeed,
    WorldSnapshot World,
    IReadOnlyDictionary<string, string> MissionState,
    IReadOnlyDictionary<string, string> AiState);

public sealed record WorldSnapshot(IReadOnlyList<EntitySnapshot> Entities);

public sealed record SaveMetadata(
    string SlotId,
    DateTimeOffset CreatedAt,
    string MissionId,
    Tick Tick,
    int SerializerVersion);
```

## Audio and Event Bus
```csharp
public interface IEventBus
{
    void Publish<TEvent>(TEvent evt) where TEvent : GameEvent;
    IDisposable Subscribe<TEvent>(Action<TEvent> handler) where TEvent : GameEvent;
}

public abstract record GameEvent(string Type, Tick Tick);

public sealed record CommandAppliedEvent(
    Guid CommandId,
    TeamId Team,
    Tick Tick,
    int UnitCount,
    string CommandType) : GameEvent("command_applied", Tick);

public sealed record UnitDestroyedEvent(
    EntityId Unit,
    TeamId Team,
    Tick Tick,
    EntityId? Killer) : GameEvent("unit_destroyed", Tick);

public sealed record ObjectiveStateChangedEvent(
    string ObjectiveId,
    ObjectiveState State,
    Tick Tick) : GameEvent("objective_state_changed", Tick);

public interface IAudioEventRouter
{
    void Bind(IEventBus eventBus);
    void Unbind();
    void RegisterMapping<TEvent>(AudioCue cue) where TEvent : GameEvent;
}

public sealed record AudioCue(
    string CueId,
    float Volume,
    float Pitch,
    bool Spatial,
    float CooldownSeconds);
```

## Minimal Composition Root
```csharp
public static class RtsCompositionRoot
{
    public static IGameRuntime BuildDefault(GameBootstrapConfig config)
    {
        // Wire concrete implementations in one place for easy replacement in tests.
        throw new NotImplementedException();
    }
}
```

## Suggested First Implementation Pass
1. Implement deterministic tick loop (`ISimulationClock`, `IGameLoopController`).
2. Implement world entities + movement commands + selection rectangle.
3. Add basic formation planner (`Sphere`, `Wall`) and camera focus/follow.
4. Add one AI brain (`AggressiveSkirmishBrain`) issuing move/attack commands.
5. Add mission runtime hooks and one scripted objective.
6. Add JSON-backed serializer + local file storage.
7. Bind event bus to audio cue router for key feedback loops.
