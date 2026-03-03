using UnityEngine;

namespace Example001.Pong
{
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class BallController : MonoBehaviour
    {
        [SerializeField] private float launchSpeed = 7f;
        [SerializeField] private float maxYDirection = 0.75f;

        private Rigidbody2D _rigidbody2D;

        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }

        public void Configure(float speed, float yDirectionLimit)
        {
            launchSpeed = Mathf.Max(0.1f, speed);
            maxYDirection = Mathf.Clamp(yDirectionLimit, 0.05f, 0.99f);
        }

        public void ResetBall()
        {
            transform.position = Vector3.zero;
            _rigidbody2D.linearVelocity = Vector2.zero;

            float xDirection = Random.value < 0.5f ? -1f : 1f;
            float yDirection = Random.Range(-maxYDirection, maxYDirection);
            Vector2 direction = new Vector2(xDirection, yDirection).normalized;

            _rigidbody2D.AddForce(direction * launchSpeed, ForceMode2D.Impulse);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.TryGetComponent<PaddleController>(out _))
            {
                Vector2 velocity = _rigidbody2D.linearVelocity;
                velocity *= 1.05f;
                _rigidbody2D.linearVelocity = velocity;
            }
        }
    }
}
