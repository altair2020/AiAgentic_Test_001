using System.Collections.Generic;
using UnityEngine;
using Example009.HWR.Simulation;
using Example009.HWR.Camera;

namespace Example009.HWR.Core
{
    public sealed class EntityViewSync : MonoBehaviour
    {
        private IWorldModel _world;
        private readonly Dictionary<EntityId, GameObject> _views = new Dictionary<EntityId, GameObject>();

        public void Configure(IWorldModel world)
        {
            _world = world;
        }

        private void LateUpdate()
        {
            if (_world == null)
            {
                return;
            }

            var snapshots = _world.All();
            var alive = new HashSet<EntityId>();

            for (int i = 0; i < snapshots.Count; i++)
            {
                var s = snapshots[i];
                alive.Add(s.Id);

                if (!_views.TryGetValue(s.Id, out GameObject go))
                {
                    go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                    go.name = $"Entity_{s.Id.Value}";
                    _views[s.Id] = go;
                    var cam = UnityEngine.Camera.main;
                    if (cam != null && cam.GetComponent<RtsCameraController>() == null)
                    {
                        cam.gameObject.AddComponent<RtsCameraController>();
                    }
                }

                go.transform.position = new Vector3(s.Position.X, 1f, s.Position.Z);
                go.transform.localScale = new Vector3(1.3f, 1.0f, 2.2f);
                var rend = go.GetComponent<Renderer>();
                if (rend != null)
                {
                    rend.material.color = s.Team.Value == 0 ? new Color(0.3f, 0.8f, 1f) : new Color(1f, 0.45f, 0.45f);
                }
            }

            var remove = new List<EntityId>();
            foreach (var kv in _views)
            {
                if (!alive.Contains(kv.Key))
                {
                    Object.Destroy(kv.Value);
                    remove.Add(kv.Key);
                }
            }

            for (int i = 0; i < remove.Count; i++)
            {
                _views.Remove(remove[i]);
            }
        }
    }
}
