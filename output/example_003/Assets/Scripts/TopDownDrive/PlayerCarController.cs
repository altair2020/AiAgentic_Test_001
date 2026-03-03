using UnityEngine;

namespace Example003.TopDownDrive
{
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class PlayerCarController : MonoBehaviour
    {
        [Header("Driving")]
        [SerializeField] private float acceleration = 18f;
        [SerializeField] private float reverseAcceleration = 12f;
        [SerializeField] private float steering = 190f;
        [SerializeField] private float maxForwardSpeed = 12f;
        [SerializeField] private float maxReverseSpeed = 5f;

        private Rigidbody2D _body;
        private TopDownGameManager _gameManager;
        private Vector2 _spawnPosition;
        private float _spawnRotation;

        private float _throttleInput;
        private float _steerInput;

        public void Configure(TopDownGameManager gameManager, Vector2 spawnPosition, float spawnRotation)
        {
            _gameManager = gameManager;
            _spawnPosition = spawnPosition;
            _spawnRotation = spawnRotation;
        }

        private void Awake()
        {
            _body = GetComponent<Rigidbody2D>();
            _body.gravityScale = 0f;
            _body.linearDamping = 2f;
            _body.angularDamping = 4f;
            _body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            _body.interpolation = RigidbodyInterpolation2D.Interpolate;
        }

        private void Update()
        {
            bool forward = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
            bool backward = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
            bool left = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
            bool right = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);

            _throttleInput = forward ? 1f : backward ? -1f : 0f;
            _steerInput = left ? 1f : right ? -1f : 0f;
        }

        private void FixedUpdate()
        {
            float forwardSpeed = Vector2.Dot(_body.linearVelocity, transform.up);
            float accel = _throttleInput >= 0f ? acceleration : reverseAcceleration;
            _body.AddForce(transform.up * (_throttleInput * accel), ForceMode2D.Force);

            if (forwardSpeed > maxForwardSpeed)
            {
                _body.linearVelocity = _body.linearVelocity.normalized * maxForwardSpeed;
            }
            else if (forwardSpeed < -maxReverseSpeed)
            {
                _body.linearVelocity = _body.linearVelocity.normalized * maxReverseSpeed;
            }

            float steerFactor = Mathf.Clamp01(_body.linearVelocity.magnitude / 2f);
            float deltaRotation = _steerInput * steering * steerFactor * Time.fixedDeltaTime;
            _body.MoveRotation(_body.rotation + deltaRotation);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            _gameManager?.RegisterCrash();
        }

        public void ResetToSpawn()
        {
            _body.linearVelocity = Vector2.zero;
            _body.angularVelocity = 0f;
            _body.position = _spawnPosition;
            _body.rotation = _spawnRotation;
        }
    }
}
