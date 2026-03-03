using Example009.HWR.Core;

namespace Example009.HWR.Simulation
{
    public sealed class MovementSystem
    {
        private readonly float _moveSpeed;

        public MovementSystem(float moveSpeed = 12f)
        {
            _moveSpeed = moveSpeed;
        }

        public void Tick(float dt, IWorldModel world)
        {
            var all = world.All();
            for (int i = 0; i < all.Count; i++)
            {
                EntitySnapshot e = all[i];
                if (e.Health <= 0f || e.MoveTarget == null)
                {
                    continue;
                }

                Float3 target = e.MoveTarget.Value;
                Float3 delta = Float3Ops.Sub(target, e.Position);
                delta = new Float3(delta.X, 0f, delta.Z);
                float distSqr = Float3Ops.LengthSqr(delta);

                if (distSqr < 0.05f)
                {
                    e.Velocity = Float3.Zero;
                    e.MoveTarget = null;
                    world.Set(e);
                    continue;
                }

                Float3 dir = Float3Ops.Normalize(delta);
                Float3 velocity = Float3Ops.Mul(dir, _moveSpeed);
                Float3 next = Float3Ops.Add(e.Position, Float3Ops.Mul(velocity, dt));
                e.Position = next;
                e.Velocity = velocity;
                world.Set(e);
            }
        }
    }
}
