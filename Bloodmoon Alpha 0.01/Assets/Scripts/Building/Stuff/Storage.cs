using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;

public class Storage : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private class itemInfo
    {
        Item item;
        int number;
    }
    private itemInfo[] storage;
    public int capasity;
    void Start()
    {
        if (storage.Length != capasity)
        {
            storage = new itemInfo[capasity];
            for (int i = 0; i < storage.Length; i++)
            {
                storage[i] = null;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
