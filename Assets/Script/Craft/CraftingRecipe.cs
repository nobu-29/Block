using NUnit.Framework.Interfaces;
using UnityEngine;

[CreateAssetMenu(fileName = "CraftRecipe", menuName = "Game/Recipe")]
public class CraftingRecipe : ScriptableObject
{

    [Header("3x3ƒŒƒVƒs")]
    public ItemObject[] recipeGrid = new ItemObject[9];

    [Header("ì¬Œ‹‰Ê")]
    public ItemObject resultItem;
    public int resultAmount = 1;

    public ItemObject GetItemAt(int x, int y)
    {
        if (x < 0 || x > 2 || y < 0 || y > 2) return null;
        return recipeGrid[y * 3 + x];
    }
}
