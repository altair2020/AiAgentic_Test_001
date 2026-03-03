using UnityEngine;

namespace Example002.Pong3D
{
    [DefaultExecutionOrder(-100)]
    public sealed class SceneBootstrap3D : MonoBehaviour
    {
        [Header("Arena")]
        [SerializeField] private float arenaHalfWidth = 11f;
        [SerializeField] private float arenaHalfDepth = 6f;

        [Header("Paddles")]
        [SerializeField] private float paddleXOffset = 9.5f;
        [SerializeField] private Vector3 paddleSize = new Vector3(0.6f, 1.6f, 2.1f);
        [SerializeField] private float paddleSpeed = 10f;

        [Header("Ball")]
        [SerializeField] private float ballDiameter = 0.7f;
        [SerializeField] private float ballLaunchSpeed = 9f;
        [SerializeField] private float ballMaxZDirection = 0.75f;

        [Header("Rules")]
        [SerializeField] private int winningScore = 5;

        private void Awake()
        {
            BuildScene();
        }

        private void BuildScene()
        {
            GameObject root = new GameObject("GeneratedPong3D");

            CreateFloor(root.transform);

            GameManager3D gameManager = CreateGameManager(root.transform);
            BallController3D ball = CreateBall(root.transform);

            CreatePaddle(root.transform, "LeftPaddle", new Vector3(-paddleXOffset, paddleSize.y * 0.5f, 0f), KeyCode.W, KeyCode.S, Color.cyan);
            CreatePaddle(root.transform, "RightPaddle", new Vector3(paddleXOffset, paddleSize.y * 0.5f, 0f), KeyCode.UpArrow, KeyCode.DownArrow, Color.yellow);

            CreateWall(root.transform, "TopWall", new Vector3(0f, 1f, arenaHalfDepth + 0.4f), new Vector3(arenaHalfWidth * 2f + 2f, 2f, 0.6f));
            CreateWall(root.transform, "BottomWall", new Vector3(0f, 1f, -arenaHalfDepth - 0.4f), new Vector3(arenaHalfWidth * 2f + 2f, 2f, 0.6f));

            CreateGoal(root.transform, "LeftGoal", new Vector3(-arenaHalfWidth - 0.8f, 1f, 0f), new Vector3(0.6f, 3f, arenaHalfDepth * 2f + 1f), PlayerSide3D.Right, gameManager);
            CreateGoal(root.transform, "RightGoal", new Vector3(arenaHalfWidth + 0.8f, 1f, 0f), new Vector3(0.6f, 3f, arenaHalfDepth * 2f + 1f), PlayerSide3D.Left, gameManager);

            gameManager.Configure(ball, winningScore);
            ball.Configure(ballLaunchSpeed, ballMaxZDirection);

            SetupCamera();
            SetupLighting();
        }

        private static GameManager3D CreateGameManager(Transform parent)
        {
            GameObject gameManagerObject = new GameObject("GameManager3D");
            gameManagerObject.transform.SetParent(parent);
            return gameManagerObject.AddComponent<GameManager3D>();
        }

        private PaddleController3D CreatePaddle(
            Transform parent,
            string objectName,
            Vector3 position,
            KeyCode upKey,
            KeyCode downKey,
            Color color)
        {
            GameObject paddleObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            paddleObject.name = objectName;
            paddleObject.transform.SetParent(parent);
            paddleObject.transform.position = position;
            paddleObject.transform.localScale = paddleSize;

            Rigidbody body = paddleObject.AddComponent<Rigidbody>();
            body.useGravity = false;
            body.isKinematic = true;
            body.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;

            ApplyColor(paddleObject, color);

            PaddleController3D paddle = paddleObject.AddComponent<PaddleController3D>();
            paddle.Configure(upKey, downKey, paddleSpeed, arenaHalfDepth - (paddleSize.z * 0.5f));
            return paddle;
        }

        private BallController3D CreateBall(Transform parent)
        {
            GameObject ballObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            ballObject.name = "Ball";
            ballObject.transform.SetParent(parent);
            ballObject.transform.position = new Vector3(0f, ballDiameter * 0.5f, 0f);
            ballObject.transform.localScale = Vector3.one * ballDiameter;

            Rigidbody body = ballObject.AddComponent<Rigidbody>();
            body.useGravity = false;
            body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            body.interpolation = RigidbodyInterpolation.Interpolate;
            body.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;

            ApplyColor(ballObject, Color.white);

            return ballObject.AddComponent<BallController3D>();
        }

        private static void CreateFloor(Transform parent)
        {
            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            floor.name = "Floor";
            floor.transform.SetParent(parent);
            floor.transform.position = Vector3.zero;
            floor.transform.localScale = new Vector3(2.4f, 1f, 1.4f);

            Renderer renderer = floor.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = new Color(0.1f, 0.12f, 0.15f);
            }
        }

        private void CreateWall(Transform parent, string objectName, Vector3 position, Vector3 size)
        {
            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = objectName;
            wall.transform.SetParent(parent);
            wall.transform.position = position;
            wall.transform.localScale = size;
            ApplyColor(wall, new Color(0.3f, 0.35f, 0.45f));
        }

        private void CreateGoal(
            Transform parent,
            string objectName,
            Vector3 position,
            Vector3 size,
            PlayerSide3D scoringPlayer,
            GameManager3D gameManager)
        {
            GameObject goal = new GameObject(objectName);
            goal.transform.SetParent(parent);
            goal.transform.position = position;

            BoxCollider trigger = goal.AddComponent<BoxCollider>();
            trigger.size = size;
            trigger.isTrigger = true;

            GoalZone3D goalZone = goal.AddComponent<GoalZone3D>();
            goalZone.Configure(scoringPlayer, gameManager);
        }

        private static void SetupCamera()
        {
            if (Camera.main == null)
            {
                return;
            }

            Camera.main.transform.position = new Vector3(0f, 16f, -13f);
            Camera.main.transform.rotation = Quaternion.Euler(50f, 0f, 0f);
            Camera.main.backgroundColor = new Color(0.05f, 0.06f, 0.09f);
        }

        private static void SetupLighting()
        {
            Light existing = Object.FindObjectOfType<Light>();
            if (existing != null)
            {
                return;
            }

            GameObject lightObject = new GameObject("Directional Light");
            Light light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.color = new Color(1f, 0.97f, 0.9f);
            light.intensity = 1.15f;
            lightObject.transform.rotation = Quaternion.Euler(45f, -35f, 0f);
        }

        private static void ApplyColor(GameObject target, Color color)
        {
            Renderer renderer = target.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = color;
            }
        }
    }
}
