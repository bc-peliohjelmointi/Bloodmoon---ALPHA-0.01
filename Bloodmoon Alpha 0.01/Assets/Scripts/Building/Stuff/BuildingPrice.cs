using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;

public class BuildingPrice : MonoBehaviour
{
    public List<string> DefaultShapeNames;

    private SaveMyStuff items;
    public List<Price> Pricing = new List<Price>();
    public class Price
    {
        public string BuildingName;
        public List<int> Prices;
        public List<Item> Material;
        public bool OptionalMaterials;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        items = GameObject.Find("Inventory").GetComponent<SaveMyStuff>();
        foreach (string name in DefaultShapeNames)
        {
            Price price = new Price();
            price.BuildingName = name;
            price.Prices = new List<int>();
            price.Prices.Add(10);
            price.OptionalMaterials = true;
            price.Material = new List<Item>();
            foreach (Item material in items.items)
            {
                if (material.name == "Wood" || (material.name == "Stone"))
                {
                    price.Material.Add(material);
                }
            }
            Pricing.Add(price);
        }
    }
}
