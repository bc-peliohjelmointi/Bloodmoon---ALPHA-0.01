using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : AnimalNpc
{
    [Header("Horde Behaviour")]
    [SerializeField] private float hordeAlertRadius = 20f;
    [SerializeField] private float hordeAlertDuration = 6f;
    [SerializeField] private int minHordeSize = 3;
    [SerializeField] private float attackInterval = 2f;
    [SerializeField] private float attackAnimationDuration = 1.2f;

    private static readonly List<Zombie> ActiveZombies = new List<Zombie>();

    private Coroutine routine;
    private float hordeAlertUntil;
    private float attackAnimationUntil;
    private float nextAttackTime;
    private bool isAttackAnimating;

    protected override void OnEnable()
    {
        base.OnEnable();

        if (!ActiveZombies.Contains(this))
            ActiveZombies.Add(this);

        routine = StartCoroutine(ZombieCoroutine());
    }

    private void OnDisable()
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

    private IEnumerator ZombieCoroutine()
    {
        while (true)
        {
            Roam();

            yield return new WaitUntil(() =>
                IsHordeAlerted() ||
                (agent.isOnNavMesh && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance));

            if (IsHordeAlerted())
            {
                while (IsHordeAlerted())
                {
                    if (HordeNearbyCount() >= minHordeSize)
                        Attack();
                    else
                        agent.SetDestination(player.transform.position);

                    yield return null;
                }
            }
            else
            {
                float wait = Random.Range(2f, 5f);
                float t = 0f;

                while (t < wait && !IsHordeAlerted())
                {
                    t += Time.deltaTime;
                    yield return null;
                }
            }
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
            DealDamage(damage, player, transform.forward / 10);
        }
    }

    private void AlertHorde()
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

    private void ReceiveHordeAlert(float until)
    {
        hordeAlertUntil = Mathf.Max(hordeAlertUntil, until);
    }

    private bool IsHordeAlerted()
    {
        return Time.time <= hordeAlertUntil;
    }

    private int HordeNearbyCount()
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

    private void UpdateZombieAnimator()
    {
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
