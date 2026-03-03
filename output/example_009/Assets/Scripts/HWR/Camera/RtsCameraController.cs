using UnityEngine;

namespace Example009.HWR.Camera
{
    public sealed class RtsCameraController : MonoBehaviour
    {
        [SerializeField] private float panSpeed = 60f;
        [SerializeField] private float rotateSpeed = 90f;
        [SerializeField] private float zoomSpeed = 150f;
        [SerializeField] private float minY = 18f;
        [SerializeField] private float maxY = 160f;

        private void Update()
        {
            float dt = Time.deltaTime;

            float x = Input.GetAxisRaw("Horizontal");
            float z = Input.GetAxisRaw("Vertical");
            Vector3 forwardOnPlane = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
            transform.position += (transform.right * x + forwardOnPlane * z) * (panSpeed * dt);

            if (Input.GetKey(KeyCode.Q))
            {
                transform.Rotate(Vector3.up, -rotateSpeed * dt, Space.World);
            }
            if (Input.GetKey(KeyCode.E))
            {
                transform.Rotate(Vector3.up, rotateSpeed * dt, Space.World);
            }

            float wheel = Input.mouseScrollDelta.y;
            transform.position += transform.forward * (wheel * zoomSpeed * dt);

            Vector3 p = transform.position;
            p.y = Mathf.Clamp(p.y, minY, maxY);
            transform.position = p;
        }
    }
}
