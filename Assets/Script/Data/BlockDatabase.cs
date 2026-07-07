using System.Collections.Generic;
using UnityEngine;

public class BlockDatabase : MonoBehaviour
{
    public static BlockDatabase Instance;

    [System.Serializable]
    public class BlockEntry
    {
        public int blockID;
        public ItemObject item;
    }

    public List<BlockEntry> blocks = new ();

    private Dictionary<int, ItemObject> blockDictionary;

    private void Awake()
    {
        Instance = this;

        blockDictionary = new Dictionary<int, ItemObject>();

        foreach (var block in blocks)
            blockDictionary[block.blockID] = block.item;
    }

    public ItemObject GetItem(int blockID)
    {
        if (blockDictionary.TryGetValue(blockID, out ItemObject item))
            return item;

        return null;
    }
}
