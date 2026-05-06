using System;
using System.Collections.Generic;
using UnityEngine;

namespace Com.IsartDigital.HealerSurvivor.Gameplay
{
    public class HPBarManager : MonoBehaviour
    {
        [SerializeField] private LifeBar _LifeBar;
        [SerializeField] private Indicator _Indicator;
        [SerializeField] private Vector3 _BaseGap;
        private Dictionary<Transform, LifeBar> _BarDico = new();
        private Dictionary<Transform, Indicator> _IndicatorDico = new();

        private void OnEnable()
        {
            Character.OnAllySpawn += CreateBar;
            Character.OnAllyDied += DestroyBar;
        }

        private void DestroyBar(Transform pTransform)
        {
            Destroy(_BarDico[pTransform].gameObject);
            Destroy(_IndicatorDico[pTransform].gameObject);
        }

        private void CreateBar(Transform pTransform)
        {
            LifeBar lLifeBar = Instantiate(_LifeBar, transform);
            lLifeBar.SetTarget(pTransform, _BaseGap);
            _BarDico[pTransform] = lLifeBar;

            Indicator lIndicator = Instantiate(_Indicator, transform);
            lIndicator.SetValue(pTransform, Player.instance.transform);
            _IndicatorDico[pTransform] = lIndicator;
        }

        private void OnDisable()
        {
            Character.OnAllySpawn -= CreateBar;
            Character.OnAllyDied -= DestroyBar;
        }
    }
}
