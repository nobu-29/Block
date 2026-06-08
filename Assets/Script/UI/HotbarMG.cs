using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class HotbarMG : MonoBehaviour
{
    public Image[] slots;
    public int[] blockIDs;
    public int selectedIndex = 0;

    public Color normalColor = Color.white;
    public Color selectedColor = Color.green;

    private void Start()
    {
        UpdateHighlight();
    }

    private void Update()
    {
    }
    public void Select(int index)
    {
        selectedIndex = index;
        UpdateHighlight();
    }
    void UpdateHighlight()
    {
        for (int i = 0;i <slots.Length;i++)
        {
            slots[i].color = (i == selectedIndex) ? selectedColor : normalColor;
        }
    }
    public int GetSelectedBlockID()
    {
        return blockIDs[selectedIndex];
    }
}
