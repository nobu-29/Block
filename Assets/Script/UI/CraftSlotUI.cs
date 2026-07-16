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
    public Text countText;

    public Image selectionFrame;

    public bool isSelected;

    public void Start()
    {
        countText.text = "";
    }

    public void OnDrop(PointerEventData eventData)
    {
        InventorySlotUI inventorySlot = eventData.pointerDrag.GetComponent<InventorySlotUI>();

        if (inventorySlot == null) return;

        int inventoryIndex = inventorySlot.slotIndex;

        InventorySlot slot = inventorySlot.inventoryUI._inventory.myinventory[inventoryIndex];

        SetItem(slot.item);

        count = 1;

        craftUI.UpdateRecipe();
    }

    public void SetItem(ItemObject item, int itemCount = 1)
    {
        currentItem = item;
        count = itemCount;

        if (item != null)
        {
            _crafticon.sprite = item.icon;
            _crafticon.enabled = true;

            countText.text = count > 1 ? count.ToString() : "";
        }
        else
        {
            _crafticon.enabled = false;
            countText.text = "";
        }
    }

    public void ClearItem()
    {
        currentItem = null;
        count = 0;
        _crafticon.enabled = false;
        countText.text = "";
    }

    public void SetSelect(bool value)
    {
        selectionFrame.color = value ? Color.green : Color.white;
    }

    public void ConsumeItem(int amount)
    {
        count -= amount;
        if(count <= 0)
        {
            ClearItem();
            return;
        }

        countText.text = count > 1 ? count.ToString() : "";
    }

    public bool HasItem()
    {
        return currentItem != null;
    }
}
