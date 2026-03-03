using UnityEngine;

namespace Example007.HomeworldLite.Simulation
{
    public sealed class UnitState
    {
        public int Id { get; }
        public int TeamId { get; }

        public Vector3 Position;
        public Vector3 Velocity;
        public Vector3 TargetPosition;
        public bool HasMoveTarget;

        public UnitState(int id, int teamId, Vector3 position)
        {
            Id = id;
            TeamId = teamId;
            Position = position;
            Velocity = Vector3.zero;
            TargetPosition = position;
            HasMoveTarget = false;
        }
    }
}
