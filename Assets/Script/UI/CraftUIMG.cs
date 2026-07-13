using UnityEngine;

public class CraftUIMG : MonoBehaviour
{
    public CraftingSystem _craftSystem;

    public ItemObject[] grid = new ItemObject[9];

    public ItemObject resultItem;
    public CraftSlotUI resultSlot;

    public Inventory _inventory;

    public void UpdateRecipe()
    {
        CraftingRecipe recipe = _craftSystem.CheckRecipe(grid);

        if(recipe != null)
        {
            resultItem = recipe.resultItem;

            Debug.Log("çÏê¨â¬î\:" + recipe.resultItem.ItemName);

            //resultSlot.SetItem(recipe.resultItem);

        }
        else
        {
            resultItem = null;
            Debug.Log("ÉåÉVÉsÇ»Çµ");

            //resultSlot.ClearItem();
        }
    }

    public void Craft()
    {
        CraftingRecipe recipe = _craftSystem.CheckRecipe(grid);
        if (recipe == null) return;

        //_inventory.itemGet();

        for (int i = 0; i < 9; i++)
            grid[i] = null;

        UpdateRecipe();

    }
}
