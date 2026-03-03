using System.Collections.Generic;
using Example009.HWR.Core;

namespace Example009.HWR.Formations
{
    public enum FormationType
    {
        Line,
        Sphere
    }

    public interface IFormationPlanner
    {
        IReadOnlyList<Float3> Plan(int count, Float3 anchor, Float3 forward, float spacing, FormationType type);
    }

    public sealed class FormationPlanner : IFormationPlanner
    {
        public IReadOnlyList<Float3> Plan(int count, Float3 anchor, Float3 forward, float spacing, FormationType type)
        {
            var slots = new List<Float3>(count);
            if (count <= 0)
            {
                return slots;
            }

            Float3 fwd = Float3Ops.Normalize(forward);
            if (Float3Ops.LengthSqr(fwd) < 0.01f)
            {
                fwd = new Float3(0f, 0f, 1f);
            }
            Float3 right = new Float3(fwd.Z, 0f, -fwd.X);

            if (type == FormationType.Sphere)
            {
                float radius = 2f + count * 0.25f;
                for (int i = 0; i < count; i++)
                {
                    float t = (float)i / count;
                    float angle = t * 6.283185f;
                    Float3 offset = Float3Ops.Add(Float3Ops.Mul(right, (float)System.Math.Cos(angle) * radius), Float3Ops.Mul(fwd, (float)System.Math.Sin(angle) * radius));
                    slots.Add(Float3Ops.Add(anchor, offset));
                }
                return slots;
            }

            int cols = (int)System.Math.Ceiling(System.Math.Sqrt(count));
            int rows = (int)System.Math.Ceiling((double)count / cols);
            Float3 origin = Float3Ops.Sub(anchor, Float3Ops.Add(Float3Ops.Mul(right, (cols - 1) * 0.5f * spacing), Float3Ops.Mul(fwd, (rows - 1) * 0.5f * spacing)));
            for (int i = 0; i < count; i++)
            {
                int r = i / cols;
                int c = i % cols;
                slots.Add(Float3Ops.Add(origin, Float3Ops.Add(Float3Ops.Mul(right, c * spacing), Float3Ops.Mul(fwd, r * spacing))));
            }
            return slots;
        }
    }
}
