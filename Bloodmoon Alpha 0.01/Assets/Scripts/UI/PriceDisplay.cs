using TMPro;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.UI;

public class PriceDisplay : MonoBehaviour
{
    private Image image;
    private TMP_Text text;
    private void Start()
    {
        image = GetComponentInChildren<Image>();
        text = GetComponentInChildren<TMP_Text>();
        image.enabled = false;
    }
    public void UpdatePriceDisplay(Sprite sprite, int amount)
    {
        if (sprite == null)
        {
            image.enabled = false;
            text.text = "";
        }
        else
        {
            image.enabled = true;
            image.sprite = sprite;
            text.text = amount.ToString();
        }
    }
}
