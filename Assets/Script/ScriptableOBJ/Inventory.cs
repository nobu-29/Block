using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Inventory", menuName = "Game/Inventory")]
public class Inventory : ScriptableObject
{
    public List<InventorySlot> myinventory = new List<InventorySlot>();
    public int maxStack = 100;

    public void itemGet(ItemObject item)
    {
        foreach (var slot in myinventory)
        {
            if(slot.item == item && slot.count < maxStack)
            {
                slot.count++;
                return;
            }
        }

        myinventory.Add(new InventorySlot { item = item, count = 1 });
    }

    public void ItemRemove(ItemObject item)
    {
        foreach (var slot in myinventory)
        {
            if(slot.item == item)
            {
                slot.count--;
                if (slot.count <= 0)
                    myinventory.Remove(slot);
                return;
            }
        }
    }

    public bool ItemHas(ItemObject item)
    {
        foreach(var slot in myinventory)
        {
            if (slot.item == item && slot.count > 0)
                return true;
        }
        return false;
    }

    public void ResetInventory()
    {
        myinventory.Clear();
    }

/*    public void AnatherInventory()
    {
        List<ItemObject> anatherinventory = myinventory;
    }*/
}

[System.Serializable]
public class InventorySlot
{
    public ItemObject item;
    public int count;
}
