using System;
using UnityEngine;

public static class GameStateManager
{
    private static bool _IsActionPhase = false;
    private static bool _IsPaused = false;
    private static bool _CanRotateCamera = true;

    public static bool isActionPhase
    {
        get => _IsActionPhase;
        set
        {
            if (_IsActionPhase != value)
            {
                _IsActionPhase = value;
                onActionPhaseChanged?.Invoke(value);
            }
        }
    }

    public static bool isPaused
    {
        get => _IsPaused;
        set
        {
            if (_IsPaused != value)
            {
                _IsPaused = value;
                onPauseStateChanged?.Invoke(value);
            }
        }
    }

    public static bool canRotateCamera
    {
        get => _CanRotateCamera;
        set
        {
            if (_CanRotateCamera != value)
            {
                _CanRotateCamera = value;
                onCameraRotationEnabled?.Invoke(value);
            }
        }
    }

    public static event Action<bool> onActionPhaseChanged;
    public static event Action<bool> onPauseStateChanged;
    public static event Action<bool> onCameraRotationEnabled;
}

