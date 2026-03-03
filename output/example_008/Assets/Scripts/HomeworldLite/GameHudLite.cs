using UnityEngine;

namespace Example008.HomeworldLite
{
    public sealed class GameHudLite : MonoBehaviour
    {
        [SerializeField] private SelectionManager3D selection;
        [SerializeField] private ShipUnit[] allUnits;

        private int _lastPlayerAlive = -1;
        private int _lastEnemyAlive = -1;
        private int _lastSelected = -1;
        private bool _resultLogged;

        public void Configure(SelectionManager3D selectionManager, ShipUnit[] units)
        {
            selection = selectionManager;
            allUnits = units;
        }

        private void Update()
        {
            int playerAlive = 0;
            int enemyAlive = 0;

            if (allUnits != null)
            {
                for (int i = 0; i < allUnits.Length; i++)
                {
                    ShipUnit unit = allUnits[i];
                    if (unit == null || !unit.IsAlive)
                    {
                        continue;
                    }

                    if (unit.Team == TeamSide.Player)
                    {
                        playerAlive++;
                    }
                    else
                    {
                        enemyAlive++;
                    }
                }
            }

            int selectedCount = selection != null ? selection.Selected.Count : 0;
            if (selectedCount != _lastSelected || playerAlive != _lastPlayerAlive || enemyAlive != _lastEnemyAlive)
            {
                Debug.Log($"HUD -> Selected: {selectedCount} | Player: {playerAlive} | Enemy: {enemyAlive}");
                _lastSelected = selectedCount;
                _lastPlayerAlive = playerAlive;
                _lastEnemyAlive = enemyAlive;
            }

            if (!_resultLogged && (playerAlive == 0 || enemyAlive == 0))
            {
                string result = playerAlive > 0 ? "Victory" : "Defeat";
                Debug.Log(result + " - Press Play Again");
                _resultLogged = true;
            }
        }
    }
}
