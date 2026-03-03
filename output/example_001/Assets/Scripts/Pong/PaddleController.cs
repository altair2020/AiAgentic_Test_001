using UnityEngine;

namespace Example001.Pong
{
    public sealed class PaddleController : MonoBehaviour
    {
        [SerializeField] private KeyCode upKey = KeyCode.W;
        [SerializeField] private KeyCode downKey = KeyCode.S;
        [SerializeField] private float speed = 8f;
        [SerializeField] private float verticalLimit = 4.2f;

        public void Configure(KeyCode up, KeyCode down, float moveSpeed, float limit)
        {
            upKey = up;
            downKey = down;
            speed = moveSpeed;
            verticalLimit = limit;
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

            var position = transform.position;
            position.y += direction * speed * Time.deltaTime;
            position.y = Mathf.Clamp(position.y, -verticalLimit, verticalLimit);
            transform.position = position;
        }
    }
}
