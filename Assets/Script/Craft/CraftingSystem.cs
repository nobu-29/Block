using System;
using System.Collections.Generic;
using UnityEngine;

public class CraftingSystem : MonoBehaviour
{
    public List<CraftingRecipe> allRecipes;

    public CraftingRecipe CheckRecipe(ItemObject[] currentGrid)
    {
        if (currentGrid == null || currentGrid.Length != 9) return null;

        // 1. 3x3グリッド内の素材が置かれている最小・最大範囲（バウンディングボックス）を算出
        int inputMinX = 3, inputMaxX = -1, inputMinY = 3, inputMaxY = -1;
        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 3; x++)
            {
                if (currentGrid[y * 3 + x] != null)
                {
                    if (x < inputMinX) inputMinX = x;
                    if (x > inputMaxX) inputMaxX = x;
                    if (y < inputMinY) inputMinY = y;
                    if (y > inputMaxY) inputMaxY = y;
                }
            }
        }

        if (inputMaxX == -1) return null; // 盤面が空

        int inputWidth = inputMaxX - inputMinX + 1;
        int inputHeight = inputMaxY - inputMinY + 1;

        // 2. 登録済みレシピと形状およびアイテム内容を照合
        foreach (var recipe in allRecipes)
        {
            if (recipe == null || recipe.resultItem == null) continue;

            int recipeMinX = 3, recipeMaxX = -1, recipeMinY = 3, recipeMaxY = -1;
            for (int y = 0; y < 3; y++)
            {
                for (int x = 0; x < 3; x++)
                {
                    if (recipe.recipeGrid[y * 3 + x] != null)
                    {
                        if (x < recipeMinX) recipeMinX = x;
                        if (x > recipeMaxX) recipeMaxX = x;
                        if (y < recipeMinY) recipeMinY = y;
                        if (y > recipeMaxY) recipeMaxY = y;
                    }
                }
            }

            if (recipeMaxX == -1) continue;

            int recipeWidth = recipeMaxX - recipeMinX + 1;
            int recipeHeight = recipeMaxY - recipeMinY + 1;

            if (inputWidth != recipeWidth || inputHeight != recipeHeight) continue;

            // 平行移動を考慮して、中身のアイテムIDを一致判定
            bool matchFound = true;
            for (int dy = 0; dy < inputHeight; dy++)
            {
                for (int dx = 0; dx < inputWidth; dx++)
                {
                    ItemObject inputItem = currentGrid[(inputMinY + dy) * 3 + (inputMinX + dx)];
                    ItemObject recipeItem = recipe.recipeGrid[(recipeMinY + dy) * 3 + (recipeMinX + dx)];

                    if (inputItem != recipeItem)
                    {
                        matchFound = false;
                        break;
                    }
                }
                if (!matchFound) break;
            }

            if (matchFound) return recipe; // レシピ合致！
        }

        return null;
    }
}