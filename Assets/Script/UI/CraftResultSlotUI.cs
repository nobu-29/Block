using UnityEngine;
using UnityEngine.UI;

public class CraftResultSlotUI : MonoBehaviour
{
    public Image _resulticon;

    public ItemObject currentItem;

    public void SetItem(ItemObject item)
    {
        currentItem = item;

        _resulticon.sprite = item.icon;
        _resulticon.enabled = true;
    }

    public void ClearItem()
    {
        currentItem = null;
        _resulticon.enabled = false;
    }
}
