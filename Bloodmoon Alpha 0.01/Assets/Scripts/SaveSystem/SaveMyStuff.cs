using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.UI;

public class SaveMyStuff : MonoBehaviour
{
    public GameObject ItemPrefab;
    public Item[] items;
    public void Save(ref InventoryData Data) //Save
    {
        GameObject.Find("InventoryManager").GetComponent<InventoryToggle>().SetInventoryActive(true);
        Data.InventoryItems = new List<string>();
        Data.ParentsNames = new List<string>();
        Data.InventoryNumbers = new List<int>();
        foreach (InventoryItem item in transform.GetComponentsInChildren<InventoryItem>())
        {
            Data.InventoryItems.Add(item.myItem.name);
            Data.ParentsNames.Add(item.transform.parent.name);
            if (item.GetComponentInChildren<Text>().text != "")
            {
                Data.InventoryNumbers.Add(Convert.ToInt16(item.GetComponentInChildren<Text>().text));
            }
            else
            {
                Data.InventoryNumbers.Add(0);
            }
        }
        GameObject.Find("InventoryManager").GetComponent<InventoryToggle>().SetInventoryActive(false);
    }
    public void Load(InventoryData Data)
    {
        GameObject.Find("InventoryManager").GetComponent<InventoryToggle>().SetInventoryActive(true);
        foreach (InventoryItem item in transform.GetComponentsInChildren<InventoryItem>())
        {
            Destroy(item.gameObject);
        }
        for (int i = 0; i < Data.InventoryItems.Count; i++)
        {
            GameObject item = Instantiate(ItemPrefab, GameObject.Find(Data.ParentsNames[i]).transform);
            for (int j = 0; j < items.Count(); j++)
            {
                if (Data.InventoryItems[i] == items[j].name)
                {
                    item.GetComponent<InventoryItem>().myItem = items[j];
                    if (Data.InventoryNumbers[i] > 1)
                    {
                        item.GetComponentInChildren<Text>().text = Convert.ToString(Data.InventoryNumbers[i]);
                    }
                    else
                    {
                        item.GetComponentInChildren<Text>().text = "";
                    }
                }
            }
            item.GetComponentInParent<InventorySlot>().SetItem(item.GetComponent<InventoryItem>());
            item.GetComponent<Image>().sprite = item.GetComponent<InventoryItem>().myItem.sprite;
        }
        GameObject.Find("InventoryManager").GetComponent<InventoryToggle>().SetInventoryActive(false);
    }
}
[System.Serializable]
public struct InventoryData
{
    public List<string> InventoryItems;
    public List<string> ParentsNames;
    public List<int> InventoryNumbers;
}
