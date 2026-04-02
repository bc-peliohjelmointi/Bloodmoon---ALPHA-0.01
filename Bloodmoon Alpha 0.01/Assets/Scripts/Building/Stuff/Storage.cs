using System;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using Unity.VisualScripting;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class Storage : MonoBehaviour
{
    GameObject storageUI;
    bool saved = false;
    [System.Serializable]
    public struct itemInfo
    {
        public string item;
        public int number;
        public string slot;
    }
    public List<itemInfo> storage = new List<itemInfo>();
    public int capasity;

    public void StorageSave()
    {
        if (saved) {  return; }
        storageUI.SetActive(true);
        foreach (Transform t in storageUI.transform) 
        {
            t.gameObject.SetActive(true);
        }
        storage.Clear();
        saved = true;
        foreach (InventoryItem item in storageUI.GetComponentsInChildren<InventoryItem>())
        {
            itemInfo info = new itemInfo();

            info.item = item.myItem.name;
            if (item.GetComponentInChildren<Text>().text.Length > 0) 
            { info.number = Convert.ToInt16(item.GetComponentInChildren<Text>().text); }
            else { info.number = 0; }
            info.slot = item.GetComponentInParent<InventorySlot>().transform.name;

            storage.Add(info);
        }
        foreach (Transform t in storageUI.transform)
        {
            t.gameObject.SetActive(false);
        }
        storageUI.SetActive(false);
    }

    public void StorageLoad()
    {
        if (storageUI == null) { storageUI = GameObject.Find("Storage"); }
        saved = false;
        foreach(Transform child in storageUI.transform)
        {
            if (child.GetComponentInChildren<InventoryItem>() != null)
            {
                Destroy(child.GetComponentInChildren<InventoryItem>().gameObject);
            }
        }
        Item _item = null;
        for (int i = 0; i < storage.Count; i++)
        {
            GameObject item = Instantiate(storageUI.GetComponentInParent<SaveMyStuff>().ItemPrefab, GameObject.Find(storage[i].slot).transform);
            for (int j = 0; j < storageUI.GetComponentInParent<SaveMyStuff>().items.Length; j++)
            {
                if (storage[i].item == storageUI.GetComponentInParent<SaveMyStuff>().items[j].name)
                {
                    _item = storageUI.GetComponentInParent<SaveMyStuff>().items[j];
                    item.GetComponent<InventoryItem>().myItem = storageUI.GetComponentInParent<SaveMyStuff>().items[j];
                    item.GetComponent<InventoryItem>().Initialize(_item, item.GetComponentInParent<InventorySlot>());
                    item.GetComponentInParent<InventorySlot>().SetItem(item.GetComponent<InventoryItem>());
                    item.GetComponent<Image>().sprite = item.GetComponent<InventoryItem>().myItem.sprite;
                    if (storage[i].number > 1)
                    {
                        item.GetComponent<InventoryItem>().AddStack(storage[i].number-1);
                    }
                    RectTransform rt = item.GetComponent<RectTransform>();
                    rt.anchorMin = Vector2.zero;
                    rt.anchorMax = Vector2.one;
                    rt.offsetMin = Vector2.zero;
                    rt.offsetMax = Vector2.zero;
                    rt.localScale = Vector3.one;
                }
            }
        }
    }
}
