using UnityEngine;

public class FurnaceUI : MonoBehaviour
{

    public FurnaceSystem furnaceSystem;

    public FurnaceSlotUI inputSlot;

    public FurnaceSlotUI outputSlot;

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

            outputSlot.SetItem(
                recipe.outputItem);

            inputSlot.ConsumeItem(1);
        }
    }

}
