using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryUIMG : MonoBehaviour
{
    public GameObject inventoryPanel;
    public Inventory _inventory;
    public InventoryUISlot[] _inventoryslots;

    private bool isOpen = false;

    public void ToggleInventory(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        isOpen = !isOpen;

        inventoryPanel.SetActive(isOpen);
    }

    public void UpdateUI()
    {
        for (int i = 0; i < _inventoryslots.Length; i++)
        {
            if (i < _inventory.myinventory.Count)
            {
                var data = _inventory.myinventory[i];

                _inventoryslots[i].icon.sprite = data.item.icon;
                _inventoryslots[i].icon.enabled = true;

                if (data.count > 1)
                    _inventoryslots[i].countText.text = data.count.ToString();
                else
                    _inventoryslots[i].countText.text = "";
            }
            else
            {
                _inventoryslots[i].icon.enabled = false;
                _inventoryslots[i].countText.text = "";
            }
        }
    }
}

[System.Serializable]
public class InventoryUISlot
{
    public Image icon;
    public Text countText;
}
