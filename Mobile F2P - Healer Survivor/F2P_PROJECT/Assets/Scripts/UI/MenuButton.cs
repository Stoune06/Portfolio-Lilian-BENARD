using Com.IsartDigital.HealerSurvivor.Manager;
using Com.IsartDigital.HealerSurvivor.Menus;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 00/00/0000 - Beginning of the class

namespace Com.IsartDigital.HealerSurvivor.UI
{

    public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        [SerializeField] private float _ScaleMultiplier = 1.9f;
        [SerializeField] private float _Duration = .5f;
        [SerializeField] private EMenuType _TargetMenu;

        public EMenuType TargetMenu => _TargetMenu;

        private Tween _CurrentTween;

        private Vector3 _OriginScale;

        private Button _CurrentButton;

        private GameManager _GameManager => GameManager.Instance;

        private void Awake()
        {
            _OriginScale = transform.localScale;
            _CurrentButton = GetComponent<Button>();
        }

        public void OnPointerEnter(PointerEventData pEventData)
        {
            _CurrentTween?.Kill();

            _CurrentTween = transform.DOScale(_OriginScale * _ScaleMultiplier, _Duration)
                .SetEase(Ease.OutElastic);
        }

        public void OnPointerExit(PointerEventData pEventData)
        {
            _CurrentTween?.Kill();

            _CurrentTween = transform.DOScale(_OriginScale, _Duration)
                .SetEase(Ease.OutElastic);
        }

        private void OnEnable()
        {
            if (_CurrentButton == null || _GameManager == null)
                return;

            _CurrentTween?.Kill();
            transform.localScale = _OriginScale;
        }

        private void OnDisable()
        {
            _CurrentTween?.Kill();
            transform.localScale = _OriginScale;
        }
    }
}