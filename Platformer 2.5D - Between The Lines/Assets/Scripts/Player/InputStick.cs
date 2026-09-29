using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Platformer
{
    [RequireComponent(typeof(RectTransform))]
    public class InputStick : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        private CancelDrawInput  _CancelDrawInput;
        private RectTransform _RectTransform;
        public RectTransform Canva;
        public float radius = 100f;

        private Vector3 _StartPosition;
        public Vector2 vectoredAngle { get; private set; }
        public float ZAngle { get; private set; }
        public bool isDragging { get; private set; }
        [field: SerializeField] public bool MoveStickOnDown { get; private set; } = true;

        private void Awake()
        {
            _RectTransform = GetComponent<RectTransform>();
            _CancelDrawInput = GetComponent<CancelDrawInput>();
            _StartPosition = _RectTransform.anchoredPosition;

            if (Canva == null) Canva = FindAnyObjectByType<Canvas>().GetComponent<RectTransform>();
        }

        public void OnDrag(PointerEventData pEvent)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(Canva, pEvent.position, pEvent.pressEventCamera,out Vector2 lLocalPoint);
            vectoredAngle = lLocalPoint - (Vector2)_RectTransform.localPosition;
            ZAngle = Mathf.Atan2(vectoredAngle.y, vectoredAngle.x) * Mathf.Rad2Deg;
            _RectTransform.eulerAngles = new Vector3(0f, 0f, ZAngle);
        }

        public void OnPointerDown(PointerEventData pEvent)
        {
            isDragging = true;
            _CancelDrawInput.InvokeOnCancelDrawInput(false);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(Canva, pEvent.position, pEvent.pressEventCamera, out Vector2 lLocalPoint);
            if(MoveStickOnDown) _RectTransform.anchoredPosition = lLocalPoint;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isDragging = false;
            _RectTransform.anchoredPosition = _StartPosition;
            _CancelDrawInput.InvokeOnCancelDrawInput(true);
            _RectTransform.rotation = Quaternion.identity;
            vectoredAngle = Vector2.zero;
            ZAngle = 0f;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float GetXAxis()
        {
            if (ZAngle < 90f && ZAngle > -90f) return 1f;
            else return -1f;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsJoyStickAxisInDirection(float pAngleMin, float pAngleMax) 
            => ZAngle < pAngleMax &&  ZAngle > pAngleMin;
    }
}