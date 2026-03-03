namespace Example009.HWR.Core
{
    public interface ISimulationClock
    {
        float TickDeltaSeconds { get; }
        Tick CurrentTick { get; }
        int ConsumeTicks(float deltaTime);
        void Advance();
    }

    public sealed class SimulationClock : ISimulationClock
    {
        private readonly int _maxCatchupTicks;
        private float _accumulator;

        public float TickDeltaSeconds { get; }
        public Tick CurrentTick { get; private set; }

        public SimulationClock(float tickDeltaSeconds = 0.05f, int maxCatchupTicks = 4)
        {
            TickDeltaSeconds = tickDeltaSeconds;
            _maxCatchupTicks = maxCatchupTicks;
            CurrentTick = new Tick(0);
        }

        public int ConsumeTicks(float deltaTime)
        {
            _accumulator += deltaTime;
            int count = 0;
            while (_accumulator >= TickDeltaSeconds && count < _maxCatchupTicks)
            {
                _accumulator -= TickDeltaSeconds;
                count++;
            }
            return count;
        }

        public void Advance()
        {
            CurrentTick = new Tick(CurrentTick.Value + 1);
        }
    }
}
