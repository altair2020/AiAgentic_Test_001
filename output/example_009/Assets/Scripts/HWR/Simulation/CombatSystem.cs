using System.Collections.Generic;
using Example009.HWR.Core;

namespace Example009.HWR.Simulation
{
    public sealed class CombatSystem
    {
        private readonly Dictionary<EntityId, float> _nextFireAt = new Dictionary<EntityId, float>();
        private readonly float _range;
        private readonly float _damage;
        private readonly float _cooldown;
        private float _time;

        public CombatSystem(float range = 16f, float damage = 9f, float cooldown = 0.7f)
        {
            _range = range;
            _damage = damage;
            _cooldown = cooldown;
        }

        public void Tick(float dt, IWorldModel world)
        {
            _time += dt;
            var all = world.All();

            for (int i = 0; i < all.Count; i++)
            {
                EntitySnapshot attacker = all[i];
                if (attacker.Health <= 0f || attacker.AttackTarget == null)
                {
                    continue;
                }

                EntityId targetId = attacker.AttackTarget.Value;
                if (!world.TryGet(targetId, out EntitySnapshot target) || target.Health <= 0f)
                {
                    attacker.AttackTarget = null;
                    world.Set(attacker);
                    continue;
                }

                float distSqr = Float3Ops.LengthSqr(Float3Ops.Sub(target.Position, attacker.Position));
                if (distSqr > _range * _range)
                {
                    attacker.MoveTarget = target.Position;
                    world.Set(attacker);
                    continue;
                }

                if (_nextFireAt.TryGetValue(attacker.Id, out float readyAt) && _time < readyAt)
                {
                    continue;
                }

                _nextFireAt[attacker.Id] = _time + _cooldown;
                float hp = target.Health - _damage;
                if (hp <= 0f)
                {
                    world.Despawn(target.Id);
                }
                else
                {
                    target.Health = hp;
                    world.Set(target);
                }
            }
        }
    }
}
