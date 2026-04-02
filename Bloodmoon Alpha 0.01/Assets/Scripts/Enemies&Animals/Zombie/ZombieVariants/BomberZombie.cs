using UnityEngine;

public class BomberZombie : Zombie
{
    [Header("Bomber Settings")]
    [SerializeField] private float explosionRadius = 3f;
    [SerializeField] private float explosionDamage = 30f;
    [SerializeField] private float explosionForce = 5f;
    [SerializeField] private ParticleSystem explosionEffect;
    [SerializeField] private AudioSource explosionSound;

    protected override void Update()
    {
        base.Update();

        if (isAttackAnimating && Time.time >= attackAnimationUntil)
        {
            Explode();
        }
    }
    /*
    protected override IEnumerator BehaviorLoop()
    {
        while (true)
        {
            if (!isAlive) yield break;

            if (canSeePlayer || inDetectionRange)
            {
                if (!isAttackAnimating && Time.time >= nextAttackTime)
                {
                    StartAttack();
                }
            }
            else
            {
                Roam();
            }

            yield return null;
        }
    }
    */

    private void StartAttack()
    {
        isAttackAnimating = true;
        attackAnimationUntil = Time.time + attackAnimationDuration;
        animator.SetTrigger("Attack");
    }

    private void Explode()
    {
        isAttackAnimating = false;
        nextAttackTime = Time.time + attackInterval;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (var hit in hitColliders)
        {
            if (hit.TryGetComponent(out IDamageable damageable))
            {
                Vector3 knockBackDir = (hit.transform.position - transform.position).normalized;
                damageable.TakeDamage(explosionDamage, knockBackDir * explosionForce);
            }
        }

        if (explosionEffect != null)
            Instantiate(explosionEffect, transform.position, Quaternion.identity);

        if (explosionSound != null)
            explosionSound.Play();

        Die();
    }
}