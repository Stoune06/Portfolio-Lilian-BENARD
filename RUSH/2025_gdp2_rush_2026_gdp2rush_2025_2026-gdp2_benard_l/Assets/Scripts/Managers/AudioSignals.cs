using System;
using UnityEngine;

public class AudioSignals : MonoBehaviour
{
    public static event Action<AudioClipsEnum, Vector3> OnPlaySound;

    public static event Action<AudioClipsEnum> OnPlayMusic;

    public static event Action OnStopMusic;
    public static void TriggerSound(AudioClipsEnum pType, Vector3 pPosition)
    {
        OnPlaySound?.Invoke(pType, pPosition);
    }

    public static void TriggerMusic(AudioClipsEnum pType)
    {
        OnPlayMusic?.Invoke(pType);
    }

    public static void TriggerStopMusic()
    {
        OnStopMusic?.Invoke();
    }
}
