using System.Collections.Generic;
using Example009.HWR.Core;
using Example009.HWR.Simulation;

namespace Example009.HWR.Commands
{
    public interface ICommandProcessor
    {
        void Apply(Tick tick, IWorldModel world, IReadOnlyList<ICommand> commands);
    }

    public sealed class CommandProcessor : ICommandProcessor
    {
        public void Apply(Tick tick, IWorldModel world, IReadOnlyList<ICommand> commands)
        {
            for (int i = 0; i < commands.Count; i++)
            {
                ICommand command = commands[i];
                if (command is MoveCommand move)
                {
                    ApplyMove(world, move);
                }
                else if (command is AttackCommand attack)
                {
                    ApplyAttack(world, attack);
                }
                else if (command is StopCommand stop)
                {
                    ApplyStop(world, stop);
                }
            }
        }

        private static void ApplyMove(IWorldModel world, MoveCommand move)
        {
            for (int i = 0; i < move.Units.Count; i++)
            {
                EntityId id = move.Units[i];
                if (!world.TryGet(id, out EntitySnapshot snapshot))
                {
                    continue;
                }

                snapshot.MoveTarget = move.Destination;
                snapshot.AttackTarget = null;
                world.Set(snapshot);
            }
        }

        private static void ApplyAttack(IWorldModel world, AttackCommand attack)
        {
            for (int i = 0; i < attack.Units.Count; i++)
            {
                EntityId id = attack.Units[i];
                if (!world.TryGet(id, out EntitySnapshot snapshot))
                {
                    continue;
                }

                snapshot.AttackTarget = attack.Target;
                world.Set(snapshot);
            }
        }

        private static void ApplyStop(IWorldModel world, StopCommand stop)
        {
            for (int i = 0; i < stop.Units.Count; i++)
            {
                EntityId id = stop.Units[i];
                if (!world.TryGet(id, out EntitySnapshot snapshot))
                {
                    continue;
                }

                snapshot.MoveTarget = null;
                snapshot.AttackTarget = null;
                snapshot.Velocity = Float3.Zero;
                world.Set(snapshot);
            }
        }
    }
}
