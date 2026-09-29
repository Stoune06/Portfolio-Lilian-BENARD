using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 13/04/2026 - Beginning of the class

namespace Com.IsartDigital.HealerSurvivor.UI
{
    public class Description : MonoBehaviour
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        [SerializeField] private Button _QuitButton;
        [SerializeField] private Image _DescriptionImage;

        public Action onHide;
        
        private Vector3 MiddleScreen => new Vector3(Screen.width / 2f, Screen.height / 2f, 0);

        private const float TWEEN_DURATION = .5f;

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // READY
        private void Awake() => _QuitButton.onClick.AddListener(Hide);
        
        public void Show()
        {
            gameObject.SetActive(true);
            
            RectTransform lRect = transform as RectTransform;
            if (lRect != null)
                lRect.anchoredPosition = Vector2.zero; 
            else
                transform.localPosition = Vector3.zero;
            
            transform.DOScale(Vector3.one, TWEEN_DURATION)
                .From(Vector3.zero)
                .SetEase(Ease.OutElastic);
        }

        public void Hide()
        {
            transform.DOScale(Vector3.zero, TWEEN_DURATION)
                .SetEase(Ease.InBack)
                .OnComplete(() =>
                {
                    gameObject.SetActive(false);
                    onHide?.Invoke();
                });
        }

        public void SetDescriptionSprite(Sprite pSprite) => _DescriptionImage.sprite = pSprite;

        private void OnDestroy()
        {
            _QuitButton.onClick.RemoveAllListeners();
            onHide = null;
        }
    }
}