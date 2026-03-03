using UnityEngine;

namespace Example008.HomeworldLite
{
    public sealed class CameraFocusHotkey : MonoBehaviour
    {
        private SelectionManager3D _selection;
        private RtsCameraController _camera;

        public void Configure(SelectionManager3D selection, RtsCameraController camera)
        {
            _selection = selection;
            _camera = camera;
        }

        private void Update()
        {
            if (_selection == null || _camera == null)
            {
                return;
            }

            if (!Input.GetKeyDown(KeyCode.F) || _selection.Selected.Count == 0)
            {
                return;
            }

            _camera.FocusOn(_selection.GetSelectionCenter());
        }
    }
}
