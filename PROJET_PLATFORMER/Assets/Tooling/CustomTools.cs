using System;
using System.Collections;
using UnityEngine;


namespace Platformer.Tools
{
    public static class CustomTools
    {
        public static IEnumerator RefreshCollisions(this Rigidbody2D pRigidbody)
        {
            pRigidbody.bodyType = RigidbodyType2D.Static;
            yield return null;
            pRigidbody.bodyType = RigidbodyType2D.Kinematic;
        }

        public static Vector2 GetChildSize<T>(this T pBehavior, int pChildIndex) where T : Behaviour
        {
            SpriteRenderer lRenderer = pBehavior.transform.GetChild(pChildIndex).GetComponent<SpriteRenderer>();
            return lRenderer.size;
        }

        public static bool IsInsideSprite(this Vector3 pPosition, SpriteRenderer pRenderer)
        {
            Bounds lBounds = pRenderer.bounds;
            return lBounds.Contains(pPosition);
        }

        public static IEnumerator Timer(float pTime, Action pActionToExecute)
        {
            float lElpasedTime = 0;
            while (lElpasedTime < pTime)
            {
                lElpasedTime += Time.deltaTime;
                yield return null;
            }
            pActionToExecute();
        }
        public static float GetDurationFromName(this Animator pAnimator, string pName)
        {
            AnimationClip[] lClips = pAnimator.runtimeAnimatorController.animationClips;
            foreach (AnimationClip lClip in lClips)
            {
                if (lClip.name == pName)
                {
                    return lClip.length;
                }
            }
            return 0f;
        }
        public static bool HasAnimation(this Animator pAnimator, string pAnimName)
        {
            if (pAnimator == null || pAnimator.runtimeAnimatorController == null) return false;

            foreach (AnimationClip lClip in pAnimator.runtimeAnimatorController.animationClips)
            {
                if (lClip.name == pAnimName)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
