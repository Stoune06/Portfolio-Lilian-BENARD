using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    private string _HorizontalAxis = "Horizontal";
    [SerializeField]
    private string _VerticalAxis = "Vertical";

    [SerializeField]
    private float _HorizontalDeadZone = 0.01f;

    [SerializeField]
    private float _VerticalDeadZone = 0.01f;

    public static event Action<float, float> onCameraMove;
    public static event Action<float, float> onCameraRotate;
    public static event Action<float> onCameraZoom;


    public static event Action onResetLevel;
    public static event Action onTilePlace;
    public static event Action onTileRemove;
    public static event Action onPause;


    private Vector2 _LastTouchPosition;
    private bool _IsDragging = false;
    private float _TouchZoomSpeed = 0.05f;
    private float _TouchRotateSpeed = 0.2f;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Return))
        {
            onPause?.Invoke();
        }
        if (GameStateManager.isPaused) return;

        if (GameStateManager.canRotateCamera)
        {
            if (Input.touchCount > 0)
            {
                HandleTouchInput();
            }
            else
            {
                HandlePCInput();
            }
        }
    }

    private void HandleTouchInput()
    {
        
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
        {
            return;
        }

        if (Input.touchCount == 2)
        {
            Touch lTouch0 = Input.GetTouch(0);
            Touch lTouch1 = Input.GetTouch(1);

            Vector2 lTouchPrevPos0 = lTouch0.position - lTouch0.deltaPosition;
            Vector2 lTouchPrevPos1 = lTouch1.position - lTouch1.deltaPosition;

            float lPrevTouchDeltaMag = (lTouchPrevPos0 - lTouchPrevPos1).magnitude;
            float lTouchDeltaMag = (lTouch0.position - lTouch1.position).magnitude;

            float lDeltaMagnitudeDiff = lTouchDeltaMag - lPrevTouchDeltaMag;


            onCameraZoom?.Invoke(lDeltaMagnitudeDiff * _TouchZoomSpeed);
            return; 
        }

        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    _IsDragging = false; 
                    break;

                case TouchPhase.Moved:
                    onCameraRotate?.Invoke(touch.deltaPosition.x * _TouchRotateSpeed, touch.deltaPosition.y * _TouchRotateSpeed);
                    _IsDragging = true;
                    break;

                case TouchPhase.Ended:
                    if (!_IsDragging)
                    {
                        if (GameStateManager.isActionPhase) onResetLevel?.Invoke();
                        else onTilePlace?.Invoke();
                    }
                    break;
            }
        }
    }

    private void HandlePCInput()
    {

        if (EventSystem.current == null || !EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (GameStateManager.isActionPhase) onResetLevel?.Invoke();
                else onTilePlace?.Invoke();
            }

            if (Input.GetMouseButtonDown(1))
            {
                if (!GameStateManager.isActionPhase) onTileRemove?.Invoke();
            }
        }

        float lH = Input.GetAxis(_HorizontalAxis);
        float lV = Input.GetAxis(_VerticalAxis);

        if (Mathf.Abs(lH) > 0.01f || Mathf.Abs(lV) > 0.01f)
        {
            onCameraMove?.Invoke(lH, lV);
        }

        if (Input.GetMouseButton(1))
        {
            float lMouseX = Input.GetAxis("Mouse X");
            float lMouseY = Input.GetAxis("Mouse Y");
            onCameraRotate?.Invoke(lMouseX, lMouseY);
        }

        float lScroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(lScroll) > 0.01f)
        {
            onCameraZoom?.Invoke(lScroll);
        }

        if(Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Return))
        {
            onPause?.Invoke();
        }
    }
}
