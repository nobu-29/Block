using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryUIMG : MonoBehaviour
{
    public GameObject inventoryPanel;
    public Inventory _inventory;
    public InventoryUISlot[] _inventoryslots;
    public HotbarMG _hotbarslots;

    public int dragIndex = -1;

    public bool dragFromHotbar;
    public bool selectingHotbar = false;

    public int currentSlot = 0;

    public PlayerInput _playerInput;

    public int selectedSlot;

    public Color normalColor = Color.white;
    public Color selectedColor = Color.green;
    public Color heldColor = Color.yellow;

    private bool isOpen = false;
    private bool heldFromHotbar;

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
            UpdateSelection();
            _hotbarslots.UpdateUI();
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
        _hotbarslots.UpdateUI();
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
        {
            if (selectingHotbar)
                return;

            currentSlot -= 7;

            if(currentSlot < 0)
            {
                selectingHotbar = true;
                currentSlot = Mathf.Abs(currentSlot);
            }
        }
        else if (input.y < -0.5f)
        {
            if (selectingHotbar)
                selectingHotbar = false;
            else
                currentSlot += 7;
        }
        else
            return;

        nextMoveTime = Time.time + moveDelay;

        if(selectingHotbar)
            currentSlot = Mathf.Clamp(currentSlot, 0, _hotbarslots.slots.Length - 1);
        else
            currentSlot = Mathf.Clamp(currentSlot, 0, _inventoryslots.Length - 1);

        UpdateSelection();
    }

    void UpdateSelection()
    {
        //Inventoryカラー処理
        for (int i = 0; i < _inventoryslots.Length; i++)
        {
            Color color = normalColor;

            if (!selectingHotbar)
            {
                if (i == heldSlot && !heldFromHotbar)
                    color = heldColor;

                else if (i == currentSlot)
                    color = selectedColor;
            }

            _inventoryslots[i].selecctionFrame.color = color;

        }

        //Hotbarカラー処理
        for (int i = 0; i < _hotbarslots.slots.Length; i++)
        {
            Color color = normalColor;

            if (selectingHotbar)
            {
                if (heldFromHotbar && heldSlot == i)
                    color = heldColor;

                else if (currentSlot == i)
                    color = selectedColor;
            }
            _hotbarslots.slots[i].select.color = color;
        }
    }

    public void Submit(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (heldSlot == -1)
        {
            heldSlot = currentSlot;
            heldFromHotbar = selectingHotbar;
        }
        else
        {
            int from = heldFromHotbar ? heldSlot : heldSlot + _inventory.hotbarSize;

            int to = selectingHotbar ? currentSlot : currentSlot + _inventory.hotbarSize;
            
            SwapSlots(from, to);

            heldSlot = -1;
            heldFromHotbar = false;
        }

        UpdateSelection();
        _hotbarslots.UpdateUI();
    }
}

[System.Serializable]
public class InventoryUISlot
{
    public Image icon;
    public Text countText;

    public Image selecctionFrame;
}
