using System;
using UnityEngine;

public class TickManager : MonoBehaviour
{
    [Range(0.1f, 5f)]
    private static float _Speed = 1f;
    public static float _ElapsedTime = 0f;
    public static float _DurationBetweenTicks = 1f;

    private static bool _IsPaused = true;
    

    public static event Action onTick;

    public static TickManager instance {  get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Update()
    {
        if(!_IsPaused)Tick();
    }

    private void Tick()
    {
        if (_ElapsedTime >= _DurationBetweenTicks)
        {
            //Debug.Log("Tick");
            onTick.Invoke();
            _ElapsedTime = 0f;
            

        }
        _ElapsedTime += Time.deltaTime * _Speed;
    }

    public static void Pause()
    {
        _IsPaused = true;
    }

    public static void Resume()
    {
        _IsPaused = false;
    }

    public static void Restart()
    {
        _ElapsedTime = 0;
        _IsPaused = true;
    }

    public static void UpdateSpeed(float pNewSpeed)
    {
        _Speed = pNewSpeed;
    }

    public static float GetRatioBetweenTicks()
    {
        return _ElapsedTime / _DurationBetweenTicks;
    }
}
