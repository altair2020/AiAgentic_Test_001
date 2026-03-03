using UnityEngine;

namespace Example001.Pong
{
    [DefaultExecutionOrder(-100)]
    public sealed class SceneBootstrap : MonoBehaviour
    {
        [Header("Arena")]
        [SerializeField] private float arenaHalfWidth = 8.5f;
        [SerializeField] private float arenaHalfHeight = 4.5f;

        [Header("Paddles")]
        [SerializeField] private float paddleXOffset = 7f;
        [SerializeField] private Vector2 paddleSize = new Vector2(0.35f, 1.8f);
        [SerializeField] private float paddleSpeed = 9f;

        [Header("Ball")]
        [SerializeField] private float ballRadius = 0.22f;
        [SerializeField] private float ballLaunchSpeed = 7f;
        [SerializeField] private float ballMaxYDirection = 0.75f;

        [Header("Rules")]
        [SerializeField] private int winningScore = 5;

        private Sprite _whiteSprite;

        private void Awake()
        {
            BuildScene();
        }

        private void BuildScene()
        {
            _whiteSprite = CreateSolidSprite();

            var root = new GameObject("GeneratedPong");

            GameManager gameManager = CreateGameManager(root.transform);
            BallController ball = CreateBall(root.transform);

            CreatePaddle(
                root.transform,
                "LeftPaddle",
                new Vector2(-paddleXOffset, 0f),
                KeyCode.W,
                KeyCode.S,
                Color.cyan);

            CreatePaddle(
                root.transform,
                "RightPaddle",
                new Vector2(paddleXOffset, 0f),
                KeyCode.UpArrow,
                KeyCode.DownArrow,
                Color.yellow);

            CreateWall(root.transform, "TopWall", new Vector2(0f, arenaHalfHeight + 0.35f), new Vector2(arenaHalfWidth * 2f + 2f, 0.5f));
            CreateWall(root.transform, "BottomWall", new Vector2(0f, -arenaHalfHeight - 0.35f), new Vector2(arenaHalfWidth * 2f + 2f, 0.5f));

            CreateGoal(root.transform, "LeftGoal", new Vector2(-arenaHalfWidth - 0.6f, 0f), new Vector2(0.5f, arenaHalfHeight * 2f + 1f), PlayerSide.Right, gameManager);
            CreateGoal(root.transform, "RightGoal", new Vector2(arenaHalfWidth + 0.6f, 0f), new Vector2(0.5f, arenaHalfHeight * 2f + 1f), PlayerSide.Left, gameManager);

            gameManager.Configure(ball, winningScore);
            ball.Configure(ballLaunchSpeed, ballMaxYDirection);

            SetupCamera();
        }

        private GameManager CreateGameManager(Transform parent)
        {
            var gameManagerObject = new GameObject("GameManager");
            gameManagerObject.transform.SetParent(parent);
            return gameManagerObject.AddComponent<GameManager>();
        }

        private PaddleController CreatePaddle(
            Transform parent,
            string objectName,
            Vector2 position,
            KeyCode upKey,
            KeyCode downKey,
            Color color)
        {
            var paddleObject = new GameObject(objectName);
            paddleObject.transform.SetParent(parent);
            paddleObject.transform.position = position;
            paddleObject.transform.localScale = new Vector3(paddleSize.x, paddleSize.y, 1f);

            var spriteRenderer = paddleObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = _whiteSprite;
            spriteRenderer.color = color;

            var collider = paddleObject.AddComponent<BoxCollider2D>();
            collider.size = Vector2.one;

            var body = paddleObject.AddComponent<Rigidbody2D>();
            body.bodyType = RigidbodyType2D.Kinematic;
            body.gravityScale = 0f;
            body.constraints = RigidbodyConstraints2D.FreezeRotation;

            var paddle = paddleObject.AddComponent<PaddleController>();
            paddle.Configure(upKey, downKey, paddleSpeed, arenaHalfHeight - (paddleSize.y * 0.5f));

            return paddle;
        }

        private BallController CreateBall(Transform parent)
        {
            var ballObject = new GameObject("Ball");
            ballObject.transform.SetParent(parent);
            ballObject.transform.position = Vector3.zero;
            ballObject.transform.localScale = new Vector3(ballRadius * 2f, ballRadius * 2f, 1f);

            var spriteRenderer = ballObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = _whiteSprite;
            spriteRenderer.color = Color.white;

            var circleCollider = ballObject.AddComponent<CircleCollider2D>();
            circleCollider.radius = 0.5f;

            var body = ballObject.AddComponent<Rigidbody2D>();
            body.bodyType = RigidbodyType2D.Dynamic;
            body.gravityScale = 0f;
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            body.interpolation = RigidbodyInterpolation2D.Interpolate;
            body.freezeRotation = true;

            return ballObject.AddComponent<BallController>();
        }

        private void CreateWall(Transform parent, string objectName, Vector2 position, Vector2 size)
        {
            var wallObject = new GameObject(objectName);
            wallObject.transform.SetParent(parent);
            wallObject.transform.position = position;

            var collider = wallObject.AddComponent<BoxCollider2D>();
            collider.size = size;
        }

        private void CreateGoal(
            Transform parent,
            string objectName,
            Vector2 position,
            Vector2 size,
            PlayerSide scoringPlayer,
            GameManager gameManager)
        {
            var goalObject = new GameObject(objectName);
            goalObject.transform.SetParent(parent);
            goalObject.transform.position = position;

            var collider = goalObject.AddComponent<BoxCollider2D>();
            collider.size = size;
            collider.isTrigger = true;

            var goalZone = goalObject.AddComponent<GoalZone>();
            goalZone.Configure(scoringPlayer, gameManager);
        }

        private static Sprite CreateSolidSprite()
        {
            var texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, Color.white);
            texture.Apply();
            texture.filterMode = FilterMode.Point;
            return Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
        }

        private void SetupCamera()
        {
            if (Camera.main == null)
            {
                return;
            }

            Camera.main.orthographic = true;
            Camera.main.orthographicSize = arenaHalfHeight + 1f;
            Camera.main.transform.position = new Vector3(0f, 0f, -10f);
            Camera.main.backgroundColor = new Color(0.07f, 0.07f, 0.1f);
        }
    }
}
