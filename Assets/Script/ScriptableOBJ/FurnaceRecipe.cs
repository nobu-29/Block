using UnityEngine;

[CreateAssetMenu(fileName = "FurnaceRecipe", menuName = "Game/FurnaceRecipe")]
public class FurnaceRecipe : ScriptableObject
{
    public ItemObject inputItem;
    public ItemObject outputItem;
    public float smeltTime = 5f;
}
