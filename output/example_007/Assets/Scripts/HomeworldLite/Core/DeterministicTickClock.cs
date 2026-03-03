using UnityEngine;

namespace Example007.HomeworldLite.Core
{
    public sealed class DeterministicTickClock
    {
        private readonly float _tickDelta;
        private float _accumulator;

        public int TickRateHz { get; }

        public float TickDelta => _tickDelta;

        public float Alpha
        {
            get
            {
                if (_tickDelta <= 0f)
                {
                    return 0f;
                }

                return Mathf.Clamp01(_accumulator / _tickDelta);
            }
        }

        public DeterministicTickClock(int tickRateHz)
        {
            TickRateHz = Mathf.Max(1, tickRateHz);
            _tickDelta = 1f / TickRateHz;
            _accumulator = 0f;
        }

        public int ConsumeTicks(float frameDeltaTime)
        {
            _accumulator += Mathf.Max(0f, frameDeltaTime);

            var ticks = 0;
            while (_accumulator >= _tickDelta)
            {
                _accumulator -= _tickDelta;
                ticks++;
            }

            return ticks;
        }
    }
}
