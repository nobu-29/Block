using System.Collections.Generic;
using UnityEngine;

public class FurnaceSystem : MonoBehaviour
{
    public List<FurnaceRecipe> recipes;

    public FurnaceRecipe GetRecipe(ItemObject item)
    {
        foreach(var recipe in recipes)
        {
            if (recipe.inputItem == item)
                return recipe;
        }

        return null;
    }
}
