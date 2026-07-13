using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public int slotIndex;

    public InventoryUIMG inventoryUI;
    public Image slotIcon;

    public Image selection;

    public void OnBeginDrag(PointerEventData eventData)
    {
        inventoryUI.dragFromHotbar = false;

        inventoryUI.dragIndex = slotIndex + inventoryUI._inventory.hotbarSize;

        if(slotIcon != null && slotIcon.sprite != null)
            UIDragIcon.Instance.Show(slotIcon.sprite);
    }

    public void OnDrag(PointerEventData eventData)
    {
        inventoryUI.SwapSlots(inventoryUI.dragIndex, slotIndex);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        UIDragIcon.Instance.Hide();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (inventoryUI.dragIndex == slotIndex) return;

        inventoryUI.SwapSlots(inventoryUI.dragIndex,slotIndex + inventoryUI._inventory.hotbarSize);
    }

}
