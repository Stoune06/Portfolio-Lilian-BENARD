using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour
{
    public static LevelSelector instance { get; private set; }

    [SerializeField]
    private List<LevelData> _AllLevelsData = new List<LevelData>();

    [SerializeField]
    private Transform _ButtonContainer;

    [SerializeField]
    private GameObject _ButtonPrefab;

    private List<Button> _Buttons = new List<Button>();
    public static event Action<LevelData> onLevelSelected;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    void Start()
    {
        foreach(LevelData lLevel in _AllLevelsData)
        {
            Button lButton = Instantiate(_ButtonPrefab,_ButtonContainer).GetComponent<Button>();
            lButton.GetComponentInChildren<Text>().text = lLevel.name;
            lButton.onClick.AddListener(() => OnButtonClick(lButton));
            _Buttons.Add(lButton);
        }

    }

    private void OnButtonClick(Button pButton)
    {
        int lIndex = _Buttons.IndexOf(pButton);
        onLevelSelected?.Invoke(_AllLevelsData[lIndex]);
    }
}
