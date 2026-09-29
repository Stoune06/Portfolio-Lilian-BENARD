using Com.IsartDigital.HealerSurvivor.Enum;
using Com.IsartDigital.HealerSurvivor.Other;
using Com.IsartDigital.HealerSurvivor.Manager;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 00/00/0000 - Beginning of the class

namespace Com.IsartDigital.HealerSurvivor.Menus
{
    
    public class SwipeMenuManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        [SerializeField] private PageConfig[] _AllPages;
        [SerializeField] private NavBarManager _NavBar;

        private const float SWIPE_THRESHOLD_RATIO = .1f;
        private const float TWEEN_DURATION = .5f;
        private const float DRAG_DEADZONE = 5f;

        private float SwipeThreshold => Screen.width * SWIPE_THRESHOLD_RATIO;

        private int _CurrentPage = 0;
        private int _PreviousPage = 0;

        private Vector2 _StartTouch;
        private Vector2 _StartPos;
        private Vector2 _TargetPos;
        private Vector2 _StartPagePos;

        private Tween _MovementTween;

        private bool _IsDragging;

        private RectTransform _Rect;

        private EDragDirection _CurrentDirection;

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // READY
        private void Awake()
        {
            _Rect = (RectTransform)transform;

            UpdateTargetPosition(true);
        }

        public void OnPointerDown(PointerEventData pEventData)
        {
            _StartTouch = pEventData.position;
            _StartPos = _Rect.anchoredPosition;
            _StartPagePos = _AllPages[_CurrentPage].rect.anchoredPosition;
            _IsDragging = true;

            _MovementTween?.Kill();
        }

        public void OnPointerUp(PointerEventData pEventData)
        {
            if (!_IsDragging) return;

            _IsDragging = false;
            if (_CurrentDirection == EDragDirection.HORIZONTAL)
                HandleSwipe(pEventData.position);
            else
                UpdateTargetPosition(false);

            _CurrentDirection = EDragDirection.UNDEFINED;
        }

        public void OnDrag(PointerEventData pEventData)
        {
            if (!_IsDragging) return;

            Vector2 lDelta = pEventData.position - _StartTouch;

            if (_CurrentDirection == EDragDirection.UNDEFINED)
            {
                if (Mathf.Abs(lDelta.x) > DRAG_DEADZONE) 
                    _CurrentDirection = EDragDirection.HORIZONTAL;
                else if (Mathf.Abs(lDelta.y) > DRAG_DEADZONE) 
                    _CurrentDirection = EDragDirection.VERTICAL;
                return;
            }

            if (_CurrentDirection == EDragDirection.HORIZONTAL)
                _Rect.anchoredPosition = new Vector2(_StartPos.x + lDelta.x, _StartPos.y);
            else if (_CurrentDirection == EDragDirection.VERTICAL)
            {
                if (_AllPages[_CurrentPage].scrollMode != EPageScroll.VERTICAL)
                    return;

                float lNewY = _StartPagePos.y + lDelta.y;
                if (lNewY < 0) 
                    lNewY = 0;

                _AllPages[_CurrentPage].rect.anchoredPosition = new Vector2(_AllPages[_CurrentPage].rect.anchoredPosition.x, lNewY);
            }
            
        }

        private void HandleSwipe(Vector2 pEndTouch)
        {
            _MovementTween?.Kill();

            int lDirection = 0;
            float lDelta = pEndTouch.x - _StartTouch.x;

            if (Mathf.Abs(lDelta) >= SwipeThreshold)
                lDirection = lDelta < 0 ? 1 : -1;

            if (lDirection != 0)
            {
                _PreviousPage = _CurrentPage;
                _CurrentPage = Mathf.Clamp(_CurrentPage + lDirection, 0, _AllPages.Length - 1);
            }

            _CurrentPage = Mathf.Clamp(_CurrentPage, 0, _AllPages.Length - 1);

            UpdateTargetPosition(false);
        }

        private void UpdateTargetPosition(bool pInstant)
        {
            if(_AllPages == null || _AllPages.Length == 0) 
                return;
            
            if (_NavBar != null) 
                _NavBar.UpdateSelectedPage(_CurrentPage);

            _TargetPos = new Vector2(-_AllPages[_CurrentPage].rect.anchoredPosition.x, 0f);

            if (pInstant)
            {
                _Rect.anchoredPosition = _TargetPos;
                return;
            }

            _MovementTween?.Kill();
            _MovementTween = _Rect.DOAnchorPos(_TargetPos, TWEEN_DURATION).SetEase(Ease.OutCubic).OnComplete(() => ResetPagePosition(_PreviousPage));
        }

        private void ResetPagePosition(int pPageIndex)
        {
            if (pPageIndex >= 0 && pPageIndex < _AllPages.Length && pPageIndex != _CurrentPage)
                _AllPages[pPageIndex].rect.DOAnchorPosY(0, TWEEN_DURATION);
        }
        
        public void GoToPage(int pIndex)
        {
            _CurrentPage = Mathf.Clamp(pIndex, 0, _AllPages.Length - 1);
            UpdateTargetPosition(false);
        }
    }
}