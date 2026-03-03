using System.Collections.Generic;
using UnityEngine;

namespace Example008.HomeworldLite
{
    [DefaultExecutionOrder(-100)]
    public sealed class SceneBootstrapHomeworldLite : MonoBehaviour
    {
        [Header("World")]
        [SerializeField] private int playerUnitCount = 8;
        [SerializeField] private int enemyUnitCount = 10;
        [SerializeField] private float spawnRadius = 34f;

        [Header("Visual")]
        [SerializeField] private Color playerColor = new Color(0.3f, 0.78f, 0.94f);
        [SerializeField] private Color enemyColor = new Color(0.98f, 0.42f, 0.42f);

        private readonly List<ShipUnit> _playerUnits = new List<ShipUnit>();
        private readonly List<ShipUnit> _enemyUnits = new List<ShipUnit>();

        private void Awake()
        {
            BuildScene();
        }

        private void BuildScene()
        {
            GameObject root = new GameObject("GeneratedHomeworldLite");

            CreateBoard(root.transform);
            SpawnFleet(root.transform, TeamSide.Player, playerUnitCount, new Vector3(-spawnRadius, 10f, 0f), playerColor, _playerUnits);
            SpawnFleet(root.transform, TeamSide.Enemy, enemyUnitCount, new Vector3(spawnRadius, 10f, 0f), enemyColor, _enemyUnits);

            Camera cam = SetupCamera();
            if (cam == null)
            {
                return;
            }

            SelectionManager3D selection = cam.gameObject.AddComponent<SelectionManager3D>();
            selection.Configure(cam);

            Transform marker = CreateDestinationMarker(root.transform);
            CommandLayerLite command = cam.gameObject.AddComponent<CommandLayerLite>();
            command.Configure(selection, cam, marker);

            RtsCameraController rtsCam = cam.gameObject.AddComponent<RtsCameraController>();
            SimpleAiController ai = root.AddComponent<SimpleAiController>();
            ai.Configure(_enemyUnits, _playerUnits);

            ShipUnit[] allUnits = FindObjectsOfType<ShipUnit>();
            GameHudLite hud = root.AddComponent<GameHudLite>();
            hud.Configure(selection, allUnits);

            root.AddComponent<SimulationTickDriver>().Configure(allUnits);
            root.AddComponent<CameraFocusHotkey>().Configure(selection, rtsCam);
        }

        private void CreateBoard(Transform parent)
        {
            GameObject board = GameObject.CreatePrimitive(PrimitiveType.Plane);
            board.name = "SpaceBoard";
            board.transform.SetParent(parent);
            board.transform.position = new Vector3(0f, 0f, 0f);
            board.transform.localScale = new Vector3(8f, 1f, 8f);

            Renderer rend = board.GetComponent<Renderer>();
            if (rend != null)
            {
                rend.material.color = new Color(0.04f, 0.06f, 0.1f);
            }
        }

        private void SpawnFleet(Transform parent, TeamSide team, int count, Vector3 center, Color color, List<ShipUnit> sink)
        {
            for (int i = 0; i < count; i++)
            {
                Vector3 jitter = new Vector3((i % 4) * 4f, 0f, (i / 4) * 4f);
                if (team == TeamSide.Enemy)
                {
                    jitter.x *= -1f;
                }

                GameObject ship = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                ship.name = team + "Ship_" + i;
                ship.transform.SetParent(parent);
                ship.transform.position = center + jitter;
                ship.transform.localScale = new Vector3(1.4f, 0.8f, 2.2f);

                GameObject ring = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                ring.name = "SelectionRing";
                ring.transform.SetParent(ship.transform);
                ring.transform.localPosition = new Vector3(0f, -0.85f, 0f);
                ring.transform.localScale = new Vector3(1.1f, 0.05f, 1.1f);
                Renderer ringRenderer = ring.GetComponent<Renderer>();
                if (ringRenderer != null)
                {
                    ringRenderer.material.color = color;
                }

                Collider ringCollider = ring.GetComponent<Collider>();
                if (ringCollider != null)
                {
                    ringCollider.enabled = false;
                }

                ShipUnit unit = ship.AddComponent<ShipUnit>();
                unit.Configure(team == TeamSide.Player ? 1000 + i : 2000 + i, team, color);
                sink.Add(unit);
            }
        }

        private static Transform CreateDestinationMarker(Transform parent)
        {
            GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            marker.name = "DestinationMarker";
            marker.transform.SetParent(parent);
            marker.transform.localScale = Vector3.one * 1.4f;
            marker.SetActive(false);
            Renderer rend = marker.GetComponent<Renderer>();
            if (rend != null)
            {
                rend.material.color = new Color(0.9f, 0.95f, 1f);
            }
            return marker.transform;
        }

        private static Camera SetupCamera()
        {
            Camera cam = Camera.main;
            if (cam == null)
            {
                GameObject camObj = new GameObject("Main Camera");
                camObj.tag = "MainCamera";
                cam = camObj.AddComponent<Camera>();
            }

            cam.transform.position = new Vector3(0f, 70f, -60f);
            cam.transform.rotation = Quaternion.Euler(45f, 0f, 0f);
            cam.backgroundColor = new Color(0.01f, 0.02f, 0.04f);
            return cam;
        }
    }
}
