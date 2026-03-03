using System.Collections.Generic;
using UnityEngine;

namespace Example008.HomeworldLite
{
    public sealed class SelectionManager3D : MonoBehaviour
    {
        [SerializeField] private Camera worldCamera;

        private readonly List<ShipUnit> _selected = new List<ShipUnit>();
        private Vector2 _dragStart;
        private bool _isDragging;

        public IReadOnlyList<ShipUnit> Selected => _selected;

        public void Configure(Camera cam)
        {
            worldCamera = cam;
        }

        private void Update()
        {
            if (worldCamera == null)
            {
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                _dragStart = Input.mousePosition;
                _isDragging = true;
            }

            if (Input.GetMouseButtonUp(0) && _isDragging)
            {
                Vector2 end = Input.mousePosition;
                _isDragging = false;

                if ((end - _dragStart).sqrMagnitude < 36f)
                {
                    SelectSingle(end);
                }
                else
                {
                    SelectBox(_dragStart, end);
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ClearSelection();
            }
        }

        public Vector3 GetSelectionCenter()
        {
            if (_selected.Count == 0)
            {
                return Vector3.zero;
            }

            Vector3 center = Vector3.zero;
            for (int i = 0; i < _selected.Count; i++)
            {
                center += _selected[i].transform.position;
            }
            return center / _selected.Count;
        }

        private void SelectSingle(Vector2 screenPos)
        {
            ClearSelection();
            Ray ray = worldCamera.ScreenPointToRay(screenPos);
            if (Physics.Raycast(ray, out RaycastHit hit, 5000f))
            {
                ShipUnit unit = hit.collider.GetComponentInParent<ShipUnit>();
                if (unit != null && unit.Team == TeamSide.Player && unit.IsAlive)
                {
                    _selected.Add(unit);
                    unit.SetSelected(true);
                }
            }
        }

        private void SelectBox(Vector2 start, Vector2 end)
        {
            ClearSelection();

            Vector2 min = Vector2.Min(start, end);
            Vector2 max = Vector2.Max(start, end);

            ShipUnit[] allUnits = FindObjectsOfType<ShipUnit>();
            foreach (ShipUnit unit in allUnits)
            {
                if (!unit.IsAlive || unit.Team != TeamSide.Player)
                {
                    continue;
                }

                Vector3 screen = worldCamera.WorldToScreenPoint(unit.transform.position);
                if (screen.z > 0f && screen.x >= min.x && screen.x <= max.x && screen.y >= min.y && screen.y <= max.y)
                {
                    _selected.Add(unit);
                    unit.SetSelected(true);
                }
            }
        }

        private void ClearSelection()
        {
            foreach (ShipUnit unit in _selected)
            {
                if (unit != null)
                {
                    unit.SetSelected(false);
                }
            }
            _selected.Clear();
        }
    }
}
