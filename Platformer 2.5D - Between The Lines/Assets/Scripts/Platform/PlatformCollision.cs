using Tooling;
using UnityEngine;

//Author : Noé SALES
namespace Platformer.Platforms
{
    public class PlatformCollision : MonoBehaviour
    {
        protected Transform m_Body;
        protected Collider2D m_Collider;
        protected SpriteRenderer m_Renderer;

        protected void Start()
        {
            m_Body = transform.GetChild(0);
            if(TryGetComponent(out Collider2D lCollider)) m_Collider = lCollider;
            m_Renderer = GetComponentInChildren<SpriteRenderer>();
        }
        protected void Update()
        {
            PlatformBehavior();
        }

        protected virtual void PlatformBehavior() { }
    }
}