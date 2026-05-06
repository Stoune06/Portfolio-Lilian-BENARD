using System.Collections.Generic;
using Com.IsartDigital.HealerSurvivor.Gameplay;
using Com.IsartDigital.HealerSurvivor.SO.Triggers;
using UnityEngine;
namespace Com.IsartDigital.HealerSurvivor.SO.Actions
{
    public static class TargetResolver
    {
        private static readonly Collider[] _Buffer = new Collider[32];
        private static readonly LayerMask _AllyLayer = LayerMask.GetMask("Ally");
        private const float SEARCH_RADIUS = 50f;

        public static List<Transform> Resolve(TargetMode pMode, IAugmentable pPlayer, AugmentTrigger pTrigger)
        {
            List<Transform> lResults = new List<Transform>();
            Transform lPlayerTransform = pPlayer.transform;

            switch (pMode)
            {
                case TargetMode.Self:
                    lResults.Add(lPlayerTransform);
                    break;

                case TargetMode.EventTarget:
                    if (pTrigger.lastTarget != null)
                        lResults.Add(pTrigger.lastTarget);
                    break;

                case TargetMode.AllAllies:
                    GetAllAllies(lPlayerTransform, lResults);
                    break;

                case TargetMode.NearestAlly:
                {
                    Transform lNearest = GetAllyByDistance(lPlayerTransform, nearest: true);
                    if (lNearest != null) lResults.Add(lNearest);
                    break;
                }

                case TargetMode.FarthestAlly:
                {
                    Transform lFarthest = GetAllyByDistance(lPlayerTransform, nearest: false);
                    if (lFarthest != null) lResults.Add(lFarthest);
                    break;
                }

                case TargetMode.RandomAlly:
                {
                    GetAllAllies(lPlayerTransform, lResults);
                    if (lResults.Count > 0)
                    {
                        Transform lRandom = lResults[Random.Range(0, lResults.Count)];
                        lResults.Clear();
                        lResults.Add(lRandom);
                    }
                    break;
                }
            }

            return lResults;
        }

        private static Transform ResolveCharacterTransform(Collider pCollider)
        {
            Character lCharacter = pCollider.GetComponentInParent<Character>();
            return lCharacter != null ? lCharacter.transform : pCollider.transform;
        }

        private static void GetAllAllies(Transform pOrigin, List<Transform> pResults)
        {
            int lCount = Physics.OverlapSphereNonAlloc(pOrigin.position, SEARCH_RADIUS, _Buffer, _AllyLayer);
            for (int i = 0; i < lCount; i++)
            {
                Transform lTransform = ResolveCharacterTransform(_Buffer[i]);
                if (!pResults.Contains(lTransform)) pResults.Add(lTransform);
            }
        }

        private static Transform GetAllyByDistance(Transform pOrigin, bool nearest)
        {
            int lCount = Physics.OverlapSphereNonAlloc(pOrigin.position, SEARCH_RADIUS, _Buffer, _AllyLayer);
            Transform lBest = null;
            float lBestSqr = nearest ? float.MaxValue : float.MinValue;

            for (int i = 0; i < lCount; i++)
            {
                Transform lTransform = ResolveCharacterTransform(_Buffer[i]);
                float lSqr = (lTransform.position - pOrigin.position).sqrMagnitude;
                if (nearest ? lSqr < lBestSqr : lSqr > lBestSqr)
                {
                    lBestSqr = lSqr;
                    lBest = lTransform;
                }
            }

            return lBest;
        }
    }
}
