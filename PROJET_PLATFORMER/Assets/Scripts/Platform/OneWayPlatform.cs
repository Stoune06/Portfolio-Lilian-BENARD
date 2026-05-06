using UnityEngine;

//Author : Noé SALES
namespace Platformer.Platforms
{
    public class OneWayPlatform : PlatformCollision
    {
        [SerializeField] private float _Margin = 0.05f;
        private Collider2D PlayerCollider;

        private new void Start()
        {
            base.Start();
            if(Player.Player.Instance != null && Player.Player.Instance.TryGetComponent(out Collider2D lCollider)) PlayerCollider = lCollider;
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

        protected override void PlatformBehavior()
        {
            if (PlayerCollider == null) return;

            if (PlayerCompletelyAbove())
                EnableCollisions();
            else
                DisableCollisions();
        }
    }
}