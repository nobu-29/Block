//using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryUIMG : MonoBehaviour
{
    public enum UIArea
    {
        Inventory,
        CraftGrid,
        CraftResult,

        FurnaceInput,
        FurnaceResult
    }

    public UIArea currentArea = UIArea.Inventory;

    public enum UITab
    {
        Craft,
        Furnace
    }
    public UITab TabArea = UITab.Craft;

    public GameObject inventoryPanel;
    public GameObject _furnacePanel;
    public Inventory _inventory;
    public InventoryUISlot[] _inventoryslots;
    public HotbarMG _hotbarslots;
    public CraftUIMG _craftUI;
    public FurnaceUI _furnaceUI;

    public int dragIndex = -1;

    public bool dragFromHotbar;
    public bool selectingHotbar = false;

    public int currentSlot = 0;

    public PlayerInput _playerInput;

    public int selectedSlot;

    public Color normalColor = Color.white;
    public Color selectedColor = Color.green;
    public Color heldColor = Color.yellow;

    public float _inventoryMoveDelay = 0.05f;
    public float _craftMoveDelay = 0.25f;

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
            _craftUI.UpdataCraftSelection();
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

        if(_inventory.MergeStack(from, to))
        {
            UpdateUI();
            _hotbarslots.UpdateUI();
            return;
        }

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

        switch (currentArea)
        {
            case UIArea.Inventory :
                moveDelay = _inventoryMoveDelay;
                MoveInventoryCursor(input);
                break;

            case UIArea.CraftGrid:
                moveDelay = _craftMoveDelay;
                MoveCraftCursor(input);
                break;

            case UIArea.CraftResult:
                if(input.x < -0.5f)
                {
                    currentArea = UIArea.CraftGrid;
                    _craftUI.currentCraftSlot = 8;
                    RefreshSelection();
                }
                break;
            case UIArea.FurnaceInput:
            case UIArea.FurnaceResult:
                MoveFurnaceCursor(input);
                break;
        }

        nextMoveTime = Time.time + moveDelay;

        
    }

    //↓インベントリもしくはホットバーでの移動
    void MoveInventoryCursor(Vector2 input)
    {
        //Hotbar選択中
        if (selectingHotbar)
        {

            if (input.x > 0.5f)
                currentSlot++;

            else if (input.x < -0.5f)
                currentSlot--;

            else if (input.y > 0.5f)
            {
                // Inventoryへ戻る
                selectingHotbar = false;

                currentSlot = Mathf.Clamp(currentSlot + _inventoryslots.Length - 7, 0, _inventoryslots.Length -1);

                RefreshSelection();
                return;
            }

            currentSlot = Mathf.Clamp(
                currentSlot,
                0,
                _hotbarslots.slots.Length - 1);

            UpdateSelection();
            return;

        }


        if (input.x > 0.5f)
            currentSlot++;
        else if (input.x < -0.5f)
            currentSlot--;
        else if (input.y > 0.5f)
        {
            if (selectingHotbar)
                return;

            currentSlot -= 7;

            if (currentSlot < 0)
            {
                if (TabArea == UITab.Furnace)
                {
                    currentArea = UIArea.FurnaceInput;
                    _furnaceUI.currentSlot = 0;
                    _furnaceUI.UpdateSelection();
                }
                else
                {
                    currentArea = UIArea.CraftGrid;
                    _craftUI.currentCraftSlot = Mathf.Clamp(currentSlot + 7, 6, 8);
                    _craftUI.UpdataCraftSelection();
                }
                RefreshSelection();
                return;
            }
        }
        else if (input.y < -0.5f)
        {
            currentSlot += 7;

            if (currentSlot >= _inventoryslots.Length)
            {
                selectingHotbar = true;

                currentSlot = Mathf.Clamp(currentSlot - _inventoryslots.Length, 0, _hotbarslots.slots.Length - 1);
                RefreshSelection();
                return;
            }   
        }
        else
            return;

        //nextMoveTime = Time.time + moveDelay;

        if (selectingHotbar)
            currentSlot = Mathf.Clamp(currentSlot, 0, _hotbarslots.slots.Length - 1);
        else
            currentSlot = Mathf.Clamp(currentSlot, 0, _inventoryslots.Length - 1);

        UpdateSelection();
        RefreshSelection();
    }

    //↓クラフト画面での移動
    void MoveCraftCursor(Vector2 input)
    {
        if (input.x > 0.5f)
        {
            if(_craftUI.currentCraftSlot == 8)
            {
                currentArea = UIArea.CraftResult;

                RefreshSelection();
                return;
            }

            _craftUI.currentCraftSlot++;
        }
        else if (input.x < -0.5f)
            _craftUI.currentCraftSlot--;

        else if (input.y > 0.5f)
            _craftUI.currentCraftSlot -= 3;

        else if (input.y < -0.5f)
        {
            if (_craftUI.currentCraftSlot >= 6)
            {
                currentArea = UIArea.Inventory;
                currentSlot = Mathf.Clamp(_craftUI.currentCraftSlot + 3, 0, 2);
                RefreshSelection();
                return;
            }

            _craftUI.currentCraftSlot += 3;
        }

        _craftUI.currentCraftSlot = Mathf.Clamp(_craftUI.currentCraftSlot,0,8);

        _craftUI.UpdataCraftSelection();
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

        if (currentArea == UIArea.CraftGrid)
        {
            CraftSlotUI craftSlot = _craftUI._craftSlots[_craftUI.currentCraftSlot];

            //　Craft → Inventory
            if (heldSlot == -1 && craftSlot.currentItem != null)
            {
                int emptySlot = _inventory.GetEmptySlot();

                if (emptySlot == -1) return;

                _inventory.myinventory[emptySlot].item = craftSlot.currentItem;
                _inventory.myinventory[emptySlot].count = craftSlot.count;

                craftSlot.ClearItem();

                UpdateUI();
                _hotbarslots.UpdateUI();

                _craftUI.UpdateRecipe();

                RefreshSelection();

                heldSlot = -1;
                heldFromHotbar = false;

                return;
            }

            // Inventory → Craft
            if (heldSlot != -1)
            {
                int _inventoryIndex = heldFromHotbar ? heldSlot : heldSlot + _inventory.hotbarSize;

                if (_inventoryIndex >= _inventory.myinventory.Length) return;

                InventorySlot invSlot = _inventory.myinventory[_inventoryIndex];

                if (invSlot.item == null)
                    return;

                // 空スロット
                if (craftSlot.currentItem == null)
                {
                    craftSlot.SetItem(
                        invSlot.item,
                        1);

                    invSlot.count--;

                    if (invSlot.count <= 0)
                    {
                        invSlot.item = null;
                        invSlot.count = 0;
                    }
                }
                // 同アイテムなら追加
                else if (craftSlot.currentItem ==
                         invSlot.item)
                {
                    craftSlot.count++;

                    craftSlot.countText.text =
                        craftSlot.count > 1
                        ? craftSlot.count.ToString()
                        : "";

                    invSlot.count--;

                    if (invSlot.count <= 0)
                    {
                        invSlot.item = null;
                        invSlot.count = 0;
                    }
                }

                UpdateUI();
                _hotbarslots.UpdateUI();

                _craftUI.UpdateRecipe();

                heldSlot = -1;
                heldFromHotbar = false;

                RefreshSelection();

                return;

            }
        }

        //CraftResult
        else if (currentArea == UIArea.CraftResult)
        {
            _craftUI.Craft();
            return;
        }

        //Furnace
        else if (currentArea == UIArea.FurnaceInput)
        {
            FurnaceSlotUI furnaceSlot = _furnaceUI.inputSlot;
            //Furnace → Inventory
            if (heldSlot == -1 && furnaceSlot.currentItem != null)
            {
                int emptySlot = _inventory.GetEmptySlot();
                if (emptySlot == -1) return;
                _inventory.myinventory[emptySlot].item = furnaceSlot.currentItem;
                _inventory.myinventory[emptySlot].count = furnaceSlot.count;

                furnaceSlot.ClearItem();
                _furnaceUI.UpdateSelection();
                return;
            }

            // Inventory → Furnace

            if (heldSlot != -1)
            {
                int inventoryIndex =
                    heldFromHotbar
                    ? heldSlot
                    : heldSlot + _inventory.hotbarSize;

                InventorySlot invSlot =
                    _inventory.myinventory[inventoryIndex];

                if (invSlot.item == null)
                    return;

                if (furnaceSlot.currentItem == null)
                {
                    furnaceSlot.SetItem(
                        invSlot.item,
                        1);

                    invSlot.count--;

                    if (invSlot.count <= 0)
                    {
                        invSlot.item = null;
                        invSlot.count = 0;
                    }
                }
                else if (
                    furnaceSlot.currentItem ==
                    invSlot.item)
                {
                    furnaceSlot.count++;

                    furnaceSlot.countText.text =
                        furnaceSlot.count.ToString();

                    invSlot.count--;

                    if (invSlot.count <= 0)
                    {
                        invSlot.item = null;
                        invSlot.count = 0;
                    }
                }

                heldSlot = -1;
                heldFromHotbar = false;

                _inventory.OnInventoryChanged?.Invoke();

                return;

            }
        }

        //FurnaceResult
        else if (currentArea == UIArea.FurnaceResult)
        {
            FurnaceSlotUI resultSlot =
                _furnaceUI.outputSlot;

            if (resultSlot.currentItem == null)
                return;

            _inventory.itemGet(
                resultSlot.currentItem);

            resultSlot.ConsumeItem(1);

            return;
        }


        //Inventory / Hotbar
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



    public void SwitchArea(
        InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        switch (currentArea)
        {
            case UIArea.Inventory:
                currentArea =
                    UIArea.CraftGrid;
                break;

            case UIArea.CraftGrid:
                currentArea =
                    UIArea.CraftResult;
                break;

            case UIArea.CraftResult:
                currentArea =
                    UIArea.Inventory;
                break;
        }

        RefreshSelection();
    }

    public void SwitchTabArea(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        switch (TabArea)
        {
            case UITab.Craft:
                TabArea = UITab.Furnace;
                _furnacePanel.SetActive(true);
                currentArea = UIArea.FurnaceInput;
                _furnaceUI.currentSlot = 0;
                _furnaceUI.UpdateSelection();
                break;
            case UITab.Furnace:
                TabArea = UITab.Craft;
                _furnacePanel.SetActive(false);
                currentArea = UIArea.CraftGrid;
                _craftUI.currentCraftSlot = 0;
                _craftUI.UpdataCraftSelection();
                break;
        }
    }

    void RefreshSelection()
    {
        UpdateSelection();

        _craftUI.UpdataCraftSelection();

        _craftUI.resultSlot.SetSelect(currentArea == UIArea.CraftResult);
    }

    void MoveFurnaceCursor(Vector2 input)
    {
        if(input.x > 0.5f)
        {
            _furnaceUI.currentSlot = 1;
            currentArea = UIArea.FurnaceResult;
        }
        else if(input.x < -0.5f)
        {
            _furnaceUI.currentSlot = 0;
            currentArea = UIArea.FurnaceInput;
        }
        else if(input.y < -0.5)
        {
            currentArea = UIArea.Inventory;
            currentSlot = 0;
            RefreshSelection();
            return;
        }

        _furnaceUI.UpdateSelection();
    }

    private void OnEnable()
    {
        _inventory.OnInventoryChanged += UpdateUI;
        _inventory.OnInventoryChanged += _hotbarslots.UpdateUI;
    }

    private void OnDisable()
    {
        _inventory.OnInventoryChanged -= UpdateUI;
        _inventory.OnInventoryChanged -= _hotbarslots.UpdateUI;
    }
}

[System.Serializable]
public class InventoryUISlot
{
    public Image icon;
    public Text countText;

    public Image selecctionFrame;
}
