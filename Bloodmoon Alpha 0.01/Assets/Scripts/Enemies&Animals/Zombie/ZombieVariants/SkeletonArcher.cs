using System.Collections;
using UnityEngine;

public class SkeletonArcher : Zombie
{
    [Header("Archery")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float arrowDamage = 15f;
    [SerializeField] private float arrowKnockback = 0.3f;
    [SerializeField] private float arrowSpeed = 25f;
    [SerializeField] private float shotInterval = 3f;

    [Header("Range Keeping")]
    [SerializeField] private float minRange = 4f;
    [SerializeField] private float preferredRange = 10f;
    [SerializeField] private float maxRange = 18f;
    [SerializeField] private LayerMask sightObstacles = ~0;

    private float nextShotTime;

    protected override IEnumerator AggressiveState()
    {
        while (IsHordeAlerted())
        {
            if (player == null) { yield return null; continue; }

            float dist = Vector3.Distance(transform.position, player.transform.position);
            bool hasShot = HasLineOfSight();

            if (dist < minRange)
            {
                Vector3 away = (transform.position - player.transform.position).normalized;
                TrySetDestination(transform.position + away * (preferredRange - dist), 2f);
            }
            else if (dist > maxRange || !hasShot)
            {
                if (agent != null && agent.isOnNavMesh) agent.SetDestination(player.transform.position);
            }
            else
            {
                if (agent != null && agent.isOnNavMesh && agent.hasPath) agent.ResetPath();
                FacePlayer();
                TryShoot();
            }

            isAttackAnimating = Time.time <= attackAnimationUntil;
            yield return null;
        }
    }

    protected override void Attack() { }

    private bool HasLineOfSight()
    {
        Transform spawn = firePoint != null ? firePoint : transform;
        Vector3 origin = spawn.position;
        Vector3 dir = (player.transform.position + Vector3.up * 0.5f) - origin;
        float dist = dir.magnitude;
        if (dist < 0.001f) return true;

        if (Physics.Raycast(origin, dir.normalized, out RaycastHit hit, dist, sightObstacles, QueryTriggerInteraction.Ignore))
            return hit.collider.gameObject == player || hit.collider.transform.IsChildOf(player.transform);

        return true;
    }

    private void FacePlayer()
    {
        Vector3 dir = player.transform.position - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.001f) return;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 6f * Time.deltaTime);
    }

    private void TryShoot()
    {
        if (Time.time < nextShotTime) return;
        if (arrowPrefab == null) return;

        Transform spawn = firePoint != null ? firePoint : transform;
        Vector3 dir = (player.transform.position + Vector3.up * 0.5f) - spawn.position;
        if (dir.sqrMagnitude < 0.001f) return;
        dir.Normalize();

        GameObject proj = Instantiate(arrowPrefab, spawn.position, Quaternion.LookRotation(dir));
        IgnoreOwnerCollisions(proj);
        ArrowProjectile arrow = proj.GetComponent<ArrowProjectile>();
        if (arrow != null)
        {
            arrow.speed = arrowSpeed;
            arrow.Launch(dir, arrowDamage, arrowKnockback);
        }

        attackAnimationUntil = Time.time + attackAnimationDuration;
        isAttackAnimating = true;
        nextShotTime = Time.time + shotInterval;
    }

    private void IgnoreOwnerCollisions(GameObject projectile)
    {
        Collider[] projCols = projectile.GetComponentsInChildren<Collider>();
        Collider[] ownerCols = GetComponentsInChildren<Collider>();
        for (int i = 0; i < projCols.Length; i++)
        {
            for (int j = 0; j < ownerCols.Length; j++)
            {
                if (projCols[i] != null && ownerCols[j] != null)
                    Physics.IgnoreCollision(projCols[i], ownerCols[j], true);
            }
        }
    }
}
