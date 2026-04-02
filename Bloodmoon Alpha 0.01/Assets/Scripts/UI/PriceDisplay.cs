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
    private void Start()
    {
        Mainimage = GetComponentInChildren<Image>();
        Maintext = GetComponentInChildren<TMP_Text>();
        Midle = Mainimage.rectTransform.position;
        Mainimage.enabled = false;
    }
    public void UpdatePriceDisplay(List<Sprite> sprite, List<int> amount)
    {
        if (sprite == null)
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
        }
        else 
        {
            if (sprite.Count == 1)
            {
                Mainimage.rectTransform.position = Midle;
                Mainimage.enabled = true;
                Mainimage.sprite = sprite[0];
                Maintext.text = amount[0].ToString();
            }
            if (sprite.Count != PriceClones.Count)
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
            if (sprite.Count > 1)
            {
                while (PriceClones.Count < sprite.Count)
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
                    PriceClones[i].GetComponent<Image>().sprite = sprite[i];
                    PriceClones[i].GetComponentInChildren<TMP_Text>().text = amount[i].ToString();
                }
                Mainimage.enabled = true;
                Mainimage.sprite = sprite[sprite.Count - 1];
                Maintext.text = amount[sprite.Count - 1].ToString();
            }
        }
    }
}
