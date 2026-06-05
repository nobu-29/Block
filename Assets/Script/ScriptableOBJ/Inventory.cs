using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Inventory", menuName = "Game/Inventory")]
public class Inventory : ScriptableObject
{
    public List<ItemObject> myinventory = new List<ItemObject>();

    public void itemGet(ItemObject item)
    {
        if(!myinventory.Contains(item))
            myinventory.Add(item);
    }

    public void ItemRemove(ItemObject item)
    {
        if(myinventory.Contains(item))
            myinventory.Remove(item);
    }

    public bool ItemHas(ItemObject item)
    {
        return myinventory.Contains(item);
    }

    public void ResetInventory()
    {
        myinventory.Clear();
    }

    public void AnatherInventory()
    {
        List<ItemObject> anatherinventory = myinventory;
    }
}
