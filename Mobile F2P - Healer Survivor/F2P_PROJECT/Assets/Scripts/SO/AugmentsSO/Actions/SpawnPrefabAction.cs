using System;
using System.Collections;
using System.Collections.Generic;
using Com.IsartDigital.HealerSurvivor.SO.Triggers;
using UnityEngine;
using Object = UnityEngine.Object;
namespace Com.IsartDigital.HealerSurvivor.SO.Actions
{
    [Serializable]
    public class SpawnPrefabAction: AugmentAction
    {
        [SerializeField]
        private GameObject _Prefab;
        [SerializeField]
        private float _Duration;

        private readonly List<GameObject> _SpawnedInstances = new List<GameObject>();
        private Coroutine _DespawnCoroutine;

        public override void Execute(IAugmentable pPlayer, AugmentTrigger pTrigger)
        {
            if (_Prefab == null)
            {
                Debug.LogWarning("SpawnPrefabAction: _Prefab is null.");
                return;
            }

            DestroyAllInstances();
            if (_DespawnCoroutine != null)
            {
                pPlayer.StopCoroutine(_DespawnCoroutine);
                _DespawnCoroutine = null;
            }

            List<Transform> lTargets = TargetResolver.Resolve(m_TargetMode, pPlayer, pTrigger);
            if (lTargets.Count == 0)
            {
                _SpawnedInstances.Add(Object.Instantiate(_Prefab, pPlayer.transform.position, Quaternion.identity));
            }
            else
            {
                foreach (Transform lTarget in lTargets)
                {
                    _SpawnedInstances.Add(Object.Instantiate(_Prefab, lTarget.position, Quaternion.identity));
                }
            }

            if (_Duration > 0f)
            {
                _DespawnCoroutine = pPlayer.StartCoroutine(DespawnAfterDelay());
            }
        }

        public override string GetDescription()
        {
            string lName = _Prefab != null ? _Prefab.name : "???";
            return _Duration > 0 ? $"Spawn {lName} for {_Duration}s" : $"Spawn {lName}";
        }

        private IEnumerator DespawnAfterDelay()
        {
            yield return new WaitForSeconds(_Duration);
            DestroyAllInstances();
            _DespawnCoroutine = null;
        }

        private void DestroyAllInstances()
        {
            for (int i = 0; i < _SpawnedInstances.Count; i++)
            {
                if (_SpawnedInstances[i] != null) Object.Destroy(_SpawnedInstances[i]);
            }
            _SpawnedInstances.Clear();
        }

        public override void OnRemove(IAugmentable pPlayer)
        {
            if (_DespawnCoroutine != null)
            {
                pPlayer.StopCoroutine(_DespawnCoroutine);
                _DespawnCoroutine = null;
            }
            DestroyAllInstances();
        }

    }
}
