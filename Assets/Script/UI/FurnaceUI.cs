using UnityEngine;

public class FurnaceUI : MonoBehaviour
{
    public FurnaceSystem furnaceSystem;

    public FurnaceSlotUI inputSlot;

    public FurnaceSlotUI outputSlot;

    public int currentSlot = 0;

    private float timer;

    void Update()
    {
        if (inputSlot.currentItem == null)
            return;

        FurnaceRecipe recipe =
            furnaceSystem.GetRecipe(
                inputSlot.currentItem);

        if (recipe == null)
            return;

        timer += Time.deltaTime;

        if (timer >= recipe.smeltTime)
        {
            timer = 0;

            if(outputSlot.currentItem == null)
                outputSlot.SetItem(recipe.outputItem,1);
            else if(outputSlot.currentItem == recipe.outputItem)
            {
                outputSlot.count++;
                outputSlot.countText.text = outputSlot.count.ToString();
            }

            inputSlot.ConsumeItem(1);
        }
    }

    public void UpdateSelection()
    {
        inputSlot.SetSelect(currentSlot == 0);
        outputSlot.SetSelect(currentSlot == 1);
    }
}
