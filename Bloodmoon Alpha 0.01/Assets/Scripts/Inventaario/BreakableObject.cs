using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ItemDrop
{
    public Item item;
    public int minQuantity = 1;
    public int maxQuantity = 3;
}

public class BreakableObject : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 5;
    public float currentHealth = 5;
    public ParticleSystem BreackEffect;

    [Header("Break Settings")]
    public BreakType breakType = BreakType.Tree;

    [Header("Drops")]
    public List<ItemDrop> lootTable;
    public GameObject worldItemPrefab;
    public float dropRadius = 0.5f;

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0) Break();
    }

    private void Break()
    {
        // Loop through every item type in the loot table
        foreach (ItemDrop dropData in lootTable)
        {
            int dropCount = Random.Range(dropData.minQuantity, dropData.maxQuantity + 1);

            for (int i = 0; i < dropCount; i++)
            {
                if (worldItemPrefab != null && dropData.item != null)
                {
                    SpawnDrop(dropData.item);
                }
            }
        }

        if (BreackEffect != null)
        {
            Instantiate(BreackEffect, transform.position, transform.rotation);
        }

        Destroy(gameObject);
    }

    private void SpawnDrop(Item item)
    {
        Vector3 offset = Random.insideUnitSphere * dropRadius;
        offset.y = 0.5f; // Spawn slightly above ground

        GameObject dropObj = Instantiate(
            worldItemPrefab,
            transform.position + offset,
            Quaternion.identity
        );

        // Assign the specific item to the pickup script
        if (dropObj.TryGetComponent(out WorldItemPickup pickup))
        {
            pickup.item = item;
        }
    }

    public void Repair(float amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    }
}