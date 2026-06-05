using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Game/ItemObject")]
public class ItemObject : ScriptableObject
{
    public string ItemName;
    [TextArea(1,4)] public string Itemdel;
}
