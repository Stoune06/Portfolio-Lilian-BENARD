using System.Collections;
using UnityEngine;

//Author : Noé SALES
namespace Platformer.Areas
{
    public class TrapWallTrigger : Area_System
    {
        [SerializeField] private float _Duration = 1;
        [SerializeField] private Vector3 _DistanceAdded;
        [SerializeField] private Transform _WallTransform;

        private Coroutine _WallCoroutine;
        private WaitForEndOfFrame _WaitForFrame = new WaitForEndOfFrame();

        protected override void OnCollision()
        {
            if (_WallCoroutine != null) return;

            m_Collider.enabled = false;
            _WallCoroutine = StartCoroutine(WallGrow());
        }

        private IEnumerator WallGrow()
        {
            float lElapsedTime = 0f;
            Vector3 lStartPos = _WallTransform.position;
            Vector3 lEndPos = lStartPos + _DistanceAdded;

            while (lElapsedTime < _Duration)
            {
                lElapsedTime += Time.deltaTime;
                _WallTransform.position = Vector3.Lerp(lStartPos, lEndPos, lElapsedTime / _Duration);

                yield return _WaitForFrame;
            }
            _WallTransform.position = lEndPos;
            _WallCoroutine = null;
        }
    }
}

