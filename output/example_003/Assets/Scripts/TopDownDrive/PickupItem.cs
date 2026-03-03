using UnityEngine;

namespace Example003.TopDownDrive
{
    public sealed class PickupItem : MonoBehaviour
    {
        [SerializeField] private int points = 100;

        private TopDownGameManager _gameManager;

        public void Configure(TopDownGameManager gameManager, int scorePoints)
        {
            _gameManager = gameManager;
            points = Mathf.Max(1, scorePoints);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.TryGetComponent<PlayerCarController>(out _))
            {
                return;
            }

            _gameManager?.RegisterPickup(points);
            Destroy(gameObject);
        }
    }
}
