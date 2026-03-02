using UnityEngine;

public class IDamageable : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] protected float health = 50f;
    [SerializeField] protected float maxHealth = 50f;
    protected virtual void DealDamage(float dmg, GameObject target)
    {
        if (target.TryGetComponent(out IDamageable damageable))
        {
            Debug.Log($"Dealing {dmg} damage to {target.name}");
            damageable.TakeDamage(dmg);
        }
        else
        {
            Debug.LogWarning($"{target.name} does not implement IDamageable.");
        }
    }

    protected virtual void TakeDamage(float dmg)
    {
        health = Mathf.Clamp(health - dmg, 0f, maxHealth);

        Debug.Log($"{gameObject.name} took {dmg} damage. Remaining health: {health}/{maxHealth}");

        if (health <= 0f)
            Die();
    }

    protected virtual void Heal(float amount)
    {
        health = Mathf.Clamp(health + amount, 0f, maxHealth);
    }

    protected virtual void Die()
    {
        Debug.Log($"{gameObject.name} has died.");

    }
}
