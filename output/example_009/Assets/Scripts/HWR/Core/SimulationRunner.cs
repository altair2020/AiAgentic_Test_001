using UnityEngine;
using Example009.HWR.AI;
using Example009.HWR.Commands;
using Example009.HWR.Simulation;

namespace Example009.HWR.Core
{
    public sealed class SimulationRunner : MonoBehaviour
    {
        public ISimulationClock Clock { get; private set; }
        public IWorldModel World { get; private set; }
        public ICommandQueue CommandQueue { get; private set; }
        public ICommandProcessor CommandProcessor { get; private set; }
        public IAiSystem AiSystem { get; private set; }
        public MovementSystem MovementSystem { get; private set; }
        public CombatSystem CombatSystem { get; private set; }

        public void Configure(
            ISimulationClock clock,
            IWorldModel world,
            ICommandQueue commandQueue,
            ICommandProcessor commandProcessor,
            IAiSystem aiSystem,
            MovementSystem movementSystem,
            CombatSystem combatSystem)
        {
            Clock = clock;
            World = world;
            CommandQueue = commandQueue;
            CommandProcessor = commandProcessor;
            AiSystem = aiSystem;
            MovementSystem = movementSystem;
            CombatSystem = combatSystem;
        }

        private void Update()
        {
            if (Clock == null || World == null)
            {
                return;
            }

            int ticks = Clock.ConsumeTicks(Time.deltaTime);
            for (int i = 0; i < ticks; i++)
            {
                Clock.Advance();
                World.CurrentTick = Clock.CurrentTick;

                CommandProcessor.Apply(Clock.CurrentTick, World, CommandQueue.DrainForTick(Clock.CurrentTick));
                AiSystem.Tick(Clock.CurrentTick, World, CommandQueue);
                MovementSystem.Tick(Clock.TickDeltaSeconds, World);
                CombatSystem.Tick(Clock.TickDeltaSeconds, World);
            }
        }
    }
}
