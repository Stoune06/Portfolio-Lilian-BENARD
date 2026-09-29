using System.Collections;
using UnityEngine;

namespace Platformer.Killzone
{
    public class GrabbingKillzone : MobileKillzone
    {
        [SerializeField]
        private float _MaxRange = 500f;

        [SerializeField]
        private Transform _Killzone;

        [SerializeField]
        private LayerMask _LayerMaskRay;

        [SerializeField]
        private float _Duration = 2f;

        private Vector3 _StartPosition;
        private Vector3 _EndPosition;

        private float _CurrentProgress = 0;
        private float _ScaleOffset = 5f;

        private void Start()
        {
            _StartPosition = transform.position;
            _EndPosition = _StartPosition + _Direction.normalized * _MaxRange;
            _Killzone.localScale = new Vector3(_MaxRange + _ScaleOffset, _Killzone.localScale.y, _Killzone.localScale.z);
            Debug.Log("StartPosition :" +  _StartPosition + " EndPosition : " + _EndPosition);
        }


        protected override void DetectCollision()
        {
            Debug.DrawRay(_StartPosition, _Direction * _MaxRange, Color.yellow);
            Collider2D lCollider = Physics2D.Raycast(_StartPosition, _Direction, _MaxRange, m_TargetLayer).collider;
            Collider2D lPlatformCollider = Physics2D.Raycast(_StartPosition, _Direction, _MaxRange, _LayerMaskRay).collider;
            
            

            if (lCollider &&  (!lPlatformCollider || Vector2.Distance(lPlatformCollider.transform.position, _StartPosition) > Vector2.Distance(lCollider.transform.position,_StartPosition)))
            {
                _CurrentProgress += Time.deltaTime / _Duration;
            }
            else
            {
                _CurrentProgress -= Time.deltaTime / _Duration;
            }
            _CurrentProgress = Mathf.Clamp01(_CurrentProgress);
            _Killzone.position = Vector3.Lerp(_StartPosition , _EndPosition , _CurrentProgress) - Vector3.right * _Killzone.localScale.x/2f;
        }
    }
}

