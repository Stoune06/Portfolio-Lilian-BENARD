using System;
using UnityEngine;
using Platformer;
using Platformer.Player;
using Platformer.Player.Joystick;

namespace Tooling
{
    /// <summary>
    /// InputManager is a class made to Ease the management of inputs with multiples callbacks and also by mimicking computer inputs on phones
    /// </summary>
    [System.Diagnostics.DebuggerStepThrough, AutoLoad(LoadType = LoadType.DestroyOnLoad)]
    public class InputManager : MonoBehaviour
    {
        private const string MOUSE_SCROLL = "Mouse ScrollWheel";

        public static event Action<int> OnScrollMouse;

        public static event Action OnClick, OnClickReleased;
        public static event Action<Vector2> OnClickPosition, OnClickReleasedPosition;

        public static event Action OnAnyKeyDown;

        public static event Action<Vector2, int> startDrag;
        public static event Action<Vector2, int> endDrag;

        public static event Action onJump;
        public static event Action onJumpReleased;
        public static Vector2 StartDragPosition { get; private set; } = new Vector2();
        public static Vector2 EndDragPosition { get; private set; } = new Vector2();

        public static Vector2 MovementDirection { get; private set; } //TODO Handle direction function
        public static bool isDragging { get; private set; } = false;

        public static NewInputStick InputStick { get; private set; }
        public static JumpButton JumpButton { get; private set; }

        public static bool EnabledInputControl = true;

        private bool _IsPC = false;

        private void Start()
        {
            InputStick = FindAnyObjectByType<NewInputStick>();
            JumpButton = FindAnyObjectByType<JumpButton>();

            if (JumpButton == null) return;
            JumpButton.MobileJumpPressedEvent += () => onJump?.Invoke();
            JumpButton.MobileJumpReleasedEvent += () => onJumpReleased?.Invoke();
        }


        private void Update()
        {
            if (!EnabledInputControl) return;

#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
            HandleComputer();
#endif
#if UNITY_IOS || UNITY_ANDROID || UNITY_EDITOR
            HandlePhone();
#endif
        }


        private void HandleComputer()
        {
            HandleMouseInput();
            _IsPC = true;
            if (Input.anyKey)
            {
                OnAnyKeyDown?.Invoke();
                MovementDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
                if (Input.GetKeyDown(KeyCode.Space)) onJump?.Invoke();
            }
            else MovementDirection = Vector2.zero;

            if (Input.GetKeyUp(KeyCode.Space)) onJumpReleased?.Invoke();
            float lMouseScroll = Input.GetAxis(MOUSE_SCROLL);
            if (lMouseScroll != 0f)
            {
                OnScrollMouse?.Invoke(lMouseScroll > 0f ? 1 : -1);
            }
        }

        private void HandleMouseInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                isDragging = true;
                OnClick?.Invoke();
                OnClickPosition?.Invoke(Input.mousePosition);
                StartDragPosition = Input.mousePosition;
                if (JumpButton && !JumpButton.PointerInButton) startDrag?.Invoke(Input.mousePosition, 0);
                EndDragPosition = Vector2.zero;
            }
            if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
                OnClickReleased?.Invoke();
                OnClickReleasedPosition?.Invoke(Input.mousePosition);
                StartDragPosition = Vector2.zero;
                EndDragPosition = Input.mousePosition;
                endDrag?.Invoke(Input.mousePosition, 0);
            }
        }

        private void HandlePhone()
        {
            if (Input.anyKey)
            {
                OnAnyKeyDown?.Invoke();
            }
            if (InputStick != null && InputStick._IsActive) MovementDirection = new Vector2(InputStick.GetAxisX(), MovementDirection.y);
            else if (!_IsPC) MovementDirection = new Vector2(0, MovementDirection.y);

            Touch[] lTouches = Input.touches;
            for (int i = 0; i < lTouches.Length; i++)
            {
                Touch lCurrentTouch = lTouches[i];
                switch ((lCurrentTouch.phase))
                {
                    case TouchPhase.Began:
                        isDragging = true;
                        OnClick?.Invoke();
                        OnClickPosition?.Invoke(lCurrentTouch.position);
                        StartDragPosition = lCurrentTouch.position;
                        if (JumpButton != null && !JumpButton.PointerInButton) startDrag?.Invoke(lCurrentTouch.position, i);
                        EndDragPosition = Vector2.one;
                        break;

                    case TouchPhase.Moved:
                        StartDragPosition = lCurrentTouch.position;
                        EndDragPosition = Vector2.one;
                        break;

                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                        isDragging = false;
                        OnClickReleased?.Invoke();
                        OnClickReleasedPosition?.Invoke(lCurrentTouch.position);
                        StartDragPosition = Vector2.one;
                        EndDragPosition = lCurrentTouch.position;
                        endDrag?.Invoke(lCurrentTouch.position, i);
                        break;
                }
            }
        }
        public void ForceSetInputStick(NewInputStick pInputStick) => InputStick = pInputStick;
        public void ForceSetInputStick() => InputStick = FindAnyObjectByType<NewInputStick>();
        private void OnDestroy()
        {
            onJump = null;
            onJumpReleased = null;
            OnAnyKeyDown = null;
            OnClick = null;
            OnClickReleased = null;
            startDrag = null;
            endDrag = null;
            OnScrollMouse = null;
        }
    }

    public static class HapticManager
    {
#if UNITY_ANDROID && !UNITY_EDITOR
    public static void Vibrate(int milliseconds)
    {
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject vibrator = activity.Call<AndroidJavaObject>("getSystemService", "vibrator");

            if (AndroidVersion() >= 26)
            {
                using (AndroidJavaClass vibrationEffectClass = new AndroidJavaClass("android.os.VibrationEffect"))
                {
                    AndroidJavaObject effect = vibrationEffectClass.CallStatic<AndroidJavaObject>(
                        "createOneShot",
                        milliseconds,
                        vibrationEffectClass.GetStatic<int>("DEFAULT_AMPLITUDE")
                    );
                    vibrator.Call("vibrate", effect);
                }
            }
            else
            {
                vibrator.Call("vibrate", (long)milliseconds);
            }
        }
    }

    static int AndroidVersion()
    {
        using (AndroidJavaClass version = new AndroidJavaClass("android.os.Build$VERSION"))
        {
            return version.GetStatic<int>("SDK_INT");
        }
    }
#endif
    }
}