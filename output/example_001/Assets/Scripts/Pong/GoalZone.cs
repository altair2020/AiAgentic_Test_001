using UnityEngine;

namespace Example001.Pong
{
    public sealed class GoalZone : MonoBehaviour
    {
        [SerializeField] private PlayerSide scoringPlayer = PlayerSide.Left;
        [SerializeField] private GameManager gameManager;

        public void Configure(PlayerSide player, GameManager manager)
        {
            scoringPlayer = player;
            gameManager = manager;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.TryGetComponent<BallController>(out _))
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
