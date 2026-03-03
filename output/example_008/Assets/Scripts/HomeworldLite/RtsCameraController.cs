using UnityEngine;

namespace Example008.HomeworldLite
{
    public sealed class RtsCameraController : MonoBehaviour
    {
        [SerializeField] private float panSpeed = 55f;
        [SerializeField] private float rotateSpeed = 85f;
        [SerializeField] private float zoomSpeed = 140f;
        [SerializeField] private float minHeight = 20f;
        [SerializeField] private float maxHeight = 120f;

        private void Update()
        {
            float dt = Time.deltaTime;

            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            Vector3 pan = (transform.right * horizontal + Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized * vertical) * (panSpeed * dt);
            transform.position += pan;

            if (Input.GetKey(KeyCode.Q))
            {
                transform.Rotate(Vector3.up, -rotateSpeed * dt, Space.World);
            }
            if (Input.GetKey(KeyCode.E))
            {
                transform.Rotate(Vector3.up, rotateSpeed * dt, Space.World);
            }

            float wheel = Input.mouseScrollDelta.y;
            if (Mathf.Abs(wheel) > 0.01f)
            {
                transform.position += transform.forward * (wheel * zoomSpeed * dt);
            }

            float y = Mathf.Clamp(transform.position.y, minHeight, maxHeight);
            transform.position = new Vector3(transform.position.x, y, transform.position.z);
        }

        public void FocusOn(Vector3 worldPosition)
        {
            Vector3 pos = transform.position;
            transform.position = new Vector3(worldPosition.x, pos.y, worldPosition.z - 25f);
        }
    }
}
