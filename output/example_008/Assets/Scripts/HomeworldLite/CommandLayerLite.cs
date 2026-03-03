using System.Collections.Generic;
using UnityEngine;

namespace Example008.HomeworldLite
{
    public sealed class CommandLayerLite : MonoBehaviour
    {
        [SerializeField] private SelectionManager3D selection;
        [SerializeField] private Camera worldCamera;
        [SerializeField] private Transform destinationMarker;
        [SerializeField] private float slotSpacing = 8f;

        private FormationType _formation = FormationType.Line;

        public void Configure(SelectionManager3D selectionManager, Camera cam, Transform marker)
        {
            selection = selectionManager;
            worldCamera = cam;
            destinationMarker = marker;
        }

        private void Update()
        {
            if (selection == null || worldCamera == null)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                _formation = FormationType.Line;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                _formation = FormationType.Sphere;
            }

            if (!Input.GetMouseButtonDown(1))
            {
                return;
            }

            IReadOnlyList<ShipUnit> units = selection.Selected;
            if (units.Count == 0)
            {
                return;
            }

            Ray ray = worldCamera.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out RaycastHit hit, 5000f))
            {
                return;
            }

            ShipUnit enemy = hit.collider.GetComponentInParent<ShipUnit>();
            if (enemy != null && enemy.Team == TeamSide.Enemy && enemy.IsAlive)
            {
                for (int i = 0; i < units.Count; i++)
                {
                    units[i].SetAttackTarget(enemy);
                }
                return;
            }

            Vector3 destination = hit.point;
            Vector3 center = selection.GetSelectionCenter();
            Vector3 forward = (destination - center).normalized;
            if (forward.sqrMagnitude < 0.01f)
            {
                forward = Vector3.forward;
            }

            List<Vector3> slots = FormationServiceLite.BuildSlots(units.Count, destination, forward, slotSpacing, _formation);
            for (int i = 0; i < units.Count; i++)
            {
                units[i].SetMoveTarget(slots[i]);
            }

            if (destinationMarker != null)
            {
                destinationMarker.position = destination + Vector3.up * 0.2f;
                destinationMarker.gameObject.SetActive(true);
            }
        }
    }
}
