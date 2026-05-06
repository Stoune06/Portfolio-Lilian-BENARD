using UnityEngine;
using UnityEngine.Serialization;

//Author : Lilian Benard
namespace Platformer.Killzone
{
    public class MobileKillzone : Killzone
    {
        [SerializeField]
        protected Vector3 _Direction = default;

        [SerializeField]
        protected float _Speed = 10f;

        [SerializeField]
        private float _RaycastRange = 5f;

        [SerializeField]
        private string _PlatformTag = "Platform";

        [FormerlySerializedAs("_TargetLayer")]
        [SerializeField]
        protected LayerMask m_TargetLayer;

        protected void Update()
        {
            DetectCollision();
        }

        protected virtual void DetectCollision()
        {
            Debug.DrawRay(transform.position, _Direction.normalized * _RaycastRange, Color.red);

            RaycastHit2D lHit = Physics2D.Raycast(transform.position, _Direction.normalized, _RaycastRange, m_TargetLayer);
            
            if(!lHit) return;
            //else Debug.Log(lHit.collider.gameObject.name);

            if (!lHit.collider.gameObject.CompareTag(_PlatformTag)) return;
            OnCollision(lHit.collider);
        }

        protected virtual void OnCollision(Collider2D pCollider) { }
    }
}

