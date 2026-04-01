using UnityEngine;
using System;

public class IDamageable : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] protected float health = 50f;
    [SerializeField] protected float maxHealth = 50f;
    [SerializeField] protected bool CanTakeKnockback = true;

    [Header("Effects")]
    [SerializeField] protected ParticleSystem Blood;

    public static Action OnPlayerDeath;

    protected virtual void DealDamage(float dmg, GameObject target, Vector3 KnockBack)
    {
        if (target.TryGetComponent(out IDamageable damageable))
        {
            Debug.Log($"Dealing {dmg} damage to {target.name}");
            damageable.TakeDamage(dmg, KnockBack);
        }
    }

    public virtual void TakeDamage(float dmg, Vector3 KnockBack)
    {
        health = Mathf.Clamp(health - dmg, 0f, maxHealth);

        if (CanTakeKnockback) { transform.position += KnockBack; }

        Instantiate(Blood, transform.position, Quaternion.identity);

        Debug.Log($"{gameObject.name} took {dmg} damage. Remaining health: {health}/{maxHealth}");

        if (health <= 0f)
            Die();
    }

    protected virtual void Die()
    {
        Debug.Log($"{gameObject.name} has died.");

        if (CompareTag("Player"))
        {
            OnPlayerDeath?.Invoke();
        }
    }
}