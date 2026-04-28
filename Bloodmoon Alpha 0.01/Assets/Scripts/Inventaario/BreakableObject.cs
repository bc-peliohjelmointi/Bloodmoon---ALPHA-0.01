using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 5;
    public float currentHealth = 5;
    public ParticleSystem BreackEffect;

    [Header("Break Settings")]
    public BreakType breakType = BreakType.Tree;

    [Header("Drops")]
    public Item dropItem;
    public int minDrops = 1;
    public int maxDrops = 3;

    public GameObject worldItemPrefab;
    public float dropRadius = 0.5f;

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
            Break();
    }

    private void Break()
    {
        int dropCount = Random.Range(minDrops, maxDrops + 1);

        for (int i = 0; i < dropCount; i++)
        {
            if (worldItemPrefab != null)
            {
                Vector3 offset = Random.insideUnitSphere * dropRadius;
                offset.y = 0f;

                GameObject drop = Instantiate(
                    worldItemPrefab,
                    transform.position + offset,
                    Quaternion.identity
                );

                drop.GetComponent<WorldItemPickup>().item = dropItem;
            }
        }
        if (BreackEffect != null)
        {
            Instantiate(BreackEffect, transform.position, transform.rotation);
        }
        Destroy(gameObject);
    }
    public void Repair(float amount)
    {
        // Increase health but don't go over the max
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    }
}