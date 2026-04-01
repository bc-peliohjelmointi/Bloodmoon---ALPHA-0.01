using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    [Header("Item Data")]
    [SerializeField] private Item weaponItem;

    [Header("Attack Settings")]
    [SerializeField] private float range = 2f;
    [SerializeField] private float attackCooldown = 0.5f;

    [Header("Hit Detection")]
    [SerializeField] private LayerMask hitMask;

    [Header("Swing Animation")]
    [SerializeField] private float tiltAngle = 35f;
    [SerializeField] private float swingSpeed = 10f;

    [Header("Attack Origin")]
    [SerializeField] private Transform attackPoint; // usually in front of weapon

    private float lastAttackTime = -Mathf.Infinity;
    private Quaternion originalRotation;
    private bool isSwinging;

    private void Start()
    {
        originalRotation = transform.localRotation;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            TryAttack();

        HandleSwingAnimation();
    }

    void TryAttack()
    {
        if (Time.time < lastAttackTime + attackCooldown)
            return;

        lastAttackTime = Time.time;
        isSwinging = true;

        // Raycast from attackPoint or weapon position
        Vector3 origin = attackPoint != null ? attackPoint.position : transform.position;
        Vector3 direction = transform.forward; // weapon’s forward

        if (Physics.Raycast(origin, direction, out RaycastHit hit, range, hitMask))
        {
            IDamageable dmg = hit.collider.GetComponentInParent<IDamageable>();
            if (dmg != null)
            {
                Vector3 knockback = direction * weaponItem.knockbackForce;
                dmg.TakeDamage(weaponItem.damage, knockback);
                Debug.Log($"Hit {dmg.name} for {weaponItem.damage} damage!");
            }
        }
        else
        {
            Debug.Log("Melee attack missed.");
        }
    }

    void HandleSwingAnimation()
    {
        if (isSwinging)
        {
            Quaternion targetRotation = originalRotation * Quaternion.Euler(-tiltAngle, 0f, 0f);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * swingSpeed);

            if (Quaternion.Angle(transform.localRotation, targetRotation) < 1f)
                isSwinging = false;
        }
        else
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, originalRotation, Time.deltaTime * swingSpeed);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 origin = attackPoint != null ? attackPoint.position : transform.position;
        Gizmos.color = Color.red;
        Gizmos.DrawRay(origin, transform.forward * range);
    }
}