using UnityEngine;

namespace Example002.Pong3D
{
    [RequireComponent(typeof(Rigidbody))]
    public sealed class BallController3D : MonoBehaviour
    {
        [SerializeField] private float launchSpeed = 9f;
        [SerializeField] private float maxZDirection = 0.75f;

        private Rigidbody _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        public void Configure(float speed, float zDirectionLimit)
        {
            launchSpeed = Mathf.Max(0.1f, speed);
            maxZDirection = Mathf.Clamp(zDirectionLimit, 0.05f, 0.99f);
        }

        public void ResetBall()
        {
            transform.position = new Vector3(0f, transform.localScale.y * 0.5f, 0f);
            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;

            float xDirection = Random.value < 0.5f ? -1f : 1f;
            float zDirection = Random.Range(-maxZDirection, maxZDirection);
            Vector3 direction = new Vector3(xDirection, 0f, zDirection).normalized;

            _rigidbody.linearVelocity = direction * launchSpeed;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.contactCount == 0)
            {
                return;
            }

            Vector3 incomingVelocity = _rigidbody.linearVelocity;
            Vector3 normal = collision.GetContact(0).normal;
            Vector3 reflectedVelocity = Vector3.Reflect(incomingVelocity.normalized, normal);

            float speed = Mathf.Max(launchSpeed, incomingVelocity.magnitude);
            if (collision.collider.TryGetComponent<PaddleController3D>(out _))
            {
                speed *= 1.05f;
            }

            _rigidbody.linearVelocity = reflectedVelocity * speed;
        }
    }
}
