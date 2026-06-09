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
            if(i < _inventory.myinventory.Count)
            {
                slots[i].item = _inventory.myinventory[i].item;
                slots[i].icon.sprite = slots[i].item.icon;
                slots[i].icon.enabled = true;
            }
            else
            {
                slots[i].item = null;
                slots[i].icon.sprite = null;
                slots[i].icon.enabled = false;
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
}
