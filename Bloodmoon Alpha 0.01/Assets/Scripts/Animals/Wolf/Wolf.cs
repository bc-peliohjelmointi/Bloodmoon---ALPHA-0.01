using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Wolf : AnimalNpc
{
    [Header("Pack Behaviour")]
    [SerializeField] private float packSenseRadius = 20f;
    [SerializeField] private float packCohesionRadius = 8f;
    [SerializeField] private float packSpacing = 2.5f;
    [SerializeField] private float packAlertDuration = 4f;
    [SerializeField] private float alertAnimationDuration = 1.1f;
    [SerializeField] private float attackFaceAngle = 15f;
    [SerializeField] private float attackTurnSpeed = 240f;
    [SerializeField] private float attackInterval = 5f;
    [SerializeField] private float attackAnimationDuration = 1.8f;

    private static readonly List<Wolf> ActiveWolves = new List<Wolf>();

    private Coroutine routine;
    private float packAlertUntil;
    private float alertAnimationUntil;
    private float attackAnimationUntil;
    private float nextAttackTime;
    private bool isAttackAnimating;

    protected override void OnEnable()
    {
        base.OnEnable();

        if (!ActiveWolves.Contains(this))
            ActiveWolves.Add(this);

        routine = StartCoroutine(WolfCoroutine());
    }

    private void OnDisable()
    {
        if (routine != null) StopCoroutine(routine);
        routine = null;

        ActiveWolves.Remove(this);
    }

    protected override void Update()
    {
        base.Update();

        if (canSeePlayer || inDetectionRange)
        {
            bool wasPackAlerted = IsPackAlerted();
            AlertPack();

            if (!wasPackAlerted)
                alertAnimationUntil = Time.time + alertAnimationDuration;
        }

        UpdateWolfAnimator();
    }

    private IEnumerator WolfCoroutine()
    {
        while (true)
        {
            PackRoam();

            yield return new WaitUntil(() =>
                canSeePlayer || inDetectionRange || IsPackAlerted() ||
                (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance));

            if (canSeePlayer || inDetectionRange || IsPackAlerted())
            {
                while (canSeePlayer || inDetectionRange || IsPackAlerted())
                {
                    Attack();
                    yield return null;
                }
            }
            else
            {
                float wait = Random.Range(2f, 5f);
                float t = 0f;

                while (t < wait && !canSeePlayer && !inDetectionRange)
                {
                    t += Time.deltaTime;
                    yield return null;
                }
            }
        }
    }

    private void PackRoam()
    {
        if (agent == null || !agent.isOnNavMesh)
            return;

        Vector3 center = Vector3.zero;
        int count = 0;
        Wolf nearest = null;
        float nearestDist = float.MaxValue;

        for (int i = 0; i < ActiveWolves.Count; i++)
        {
            Wolf other = ActiveWolves[i];
            if (other == null || other == this)
                continue;

            float dist = Vector3.Distance(transform.position, other.transform.position);
            if (dist > packSenseRadius)
                continue;

            center += other.transform.position;
            count++;

            if (dist < nearestDist)
            {
                nearestDist = dist;
                nearest = other;
            }
        }

        if (count == 0)
        {
            Roam();
            return;
        }

        center /= count;

        if (Vector3.Distance(transform.position, center) > packCohesionRadius)
        {
            TrySetDestinationOnNavMesh(center, 2f);
            return;
        }

        if (nearest != null && nearestDist < packSpacing)
        {
            Vector3 away = transform.position - nearest.transform.position;
            away.y = 0f;

            if (away.sqrMagnitude > 0.001f)
            {
                Vector3 side = Vector3.Cross(Vector3.up, away.normalized);
                float sideSign = ((GetInstanceID() & 1) == 0) ? 1f : -1f;
                Vector3 target = transform.position + away.normalized * packSpacing + side * (packSpacing * 0.5f * sideSign);
                if (TrySetDestinationOnNavMesh(target, 2f))
                    return;

                if (TrySetDestinationOnNavMesh(center, 2f))
                    return;
            }
        }

        Roam();
    }

    public override void Attack()
    {
        if (agent == null || player == null || !agent.isOnNavMesh)
        {
            isAttackAnimating = false;
            return;
        }

        Vector3 target = GetPackAttackTarget();
        TrySetDestinationOnNavMesh(target, 2f);

        float stopDistance = Mathf.Max(agent.stoppingDistance, packSpacing * 0.75f);
        bool isPlayingAlertAnimation = IsPlayingAlertAnimation();
        bool inAttackRange = !agent.pathPending && agent.remainingDistance <= stopDistance;
        bool isFacingPlayer = true;

        if (inAttackRange)
            isFacingPlayer = TurnTowardsPlayer();

        bool canAttackNow = !isPlayingAlertAnimation && inAttackRange && isFacingPlayer;

        bool startedAttackThisFrame = false;

        if (!canAttackNow)
        {
            isAttackAnimating = false;
        }
        else
        {
            if (Time.time >= nextAttackTime)
            {
                attackAnimationUntil = Time.time + attackAnimationDuration;
                nextAttackTime = Time.time + attackInterval;
                startedAttackThisFrame = true;
            }

            isAttackAnimating = Time.time <= attackAnimationUntil;
        }

        if (startedAttackThisFrame)
        {
            if (debug) Debug.Log($"{gameObject.name} attacks the player!");
            DealDamage(damage, player);
        }
    }

    private void AlertPack()
    {
        float until = Time.time + packAlertDuration;
        packAlertUntil = Mathf.Max(packAlertUntil, until);

        for (int i = 0; i < ActiveWolves.Count; i++)
        {
            Wolf other = ActiveWolves[i];
            if (other == null || other == this)
                continue;

            if (Vector3.Distance(transform.position, other.transform.position) <= packSenseRadius)
                other.ReceivePackAlert(until);
        }
    }

    private void ReceivePackAlert(float until)
    {
        packAlertUntil = Mathf.Max(packAlertUntil, until);
    }

    private bool IsPackAlerted()
    {
        return Time.time <= packAlertUntil;
    }

    private Vector3 GetPackAttackTarget()
    {
        Vector3 playerPos = player.transform.position;
        float radius = Mathf.Max(1.25f, packSpacing);

        int slot = 0;
        int packCount = 1;

        for (int i = 0; i < ActiveWolves.Count; i++)
        {
            Wolf other = ActiveWolves[i];
            if (other == null || other == this)
                continue;

            if (Vector3.Distance(transform.position, other.transform.position) > packSenseRadius)
                continue;

            if (other.GetInstanceID() < GetInstanceID())
                slot++;

            packCount++;
        }

        float angleStep = 360f / Mathf.Max(packCount, 1);
        float angle = slot * angleStep;
        Vector3 offset = Quaternion.Euler(0f, angle, 0f) * Vector3.forward * radius;

        return playerPos + offset;
    }

    private bool TrySetDestinationOnNavMesh(Vector3 target, float sampleRadius)
    {
        if (agent == null || !agent.isOnNavMesh)
            return false;

        if (NavMesh.SamplePosition(target, out NavMeshHit hit, sampleRadius, NavMesh.AllAreas))
            return agent.SetDestination(hit.position);

        return false;
    }

    private bool TurnTowardsPlayer()
    {
        Vector3 toPlayer = player.transform.position - transform.position;
        toPlayer.y = 0f;

        if (toPlayer.sqrMagnitude <= 0.0001f)
            return true;

        Quaternion targetRotation = Quaternion.LookRotation(toPlayer.normalized, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            attackTurnSpeed * Time.deltaTime);

        float angleToPlayer = Quaternion.Angle(transform.rotation, targetRotation);
        return angleToPlayer <= attackFaceAngle;
    }

    private void UpdateWolfAnimator()
    {
        bool isAlerted = canSeePlayer || inDetectionRange || IsPackAlerted();
        if (!isAlerted)
        {
            isAttackAnimating = false;
            alertAnimationUntil = 0f;
            attackAnimationUntil = 0f;
            nextAttackTime = 0f;
        }

        bool isMoving = agent != null && agent.velocity.sqrMagnitude > 0.01f;
        bool idle = !isAlerted && !isAttackAnimating && !isMoving;
        bool playAlertAnimation = IsPlayingAlertAnimation();

        animator.SetBool("alert", playAlertAnimation);
        animator.SetBool("Attack", isAttackAnimating);
        animator.SetBool("Idlling", idle);
    }

    private bool IsPlayingAlertAnimation()
    {
        return Time.time <= alertAnimationUntil;
    }
}
