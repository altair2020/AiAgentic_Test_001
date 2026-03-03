using UnityEngine;

namespace Example003.TopDownDrive
{
    public sealed class TopDownGameManager : MonoBehaviour
    {
        [SerializeField] private int startingLives = 3;

        private PlayerCarController _player;
        private int _lives;
        private int _score;
        private float _nextCrashAllowedAt;

        public void Configure(PlayerCarController player, int lives)
        {
            _player = player;
            startingLives = Mathf.Max(1, lives);
        }

        private void Start()
        {
            _lives = startingLives;
            Debug.Log($"Drive Start -> Lives: {_lives} | Score: {_score}");
        }

        public void RegisterPickup(int points)
        {
            _score += Mathf.Max(1, points);
            Debug.Log($"Cash Collected -> Score: {_score}");
        }

        public void RegisterCrash()
        {
            if (Time.time < _nextCrashAllowedAt)
            {
                return;
            }

            _nextCrashAllowedAt = Time.time + 1f;
            _lives--;

            if (_lives <= 0)
            {
                Debug.Log($"Busted -> Final Score: {_score}. Restarting lives.");
                _lives = startingLives;
                _score = 0;
            }

            if (_player != null)
            {
                _player.ResetToSpawn();
            }

            Debug.Log($"Crash -> Lives: {_lives} | Score: {_score}");
        }
    }
}
