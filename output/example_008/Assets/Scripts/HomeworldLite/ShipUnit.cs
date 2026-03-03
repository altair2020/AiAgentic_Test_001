using UnityEngine;

namespace Example008.HomeworldLite
{
    public enum TeamSide
    {
        Player = 0,
        Enemy = 1
    }

    [RequireComponent(typeof(Rigidbody))]
    public sealed class ShipUnit : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 16f;
        [SerializeField] private float turnSpeed = 180f;
        [SerializeField] private float attackRange = 20f;
        [SerializeField] private float attackDamage = 8f;
        [SerializeField] private float attackCooldown = 0.65f;
        [SerializeField] private float maxHealth = 100f;

        private Rigidbody _body;
        private Renderer _renderer;
        private Transform _ring;
        private float _health;
        private float _nextAttackAt;

        public int UnitId { get; private set; }
        public TeamSide Team { get; private set; }
        public Vector3 MoveTarget { get; private set; }
        public ShipUnit AttackTarget { get; private set; }
        public bool IsSelected { get; private set; }
        public bool IsAlive => _health > 0f;

        private void Awake()
        {
            _body = GetComponent<Rigidbody>();
            _renderer = GetComponentInChildren<Renderer>();
            _ring = transform.Find("SelectionRing");

            _body.useGravity = false;
            _body.isKinematic = true;
            _body.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

            _health = maxHealth;
            MoveTarget = transform.position;
            SetSelected(false);
        }

        public void Configure(int unitId, TeamSide team, Color teamColor)
        {
            UnitId = unitId;
            Team = team;
            if (_renderer != null)
            {
                _renderer.material.color = teamColor;
            }
        }

        public void SetSelected(bool selected)
        {
            IsSelected = selected;
            if (_ring != null)
            {
                _ring.gameObject.SetActive(selected);
            }
        }

        public void SetMoveTarget(Vector3 target)
        {
            MoveTarget = target;
            AttackTarget = null;
        }

        public void SetAttackTarget(ShipUnit target)
        {
            AttackTarget = target;
        }

        public void ReceiveDamage(float amount)
        {
            if (!IsAlive)
            {
                return;
            }

            _health -= Mathf.Max(0f, amount);
            if (_health <= 0f)
            {
                _health = 0f;
                gameObject.SetActive(false);
            }
        }

        public void Tick(float dt)
        {
            if (!IsAlive)
            {
                return;
            }

            if (AttackTarget != null && AttackTarget.IsAlive)
            {
                MoveTarget = AttackTarget.transform.position;
                float sqrDist = (MoveTarget - transform.position).sqrMagnitude;
                if (sqrDist <= attackRange * attackRange)
                {
                    TryAttack();
                }
            }

            MoveTowardsTarget(dt);
        }

        private void MoveTowardsTarget(float dt)
        {
            Vector3 delta = MoveTarget - transform.position;
            delta.y = 0f;
            if (delta.sqrMagnitude < 0.05f)
            {
                return;
            }

            Vector3 direction = delta.normalized;
            Quaternion targetRot = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRot,
                turnSpeed * dt);

            Vector3 next = transform.position + direction * (moveSpeed * dt);
            _body.MovePosition(next);
        }

        private void TryAttack()
        {
            if (Time.time < _nextAttackAt || AttackTarget == null)
            {
                return;
            }

            _nextAttackAt = Time.time + attackCooldown;
            AttackTarget.ReceiveDamage(attackDamage);
        }
    }
}
