using Example009.HWR.Core;
using Example009.HWR.Simulation;

namespace Example009.HWR.Missions
{
    public enum MissionStatus
    {
        Running,
        Success,
        Failed
    }

    public interface IMissionRuntime
    {
        MissionStatus Evaluate(IWorldModel world);
    }

    public sealed class SkirmishMissionRuntime : IMissionRuntime
    {
        public MissionStatus Evaluate(IWorldModel world)
        {
            bool hasPlayer = false;
            bool hasEnemy = false;
            var all = world.All();
            for (int i = 0; i < all.Count; i++)
            {
                if (all[i].Team.Value == 0)
                {
                    hasPlayer = true;
                }
                else if (all[i].Team.Value == 1)
                {
                    hasEnemy = true;
                }
            }

            if (!hasEnemy)
            {
                return MissionStatus.Success;
            }
            if (!hasPlayer)
            {
                return MissionStatus.Failed;
            }
            return MissionStatus.Running;
        }
    }
}
