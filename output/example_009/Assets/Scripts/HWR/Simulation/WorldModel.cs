using System.Collections.Generic;
using Example009.HWR.Core;

namespace Example009.HWR.Simulation
{
    public struct EntitySnapshot
    {
        public EntitySnapshot(
            EntityId id,
            TeamId team,
            string archetype,
            Float3 position,
            Float3 velocity,
            float health,
            float maxHealth,
            EntityId? attackTarget,
            Float3? moveTarget)
        {
            Id = id;
            Team = team;
            Archetype = archetype;
            Position = position;
            Velocity = velocity;
            Health = health;
            MaxHealth = maxHealth;
            AttackTarget = attackTarget;
            MoveTarget = moveTarget;
        }

        public EntityId Id { get; set; }
        public TeamId Team { get; set; }
        public string Archetype { get; set; }
        public Float3 Position { get; set; }
        public Float3 Velocity { get; set; }
        public float Health { get; set; }
        public float MaxHealth { get; set; }
        public EntityId? AttackTarget { get; set; }
        public Float3? MoveTarget { get; set; }
    }

    public struct SpawnSpec
    {
        public SpawnSpec(TeamId team, string archetype, Float3 position, float maxHealth)
        {
            Team = team;
            Archetype = archetype;
            Position = position;
            MaxHealth = maxHealth;
        }

        public TeamId Team { get; }
        public string Archetype { get; }
        public Float3 Position { get; }
        public float MaxHealth { get; }
    }

    public interface IWorldModel
    {
        Tick CurrentTick { get; set; }
        EntityId Spawn(SpawnSpec spec);
        bool Exists(EntityId id);
        bool Despawn(EntityId id);
        bool TryGet(EntityId id, out EntitySnapshot snapshot);
        void Set(EntitySnapshot snapshot);
        IReadOnlyList<EntitySnapshot> All();
    }

    public sealed class WorldModel : IWorldModel
    {
        private readonly Dictionary<EntityId, EntitySnapshot> _entities = new Dictionary<EntityId, EntitySnapshot>();
        private int _nextEntityId = 1;

        public Tick CurrentTick { get; set; }

        public EntityId Spawn(SpawnSpec spec)
        {
            var id = new EntityId(_nextEntityId++);
            _entities[id] = new EntitySnapshot(
                id,
                spec.Team,
                spec.Archetype,
                spec.Position,
                Float3.Zero,
                spec.MaxHealth,
                spec.MaxHealth,
                null,
                null);
            return id;
        }

        public bool Exists(EntityId id) => _entities.ContainsKey(id);

        public bool Despawn(EntityId id)
        {
            return _entities.Remove(id);
        }

        public bool TryGet(EntityId id, out EntitySnapshot snapshot)
        {
            return _entities.TryGetValue(id, out snapshot);
        }

        public void Set(EntitySnapshot snapshot)
        {
            _entities[snapshot.Id] = snapshot;
        }

        public IReadOnlyList<EntitySnapshot> All()
        {
            var list = new List<EntitySnapshot>(_entities.Count);
            foreach (var kv in _entities)
            {
                list.Add(kv.Value);
            }
            return list;
        }
    }
}
