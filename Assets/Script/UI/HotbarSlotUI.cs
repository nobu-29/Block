using UnityEngine;
using UnityEngine.EventSystems;

public class HotbarSlotUI : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDropHandler
{
    public int slotIndex;

    public Inventory _inventory;
    public InventoryUIMG _inventoryUI;

    public void OnBeginDrag(PointerEventData eventData)
    {
        _inventoryUI.dragFromHotbar = true;

        _inventoryUI.dragIndex = slotIndex;

        if (slotIndex < _inventory.myinventory.Length)
        {
            UIDragIcon.Instance.Show(
                _inventory.myinventory[slotIndex]
                .item.icon);
        }
    }

    public void OnEndDrag(
        PointerEventData eventData)
    {
        UIDragIcon.Instance.Hide();
    }

    public void OnDrop(
        PointerEventData eventData)
    {
        if (_inventoryUI.dragIndex == slotIndex)
            return;


        Debug.Log(
                $"Hotbar Drop : {_inventoryUI.dragIndex} -> {slotIndex}"
            );


        _inventoryUI.SwapSlots(
            _inventoryUI.dragIndex,
            slotIndex);
    }

}
