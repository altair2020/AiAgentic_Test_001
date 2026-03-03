using System.Collections.Generic;
using UnityEngine;

namespace Example008.HomeworldLite
{
    public sealed class SimpleAiController : MonoBehaviour
    {
        [SerializeField] private float retargetInterval = 1.1f;

        private readonly List<ShipUnit> _enemyUnits = new List<ShipUnit>();
        private readonly List<ShipUnit> _playerUnits = new List<ShipUnit>();
        private float _nextRetargetAt;

        public void Configure(List<ShipUnit> enemyUnits, List<ShipUnit> playerUnits)
        {
            _enemyUnits.Clear();
            _enemyUnits.AddRange(enemyUnits);
            _playerUnits.Clear();
            _playerUnits.AddRange(playerUnits);
        }

        private void Update()
        {
            if (Time.time < _nextRetargetAt)
            {
                return;
            }

            _nextRetargetAt = Time.time + retargetInterval;

            for (int i = 0; i < _enemyUnits.Count; i++)
            {
                ShipUnit enemy = _enemyUnits[i];
                if (enemy == null || !enemy.IsAlive)
                {
                    continue;
                }

                ShipUnit closest = FindClosestAlivePlayer(enemy.transform.position);
                if (closest == null)
                {
                    continue;
                }

                enemy.SetAttackTarget(closest);
            }
        }

        private ShipUnit FindClosestAlivePlayer(Vector3 from)
        {
            ShipUnit closest = null;
            float bestSqr = float.MaxValue;

            for (int i = 0; i < _playerUnits.Count; i++)
            {
                ShipUnit player = _playerUnits[i];
                if (player == null || !player.IsAlive)
                {
                    continue;
                }

                float sqr = (player.transform.position - from).sqrMagnitude;
                if (sqr < bestSqr)
                {
                    bestSqr = sqr;
                    closest = player;
                }
            }

            return closest;
        }
    }
}
