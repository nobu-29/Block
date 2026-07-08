using System.Transactions;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public int slotIndex;

    public InventoryUIMG inventoryUI;

    public void OnBeginDrag(PointerEventData eventData)
    {
        inventoryUI.dragIndex = slotIndex;
    }

    public void OnDrag(PointerEventData eventData)
    {

    }

    public void OnEndDrag(PointerEventData eventData)
    {

    }

    public void OnDrop(PointerEventData eventData)
    {
        inventoryUI.SwapSlots(
            inventoryUI.dragIndex,
            slotIndex
        );
    }

}
