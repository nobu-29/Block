using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Inventory", menuName = "Game/Inventory")]
public class Inventory : ScriptableObject
{
    public InventorySlot[] myinventory = new InventorySlot[28];
    public int maxStack = 100;

    public int hotbarSize = 7;

    public Action OnInventoryChanged;

    public void itemGet(ItemObject item)
    {
       for(int i = 0; i < myinventory.Length; i++)
        {
            var slot = myinventory[i];

            if (slot == null) continue;

            if(slot.item == item && slot.count < maxStack)
            {
                slot.count++;
                OnInventoryChanged?.Invoke();
                return;
            }
        }

        for (int i = 0; i < myinventory.Length; i++)
        {
            if (myinventory[i].item == null)
            {
                myinventory[i].item = item;
                myinventory[i].count = 1;
                OnInventoryChanged?.Invoke();
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
                OnInventoryChanged?.Invoke();
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
        OnInventoryChanged?.Invoke();
    }

    public void SwapSlot(int a,int b)
    {
        var temp = myinventory[a];

        myinventory[a] = myinventory[b];

        myinventory[b] = temp;

        OnInventoryChanged?.Invoke();
    }

    private void OnEnable()
    {
        for(int i = 0; i < myinventory.Length; i++)
        {
            if (myinventory[i] == null)
                myinventory[i] = new InventorySlot();
        }
    }

    public int GetEmptySlot()
    {
        for (int i = 0; i < myinventory.Length; i++)
        {
            if (myinventory[i].item == null)
                return i;
        }
        return -1;
    }

    //↓インベントリ内でのアイテム合体
    public bool MergeStack(int from, int to)
    {
        if (from < 0 || from >= myinventory.Length) return false;

        if(to <  0 || to >= myinventory.Length) return false;

        InventorySlot fromSlot = myinventory[from];
        InventorySlot toSlot = myinventory[to];

        if(fromSlot.item == null || toSlot.item == null) return false;

        if (fromSlot.item != toSlot.item) return false;

        if (toSlot.count >= maxStack) return false;

        int moveCount = Mathf.Min(fromSlot.count, maxStack - toSlot.count);

        toSlot.count += moveCount;

        fromSlot.count -= moveCount;

        if(fromSlot.count <= 0)
        {
            fromSlot.item = null;
            fromSlot.count = 0;
        }
        OnInventoryChanged?.Invoke();
        return true;
    }

    public int FindStack(ItemObject item)
    {
        for (int i = 0; i < myinventory.Length; i++)
        {
            if (myinventory[i].item == item && myinventory[i].count < maxStack)
                return i;
        }
        return -1;
    }

    public void AddItem(ItemObject item, int count)
    {
        while(count > 0)
        {
            int stackIndex = FindStack(item);

            if(stackIndex != -1)
            {
                InventorySlot slot = myinventory[stackIndex];

                int addinventoryCount = Mathf.Min(count, maxStack - slot.count);

                slot.count += addinventoryCount;

                count -= addinventoryCount;

                continue;

            }

            int emptyIndex =
                       GetEmptySlot();

            if (emptyIndex == -1)
                break;

            int addCount =
                Mathf.Min(count, maxStack);

            myinventory[emptyIndex].item =
                item;

            myinventory[emptyIndex].count =
                addCount;

            count -= addCount;
        }

        OnInventoryChanged?.Invoke();
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
