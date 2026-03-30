using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float range = 1.5f;
    [SerializeField] private float attackCooldown = 0.5f;

    [Header("Hit Detection")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask hitMask;

    [Header("Swing Animation")]
    [SerializeField] private float tiltAngle = 35f;
    [SerializeField] private float swingSpeed = 10f;

    private float lastAttackTime;
    private Quaternion originalRotation;
    private bool isSwinging;

    private void Start()
    {
        originalRotation = transform.localRotation;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryAttack();
        }

        HandleSwingAnimation();
    }

    void TryAttack()
    {
        if (Time.time < lastAttackTime + attackCooldown)
            return;

        lastAttackTime = Time.time;
        Attack();
        isSwinging = true;
    }

    void Attack()
    {
        Vector3 center = attackPoint != null ? attackPoint.position : transform.position;

        Collider[] hits = Physics.OverlapSphere(center, range, hitMask);

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Enemy") ||
                hit.gameObject.layer == LayerMask.NameToLayer("Entitys"))
            {
                Destroy(hit.transform.root.gameObject);
            }
        }
    }

    void HandleSwingAnimation()
    {
        if (isSwinging)
        {
            Quaternion targetRotation =
                originalRotation * Quaternion.Euler(-tiltAngle, 0f, 0f);

            transform.localRotation = Quaternion.Slerp(
                transform.localRotation,
                targetRotation,
                Time.deltaTime * swingSpeed
            );

            if (Quaternion.Angle(transform.localRotation, targetRotation) < 1f)
            {
                isSwinging = false;
            }
        }
        else
        {
            transform.localRotation = Quaternion.Slerp(
                transform.localRotation,
                originalRotation,
                Time.deltaTime * swingSpeed
            );
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, range);
    }
}