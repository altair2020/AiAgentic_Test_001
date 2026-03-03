using System;
using System.Collections.Generic;
using UnityEngine;
using Example009.HWR.AI;
using Example009.HWR.Camera;
using Example009.HWR.Commands;
using Example009.HWR.Missions;
using Example009.HWR.Simulation;
using Example009.HWR.UI;

namespace Example009.HWR.Core
{
    [DefaultExecutionOrder(-100)]
    public sealed class SceneBootstrapHwr : MonoBehaviour
    {
        [SerializeField] private int playerCount = 12;
        [SerializeField] private int enemyCount = 14;

        private IWorldModel _world;
        private ICommandQueue _queue;
        private IMissionRuntime _mission;

        private void Awake()
        {
            Build();
        }

        private void Build()
        {
            GameObject root = new GameObject("GeneratedHWR009");

            var board = GameObject.CreatePrimitive(PrimitiveType.Plane);
            board.transform.SetParent(root.transform);
            board.transform.localScale = new Vector3(10f, 1f, 10f);
            board.GetComponent<Renderer>().material.color = new Color(0.04f, 0.06f, 0.1f);

            UnityEngine.Camera cam = UnityEngine.Camera.main;
            if (cam == null)
            {
                var camGo = new GameObject("Main Camera");
                camGo.tag = "MainCamera";
                cam = camGo.AddComponent<UnityEngine.Camera>();
            }
            cam.transform.position = new Vector3(0f, 85f, -70f);
            cam.transform.rotation = Quaternion.Euler(47f, 0f, 0f);
            cam.backgroundColor = new Color(0.01f, 0.02f, 0.05f);
            if (cam.GetComponent<RtsCameraController>() == null)
            {
                cam.gameObject.AddComponent<RtsCameraController>();
            }

            var clock = new SimulationClock();
            _world = new WorldModel();
            _queue = new CommandQueue();
            var commands = new CommandProcessor();
            var ai = new AiSystem();
            var movement = new MovementSystem();
            var combat = new CombatSystem();
            _mission = new SkirmishMissionRuntime();

            SpawnFleet(new TeamId(0), playerCount, -40f);
            SpawnFleet(new TeamId(1), enemyCount, 40f);

            var runner = root.AddComponent<SimulationRunner>();
            runner.Configure(clock, _world, _queue, commands, ai, movement, combat);

            var sync = root.AddComponent<EntityViewSync>();
            sync.Configure(_world);

            var overlay = root.AddComponent<RuntimeStatusOverlay>();
            overlay.Configure(_world, _mission);

            root.AddComponent<PlayerInputBridge>().Configure(_world, _queue);
        }

        private void SpawnFleet(TeamId team, int count, float xCenter)
        {
            for (int i = 0; i < count; i++)
            {
                int row = i / 4;
                int col = i % 4;
                float x = xCenter + (team.Value == 0 ? col * 5f : -col * 5f);
                float z = -15f + row * 5f;
                _world.Spawn(new SpawnSpec(team, "frigate", new Float3(x, 0f, z), 100f));
            }
        }
    }

    public sealed class PlayerInputBridge : MonoBehaviour
    {
        private IWorldModel _world;
        private ICommandQueue _queue;
        private readonly List<EntityId> _playerIds = new List<EntityId>();

        public void Configure(IWorldModel world, ICommandQueue queue)
        {
            _world = world;
            _queue = queue;
        }

        private void Update()
        {
            if (_world == null || _queue == null)
            {
                return;
            }

            if (!Input.GetMouseButtonDown(1))
            {
                return;
            }

            _playerIds.Clear();
            var all = _world.All();
            for (int i = 0; i < all.Count; i++)
            {
                if (all[i].Team.Value == 0)
                {
                    _playerIds.Add(all[i].Id);
                }
            }
            if (_playerIds.Count == 0)
            {
                return;
            }

            Ray ray = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out RaycastHit hit, 5000f))
            {
                return;
            }

            _queue.Enqueue(new MoveCommand(Guid.NewGuid(), new TeamId(0), _world.CurrentTick, _playerIds, new Float3(hit.point.x, 0f, hit.point.z)));
        }
    }
}
