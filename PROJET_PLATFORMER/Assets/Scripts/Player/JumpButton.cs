using System;
using Tooling;
using UnityEngine;
using UnityEngine.EventSystems;

//Author : SALES Noe
namespace Platformer.Player
{
    public class JumpButton : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
    {
        private CancelDrawInput _CancelDrawInput;
        public event Action MobileJumpPressedEvent;
        public event Action MobileJumpReleasedEvent;

        [SerializeField] private TweenProperty _Property;
        [SerializeField] private float _StartScaleRatio = .8f;
        [SerializeField] private Vector3 _StartScale;

        public bool PointerInButton = false;

        private void Awake()
        {
            _CancelDrawInput = GetComponent<CancelDrawInput>();
        }

        public void OnPointerEnter(PointerEventData eventData) => PointerInButton = true;

        public void OnPointerExit(PointerEventData eventData) => PointerInButton = false;

        public void OnPointerUp(PointerEventData eventData)
        {
            MobileJumpReleasedEvent?.Invoke();
            _CancelDrawInput.InvokeOnCancelDrawInput(true);

            transform.localScale *= _StartScaleRatio;
            transform.ScaleTo(_StartScale, _Property);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _StartScale = transform.localScale;
            transform.localScale *= _StartScaleRatio;
            MobileJumpPressedEvent?.Invoke();
            _CancelDrawInput.InvokeOnCancelDrawInput(false);
        }
    }
}