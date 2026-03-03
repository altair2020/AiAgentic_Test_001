using UnityEngine;

namespace Example001.Pong
{
    public enum PlayerSide
    {
        Left,
        Right
    }

    public sealed class GameManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private BallController ball;

        [Header("Rules")]
        [SerializeField] private int winningScore = 5;

        private int _leftScore;
        private int _rightScore;

        public void Configure(BallController ballController, int targetWinningScore)
        {
            ball = ballController;
            winningScore = Mathf.Max(1, targetWinningScore);
        }

        private void Start()
        {
            UpdateScoreUi();
            ResetRound();
        }

        public void RegisterGoal(PlayerSide scoringPlayer)
        {
            if (scoringPlayer == PlayerSide.Left)
            {
                _leftScore++;
            }
            else
            {
                _rightScore++;
            }

            UpdateScoreUi();

            if (_leftScore >= winningScore || _rightScore >= winningScore)
            {
                Debug.Log($"Winner: {( _leftScore >= winningScore ? PlayerSide.Left : PlayerSide.Right )}");
                _leftScore = 0;
                _rightScore = 0;
                UpdateScoreUi();
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

        private void UpdateScoreUi()
        {
            Debug.Log($"Score Update -> Left: {_leftScore} | Right: {_rightScore}");
        }
    }
}
