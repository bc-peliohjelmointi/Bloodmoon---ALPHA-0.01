using System.Collections;
using UnityEngine;

public class GargoyleZombie : Zombie
{
    [Header("Flight")]
    [SerializeField] private float flySpeed = 5f;
    [SerializeField] private float hoverHeight = 6f;
    [SerializeField] private float circleRadius = 8f;
    [SerializeField] private float orbitSpeed = 40f;
    [SerializeField] private float bobAmplitude = 0.4f;
    [SerializeField] private float bobFrequency = 1.5f;
    [SerializeField] private LayerMask groundMask = ~0;

    [Header("Dive Attack")]
    [SerializeField] private float diveSpeed = 14f;
    [SerializeField] private float diveCooldown = 5f;
    [SerializeField] private float diveTriggerDistance = 12f;
    [SerializeField] private float diveHitDistance = 1.6f;
    [SerializeField] private float diveDamage = 15f;
    [SerializeField] private float climbAfterDiveTime = 0.7f;

    private float nextDiveTime;
    private float bobOffset;
    private float orbitAngle;
    private int orbitDir = 1;

    protected override void OnEnable()
    {
        base.OnEnable();
        if (agent != null) agent.enabled = false;
        bobOffset = Random.value * Mathf.PI * 2f;
        orbitAngle = Random.value * 360f;
        orbitDir = (Random.value < 0.5f) ? -1 : 1;
    }

    protected override IEnumerator BehaviorLoop()
    {
        while (true)
        {
            if (!isAlive) yield break;
            if (player == null) { yield return null; continue; }

            bool aware = canSeePlayer || inDetectionRange || IsHordeAlerted();

            if (aware && Vector3.Distance(transform.position, player.transform.position) <= diveTriggerDistance && Time.time >= nextDiveTime)
            {
                yield return StartCoroutine(Dive());
                nextDiveTime = Time.time + diveCooldown;
            }
            else if (aware)
            {
                FlyToward(player.transform.position);
            }
            else
            {
                Hover(transform.position);
            }

            yield return null;
        }
    }

    protected override void Attack() { }

    private void FlyToward(Vector3 target)
    {
        orbitAngle += orbitSpeed * orbitDir * Time.deltaTime;
        float rad = orbitAngle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad)) * circleRadius;
        Vector3 desired = HoverAbove(target + offset);
        transform.position = Vector3.MoveTowards(transform.position, desired, flySpeed * Time.deltaTime);
        FacePlanar(target);
    }

    private void Hover(Vector3 anchor)
    {
        Vector3 desired = HoverAbove(anchor);
        transform.position = Vector3.MoveTowards(transform.position, desired, flySpeed * 0.5f * Time.deltaTime);
    }

    private Vector3 HoverAbove(Vector3 reference)
    {
        float groundY = reference.y;
        if (Physics.Raycast(reference + Vector3.up * 50f, Vector3.down, out RaycastHit hit, 100f, groundMask, QueryTriggerInteraction.Ignore))
            groundY = hit.point.y;

        float bob = Mathf.Sin(Time.time * bobFrequency + bobOffset) * bobAmplitude;
        return new Vector3(reference.x, groundY + hoverHeight + bob, reference.z);
    }

    private void FacePlanar(Vector3 target)
    {
        Vector3 dir = target - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 5f * Time.deltaTime);
    }

    private IEnumerator Dive()
    {
        isAttackAnimating = true;
        attackAnimationUntil = Time.time + attackAnimationDuration;

        Vector3 diveTarget = player.transform.position;

        while (isAlive && Vector3.Distance(transform.position, diveTarget) > diveHitDistance)
        {
            transform.position = Vector3.MoveTowards(transform.position, diveTarget, diveSpeed * Time.deltaTime);

            Vector3 dir = diveTarget - transform.position;
            if (dir.sqrMagnitude > 0.001f)
                transform.forward = dir.normalized;

            yield return null;
        }

        if (isAlive && player != null && Vector3.Distance(transform.position, player.transform.position) <= diveHitDistance + 0.5f)
        {
            Vector3 knock = (player.transform.position - transform.position).normalized * attackKnockback;
            DealDamage(diveDamage, player, knock);
        }

        float t = 0f;
        while (t < climbAfterDiveTime && isAlive)
        {
            FlyToward(player != null ? player.transform.position : transform.position);
            t += Time.deltaTime;
            yield return null;
        }

        isAttackAnimating = false;
    }
}
