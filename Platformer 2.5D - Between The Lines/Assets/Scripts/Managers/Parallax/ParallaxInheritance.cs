using UnityEngine;

namespace Platformer.Managers
{
    [System.Serializable]
    public abstract class ParallaxLayer
    {
        public float speed;

        public abstract void Move(Vector3 pMovement);
    }

    [System.Serializable]
    public class ParallaxTransform : ParallaxLayer
    {
        public Transform transform;

        public override void Move(Vector3 pMovement)
        {
            transform.position -= new Vector3 (pMovement.x * speed * Time.deltaTime, 0f, 0f);
            //transform.position -= pMovement * speed * Time.deltaTime;
        }
    }

    [System.Serializable]
    public class ParallaxTiledLayer : ParallaxLayer
    {
        public SpriteRenderer spriteRenderer;

        public override void Move(Vector3 pMovement)
        {

        }
    }
}