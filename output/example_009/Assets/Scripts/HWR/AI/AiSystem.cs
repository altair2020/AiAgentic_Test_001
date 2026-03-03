using System.Collections.Generic;
using Example009.HWR.Core;
using Example009.HWR.Commands;
using Example009.HWR.Simulation;

namespace Example009.HWR.AI
{
    public interface IAiSystem
    {
        void Tick(Tick tick, IWorldModel world, ICommandQueue queue);
    }

    public sealed class AiSystem : IAiSystem
    {
        private float _retargetAccumulator;

        public void Tick(Tick tick, IWorldModel world, ICommandQueue queue)
        {
            _retargetAccumulator += 1f;
            if (_retargetAccumulator < 8f)
            {
                return;
            }
            _retargetAccumulator = 0f;

            var all = world.All();
            var enemies = new List<EntitySnapshot>();
            var players = new List<EntitySnapshot>();

            for (int i = 0; i < all.Count; i++)
            {
                if (all[i].Team.Value == 1)
                {
                    enemies.Add(all[i]);
                }
                else
                {
                    players.Add(all[i]);
                }
            }

            for (int i = 0; i < enemies.Count; i++)
            {
                EntitySnapshot enemy = enemies[i];
                if (players.Count == 0)
                {
                    break;
                }

                EntitySnapshot closest = players[0];
                float best = float.MaxValue;
                for (int j = 0; j < players.Count; j++)
                {
                    float d = Float3Ops.LengthSqr(Float3Ops.Sub(players[j].Position, enemy.Position));
                    if (d < best)
                    {
                        best = d;
                        closest = players[j];
                    }
                }

                queue.Enqueue(new AttackCommand(System.Guid.NewGuid(), enemy.Team, tick, new[] { enemy.Id }, closest.Id));
            }
        }
    }
}
