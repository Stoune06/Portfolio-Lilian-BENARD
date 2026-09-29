using FMODUnity;
using System;
using UnityEngine;
using FMOD.Studio;
using UnityEngine.UI;

public class PauseManage : MonoBehaviour
{
    public static Action<int> onReturnCheckpoint;

    [Header("FMOD Global Labeled Parameter")]
    public string globalParameterName = "OpenSettings"; // Nom exact dans FMOD
    public string pausedLabel = "Opened";
    public string playingLabel = "Closed";

    [SerializeField] private GameObject[] _ToHide;

    public static event Action OnPause;
    public static event Action OnResume;

    private void Start()
    {
        OnPause += Pause;
        OnResume += Unpause;
    }
    private void SetGlobalLabeledParameter(string label)
    {
        // Change directement le global parameter Labeled
        FMOD.RESULT result = RuntimeManager.StudioSystem.setParameterByNameWithLabel(globalParameterName, label);

        //if (result == FMOD.RESULT.OK)
        //{
        //    Debug.Log($"[FMOD] Global Parameter '{globalParameterName}' mis à '{label}'");
        //}
        //else
        //{
        //    Debug.LogWarning($"[FMOD] Impossible de changer le Global Parameter '{globalParameterName}' ({result})");
        //}
    }

    private void Pause()
    {
        Time.timeScale = 0;
        SetGlobalLabeledParameter(pausedLabel);
        for(int i = 0; i<_ToHide.Length; i++)
        {
            if (_ToHide[i].activeInHierarchy) _ToHide[i].SetActive(false);
        }
    }

    private void Unpause()
    {
        Time.timeScale = 1;
        SetGlobalLabeledParameter(playingLabel);
        for (int i = 0; i < _ToHide.Length; i++)
        {
            if (!_ToHide[i].activeInHierarchy) _ToHide[i].SetActive(true);
        }
        GetComponent<Image>().color = Color.white;
    }

    public void ReturnCheckpoint()
    {
        OnResume?.Invoke();
        onReturnCheckpoint?.Invoke(0);
    }
    public void GoToNextCheckpoint()
    {
        OnResume?.Invoke();
        onReturnCheckpoint?.Invoke(+1);
    }

    public void DoPause()
    {
        OnPause?.Invoke();
    }

    public void DoUnpause()
    {
        OnResume?.Invoke();
    }

    private void OnDisable()
    {
        OnPause -= Pause;
        OnResume -= Unpause;
    }

    public void HideSelf()
    {
        GetComponent<Image>().color = Color.clear;
    }
}

