using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : AnimalNpc
{
    [Header("Horde Behaviour")]
    [SerializeField] protected float hordeAlertRadius = 20f;
    [SerializeField] protected float hordeAlertDuration = 6f;
    [SerializeField] protected int minHordeSize = 3;

    [Header("Attack Settings")]
    [SerializeField] protected float attackInterval = 2f;
    [SerializeField] protected float attackAnimationDuration = 1.2f;
    [SerializeField] protected float attackKnockback = 0.1f;

    [Header("Roaming")]
    [SerializeField] protected float idleWaitMin = 2f;
    [SerializeField] protected float idleWaitMax = 5f;

    private static readonly List<Zombie> ActiveZombies = new List<Zombie>();

    protected Coroutine routine;
    protected float hordeAlertUntil;
    protected float attackAnimationUntil;
    protected float nextAttackTime;
    protected bool isAttackAnimating;

    protected override void OnEnable()
    {
        base.OnEnable();

        if (!ActiveZombies.Contains(this))
            ActiveZombies.Add(this);

        routine = StartCoroutine(BehaviorLoop());
    }

    protected virtual void OnDisable()
    {
        if (routine != null) StopCoroutine(routine);
        routine = null;

        ActiveZombies.Remove(this);
    }

    protected override void Update()
    {
        base.Update();

        if (canSeePlayer || inDetectionRange)
            AlertHorde();

        UpdateZombieAnimator();
    }

    protected virtual IEnumerator BehaviorLoop()
    {
        while (true)
        {
            Roam();

            yield return new WaitUntil(() =>
                IsHordeAlerted() ||
                (agent.isOnNavMesh && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance));

            if (IsHordeAlerted())
            {
                yield return StartCoroutine(AggressiveState());
            }
            else
            {
                yield return StartCoroutine(IdleState());
            }
        }
    }

    protected virtual IEnumerator AggressiveState()
    {
        while (IsHordeAlerted())
        {
            if (HordeNearbyCount() >= minHordeSize)
                Attack();
            else if (player != null)
                agent.SetDestination(player.transform.position);

            yield return null;
        }
    }

    protected virtual IEnumerator IdleState()
    {
        float wait = Random.Range(idleWaitMin, idleWaitMax);
        float t = 0f;

        while (t < wait && !IsHordeAlerted())
        {
            t += Time.deltaTime;
            yield return null;
        }
    }

    public override void Attack()
    {
        if (agent == null || player == null || !agent.isOnNavMesh)
        {
            isAttackAnimating = false;
            return;
        }

        agent.SetDestination(player.transform.position);

        bool inAttackRange = !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance;

        if (!inAttackRange)
        {
            isAttackAnimating = false;
            return;
        }

        bool startedAttackThisFrame = false;

        if (Time.time >= nextAttackTime)
        {
            attackAnimationUntil = Time.time + attackAnimationDuration;
            nextAttackTime = Time.time + attackInterval;
            startedAttackThisFrame = true;
        }

        isAttackAnimating = Time.time <= attackAnimationUntil;

        if (startedAttackThisFrame)
        {
            if (debug) Debug.Log($"{gameObject.name} attacks the player!");
            DealDamage(damage, player, transform.forward * attackKnockback);
        }
    }

    protected virtual void AlertHorde()
    {
        float until = Time.time + hordeAlertDuration;
        hordeAlertUntil = Mathf.Max(hordeAlertUntil, until);

        for (int i = 0; i < ActiveZombies.Count; i++)
        {
            Zombie other = ActiveZombies[i];
            if (other == null || other == this) continue;

            if (Vector3.Distance(transform.position, other.transform.position) <= hordeAlertRadius)
                other.ReceiveHordeAlert(until);
        }
    }

    protected void ReceiveHordeAlert(float until)
    {
        hordeAlertUntil = Mathf.Max(hordeAlertUntil, until);
    }

    protected bool IsHordeAlerted()
    {
        return Time.time <= hordeAlertUntil;
    }

    protected int HordeNearbyCount()
    {
        int count = 1;

        for (int i = 0; i < ActiveZombies.Count; i++)
        {
            Zombie other = ActiveZombies[i];
            if (other == null || other == this) continue;

            if (Vector3.Distance(transform.position, other.transform.position) <= hordeAlertRadius)
                count++;
        }

        return count;
    }

    protected virtual void UpdateZombieAnimator()
    {
        if (animator == null) return;

        bool isAlerted = IsHordeAlerted();

        if (!isAlerted)
        {
            isAttackAnimating = false;
            attackAnimationUntil = 0f;
            nextAttackTime = 0f;
        }

        animator.SetBool("Attack", isAttackAnimating);
    }
}
