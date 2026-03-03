using UnityEngine;

namespace Example003.TopDownDrive
{
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class TrafficCarController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 6f;
        [SerializeField] private float turnSpeed = 180f;

        private Rigidbody2D _body;
        private Vector2[] _waypoints = new Vector2[0];
        private int _targetIndex;

        public void Configure(Vector2[] waypoints, float speed)
        {
            _waypoints = waypoints ?? new Vector2[0];
            moveSpeed = Mathf.Max(1f, speed);
            _targetIndex = 0;
        }

        private void Awake()
        {
            _body = GetComponent<Rigidbody2D>();
            _body.gravityScale = 0f;
            _body.bodyType = RigidbodyType2D.Kinematic;
            _body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            _body.interpolation = RigidbodyInterpolation2D.Interpolate;
        }

        private void FixedUpdate()
        {
            if (_waypoints.Length == 0)
            {
                return;
            }

            Vector2 position = _body.position;
            Vector2 target = _waypoints[_targetIndex];
            Vector2 toTarget = target - position;

            if (toTarget.sqrMagnitude < 0.15f)
            {
                _targetIndex = (_targetIndex + 1) % _waypoints.Length;
                target = _waypoints[_targetIndex];
                toTarget = target - position;
            }

            Vector2 direction = toTarget.normalized;
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            float angle = Mathf.MoveTowardsAngle(_body.rotation, targetAngle, turnSpeed * Time.fixedDeltaTime);

            _body.MoveRotation(angle);
            _body.MovePosition(position + direction * (moveSpeed * Time.fixedDeltaTime));
        }
    }
}
