using UnityEngine;
using System;

public class IDamageable : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] protected float health = 50f;
    [SerializeField] protected float maxHealth = 50f;
    [SerializeField] protected bool canTakeKnockback = true;

    [Header("Effects")]
    [SerializeField] protected ParticleSystem bloodEffect;

    public static Action OnPlayerDeath;

    public float Health => health;
    public float MaxHealth => maxHealth;
    public bool IsDead => health <= 0f;

    protected virtual void DealDamage(float dmg, GameObject target, Vector3 knockBack)
    {
        if (target.TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage(dmg, knockBack);
        }
    }

    protected virtual void TakeDamage(float dmg, Vector3 knockBack)
    {
        if (IsDead) return;

        health = Mathf.Clamp(health - dmg, 0f, maxHealth);

        if (canTakeKnockback)
            transform.position += knockBack;

        if (bloodEffect != null)
            Instantiate(bloodEffect, transform.position, Quaternion.identity);

        if (health <= 0f)
            Die();
    }

    public virtual void Heal(float amount)
    {
        if (IsDead) return;
        health = Mathf.Clamp(health + amount, 0f, maxHealth);
    }

    protected virtual void Die()
    {
        if (CompareTag("Player"))
        {
            OnPlayerDeath?.Invoke();
        }
    }
}