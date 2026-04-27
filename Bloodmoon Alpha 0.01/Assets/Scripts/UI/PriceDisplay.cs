using System.Collections.Generic;
using TMPro;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.UI;

public class PriceDisplay : MonoBehaviour
{
    public int spacing = 140;
    public GameObject PriceImagePrefab;
    private List<GameObject> PriceClones = new List<GameObject>();
    private Vector3 Midle;
    private Image Mainimage;
    private TMP_Text Maintext;
    public InventorySlot[] slot;
    public Item currentItem;
    private void Start()
    {
        Mainimage = GetComponentInChildren<Image>();
        Maintext = GetComponentInChildren<TMP_Text>();
        Midle = Mainimage.rectTransform.position;
        Mainimage.enabled = false;
        GameObject.Find("InventoryManager").GetComponent<InventoryToggle>().SetInventoryActive(true);
        slot = GameObject.Find("MainInventory").GetComponentsInChildren<InventorySlot>();
        GameObject.Find("InventoryManager").GetComponent<InventoryToggle>().SetInventoryActive(false);
    }
    public bool UpdatePriceDisplay(List<Item> Item, List<int> amount)
    {
        if (Item == null)
        {
            Mainimage.enabled = false;
            Maintext.text = "";
            Mainimage.rectTransform.position = Midle;
            foreach (Transform child in transform)
            {
                if (child.name.Contains("(Clone)"))
                {
                    Destroy(child.gameObject);
                }
            }
            PriceClones.Clear();
            return false;
        }
        else 
        {
            currentItem = Item[0];
            if (Item.Count == 1)
            {
                Mainimage.rectTransform.position = Midle;
                Mainimage.enabled = true;
                Mainimage.sprite = Item[0].sprite;
                Maintext.text = amount[0].ToString();
            }
            if (Item.Count != PriceClones.Count)
            {
                foreach (Transform child in transform)
                {
                    if (child.name.Contains("(Clone)"))
                    {
                        Destroy(child.gameObject);
                    }
                }
                PriceClones.Clear();
            }
            if (Item.Count > 1)
            {
                while (PriceClones.Count < Item.Count-1)
                {
                    PriceClones.Add(Instantiate(PriceImagePrefab, transform));
                    PriceClones[PriceClones.Count - 1].GetComponent<RectTransform>().position = Midle;
                }
                if (PriceClones.Count % 2 == 0)
                {
                    int mult = -1;
                    Mainimage.rectTransform.position = Midle;
                    for (int i = 0; i < PriceClones.Count; i++)
                    {
                        mult *= -1;
                        if (i % 2 == 0 && i != 0)
                        {
                            mult += 1;
                        }
                        PriceClones[i].GetComponent<RectTransform>().position = Midle + new Vector3(spacing * mult, 0, 0);
                    }
                }
                else
                {
                    int mult = -1;
                    Mainimage.rectTransform.position = Midle - new Vector3(spacing / 2, 0, 0);
                    for (int i = 0; i < PriceClones.Count; i++)
                    {
                        mult *= -1;
                        if (i % 2 == 0 && i != 0)
                        {
                            mult += 1;
                        }
                        PriceClones[i].GetComponent<RectTransform>().position = Midle + new Vector3((spacing * mult) - (spacing / 2), 0, 0);
                    }
                }
                for (int i = 0; i < PriceClones.Count; i++)
                {
                    PriceClones[i].GetComponent<Image>().enabled = true;
                    PriceClones[i].GetComponent<Image>().sprite = Item[i].sprite;
                    PriceClones[i].GetComponentInChildren<TMP_Text>().text = amount[i].ToString();
                }
                Mainimage.enabled = true;
                Mainimage.sprite = Item[Item.Count - 1].sprite;
                Maintext.text = amount[Item.Count - 1].ToString();
            }
        }
        return ValidatePrice(Item, amount);
    }

    private bool ValidatePrice(List<Item> items, List<int> amount)
    {
        for (int i = 0; i<items.Count; i++)
        {
            int totalFound = 0;

            foreach (InventorySlot slot in slot)
            {
                if (slot.myItem != null && slot.myItem.myItem == items[i])
                {
                    totalFound += slot.myItem.count;
                }
            }

            if (totalFound < amount[i])
                return false;
        }
        return true;
    }
}
