using System;
using Com.IsartDigital.HealerSurvivor.Gameplay;
using Com.IsartDigital.HealerSurvivor.Manager;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Com.IsartDigital.HealerSurvivor.Inputs
{
    public class InputManager : MonoBehaviour
    {
        [Header("Export references")]
        [SerializeField] private GameObject _OutsideJoystick;
        [SerializeField] private GameObject _InsideJoystick;

        [Header("Design")]
        [SerializeField, Range(0, 1)] private float _RadiusMargin;
        [SerializeField, Range(0f, 1f)] private float _DeadZoneMargin;
        [SerializeField, Range(0f, 1f)] private float _MaxSpeedRatio;
        [SerializeField, Range(0f, 1f)] private float _MaxJoystickPos;

        [SerializeField] private float _Offset;

        private Vector2 _InitialJoystickPosition;
        private float _MaxJoystickDegrees = 180;

        private float _JoystickRadius;
        private Vector2 _StartJoystickPosition;

        private string _HorizontalAxis = "Horizontal";
        private string _VerticalAxis = "Vertical";

        private Vector2 _JoystickDirection;
        private bool _WasPressed;
        private bool _IsTouchOverUI;
        
        public static Action<Vector2> OnJoystickMove;
        public static Action OnTouchRemove;
        
        private GameManager _GameManager => GameManager.Instance;

        void Start()
        {
            Vector2 lJoystickDiametre = _OutsideJoystick.GetComponent<RectTransform>().rect.size;
            _JoystickRadius = lJoystickDiametre.x / 2 * _OutsideJoystick.transform.localScale.x * (1 - _RadiusMargin);
            _InitialJoystickPosition = _InsideJoystick.transform.position;

            _GameManager.onGameStart += ActivateInput;
            _GameManager.onLeaveGame += DeactivateInput;
        }

        void Update()
        {
            if (Player.instance == null) return;
            MobileInput();
        }

        private void MobileInput()
        {
            if (Input.touchCount <= 0)
            {
                if (_WasPressed)
                {
                    OnTouchRemove?.Invoke();
                    _WasPressed = false;
                }
                return;
            }
            
            Touch lTouch = Input.GetTouch(0);

            if (lTouch.phase == TouchPhase.Began)
            {
                _IsTouchOverUI = EventSystem.current != null
                    && EventSystem.current.IsPointerOverGameObject(lTouch.fingerId);
                _OutsideJoystick.SetActive(true);
                _InsideJoystick.SetActive(true);
            }

            if (_IsTouchOverUI)
            {
                if (lTouch.phase == TouchPhase.Ended || lTouch.phase == TouchPhase.Canceled)
                    _IsTouchOverUI = false;
                return;
            }

            if(!_WasPressed) _WasPressed = true;

            if (lTouch.phase == TouchPhase.Began)
            {
                _OutsideJoystick.transform.position = _InsideJoystick.transform.position = lTouch.position;
                _StartJoystickPosition = lTouch.position;
            }
            Vector2 lCurrentPos = lTouch.position;
            Vector2 lOffset = lCurrentPos - _StartJoystickPosition;
            lOffset = Vector2.ClampMagnitude(lOffset, _JoystickRadius);

            _InsideJoystick.transform.position = lTouch.position;

            if ((lCurrentPos - _StartJoystickPosition).magnitude > lOffset.magnitude)
            {
                //_OutsideJoystick.transform.position += new Vector3((lCurrentPos - _StartJoystickPosition - lOffset).x, (lCurrentPos - _StartJoystickPosition - lOffset).y, 0);
                //_StartJoystickPosition = _OutsideJoystick.transform.position;
                _InsideJoystick.transform.position = 
                    _OutsideJoystick.transform.position + Vector3.ClampMagnitude(_InsideJoystick.transform.position - _OutsideJoystick.transform.position, lOffset.magnitude + _Offset);

            }

            _JoystickDirection = Vector2.ClampMagnitude(lOffset / (_JoystickRadius - _RadiusMargin) / _MaxSpeedRatio, 1f);
            _JoystickDirection = _JoystickDirection.magnitude > _DeadZoneMargin ? _JoystickDirection : Vector2.zero;
            
            OnJoystickMove?.Invoke(-_JoystickDirection);

            if (lTouch.phase == TouchPhase.Ended || lTouch.phase == TouchPhase.Canceled)
            {
                ResetJoystick();
                _OutsideJoystick.SetActive(false);
                _InsideJoystick.SetActive(false);
            }
            
        }

        public void ResetJoystick()
        {
            _InsideJoystick.transform.position = _StartJoystickPosition;

            _InsideJoystick.transform.position = _OutsideJoystick.transform.position = _InitialJoystickPosition;
        }

        private void ActivateInput() => enabled = true;
        
        private void DeactivateInput() => enabled = false;

        private void OnDisable()
        {
            if (_GameManager == null)
                return;
            
            _GameManager.onGameStart -= ActivateInput;
            _GameManager.onLeaveGame -= DeactivateInput;
        }
    }
}