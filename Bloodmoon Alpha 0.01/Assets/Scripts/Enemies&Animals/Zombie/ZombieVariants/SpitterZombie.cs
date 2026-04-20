using System.Collections;
using UnityEngine;

public class SpitterZombie : Zombie
{
    [Header("Spit Attack")]
    [SerializeField] private GameObject spitPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float spitDamage = 12f;
    [SerializeField] private float spitKnockback = 0.2f;
    [SerializeField] private float spitSpeed = 12f;
    [SerializeField] private float spitArc = 0.2f;
    [SerializeField] private float spitInterval = 2.5f;

    [Header("Range Keeping")]
    [SerializeField] private float minRange = 5f;
    [SerializeField] private float maxRange = 12f;

    private float nextSpitTime;

    protected override IEnumerator AggressiveState()
    {
        while (IsHordeAlerted())
        {
            if (player == null) { yield return null; continue; }

            float dist = Vector3.Distance(transform.position, player.transform.position);

            if (dist < minRange)
            {
                Vector3 away = (transform.position - player.transform.position).normalized;
                TrySetDestination(transform.position + away * (minRange - dist + 1f), 2f);
            }
            else if (dist > maxRange)
            {
                if (agent != null && agent.isOnNavMesh) agent.SetDestination(player.transform.position);
            }
            else
            {
                if (agent != null && agent.isOnNavMesh && agent.hasPath) agent.ResetPath();
                FacePlayer();
                TrySpit();
            }

            isAttackAnimating = Time.time <= attackAnimationUntil;
            yield return null;
        }
    }

    protected override void Attack() { }

    private void FacePlayer()
    {
        Vector3 dir = player.transform.position - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.001f) return;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 6f * Time.deltaTime);
    }

    private void TrySpit()
    {
        if (Time.time < nextSpitTime) return;
        if (spitPrefab == null) return;

        Transform spawn = firePoint != null ? firePoint : transform;
        Vector3 toPlayer = (player.transform.position + Vector3.up * 0.5f) - spawn.position;
        if (toPlayer.sqrMagnitude < 0.001f) return;

        Vector3 aim = (toPlayer.normalized + Vector3.up * spitArc).normalized;

        GameObject proj = Instantiate(spitPrefab, spawn.position, Quaternion.LookRotation(aim));
        IgnoreOwnerCollisions(proj);
        ArrowProjectile arrow = proj.GetComponent<ArrowProjectile>();
        if (arrow != null)
        {
            arrow.speed = spitSpeed;
            arrow.Launch(aim, spitDamage, spitKnockback);
        }

        attackAnimationUntil = Time.time + attackAnimationDuration;
        isAttackAnimating = true;
        nextSpitTime = Time.time + spitInterval;
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
