using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(NavMeshAgent))]
public class AnimalNpc : IDamageable
{
    [Header("References")]
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected GameObject player;
    [SerializeField] protected Transform raycastPoint;
    [SerializeField] protected Animator animator;

    [Header("Basic Stats")]
    [SerializeField] protected float roamingRange = 10f;
    [SerializeField] protected float viewDistance = 15f;
    [SerializeField] protected float fov = 60f;
    [SerializeField] protected float detectionRange = 5f;
    [SerializeField] protected int damage = 10;

    [Header("Effects")]
    [SerializeField] protected ParticleSystem effect;

    [Header("Debug Options")]
    [SerializeField] protected bool debug = true;

    protected bool canSeePlayer;
    protected bool inDetectionRange;

    protected bool isAlive = true;

    protected ParticleSystem vfx = null;

    protected virtual void OnEnable()
    {
        if (player == null) player = GameObject.Find("Character");
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (animator == null) animator = GetComponentInChildren<Animator>();

        if (agent == null) Debug.LogError("NavMeshAgent component is missing on " + gameObject.name);
    }

    protected virtual void Update()
    {
        if (!isAlive) return;

        canSeePlayer = CanSeePlayerCheck();
        inDetectionRange = InRange();
        UpdateBaseAnimator();

        if (Keyboard.current.tKey.wasPressedThisFrame) TakeDamage(10);

        if (health <= 0f && isAlive)
        {
            isAlive = false;
            Die();
        }
    }

    public void Roam()
    {
        if (!isAlive) return;
        agent.SetDestination(RandomNavMeshPoint(transform.position, roamingRange));
    }

    protected virtual bool CanSeePlayerCheck()
    {
        if (player == null || raycastPoint == null) return false;

        Vector3 toPlayer = player.transform.position - raycastPoint.position;
        float dist = toPlayer.magnitude;

        if (debug)
            Debug.DrawRay(raycastPoint.position, raycastPoint.forward * viewDistance, Color.red);

        if (dist <= 0.001f) return true;
        if (dist > viewDistance) return false;

        Vector3 dir = toPlayer / dist;
        float angle = Vector3.Angle(raycastPoint.forward, dir);
        if (angle > fov * 0.5f) return false;

        if (Physics.Raycast(raycastPoint.position, dir, out RaycastHit hit, viewDistance))
            return hit.collider.gameObject == player;

        return false;
    }

    protected virtual bool InRange()
    {
        if (player == null) return false;
        return Vector3.Distance(transform.position, player.transform.position) <= detectionRange;
    }

    public virtual void Attack()
    {
        if (!isAlive || agent == null || player == null) return;

        agent.SetDestination(player.transform.position);

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (debug) Debug.Log($"{gameObject.name} attacks the player!");
            DealDamage(damage, player);
        }
    }

    private Vector3 RandomNavMeshPoint(Vector3 origin, float range)
    {
        for (int i = 0; i < 20; i++)
        {
            Vector3 randomPoint = origin + Random.insideUnitSphere * range;
            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
                return hit.position;
        }
        return origin;
    }

    protected virtual void UpdateBaseAnimator()
    {
        if (animator == null) return;

        float speed = (agent != null) ? agent.velocity.magnitude : 0f;
        animator.SetFloat("Speed", speed);
        animator.SetBool("Alive", isAlive);
        animator.SetBool("Sit", false);
    }

    protected override void Die()
    {
        agent.isStopped = true;
        agent.ResetPath();


        animator.SetBool("Alive", false);

        if (debug) Debug.Log($"{gameObject.name} died.");

        vfx = Instantiate(effect, transform.position, Quaternion.identity);
        vfx.Stop();
        Destroy(gameObject, vfx.main.duration - 0.5f);
    }

    private void OnDestroy()
    {
        vfx.Play();
        Destroy(vfx.gameObject, vfx.main.duration + vfx.main.startLifetime.constantMax + 0.5f);
    }
}