using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tooling
{
    /// <summary>
    /// <see cref="CustomBody2D"></see> is a class that'll let you mimic basic collision behavior with classes who inherit from <see cref="Collider2D"></see>
    /// </summary>
    [Icon("Assets/Tooling/CustomClasses/RigidBody2D/RigidBodyLogo.png"), RequireComponent(typeof(Collider2D), typeof(Transform)), DisallowMultipleComponent]
    public class CustomBody2D : MonoBehaviour
    {
        private Collider2D _Collider;
        private Transform _Transform;
        [field: SerializeField] public LayerMask CollisionLayerMask { get; private set; }
        [field: SerializeField] public LayerMask TriggerLayerMask { get; private set; }

        private const float RAY_DISTANCE = 0.1f;
        [SerializeField] private Vector3 INSIDE_DISTANCE = Vector3.one * .6f;

        private const float MIN_WALL_NORMAL = 0.05f;
        private const float MAX_WALL_ANGLE = 91f;
        private const float MAX_FLOOR_ANGLE = 1f;
        private const int COLLISION_CHECK_SIZE = 5;

        public bool isOnCeiling { get; private set; } = false;
        public bool isOnGround { get; private set; } = false;
        public bool isOnWall { get; private set; } = false;
        public bool isOnWallFall { get; private set; } = false;

        public Vector3 Velocity { get; private set; }
        [field: SerializeField] public float WallCollideSize { get; private set; } = 1f;

        public Action<Vector2> OnWall;
        public Action<Vector2> OnWallFall;
        public Action OnGroundExited;
        public Action OnWallExited;
        public Action OnCeiling;
        public Action OnGround;

        public OnCollider2DDetected OnCollisionEnter, OnCollisionExited, OnCollisionStay;
        public OnCollider2DDetected OnTriggerEnter, OnTriggerExited, OnTriggerStay;

        private Dictionary<Collider2D, Vector3> _PreviousCollisionToPosition = new Dictionary<Collider2D, Vector3>();
        private Collider2D[] _TriggerBuffer = new Collider2D[COLLISION_CHECK_SIZE];
        private Collider2D[] _CollisionBuffer = new Collider2D[COLLISION_CHECK_SIZE];

        private HashSet<Collider2D> _PreviousTriggerHashSet = new HashSet<Collider2D>();
        private HashSet<Collider2D> _TriggerHashSet = new HashSet<Collider2D>();

        private HashSet<Collider2D> _PreviousCollisionHashSet = new HashSet<Collider2D>();
        private HashSet<Collider2D> _CollisionHashSet = new HashSet<Collider2D>();

        private ContactFilter2D _CollisionFilter;
        private ContactFilter2D _TriggerFilter;

        private RaycastHit2D[] _RayNonAlloc = new RaycastHit2D[1];
        [SerializeField] private bool _DrawDebugGizmo = false;

        void Awake()
        {
            _Collider = GetComponent<Collider2D>();
            _Transform = GetComponent<Transform>();

            _CollisionFilter.SetLayerMask(CollisionLayerMask);
            _CollisionFilter.useLayerMask = true;

            _TriggerFilter.SetLayerMask(TriggerLayerMask);
            _TriggerFilter.useLayerMask = true;

            Physics2D.OverlapBox(_Transform.position, _Collider.bounds.size, 0f, _CollisionFilter, _CollisionBuffer);
            ApplyPreviousPosition();
        }

        void FixedUpdate()
        {
            SetSurfaceCollider(_Transform.position, _Collider.bounds.size, ref _TriggerFilter, ref _TriggerBuffer);
            CheckTriggerAndCollision(_TriggerHashSet, _PreviousTriggerHashSet, OnTriggerEnter, OnTriggerExited, OnTriggerStay, ref _TriggerBuffer);
        }

        /// <summary>
        /// Allows you to translate in any direction while colliding with <see cref="Collider2D"></see>
        /// </summary>
        public void MoveAndSlide(Vector3 pMovement)
        {
            Velocity = pMovement;
            CheckSurface();
            _Transform.position += Velocity;
        }

        private void CheckSurface()
        {
            int lCount = Physics2D.OverlapBox(_Transform.position, _Collider.bounds.size, 0f, _CollisionFilter, _CollisionBuffer);

            if (lCount <= 0)
            {
                if (isOnGround)
                {
                    isOnGround = false;
                    OnGroundExited?.Invoke();
                }
                if (isOnWall)
                {
                    isOnWall = false;
                    OnWallExited?.Invoke();
                }
                isOnCeiling = false;
                isOnWallFall = false;
                _PreviousCollisionHashSet.Clear();
                _PreviousCollisionToPosition.Clear();
                return;
            }

            Collider2D lCollider;
            RaycastHit2D lHit;
            float lAngle;
            bool lStartGround = isOnGround;
            bool lGroundUpdated = false, lCeilUpdated = false, lWallUpdated = false;

            for (int i = 0; i < lCount; i++)
            {
                lCollider = _CollisionBuffer[i];

                if (lCollider == null || lCollider.isTrigger)
                    continue;

                Physics2D.RaycastNonAlloc(_Transform.position, lCollider.ClosestPoint(_Transform.position) - (Vector2)_Transform.position,
                    _RayNonAlloc, 5f, CollisionLayerMask);

                lHit = _RayNonAlloc[0];
                lAngle = Vector2.Angle(lHit.normal, Vector2.up);

                if (lAngle <= MAX_FLOOR_ANGLE && !lGroundUpdated)
                {
                    lGroundUpdated = true;
                    ApplyGround(lHit, lCollider);
                }
                else if (lAngle <= MAX_WALL_ANGLE && !lWallUpdated)
                {
                    if (IsColliderOverlappingWall(lCollider))
                    {
                        lWallUpdated = true;
                        ApplyWall(lHit, lCollider);

                        if (!isOnWallFall && !isOnGround && 0f > Velocity.y)
                        {
                            isOnWallFall = true;
                            OnWallFall?.Invoke(lHit.normal);
                        }
                    }
                }
                else if (!lCeilUpdated)
                {
                    lCeilUpdated = true;
                    ApplyCeiling(lHit, lCollider);
                }
            }

            if(!lGroundUpdated) lGroundUpdated = OverCheckGround() && !isOnWall;

            CheckTriggerAndCollision(_CollisionHashSet, _PreviousCollisionHashSet, OnCollisionEnter, OnCollisionExited, OnCollisionStay, ref _CollisionBuffer);
            ApplyPreviousPosition();

            if (!lGroundUpdated)
            {
                isOnGround = false;
                if (lStartGround) OnGroundExited?.Invoke();
            }
            if (!lWallUpdated)
            {
                isOnWall = false;
                isOnWallFall = false;
                OnWallExited?.Invoke();
            }
            if (!lCeilUpdated) isOnCeiling = false;
        }

        private bool OverCheckGround()
        {
            return Physics2D.BoxCast(new Vector2(transform.position.x, -_Collider.bounds.extents.y), _Collider.bounds.size - INSIDE_DISTANCE, 0f, Vector2.down, CollisionLayerMask).collider != null;
        }

        private bool IsColliderOverlappingWall(Collider2D pCollider)
        {
            Rect lBodyRect = new Rect(
                _Transform.position.x - _Collider.bounds.extents.x,
                _Transform.position.y - WallCollideSize * 0.5f,
                _Collider.bounds.size.x,
                WallCollideSize
            );

            return lBodyRect.Overlaps(new Rect(pCollider.transform.position.x - pCollider.bounds.extents.x, pCollider.transform.position.y - pCollider.bounds.extents.y,
                pCollider.bounds.size.x, pCollider.bounds.size.y));
        }

        private void ApplyPreviousPosition()
        {
            _PreviousCollisionToPosition.Clear();
            Collider2D lCollider;
            for (int i = 0; i < _CollisionBuffer.Length; i++)
            {
                lCollider = _CollisionBuffer[i];
                if (lCollider != null && !_PreviousCollisionToPosition.ContainsKey(lCollider))
                    _PreviousCollisionToPosition.Add(_CollisionBuffer[i], _CollisionBuffer[i].transform.position);
            }
        }

        private void ApplyGround(RaycastHit2D pHit, Collider2D pCollider)
        {
            if (!isOnGround)
            {
                _Transform.position = new Vector3(_Transform.position.x, pHit.point.y + _Collider.bounds.extents.y - Velocity.y, _Transform.position.z);
                Velocity = new Vector3(Velocity.x, 0f, Velocity.z);
                isOnGround = true;
                OnGround?.Invoke();
            }
            if (_PreviousCollisionHashSet.Contains(pCollider) && pCollider.transform.position != _PreviousCollisionToPosition[pCollider]
                && pCollider.transform.position.y - _PreviousCollisionToPosition[pCollider].y > 0f)
                Velocity += pCollider.transform.position - _PreviousCollisionToPosition[pCollider];
            isOnWallFall = false;
        }

        private void ApplyWall(RaycastHit2D pHit, Collider2D pCollider)
        {
            if (!isOnWall)
            {
                isOnWall = true;

                if (pHit.normal.x <= -MIN_WALL_NORMAL && Velocity.x >= 0f)
                    _Transform.position = new Vector3(pHit.point.x - _Collider.bounds.extents.x + Velocity.x, _Transform.position.y, _Transform.position.z);
                else if (pHit.normal.x >= MIN_WALL_NORMAL && Velocity.x <= 0f)
                    _Transform.position = new Vector3(pHit.point.x + _Collider.bounds.extents.x + Velocity.x, _Transform.position.y, _Transform.position.z);

                OnWall?.Invoke(pHit.normal);
            }

            if (pHit.normal.x <= -MIN_WALL_NORMAL && Velocity.x >= 0f)
            {
                Velocity = new Vector3(0f, Velocity.y, Velocity.z);
                if (_PreviousCollisionHashSet.Contains(pCollider) && pCollider.transform.position.x > _PreviousCollisionToPosition[pCollider].x)
                    Velocity -= new Vector3(pCollider.transform.position.x - _PreviousCollisionToPosition[pCollider].x, 0f, 0f);
            }
            else if (pHit.normal.x >= MIN_WALL_NORMAL && Velocity.x <= 0f)
            {
                Velocity = new Vector3(0f, Velocity.y, Velocity.z);
                if (_PreviousCollisionHashSet.Contains(pCollider) && pCollider.transform.position.x > _PreviousCollisionToPosition[pCollider].x)
                    Velocity += new Vector3(pCollider.transform.position.x - _PreviousCollisionToPosition[pCollider].x, 0f, 0f);
            }
        }

        private void ApplyCeiling(RaycastHit2D pHit, Collider2D pCollider)
        {
            if (!isOnCeiling)
            {
                isOnCeiling = true;
                OnCeiling?.Invoke();
            }

            if (Velocity.y >= 0f)
                Velocity = new Vector3(Velocity.x, 0f, Velocity.z);

            if (_PreviousCollisionHashSet.Contains(pCollider) && pCollider.transform.position.y > _PreviousCollisionToPosition[pCollider].y)
                Velocity -= new Vector3(0f, pCollider.transform.position.y - _PreviousCollisionToPosition[pCollider].y, 0f);
        }

        private void SetSurfaceCollider(Vector2 pOrigin, Vector2 pSize, ref ContactFilter2D pFilter, ref Collider2D[] pColliderBuffer)
            => Physics2D.OverlapBox(pOrigin, pSize, 0f, pFilter, pColliderBuffer);

        private void CheckTriggerAndCollision(HashSet<Collider2D> pHashSet, HashSet<Collider2D> pPreviousHashSet,
            OnCollider2DDetected pEnter, OnCollider2DDetected pExited, OnCollider2DDetected pStay, ref Collider2D[] pArray)
        {
            pHashSet.UnionWith(pArray);

            foreach (Collider2D item in pHashSet)
            {
                if (item != null)
                    CallGivenDelegate(item, pEnter, pStay, pPreviousHashSet);
            }

            foreach (Collider2D item in pPreviousHashSet)
            {
                if (item != null && !pHashSet.Contains(item))
                    pExited?.Invoke(item);
            }

            pPreviousHashSet.Clear();
            pPreviousHashSet.UnionWith(pHashSet);
            pHashSet.Clear();
        }

        private void CallGivenDelegate(Collider2D pCollider, OnCollider2DDetected pEnter, OnCollider2DDetected pStay, HashSet<Collider2D> pPreviousHashSet)
        {
            if (pPreviousHashSet.Contains(pCollider))
                pStay?.Invoke(pCollider);
            else
                pEnter?.Invoke(pCollider);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!_DrawDebugGizmo || _Collider == null) return;

            Gizmos.color = isOnGround ? Color.green : Color.red;
            Gizmos.DrawWireCube(_Collider.bounds.center, _Collider.bounds.size);

            if (isOnCeiling)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireCube(_Collider.bounds.center, _Collider.bounds.size * 1.05f);
            }

            Gizmos.color = isOnWall ? Color.black : Color.white;
            Gizmos.DrawWireCube(_Collider.bounds.center, new Vector2(_Collider.bounds.size.x, WallCollideSize));
        }
#endif
    }
    public delegate void OnCollider2DDetected(Collider2D pCollider);
}