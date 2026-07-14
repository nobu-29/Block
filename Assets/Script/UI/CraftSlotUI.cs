using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CraftSlotUI : MonoBehaviour, IDropHandler
{
    public int slotIndex;

    public CraftUIMG craftUI;

    public Image _crafticon;
    public int count;
    public ItemObject currentItem;

    public Image selectionFrame;

    public bool isSelected;

    public void OnDrop(PointerEventData eventData)
    {
        InventorySlotUI inventorySlot = eventData.pointerDrag.GetComponent<InventorySlotUI>();

        if (inventorySlot == null) return;

        int inventoryIndex = inventorySlot.slotIndex;

        InventorySlot slot = inventorySlot.inventoryUI._inventory.myinventory[inventoryIndex];

        SetItem(slot.item);

        craftUI.UpdateRecipe();
    }

    public void SetItem(ItemObject item)
    {
        currentItem = item;

        if (item != null)
        {
            _crafticon.sprite = item.icon;
            _crafticon.enabled = true;
        }
        else
            _crafticon.enabled = false;
    }

    public void ClearItem()
    {
        currentItem = null;
        _crafticon.enabled = false;
    }

    public void SetSelect(bool value)
    {
        selectionFrame.color = value ? Color.green : Color.white;
    }
}
