using UnityEngine;

namespace Example002.Pong3D
{
    public enum PlayerSide3D
    {
        Left,
        Right
    }

    public sealed class GameManager3D : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private BallController3D ball;

        [Header("Rules")]
        [SerializeField] private int winningScore = 5;

        private int _leftScore;
        private int _rightScore;

        public void Configure(BallController3D ballController, int targetWinningScore)
        {
            ball = ballController;
            winningScore = Mathf.Max(1, targetWinningScore);
        }

        private void Start()
        {
            LogScore();
            ResetRound();
        }

        public void RegisterGoal(PlayerSide3D scoringPlayer)
        {
            if (scoringPlayer == PlayerSide3D.Left)
            {
                _leftScore++;
            }
            else
            {
                _rightScore++;
            }

            LogScore();

            if (_leftScore >= winningScore || _rightScore >= winningScore)
            {
                var winner = _leftScore >= winningScore ? PlayerSide3D.Left : PlayerSide3D.Right;
                Debug.Log($"Winner: {winner}");
                _leftScore = 0;
                _rightScore = 0;
                LogScore();
            }

            ResetRound();
        }

        private void ResetRound()
        {
            if (ball != null)
            {
                ball.ResetBall();
            }
        }

        private void LogScore()
        {
            Debug.Log($"Score Update -> Left: {_leftScore} | Right: {_rightScore}");
        }
    }
}
