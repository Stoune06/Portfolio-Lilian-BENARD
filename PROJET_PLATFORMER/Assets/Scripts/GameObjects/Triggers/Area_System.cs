using UnityEngine;

//Author : Noé SALES
namespace Platformer.Areas
{
    [RequireComponent (typeof(BoxCollider2D), typeof(Rigidbody2D))]
    public class Area_System : MonoBehaviour
    {
        protected BoxCollider2D m_Collider;
        private const string PLAYER_TAG = "CustomPlayer";

        protected void Start()
        {
            if(TryGetComponent(out BoxCollider2D lCollider)) m_Collider = lCollider;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag(PLAYER_TAG)) return;
            OnCollision();
        }

        protected virtual void OnCollision() { }
    }
}

