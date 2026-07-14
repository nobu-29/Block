using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Inventory", menuName = "Game/Inventory")]
public class Inventory : ScriptableObject
{
    public InventorySlot[] myinventory = new InventorySlot[28];
    public int maxStack = 100;

    public int hotbarSize = 7;

    public void itemGet(ItemObject item)
    {
       for(int i = 0; i < myinventory.Length; i++)
        {
            var slot = myinventory[i];

            if (slot == null) continue;

            if(slot.item == item && slot.count < maxStack)
            {
                slot.count++;
                return;
            }
        }

        for (int i = 0; i < myinventory.Length; i++)
        {
            if (myinventory[i].item == null)
            {
                myinventory[i].item = item;
                myinventory[i].count = 1;
                return;
            }
        }

    }

    public void ItemRemove(ItemObject item)
    {
        foreach (var slot in myinventory)
        {
            if(slot.item == item)
            {
                slot.count--;
                if(slot.count <= 0)
                {
                    slot.item = null;
                    slot.count = 0;
                }
                return;
            }
        }
    }

    public bool ItemHas(ItemObject item)
    {
        foreach(var slot in myinventory)
        {
            if (slot == null) continue;

            if (slot.item == item && slot.count > 0)
                return true;
        }
        return false;
    }

    public void ResetInventory()
    {

        for (int i = 0; i < myinventory.Length; i++)
        {
            if (myinventory[i] == null)
                myinventory[i] = new InventorySlot();

            myinventory[i].item = null;
            myinventory[i].count = 0;
        }

    }

    public void SwapSlot(int a,int b)
    {
        var temp = myinventory[a];

        myinventory[a] = myinventory[b];

        myinventory[b] = temp;
    }

    /*    public void AnatherInventory()
        {
            List<ItemObject> anatherinventory = myinventory;
        }*/

    private void OnEnable()
    {
        for(int i = 0; i < myinventory.Length; i++)
        {
            if (myinventory[i] == null)
                myinventory[i] = new InventorySlot();
        }
    }
}

[System.Serializable]
public class InventorySlot
{
    public ItemObject item;
    public int count;

    public bool IsEmpty()
    {
        return item == null;
    }
}
