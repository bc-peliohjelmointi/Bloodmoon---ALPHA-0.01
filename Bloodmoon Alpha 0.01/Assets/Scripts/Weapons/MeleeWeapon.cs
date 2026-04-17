using UnityEngine;
using UnityEngine.InputSystem;

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
    [SerializeField] private float thrustDistance = 0.3f;
    [SerializeField] private float swingSpeed = 10f;

    private float lastAttackTime = -Mathf.Infinity;
    private Quaternion originalRotation;
    private Vector3 originalPosition;
    private bool isSwinging;

    private PlayerInput input;
    private bool readytoshoot = true;

    private void Start()
    {
        input = GameObject.Find("Character").GetComponent<PlayerInput>();
        originalRotation = transform.localRotation;
        originalPosition = transform.localPosition;
    }

    private void Update()
    {
        if (input.actions.FindAction("Attack").IsPressed() && readytoshoot)
            TryAttack();
        else if (!input.actions.FindAction("Attack").IsPressed())
            readytoshoot = true;

        HandleSwingAnimation();
    }

    void TryAttack()
    {
        if (PauseMenu.IsPaused) return;
        if (Time.time < lastAttackTime + attackCooldown) return;

        lastAttackTime = Time.time;
        isSwinging = true;
        readytoshoot = false;

        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, range, hitMask))
        {
            IDamageable dmg = hit.collider.GetComponentInParent<IDamageable>();
            if (dmg != null)
            {
                Vector3 knockback = Camera.main.transform.forward * weaponItem.knockbackForce;
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
            Quaternion targetRotation = originalRotation * Quaternion.Euler(0f, 0f, -tiltAngle);
            Vector3 targetPosition = originalPosition + new Vector3(0f, 0f, thrustDistance);

            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * swingSpeed);
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * swingSpeed);

            if (Quaternion.Angle(transform.localRotation, targetRotation) < 1f)
                isSwinging = false;
        }
        else
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, originalRotation, Time.deltaTime * swingSpeed);
            transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition, Time.deltaTime * swingSpeed);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if (Camera.main != null)
            Gizmos.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * range);
    }
}