using UnityEngine;

[CreateAssetMenu(fileName = "CraftRecipe", menuName = "Game/Recipe")]
public class CraftingRecipe : ScriptableObject
{

    [Header("3x3ƒŒƒVƒs")]
    public ItemObject[] recipeGrid = new ItemObject[9];

    [Header("ì¬Œ‹‰Ê")]
    public ItemObject resultItem;

    public int resultAmount = 1;

}
