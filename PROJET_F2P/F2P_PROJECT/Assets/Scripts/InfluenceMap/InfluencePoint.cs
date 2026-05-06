//Clement PERSYN

using Com.IsartDigital.HealerSurvivor.InfluenceMap.Personality;
using System.Collections;
using UnityEngine;

namespace Com.IsartDigital.HealerSurvivor.InfluenceMap
{
    public class InfluencePoint : MonoBehaviour
    {
        public const float UPDATE_FREQ = 0.25f;

        private InfluenceGrid _Map;

        public float influence = 1f;
        public float radius = 5;

        [HideInInspector] public float myEstimateInfluence;

        public InfluenceTypeEnum influType;

        private Coroutine _UpdateCoroutine;

        private void OnEnable()
        {
            _UpdateCoroutine = StartCoroutine(UpdateInfluence());
        }

        private IEnumerator UpdateInfluence()
        {
            while (gameObject.activeSelf)
            {
                _Map ??= InfluenceGrid.Get(transform.position);
                if (_Map != null )
                    _Map.AddGlobalInfluence(transform.position, influence, radius, influType);
                myEstimateInfluence += influence;
                yield return new WaitForSeconds(UPDATE_FREQ);
            }
            yield return null;
        }

        private void OnDisable()
        {
            if (_UpdateCoroutine != null)
            {
                StopCoroutine(_UpdateCoroutine);
            }
        }

        private void Update()
        {
            myEstimateInfluence *= Mathf.Pow(InfluenceCell.DECAY_FACTOR, Time.deltaTime);
        }
    }
}
