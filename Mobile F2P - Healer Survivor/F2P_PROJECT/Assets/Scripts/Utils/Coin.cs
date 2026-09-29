using System;
using System.Collections;
using System.Collections.Generic;
using Com.IsartDigital.HealerSurvivor.Enum;
using Com.IsartDigital.HealerSurvivor.Manager;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 00/00/0000 - Beginning of the class

namespace Com.IsartDigital.HealerSurvivor.Other
{
    
    public class Coin : MonoBehaviour
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        [SerializeField] private ParticleSystem _CoinParticles;
        private readonly float _RotationSpeed = 100f;
        private bool _IsCollected;
        private bool _IsAttracted;
        
        private Transform _TargetTransform;
        private float _CurrentAttractionSpeed = 5f;
        private const float ACCELERATION = 15f;
        
        private Quaternion _Rotation;
        
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // READY
        private void Start()
        {
            _CoinParticles.Play();
        }

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // PROCESS
        private void Update()
        {
            transform.rotation *= Quaternion.AngleAxis(_RotationSpeed * Time.deltaTime, Vector3.up);

            if (!_IsAttracted || !_TargetTransform) return;
            _CurrentAttractionSpeed += ACCELERATION * Time.deltaTime;
            
            transform.position = Vector3.MoveTowards(
                transform.position, 
                _TargetTransform.position + Vector3.up,
                _CurrentAttractionSpeed * Time.deltaTime
            );

            if (Vector3.Distance(transform.position, _TargetTransform.position + Vector3.up) < 0.2f)
                Collect();
            
        }
        
        public void StartAttraction(Transform pPlayerTransform)
        {
            if (_IsCollected || _IsAttracted) return;
            _TargetTransform = pPlayerTransform;
            _IsAttracted = true;
        }
        
        public void Launch()
        {
            float lRadius = Random.Range(1.5f, 3f);
            Vector3 lRandomDir = Random.insideUnitSphere * lRadius;
            lRandomDir.y = 0; 
            
            Vector3 lTargetPos = transform.position + lRandomDir;
            float lDelay = Random.Range(0f, .1f);

            transform.localScale = Vector3.zero;
            
            Sequence lSeq = DOTween.Sequence();
            lSeq.AppendInterval(lDelay);
            lSeq.Append(transform.DOScale(Vector3.one, .2f));
            lSeq.Join(transform.DOJump(lTargetPos, 2f, 1, .5f).SetEase(Ease.OutQuad));
            lSeq.OnComplete(() => _CoinParticles?.Play());
        }
        
        private void Collect()
        {
            if (_IsCollected) return;
            _IsCollected = true;
            _IsAttracted = false;

            GameManager.Instance.onGainingCoins?.Invoke();

            Sequence lSeq = DOTween.Sequence();
            lSeq.Append(transform.DOScale(Vector3.zero, 0.15f).SetEase(Ease.InBack));
            lSeq.OnComplete(() => {
                if (_CoinParticles != null) {
                    _CoinParticles.transform.parent = null; 
                    _CoinParticles.Stop();
                    Destroy(_CoinParticles.gameObject, 1f);
                }
                Destroy(gameObject);
            });
        }

    }
}