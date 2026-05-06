using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableTileUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private int _InventoryIndex;

    public void Initialize(int pIndex)
    {
        _InventoryIndex = pIndex;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        DragService.StartDrag(_InventoryIndex);
    }

    public void OnDrag(PointerEventData eventData)
    {
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        DragService.EndDrag();
    }
}
