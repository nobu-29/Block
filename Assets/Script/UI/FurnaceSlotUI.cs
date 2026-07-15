using UnityEngine;
using UnityEngine.UI;

public class FurnaceSlotUI : MonoBehaviour
{
    public Image icon;
    public int count;
    public Text countText;
    public ItemObject currentItem;

    public void SetItem(ItemObject item, int itemCount = 1)
    {
        currentItem = item;
        count = itemCount;

        if(item != null)
        {
            icon.sprite = item.icon;
            icon.enabled = true;

            if (countText != null)
                countText.text = count > 1 ? count.ToString() : "";
        }
        else
        {
            icon.enabled = false;

            if (countText != null)
                countText.text = count > 1 ? count.ToString() : "";
        }
    }

    public void ClearItem()
    {
        currentItem = null;
        count = 0;

        icon.sprite = null;
        icon.enabled = false;

        if (countText != null)
            countText.text = "";
    }

    public void ConsumeItem(int amount)
    {
        count -= amount;

        if(count <= 0)
        {
            ClearItem();
            return;
        }

        if (countText != null)
            countText.text = count > 1 ? count.ToString() : "";
    }
}
