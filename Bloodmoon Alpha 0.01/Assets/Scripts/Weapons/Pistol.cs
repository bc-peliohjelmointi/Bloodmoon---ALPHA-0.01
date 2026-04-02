using UnityEngine;

public class Pistol : MonoBehaviour
{
    [Header("Item Data")]
    [SerializeField] private Item pistolItem;

    [Header("References")]
    [SerializeField] private Transform firePoint;

    [Header("Settings")]
    [SerializeField] private float fireCooldown = 0.15f;
    [SerializeField] private float range = 100f;
    [SerializeField] private LayerMask hitMask;

    [Header("Fire Mode")]
    [SerializeField] private bool autoFire = false;

    private float lastShotTime;

    private void Update()
    {
        if (autoFire)
        {
            if (Input.GetMouseButton(0))
                TryShoot();
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
                TryShoot();
        }
    }

    void TryShoot()
    {
        if (Time.time < lastShotTime + fireCooldown)
            return;

        if (!ConsumeAmmo())
        {
            Debug.Log("No bullets!");
            return;
        }

        lastShotTime = Time.time;
        Shoot();
    }

    void Shoot()
    {
        Ray ray = new Ray(
            Camera.main.transform.position,
            Camera.main.transform.forward
        );

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, range, hitMask))
        {
            IDamageable dmg = hit.collider.GetComponentInParent<IDamageable>();

            if (dmg != null)
            {
                Vector3 knockback = Camera.main.transform.forward * pistolItem.knockbackForce;
                dmg.TakeDamage(pistolItem.damage, knockback);
            }
        }
    }

    bool ConsumeAmmo()
    {
        if (pistolItem == null || pistolItem.ammoType == null)
            return false;

        return Inventory.Singleton.ConsumeItem(pistolItem.ammoType, 1);
    }
}