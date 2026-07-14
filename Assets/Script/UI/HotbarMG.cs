using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[System.Serializable]
public class HotBarSlot
{
    public Image icon;
    public Image select;
    public ItemObject item;
    public Text countText;
}

public class HotbarMG : MonoBehaviour
{
    public HotBarSlot[] slots;
    public Inventory _inventory;
    public int selectedIndex = 0;

    public Color normalColor = Color.white;
    public Color selectedColor = Color.green;

    private void Start()
    {
        UpdateUI();
        UpdateHighlight();
    }

    public void Select(int index)
    {
        selectedIndex = index;
        UpdateHighlight();
    }

    void UpdateHighlight()
    {
        for (int i = 0; i <slots.Length; i++)
        {
            slots[i].select.color = (i == selectedIndex) ? selectedColor : normalColor;
        }
    }

    public void UpdateUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if(i < _inventory.hotbarSize &&
                i < _inventory.myinventory.Length && 
                _inventory.myinventory[i].item != null)
            {
                var data = _inventory.myinventory[i];

                slots[i].item = data.item;
                slots[i].icon.sprite = data.item.icon;
                slots[i].icon.enabled = true;

                //ŒÂ”•\Ž¦
                int count = _inventory.myinventory[i].count;

                if (count > 1)
                    slots[i].countText.text = count.ToString();
                else
                    slots[i].countText.text = "";
            }
            else
            {
                slots[i].item = null;
                slots[i].icon.enabled = false;

                slots[i].countText.text = "";
            }
        }
    }

    public ItemObject GetSelectedItem()
    {
        if(selectedIndex < slots.Length && slots[selectedIndex].item != null)
        {
            return slots[selectedIndex].item;
        }
        return null;
    }

    public ItemObject GetItemByID(int id)
    {
        foreach (var slot in slots)
        {
            if (slot.item != null && slot.item.blockID == id)
            {
                return slot.item;
            }
        }
        return null;
    }

}
