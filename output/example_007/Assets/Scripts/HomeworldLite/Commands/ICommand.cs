using Example007.HomeworldLite.Simulation;

namespace Example007.HomeworldLite.Commands
{
    public interface ICommand
    {
        void Apply(SimulationWorld world);
    }
}
