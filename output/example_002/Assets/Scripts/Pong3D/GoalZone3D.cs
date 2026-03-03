using UnityEngine;

namespace Example002.Pong3D
{
    public sealed class GoalZone3D : MonoBehaviour
    {
        [SerializeField] private PlayerSide3D scoringPlayer = PlayerSide3D.Left;
        [SerializeField] private GameManager3D gameManager;

        public void Configure(PlayerSide3D player, GameManager3D manager)
        {
            scoringPlayer = player;
            gameManager = manager;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent<BallController3D>(out _))
            {
                return;
            }

            if (gameManager != null)
            {
                gameManager.RegisterGoal(scoringPlayer);
            }
        }
    }
}
