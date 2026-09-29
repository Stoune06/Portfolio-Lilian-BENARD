using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

public class ReflectionPanel : MonoBehaviour
{
    [SerializeField]
    private Transform _TilesButtonContainer;

    [SerializeField]
    private GameObject _TilesButtonPrefab;

    [SerializeField]
    private Button _ClearActionsButton;

    private const string AMOUNT_PREFIX = "x";

    public static event Action onButtonStartPressed;
    public static event Action<int> onTileButtonPressed;
    public static event Action onClearActionsButtonPressed;

    private List<Button> _Buttons = new List<Button>();
    private List<int> _Quantity = new List<int>();


    private void Start()
    {
        _ClearActionsButton.onClick.AddListener(() => onClearActionsButtonPressed?.Invoke());
    }

    public void OnButtonStartClicked()
    {
        onButtonStartPressed?.Invoke();
    }

    public void SetTilesInventory(List<InventoryEntry> pInventoryEntry)
    {
        ClearButtons();

        for (int i = 0; i < pInventoryEntry.Count; i++)
        {
            CreateTileButton(pInventoryEntry[i], i);
        }
        
        UpdateSelectionVisuals(0);
    }

    private void CreateTileButton(InventoryEntry pEntry, int pIndex)
    {
        Button lButton = Instantiate(_TilesButtonPrefab, _TilesButtonContainer).GetComponent<Button>();
        _Quantity.Add(pEntry.quantity);
        _Buttons.Add(lButton);

        SetupButtonText(lButton, pEntry.quantity);
        SetupButtonClick(lButton, pIndex);
        SetupButtonPreview(lButton, pEntry);
        SetupDragComponent(lButton, pIndex);
    }

    private void SetupButtonText(Button pButton, int pQuantity)
    {
        Text[] lTexts = pButton.GetComponentsInChildren<Text>();
        if (lTexts.Length > 0)
        {
            lTexts[0].text = AMOUNT_PREFIX + pQuantity;
        }
    }

    private void SetupButtonClick(Button pButton, int pIndex)
    {
        int lIndexCopy = pIndex;
        pButton.onClick.AddListener(() =>
        {
            onTileButtonPressed.Invoke(lIndexCopy);
            UpdateSelectionVisuals(lIndexCopy);
        });
    }

    private void SetupButtonPreview(Button pButton, InventoryEntry pEntry)
    {
        UITilePreview lPreview = pButton.GetComponentInChildren<UITilePreview>();
        Tile lTilePrefab = TilesData.GetPrefabToLoad(pEntry);

        if (lPreview != null && lTilePrefab != null)
        {
            lPreview.Show3DModel(lTilePrefab.gameObject, TilesData.GetDirection(pEntry), pEntry.quantity);
        }
    }

    private void SetupDragComponent(Button pButton, int pIndex)
    {
        DraggableTileUI lDragComp = pButton.gameObject.GetComponent<DraggableTileUI>();
        if (lDragComp == null)
        {
            lDragComp = pButton.gameObject.AddComponent<DraggableTileUI>();
        }
        lDragComp.Initialize(pIndex);
    }

    private void OnTileButtonClicked(Button pButton)
    {
        int lIndex = _Buttons.IndexOf(pButton);
        onTileButtonPressed.Invoke(lIndex);
        UpdateSelectionVisuals(lIndex);
    }

    private void UpdateSelectionVisuals(int pSelectedIndex)
    {
        UITilePreview lPreview;
        for (int i = 0; i < _Buttons.Count; i++)
        {
            lPreview = _Buttons[i].GetComponentInChildren<UITilePreview>();
            if (lPreview != null)
            {
                lPreview.SetSelected(i == pSelectedIndex);
            }
        }
    }

    public void UpdateInventoryCount(List<int> pInventoryCount)
    {
        Text lText;
        for(int i = 0; i < _Buttons.Count; i++)
        {
            lText = _Buttons[i].GetComponentInChildren<Text>();
            lText.text = AMOUNT_PREFIX +( _Quantity[i] - pInventoryCount[i]);
            _Buttons[i].GetComponent<UITilePreview>().SetQuantity(_Quantity[i] - pInventoryCount[i]);
        }

    }

    private void ClearButtons()
    {
        for(int i = _Buttons.Count - 1; i >= 0; i--)
        {
            Destroy(_Buttons[i].gameObject);
            _Buttons.RemoveAt(i);
        }
        _Quantity.Clear();
    }
}
