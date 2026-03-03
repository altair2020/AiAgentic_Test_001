using System;

namespace Example009.HWR.Core
{
    public readonly struct TeamId
    {
        public TeamId(int value)
        {
            Value = value;
        }

        public int Value { get; }
    }

    public readonly struct EntityId
    {
        public EntityId(int value)
        {
            Value = value;
        }

        public int Value { get; }
    }

    public readonly struct Tick
    {
        public Tick(long value)
        {
            Value = value;
        }

        public long Value { get; }
    }

    public readonly struct Float3
    {
        public Float3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public float X { get; }
        public float Y { get; }
        public float Z { get; }

        public static Float3 Zero => new Float3(0f, 0f, 0f);
    }

    public enum SimulationPhase
    {
        PreTick,
        Commands,
        AI,
        Movement,
        Combat,
        PostTick
    }

    public readonly struct YawPitch
    {
        public YawPitch(float yawDeg, float pitchDeg)
        {
            YawDeg = yawDeg;
            PitchDeg = pitchDeg;
        }

        public float YawDeg { get; }
        public float PitchDeg { get; }
    }

    public static class Float3Ops
    {
        public static Float3 Add(Float3 a, Float3 b) => new Float3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        public static Float3 Sub(Float3 a, Float3 b) => new Float3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        public static Float3 Mul(Float3 a, float s) => new Float3(a.X * s, a.Y * s, a.Z * s);

        public static float LengthSqr(Float3 a)
        {
            return a.X * a.X + a.Y * a.Y + a.Z * a.Z;
        }

        public static Float3 Normalize(Float3 a)
        {
            float lenSqr = LengthSqr(a);
            if (lenSqr < 0.0001f)
            {
                return Float3.Zero;
            }

            float inv = 1f / (float)Math.Sqrt(lenSqr);
            return Mul(a, inv);
        }
    }
}
