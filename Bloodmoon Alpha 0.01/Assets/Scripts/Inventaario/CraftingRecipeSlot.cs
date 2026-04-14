using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class CraftingRecipeSlot : MonoBehaviour
{
    [Header("Recipe")]
    public Recipe recipe;

    [Header("UI References")]
    [SerializeField] private Text recipeNameText;
    [SerializeField] private Image resultIcon;
    [SerializeField] private Text ingredientsText;
    [SerializeField] private Button craftButton;
    [SerializeField] private Image backgroundImage;

    [Header("Colors")]
    [SerializeField] private Color craftableColor = Color.white;
    [SerializeField] private Color uncraftableColor = new Color(0.6f, 0.6f, 0.6f, 1f);

    private void Start()
    {
        if (craftButton != null)
            craftButton.onClick.AddListener(OnCraftClicked);

        Refresh();
    }

    /// <summary>Called by CraftingUI whenever inventory changes.</summary>
    public void Refresh()
    {
        if (recipe == null || Inventory.Singleton == null) return;

        bool canCraft = Inventory.Singleton.CanCraft(recipe);

        // Name
        if (recipeNameText != null && recipe.result != null)
            recipeNameText.text = recipe.result.name;

        // Icon
        if (resultIcon != null && recipe.result != null)
            resultIcon.sprite = recipe.result.sprite;

        // Ingredients list
        if (ingredientsText != null)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var ing in recipe.ingredients)
                if (ing.item != null)
                    sb.AppendLine($"{ing.item.name} x{ing.amount}");
            ingredientsText.text = sb.ToString().TrimEnd();
        }

        // Visual feedback
        if (backgroundImage != null)
            backgroundImage.color = canCraft ? craftableColor : uncraftableColor;

        if (craftButton != null)
            craftButton.interactable = canCraft;
    }

    private void OnCraftClicked()
    {
        if (recipe == null) return;
        Inventory.Singleton.Craft(recipe);
        CraftingUI.Instance?.RefreshAll(); // re-sort/filter after crafting
    }
}