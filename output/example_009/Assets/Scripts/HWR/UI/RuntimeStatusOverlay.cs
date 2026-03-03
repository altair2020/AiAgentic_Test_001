using UnityEngine;
using Example009.HWR.Missions;
using Example009.HWR.Simulation;

namespace Example009.HWR.UI
{
    public sealed class RuntimeStatusOverlay : MonoBehaviour
    {
        private IWorldModel _world;
        private IMissionRuntime _mission;
        private float _nextLogAt;

        public void Configure(IWorldModel world, IMissionRuntime mission)
        {
            _world = world;
            _mission = mission;
        }

        private void Update()
        {
            if (_world == null || _mission == null || Time.time < _nextLogAt)
            {
                return;
            }

            _nextLogAt = Time.time + 1.5f;
            Debug.Log($"Tick: {_world.CurrentTick.Value} | Entities: {_world.All().Count} | Mission: {_mission.Evaluate(_world)}");
        }
    }
}
