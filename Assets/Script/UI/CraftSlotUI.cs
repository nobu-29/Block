using UnityEngine;
using UnityEngine.EventSystems;

public class CraftSlotUI : MonoBehaviour, IDropHandler
{
    public int slotIndex;

    public CraftUIMG craftUI;

    public void OnDrop(PointerEventData eventData)
    {
        InventorySlotUI inventorySlot = eventData.pointerDrag.GetComponent<InventorySlotUI>();

        if (inventorySlot == null) return;

        int inventoryIndex = inventorySlot.slotIndex;

        InventorySlot slot = inventorySlot.inventoryUI._inventory.myinventory[inventoryIndex];

        craftUI.grid[slotIndex] = slot.item;

        craftUI.UpdateRecipe();
    }
}
