using System;
using UnityEngine;
using UnityEngine.UI;

public class HideUi : MonoBehaviour
{

    [SerializeField] private GameObject[] _ToHide;

    public static event Action OnOpen;
    public static event Action OnClose;

    private void OnEnable()
    {
        OnClose += Reveal;
    }

    public void Hide()
    {
        int lAmount = _ToHide.Length;
        for(int i = 0; i < lAmount; i++)
        {
            if (_ToHide[i].activeSelf) _ToHide[i].SetActive(false);
        }

        GetComponent<Image>().color = Color.clear;
    }

    private void Reveal()
    {
        int lAmount = _ToHide.Length;
        //Debug.Log(gameObject.name + " " + lAmount);
        for(int i = 0;i < lAmount; i++)
        {
            if (!_ToHide[i].activeSelf) _ToHide[i].SetActive(true);
        }
        GetComponent<Image>().color = Color.white;
    }

    public void DoOpen()
    {
        OnOpen?.Invoke();
    }

    public void DoClose()
    {
        OnClose?.Invoke();
    }

    private void OnDisable()
    {
        
        OnClose -= Reveal;
    }
}
