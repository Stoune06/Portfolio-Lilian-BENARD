using System;
using System.Collections.Generic;
using Com.IsartDigital.HealerSurvivor.SO.Triggers;
using UnityEngine;
using Object = UnityEngine.Object;
namespace Com.IsartDigital.HealerSurvivor.SO.Actions
{
    [Serializable]
    public class ChangeAttackTypeAction : AugmentAction
    {
        [SerializeField]
        private GameObject _AttackTypePrefab;

        private readonly List<GameObject> _SpawnedInstances = new List<GameObject>();

        public override void OnApply(IAugmentable pPlayer)
        {
            if (_AttackTypePrefab == null)
            {
                Debug.LogWarning("ChangeAttackTypeAction: _AttackTypePrefab is null.");
                return;
            }

            SpawnOn(pPlayer.transform);
        }

        public override void Execute(IAugmentable pPlayer, AugmentTrigger pTrigger) { }

        private void SpawnOn(Transform pParent)
        {
            GameObject lInstance = Object.Instantiate(_AttackTypePrefab, pParent);
            lInstance.transform.localPosition = Vector3.zero;
            lInstance.transform.localRotation = Quaternion.identity;
            _SpawnedInstances.Add(lInstance);
        }

        public override void OnRemove(IAugmentable pPlayer)
        {
            for (int i = 0; i < _SpawnedInstances.Count; i++)
            {
                if (_SpawnedInstances[i] != null) Object.Destroy(_SpawnedInstances[i]);
            }
            _SpawnedInstances.Clear();
        }

        public override string GetDescription()
        {
            return $"Add attack type {_AttackTypePrefab?.name}";
        }
    }
}