using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
    }

    protected override void Attack() {

    }

    protected override void Die() {
        if (effect != null)
        {
            vfx = Instantiate(effect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    protected override IEnumerator BehaviorLoop()
    {
        while (true)
        {

            if (!isAlive) yield break;
            if (health <= 0f) Explode();
            if (canSeePlayer)
            {
                agent.SetDestination(player.transform.position);

                if (inDetectionRange)
                {
                    Explode();
                }
            }
            else
            {
                Roam();
                yield return new WaitUntil(() =>
                    canSeePlayer ||
                    (agent.isOnNavMesh && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance));

                yield return new WaitForSeconds(Random.Range(idleWaitMin, idleWaitMax));
            }

            yield return null;
        }
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
                damageable.TakeDamage(explosionDamage, Vector3.zero);
            }
        }

        if (explosionEffect != null)
            Instantiate(explosionEffect, transform.position, Quaternion.identity);

        if (explosionSound != null)
            explosionSound.Play();

        Die();
    }
}