using UnityEngine;

namespace Example002.Pong3D
{
    public sealed class PaddleController3D : MonoBehaviour
    {
        [SerializeField] private KeyCode upKey = KeyCode.W;
        [SerializeField] private KeyCode downKey = KeyCode.S;
        [SerializeField] private float speed = 10f;
        [SerializeField] private float depthLimit = 5f;

        public void Configure(KeyCode up, KeyCode down, float moveSpeed, float zLimit)
        {
            upKey = up;
            downKey = down;
            speed = moveSpeed;
            depthLimit = zLimit;
        }

        private void Update()
        {
            float direction = 0f;

            if (Input.GetKey(upKey))
            {
                direction += 1f;
            }

            if (Input.GetKey(downKey))
            {
                direction -= 1f;
            }

            Vector3 position = transform.position;
            position.z += direction * speed * Time.deltaTime;
            position.z = Mathf.Clamp(position.z, -depthLimit, depthLimit);
            transform.position = position;
        }
    }
}
