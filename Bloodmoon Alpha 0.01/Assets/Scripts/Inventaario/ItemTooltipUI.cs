using UnityEngine;
using UnityEngine.UI;

public class ItemTooltipUI : MonoBehaviour
{
    public static ItemTooltipUI Instance;

    [SerializeField] private Text text; // your UI text

    private void Awake()
    {
        Instance = this;
        Clear();
    }

    public void Show(Item item, int count)
    {
        if (item == null) return;

        string info = item.name;

        if (!string.IsNullOrEmpty(item.description))
            info += "\n" + item.description;

        if (item.IsStackableItem())
            info += "\nAmount: " + count;

        if (item.foodRestore > 0)
            info += "\nFood: +" + item.foodRestore;

        if (item.waterRestore > 0)
            info += "\nWater: +" + item.waterRestore;

        text.text = info;
    }

    public void Clear()
    {
        text.text = "";
    }
}