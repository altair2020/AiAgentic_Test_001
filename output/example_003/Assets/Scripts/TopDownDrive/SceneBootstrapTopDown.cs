using System.Collections.Generic;
using UnityEngine;

namespace Example003.TopDownDrive
{
    [DefaultExecutionOrder(-100)]
    public sealed class SceneBootstrapTopDown : MonoBehaviour
    {
        [Header("World")]
        [SerializeField] private int gridHalfSize = 10;
        [SerializeField] private float tileWorldSize = 2f;
        [SerializeField] private int playerLives = 3;

        [Header("Traffic")]
        [SerializeField] private int trafficCount = 3;

        private Sprite _grass;
        private Sprite _road;
        private Sprite _building;
        private Sprite _playerCar;
        private Sprite _trafficCar;
        private Sprite _pickup;

        private void Awake()
        {
            LoadSprites();
            BuildScene();
        }

        private void LoadSprites()
        {
            _grass = Resources.Load<Sprite>("Sprites/grass_tile");
            _road = Resources.Load<Sprite>("Sprites/road_tile");
            _building = Resources.Load<Sprite>("Sprites/building_tile");
            _playerCar = Resources.Load<Sprite>("Sprites/player_car");
            _trafficCar = Resources.Load<Sprite>("Sprites/traffic_car");
            _pickup = Resources.Load<Sprite>("Sprites/pickup_cash");
        }

        private void BuildScene()
        {
            GameObject root = new GameObject("GeneratedTopDownDrive");
            BuildGround(root.transform);

            TopDownGameManager gameManager = new GameObject("TopDownGameManager").AddComponent<TopDownGameManager>();
            gameManager.transform.SetParent(root.transform);

            Vector2 playerSpawn = new Vector2(0f, -4f);
            PlayerCarController player = CreatePlayer(root.transform, playerSpawn);
            gameManager.Configure(player, playerLives);
            player.Configure(gameManager, playerSpawn, 0f);

            BuildTraffic(root.transform);
            BuildPickups(root.transform, gameManager);
            BuildCamera(player.transform);
        }

        private void BuildGround(Transform parent)
        {
            float tileScale = ComputeUniformScale(_grass, tileWorldSize);

            for (int x = -gridHalfSize; x <= gridHalfSize; x++)
            {
                for (int y = -gridHalfSize; y <= gridHalfSize; y++)
                {
                    Vector2 pos = new Vector2(x * tileWorldSize, y * tileWorldSize);
                    CreateSpriteObject("Grass", parent, _grass, pos, tileScale, 0);

                    if (Mathf.Abs(x) <= 1 || Mathf.Abs(y) <= 1)
                    {
                        CreateSpriteObject("Road", parent, _road, pos, tileScale, 1);
                    }
                }
            }

            List<Vector2> buildingSpots = new List<Vector2>
            {
                new Vector2(-12f, -12f),
                new Vector2(-12f, -7f),
                new Vector2(12f, -12f),
                new Vector2(12f, -7f),
                new Vector2(-12f, 12f),
                new Vector2(-12f, 7f),
                new Vector2(12f, 12f),
                new Vector2(12f, 7f)
            };

            float buildingScale = ComputeUniformScale(_building, 5f);
            foreach (Vector2 spot in buildingSpots)
            {
                GameObject block = CreateSpriteObject("Building", parent, _building, spot, buildingScale, 2);
                BoxCollider2D collider = block.AddComponent<BoxCollider2D>();
                collider.size = Vector2.one;
            }
        }

        private PlayerCarController CreatePlayer(Transform parent, Vector2 spawn)
        {
            float scale = ComputeUniformScale(_playerCar, 2.5f);
            GameObject player = CreateSpriteObject("PlayerCar", parent, _playerCar, spawn, scale, 5);

            Rigidbody2D body = player.AddComponent<Rigidbody2D>();
            body.bodyType = RigidbodyType2D.Dynamic;

            BoxCollider2D collider = player.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(0.55f, 0.85f);

            return player.AddComponent<PlayerCarController>();
        }

        private void BuildTraffic(Transform parent)
        {
            float scale = ComputeUniformScale(_trafficCar, 2.4f);
            Vector2[][] loops =
            {
                new[] { new Vector2(-2f, -10f), new Vector2(-2f, 10f) },
                new[] { new Vector2(2f, 10f), new Vector2(2f, -10f) },
                new[] { new Vector2(-10f, 2f), new Vector2(10f, 2f), new Vector2(10f, -2f), new Vector2(-10f, -2f) }
            };

            int count = Mathf.Min(trafficCount, loops.Length);
            for (int i = 0; i < count; i++)
            {
                GameObject npc = CreateSpriteObject($"TrafficCar_{i + 1}", parent, _trafficCar, loops[i][0], scale, 4);
                Rigidbody2D body = npc.AddComponent<Rigidbody2D>();
                body.bodyType = RigidbodyType2D.Kinematic;

                BoxCollider2D collider = npc.AddComponent<BoxCollider2D>();
                collider.size = new Vector2(0.55f, 0.85f);

                TrafficCarController traffic = npc.AddComponent<TrafficCarController>();
                traffic.Configure(loops[i], 5f + i);
            }
        }

        private void BuildPickups(Transform parent, TopDownGameManager gameManager)
        {
            float scale = ComputeUniformScale(_pickup, 1.2f);
            Vector2[] spots =
            {
                new Vector2(-6f, -2f),
                new Vector2(6f, 2f),
                new Vector2(-8f, 2f),
                new Vector2(8f, -2f),
                new Vector2(0f, 8f),
                new Vector2(0f, -8f)
            };

            foreach (Vector2 spot in spots)
            {
                GameObject pickup = CreateSpriteObject("Pickup", parent, _pickup, spot, scale, 3);
                CircleCollider2D trigger = pickup.AddComponent<CircleCollider2D>();
                trigger.isTrigger = true;
                trigger.radius = 0.45f;

                PickupItem item = pickup.AddComponent<PickupItem>();
                item.Configure(gameManager, 100);
            }
        }

        private static void BuildCamera(Transform target)
        {
            if (Camera.main == null)
            {
                return;
            }

            Camera.main.orthographic = true;
            Camera.main.orthographicSize = 12f;
            Camera.main.backgroundColor = new Color(0.1f, 0.12f, 0.14f);

            CameraFollow2D follow = Camera.main.gameObject.AddComponent<CameraFollow2D>();
            follow.Configure(target);
            Camera.main.transform.position = target.position + new Vector3(0f, 0f, -10f);
        }

        private GameObject CreateSpriteObject(
            string objectName,
            Transform parent,
            Sprite sprite,
            Vector2 position,
            float scale,
            int sortingOrder)
        {
            GameObject obj = new GameObject(objectName);
            obj.transform.SetParent(parent);
            obj.transform.position = position;
            obj.transform.localScale = new Vector3(scale, scale, 1f);

            SpriteRenderer renderer = obj.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.sortingOrder = sortingOrder;
            return obj;
        }

        private static float ComputeUniformScale(Sprite sprite, float targetWorldSize)
        {
            if (sprite == null || sprite.bounds.size.x <= 0f)
            {
                return 1f;
            }

            return targetWorldSize / sprite.bounds.size.x;
        }
    }
}
