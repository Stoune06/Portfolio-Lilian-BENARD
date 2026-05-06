using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 00/00/0000 - Beginning of the class

namespace Com.IsartDigital.HealerSurvivor.Manager
{
    
    public class NavBarManager : MonoBehaviour
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        [Header("Elements")]
        [SerializeField] private RectTransform _Selector; // Ton image de fond qui bouge
        [SerializeField] private RectTransform[] _Icons;   // Tes 4 icônes

        [Header("Settings")]
        [SerializeField] private float _TweenDuration = 0.4f;
        [SerializeField] private Ease _TweenEase = Ease.OutBack; // Effet de rebond léger

        public void UpdateSelectedPage(int pPageIndex)
        {
            if (pPageIndex < 0 || pPageIndex >= _Icons.Length) return;

            Vector2 targetPosition = _Icons[pPageIndex].anchoredPosition;

            _Selector.DOAnchorPos(targetPosition, _TweenDuration)
                .SetEase(_TweenEase);
                 
            for (int i = 0; i < _Icons.Length; i++)
            {
                float scale = (i == pPageIndex) ? 1.2f : 1.0f;
                _Icons[i].DOScale(scale, _TweenDuration);
            }
        }
    }
    
}