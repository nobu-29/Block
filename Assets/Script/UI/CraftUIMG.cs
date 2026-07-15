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

    public int currentCraftSlot;

    public Inventory _inventory;

    public void UpdataCraftSelection()
    {
        for(int i = 0; i < _craftSlots.Length; i++)
        {
            _craftSlots[i].SetSelect(i == currentCraftSlot);
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

            resultSlot.SetItem(recipe.resultItem);
        }
        else
        {
            Debug.Log("レシピなし");
            resultName.text = "";

            resultSlot.ClearItem();
        }
    }

    public void Craft()
    {
        ItemObject[] currentGrid = new ItemObject[9];

        for (int i = 0; i < 9; i++)
            currentGrid[i] = _craftSlots[i].currentItem;

        CraftingRecipe recipe = _craftSystem.CheckRecipe(currentGrid);
        if (recipe == null) return;

        _inventory.itemGet(recipe.resultItem);

        for (int i = 0; i < 9; i++)
            _craftSlots[i].ClearItem();

        UpdateRecipe();

    }

/*    public void MoveCraftCursor(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        Vector2 input = context.ReadValue<Vector2>();

        if (input.x > 0.5f)
            currentCraftSlot++;

        else if (input.x < -0.5f)
            currentCraftSlot--;

        else if (input.y > 0.5f)
            currentCraftSlot -= 3;

        else if (input.y < -0.5f)
            currentCraftSlot += 3;

        currentCraftSlot = Mathf.Clamp(currentCraftSlot, 0, _craftSlots.Length - 1);

        UpdataCraftSelection();
    }*/

    public void SubmitCraft(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        Debug.Log( "選択中クラフト枠 = " + currentCraftSlot);
    }
}
