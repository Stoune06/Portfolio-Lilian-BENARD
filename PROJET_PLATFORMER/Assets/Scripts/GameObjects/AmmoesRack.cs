using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Author : Noé SALES
namespace Platformer
{
    public class AmmoesRack : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private GameObject _AmmoPrefab;
        [SerializeField] private Transform _ParentTarget;

        [Header("Orbit Parameters")]
        [SerializeField] private Vector3 _BackPos = new Vector3(0, 1.5f, -0.5f);
        [SerializeField] private float _OrbitRadius = 1.5f;
        [SerializeField] private float _OrbRotationSpeed = 50f;

        [Header("Arc Parameters")]
        [SerializeField] private float _Duration = 1.0f;
        [SerializeField] private float _ArcHeight = 2.0f;
        [SerializeField] private float _ArcHorizontal = 0.0f;

        private Coroutine _Coroutine;
        private Transform _ArmedOrb = null;
        private List<Ammo> _Orbs = new List<Ammo>();
        private float _CurrentBaseAngle = 0f;

        private void Update()
        {

            if (Input.GetKeyDown(KeyCode.E))
            {
                SpawnOrb();
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                if (_Orbs.Count <= 0) return;
                for (int i = _Orbs.Count -1; i >= 0; i--)
                {
                    Destroy(_Orbs[i].gameObject);
                    _Orbs.RemoveAt(i);
                }
            }

            if (_Orbs.Count > 0)
            {
                HandleOrbitFormation();
            }
        }

        private void LateUpdate() => transform.position = _ParentTarget.position;

        private void SpawnOrb()
        {
            GameObject lOrb = Instantiate(_AmmoPrefab, transform.position, Quaternion.identity, transform);

            if (lOrb.TryGetComponent(out Ammo lBullet))
            {
                _Orbs.Add(lBullet);
            }
        }

        private void HandleOrbitFormation()
        {
            _CurrentBaseAngle += _OrbRotationSpeed * Time.deltaTime;
            _CurrentBaseAngle %= 360f;

            float lAngleStep = 360f / _Orbs.Count;

            Vector3 lPivotPoint = transform.TransformPoint(_BackPos);

            for (int lIndex = 0; lIndex < _Orbs.Count; lIndex++)
            {
                Ammo lOrb = _Orbs[lIndex];

                if (lOrb == null || !lOrb._CanRotate) continue;

                float lCurrentOrbAngle = _CurrentBaseAngle + (lAngleStep * lIndex);

                float lRadians = lCurrentOrbAngle * Mathf.Deg2Rad;

                float lX = Mathf.Cos(lRadians) * _OrbitRadius;
                float lY = Mathf.Sin(lRadians) * _OrbitRadius;

                Vector3 lLocalOffset = new Vector3(lX, lY, 0);

                Vector3 lWorldOffset = transform.rotation * lLocalOffset;

                lOrb.transform.position = lPivotPoint + lWorldOffset;
            }
        }

        private IEnumerator ArcMovement(Transform lOrb, Transform pTargetTransform, Vector3 pLocalOffset, Action pAction = null)
        {
            if (lOrb == null || pTargetTransform == null) yield break;

            Vector3 lStartPoint = lOrb.position;

            float lElapsedTime = 0f;

            while (lElapsedTime < 1.0f)
            {
                if (lOrb == null || pTargetTransform == null) yield break;

                lElapsedTime += Time.deltaTime / _Duration;

                Vector3 lCurrentEndPoint = pTargetTransform.TransformPoint(pLocalOffset);

                Vector3 lMidPoint = lStartPoint + (lCurrentEndPoint - lStartPoint) / 2;
                Vector3 lDirection = (lCurrentEndPoint - lStartPoint).normalized;
                Vector3 lRightVector = Vector3.Cross(lDirection, Vector3.up);

                Vector3 lControlPoint = lMidPoint + (Vector3.up * _ArcHeight) + (lRightVector * _ArcHorizontal);

                Vector3 lM1 = Vector3.Lerp(lStartPoint, lControlPoint, lElapsedTime);
                Vector3 lM2 = Vector3.Lerp(lControlPoint, lCurrentEndPoint, lElapsedTime); // On va vers le point mouvant
                Vector3 lNextPos = Vector3.Lerp(lM1, lM2, lElapsedTime);

                lOrb.LookAt(lNextPos);
                lOrb.position = lNextPos;

                yield return null;
            }

            if (lOrb != null && pTargetTransform != null)
            {
                lOrb.position = pTargetTransform.TransformPoint(pLocalOffset);
            }

            if (pAction != null) pAction();

            _Coroutine = null;
        }
    }
}

