using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

//Author : Noé SALES
namespace Platformer.Platforms
{
    public class MovablePlatform : PlatformCollision
    {
        [Header("Parameters")]
        [SerializeField] private float _TransitionDuration = 2f;
        [SerializeField] private bool _StartInLaunch = true;
        private bool _IsMoving = false;
        [Header("Objects")]
        [SerializeField] private Transform _PointsContainer;

        private List<Vector2> _Points = new List<Vector2>();

        private Vector2 _ActualPos;
        private Vector2 _NextPos;
        private int _PointsIndex = 0;
        private WaitForEndOfFrame _WaitForFrame = new WaitForEndOfFrame();

        private Coroutine _MoveCoroutine;
        [SerializeField] private float _Margin = 0.05f;
        private Collider2D PlayerCollider;


        private new void Start()
        {
            base.Start();
            foreach (Transform lPoint in _PointsContainer)
            {
                _Points.Add(lPoint.position);
            }
            _ActualPos = _Points[0];
            transform.position = _ActualPos;
            if (_Points.Count >= 2) _NextPos = GetNextIndex();
            if (Player.Player.Instance != null && Player.Player.Instance.TryGetComponent(out Collider2D lCollider)) PlayerCollider = lCollider;

            //TODO RESTART ON PLAYER DEATH
            //Player.Player.OnPlayerDeath += () =>
            //{
            //    StopAllCoroutines(); _IsMoving = _StartInLaunch; transform.position = _Points[0];
            //    _NextPos = _Points[0];
            //    _ActualPos = transform.position;
            //};
            _IsMoving = _StartInLaunch;

            EnableCollisions();
        }

        protected override void PlatformBehavior()
        {
            if (PlayerInXBound() && PlayerCompletelyAbove()) _IsMoving = true;

            if (_MoveCoroutine != null || _Points.Count < 2) return;
            else if (_IsMoving) _MoveCoroutine = StartCoroutine(MoveCoroutine());


            if (PlayerCollider == null) return;
        }
        bool _Check = false;

        private IEnumerator MoveCoroutine()
        {
            float lElapsedTime = 0;
            Vector3 lStartValue;
            Vector3 lAddValue;


            while (lElapsedTime < _TransitionDuration)
            {
                lElapsedTime += Time.deltaTime;
                float lRatio = lElapsedTime / _TransitionDuration;

                lStartValue = transform.position;
                transform.position = Vector3.Lerp(_ActualPos, _NextPos, lRatio);
                lAddValue = lStartValue - transform.position;

                if(lAddValue.y > 0f)
                {
                    if (PlayerAbovePlatform() && PlayerInXBound())
                    {
                        PlayerCollider.transform.position -= lAddValue;
                        if (!_Check)
                        {
                            _Check = true;
                            PlayerCollider.transform.position -= lAddValue * 3f;
                        }
                    }
                    else _Check = false;
                }

                yield return _WaitForFrame;
            }
            transform.position = _ActualPos = _NextPos;
            _NextPos = GetNextIndex();
            _MoveCoroutine = null;
        }

        private Vector2 GetNextIndex()
        {
            if (_PointsIndex >= _Points.Count - 1) _PointsIndex = 0;
            else _PointsIndex++;
            return _Points[_PointsIndex];
        }

        public void EnableCollisions()
        {
            if (m_Collider.enabled) return;
            m_Collider.enabled = true;
        }

        public void DisableCollisions()
        {
            if (!m_Collider.enabled) return;
            m_Collider.enabled = false;
        }

        private bool PlayerCompletelyAbove()
        {
            if (PlayerCollider == null) return false;
            return PlayerCollider.bounds.min.y >= m_Collider.bounds.min.y;
        }

        private bool PlayerAbovePlatform()
        {
            if (PlayerCollider == null) return false;
            return PlayerCollider.bounds.min.y + 3f >= m_Collider.bounds.max.y ;
        }

        private bool PlayerInXBound()
        {
            if (PlayerCollider == null || m_Collider == null) return false;
            return m_Collider.enabled && PlayerCollider.bounds.min.x >= m_Collider.bounds.min.x - _Margin && PlayerCollider.bounds.max.x <= m_Collider.bounds.max.x + _Margin;
        }
    }
}