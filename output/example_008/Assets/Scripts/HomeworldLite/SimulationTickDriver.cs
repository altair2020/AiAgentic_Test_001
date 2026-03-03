using UnityEngine;

namespace Example008.HomeworldLite
{
    public sealed class SimulationTickDriver : MonoBehaviour
    {
        [SerializeField] private float tickRate = 20f;

        private ShipUnit[] _units = new ShipUnit[0];
        private float _accumulator;

        public void Configure(ShipUnit[] units)
        {
            _units = units ?? new ShipUnit[0];
        }

        private void Update()
        {
            float dt = 1f / Mathf.Max(1f, tickRate);
            _accumulator += Time.deltaTime;

            int safety = 0;
            while (_accumulator >= dt && safety < 4)
            {
                _accumulator -= dt;
                safety++;

                for (int i = 0; i < _units.Length; i++)
                {
                    ShipUnit unit = _units[i];
                    if (unit != null)
                    {
                        unit.Tick(dt);
                    }
                }
            }
        }
    }
}
