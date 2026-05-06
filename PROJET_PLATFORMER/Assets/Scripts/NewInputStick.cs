using UnityEngine;

namespace Platformer.Player.Joystick
{
    public class NewInputStick : MonoBehaviour
    {
        [SerializeField] private float _MaxDistanceSlide = 100f;
        [SerializeField] private float _ScreenMarginX = 50f;
        [SerializeField][Range(0,1)] private float _MinRatio = 0.3f;
        [SerializeField] private RectTransform _UnderFinger;
        [SerializeField] private Camera _UICamera;

        private RectTransform _JoystickRect;

        private int _activeFingerId = -1;
        public bool _IsActive { get; private set; }
        private Vector2 _StartPos;
        private Vector2 _Axis;

        void Awake()
        {
            _JoystickRect = GetComponent<RectTransform>();
        }

        void Update()
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch lTouch = Input.GetTouch(i);

                if (lTouch.phase == TouchPhase.Began && !_IsActive)
                {
                    if (lTouch.position.x <= (Screen.width / 2) - _ScreenMarginX && lTouch.position.y <= Screen.height / 2)
                    {
                        _IsActive = true;
                        _activeFingerId = lTouch.fingerId;

                        RectTransform lCanvasRect = _JoystickRect.parent as RectTransform;
                        RectTransformUtility.ScreenPointToLocalPointInRectangle(
                            lCanvasRect, lTouch.position, _UICamera, out Vector2 lLocalPointInCanvas);

                        //_JoystickRect.localPosition = lLocalPointInCanvas;
                        _StartPos = lLocalPointInCanvas;
                        _UnderFinger.anchoredPosition = Vector2.zero;
                    }
                }

                if (_IsActive && lTouch.fingerId == _activeFingerId)
                {
                    RectTransform lCanvasRect = _JoystickRect.parent as RectTransform;
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        lCanvasRect, lTouch.position, _UICamera, out Vector2 lCurrentLocalPoint);

                    if (lTouch.phase == TouchPhase.Moved || lTouch.phase == TouchPhase.Stationary)
                    {
                        Slide(lCurrentLocalPoint);
                    }
                    else if (lTouch.phase == TouchPhase.Ended || lTouch.phase == TouchPhase.Canceled)
                    {
                        _IsActive = false;
                        _activeFingerId = -1;
                        _UnderFinger.anchoredPosition = _Axis = Vector2.zero;
                    }
                }
            }
        }

        private void Slide(Vector2 pCurrentLocalPoint)
        {
            Vector2 lOffset = pCurrentLocalPoint - _StartPos;
            float lDistance = lOffset.magnitude;

            float lClampedMagnitude = Mathf.Clamp01(lDistance / _MaxDistanceSlide);
            if(lClampedMagnitude < _MinRatio) lClampedMagnitude = _MinRatio;

            _Axis = lOffset.normalized * lClampedMagnitude;

            if (lDistance > _MaxDistanceSlide)
            {
                _UnderFinger.anchoredPosition = lOffset.normalized * _MaxDistanceSlide;
            }
            else
            {
                _UnderFinger.anchoredPosition = lOffset;
            }
        }

        public float GetAxisX() => _Axis.x;
        public float GetAxisY() => _Axis.y;
    }
}