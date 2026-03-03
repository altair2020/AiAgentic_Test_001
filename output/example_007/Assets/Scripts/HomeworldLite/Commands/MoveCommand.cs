using UnityEngine;
using Example007.HomeworldLite.Simulation;

namespace Example007.HomeworldLite.Commands
{
    public sealed class MoveCommand : ICommand
    {
        public int UnitId { get; }
        public Vector3 Destination { get; }

        public MoveCommand(int unitId, Vector3 destination)
        {
            UnitId = unitId;
            Destination = destination;
        }

        public void Apply(SimulationWorld world)
        {
            world.SetMoveTarget(UnitId, Destination);
        }
    }
}
