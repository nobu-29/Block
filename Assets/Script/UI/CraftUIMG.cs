using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CraftUIMG : MonoBehaviour
{
    public CraftingSystem _craftSystem;

    public CraftSlotUI[] _craftSlots;

    public ItemObject resultItem;
    public CraftResultSlotUI resultSlot;
    public Text resultName;
    public Text resultCount;

    public int currentCraftSlot;

    public Inventory _inventory;

    public InventoryUIMG _inventoryUI;

    public void UpdataCraftSelection()
    {
        for(int i = 0; i < _craftSlots.Length; i++)
        {
            if (_craftSlots[i].gameObject.activeSelf)
                _craftSlots[i].SetSelect(i == currentCraftSlot);
            else
                _craftSlots[i].SetSelect(false);
        }
    }

    public void UpdateRecipe()
    {
        ItemObject[] currentGrid = new ItemObject[9];

        for (int i = 0; i < 9; i++)
            currentGrid[i] = _craftSlots[i].currentItem;

        CraftingRecipe recipe = _craftSystem.CheckRecipe(currentGrid);

        if(recipe != null)
        {
            Debug.Log("作成可能:" + recipe.resultItem.ItemName);
            resultName.text = recipe.resultItem.ItemName;
            resultCount.text = recipe.resultAmount > 1 ? "x"+ recipe.resultAmount.ToString() : "";

            resultSlot.SetItem(recipe.resultItem);
        }
        else
        {
            Debug.Log("レシピなし");
            resultName.text = "";
            resultCount.text = "";

            resultSlot.ClearItem();
        }
    }

    public void Craft()
    {
        ItemObject[] currentGrid = new ItemObject[9];

        for (int i = 0; i < 9; i++)
        {
            currentGrid[i] = _craftSlots[i].currentItem;
        }

        CraftingRecipe recipe = _craftSystem.CheckRecipe(currentGrid);
        if (recipe == null) return;

        _inventory.AddItem(recipe.resultItem, recipe.resultAmount);

        for (int i = 0; i < 9; i++)
        {
            if (_craftSlots[i].currentItem != null)
                _craftSlots[i].ConsumeItem(1);
        }

        UpdateRecipe();

        _inventoryUI.UpdateUI();
        _inventoryUI._hotbarslots.UpdateUI();
    }

    public void CraftAll()
    {
        while (true)
        {
            ItemObject[] currentGrid = new ItemObject[9];

            for(int i = 0; i < 9; i++)
                currentGrid[i] = _craftSlots[i].currentItem;

            CraftingRecipe recipe = _craftSystem.CheckRecipe(currentGrid);

            if (recipe == null) break;

            Craft();
        }
    }

    public void SubmitCraft(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        Debug.Log( "選択中クラフト枠 = " + currentCraftSlot);
    }
}
