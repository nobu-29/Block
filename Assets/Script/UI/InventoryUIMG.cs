using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryUIMG : MonoBehaviour
{
    public GameObject inventoryPanel;
    public Inventory _inventory;
    public InventoryUISlot[] _inventoryslots;

    public int dragIndex = -1;

    public bool dragFromHotbar;

    public int currentSlot = 0;

    public PlayerInput _playerInput;

    public int selectedSlot;

    public Color normalColor = Color.white;
    public Color selectedColor = Color.yellow;
    public Color heldColor = Color.green;

    private bool isOpen = false;
    private int heldSlot = -1;

    private float moveDelay = 0.15f;
    private float nextMoveTime;

    public void ToggleInventory(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        isOpen = !isOpen;

        inventoryPanel.SetActive(isOpen);

        if (isOpen)
        {
            UpdateUI();
            _playerInput.SwitchCurrentActionMap("UI");
        }
        else
        {
            _playerInput.SwitchCurrentActionMap("Player");
        }
    }

    public void UpdateUI()
    {
        for (int i = 0; i < _inventoryslots.Length; i++)
        {
            int inventoryIndex = i + _inventory.hotbarSize;

            if (inventoryIndex < _inventory.myinventory.Length &&
                _inventory.myinventory[inventoryIndex] != null &&
                _inventory.myinventory[inventoryIndex].item != null)
            {
                var data = _inventory.myinventory[inventoryIndex];

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

    public void SwapSlots(int from, int to)
    {
        if (to < 0) return;

        if (from < 0 || from >= _inventory.myinventory.Length) return;

        if ( to >= _inventory.myinventory.Length) return;

        var temp = _inventory.myinventory[from];

        _inventory.myinventory[from] = _inventory.myinventory[to];

        _inventory.myinventory[to] = temp;

        UpdateUI();
    }

    public void MoveCursor(InputAction.CallbackContext context)
    {
        if (Time.time < nextMoveTime) return;

        var input = context.ReadValue<Vector2>();

        if (input.x > 0.5f)
            currentSlot++;
        else if (input.x < -0.5f)
            currentSlot--;
        else if (input.y > 0.5f)
            currentSlot -= 7;
        else if (input.y < -0.5f)
            currentSlot += 7;
        else
            return;

        nextMoveTime = Time.time + moveDelay;

        currentSlot = Mathf.Clamp(currentSlot, 0, _inventoryslots.Length - 1);

        UpdateSelection();
    }

    void UpdateSelection()
    {
        for (int i = 0; i < _inventoryslots.Length; i++)
        {
            if (_inventoryslots[i].selecctionFrame == null)
                continue;

            if (i == heldSlot)
                _inventoryslots[i].selecctionFrame.color = heldColor;
            else if (i == currentSlot)
                _inventoryslots[i].selecctionFrame.color = selectedColor;
            else
                _inventoryslots[i].selecctionFrame.color = normalColor;
        }
    }

    public void Submit(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        
        if (heldSlot == -1)
            heldSlot = currentSlot;
        else
        {
            SwapSlots( heldSlot + _inventory.hotbarSize, currentSlot + _inventory.hotbarSize);

            heldSlot = -1;
        }

        UpdateSelection();
    }
}

[System.Serializable]
public class InventoryUISlot
{
    public Image icon;
    public Text countText;

    public Image selecctionFrame;
}
