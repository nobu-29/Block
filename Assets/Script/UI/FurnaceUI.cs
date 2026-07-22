using UnityEngine;
using UnityEngine.UI;

public class FurnaceUI : MonoBehaviour
{
    public FurnaceSystem furnaceSystem;

    public FurnaceSlotUI inputSlot;

    public FurnaceSlotUI outputSlot;

    public int currentSlot = 0;

    public Image _progressBar;

    private float timer;

    void Update()
    {
        if (inputSlot.currentItem == null)
        {
            timer = 0;
            _progressBar.fillAmount = 0;
            return;
        }

        FurnaceRecipe recipe = furnaceSystem.GetRecipe(inputSlot.currentItem);

        if (recipe == null)
        {
            timer = 0;
            _progressBar.fillAmount = 0;
            return;
        }

        if (outputSlot.currentItem != null && outputSlot.currentItem != recipe.outputItem)
        {
            timer = 0;
            _progressBar.fillAmount = 0;
            return;
        }

        if (outputSlot.currentItem == recipe.outputItem && outputSlot.count >= 100) 
        {
            timer = 0;
            _progressBar.fillAmount = 0;
            return; 
        }

        timer += Time.deltaTime;

        if (timer >= recipe.smeltTime)
        {
            timer = 0;
            _progressBar.fillAmount = 0;

            if(outputSlot.currentItem == null)
                outputSlot.SetItem(recipe.outputItem,1);
            else if(outputSlot.currentItem == recipe.outputItem && outputSlot.count < 100)
            {
                outputSlot.count++;
                outputSlot.UpdateCountText();
            }

            inputSlot.ConsumeItem(1);
        }
        _progressBar.fillAmount = timer / recipe.smeltTime;
    }

    public void UpdateSelection()
    {
        inputSlot.SetSelect(currentSlot == 0);
        outputSlot.SetSelect(currentSlot == 1);
    }
}
