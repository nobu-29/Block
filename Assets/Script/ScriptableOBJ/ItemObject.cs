using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Game/ItemObject")]
public class ItemObject : ScriptableObject
{
    public Sprite icon;
    public string ItemName;
    public GameObject blockPrefab;
    [TextArea(1,4)] public string Itemdel;
}
