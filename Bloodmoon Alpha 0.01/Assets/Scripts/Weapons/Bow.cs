using UnityEngine;

public class Bow : MonoBehaviour
{
    [Header("Item Data")]
    [SerializeField] private Item bowItemData;

    [Header("References")]
    [SerializeField] private Transform firePoint;

    [Header("Settings")]
    [SerializeField] private float shootForce = 20f;
    [SerializeField] private float shootCooldown = 0.8f;

    private float lastShotTime;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryShoot();
        }
    }

    void TryShoot()
    {
        if (Time.time < lastShotTime + shootCooldown)
            return;

        if (bowItemData == null || bowItemData.ammoType == null)
        {
            Debug.LogWarning("Bow or ammo missing!");
            return;
        }

        if (!ConsumeAmmo())
        {
            Debug.Log("No ammo!");
            return;
        }

        lastShotTime = Time.time;
        Shoot();
    }

    void Shoot()
    {
        GameObject arrow = Instantiate(
            bowItemData.ammoType.projectilePrefab,
            firePoint.position,
            firePoint.rotation
        );

        ArrowProjectile projectile = arrow.GetComponent<ArrowProjectile>();

        if (projectile != null)
        {
            // 🔥 Use camera direction instead of weapon forward
            projectile.Launch(Camera.main.transform.forward);
        }
    }

    bool ConsumeAmmo()
    {
        return Inventory.Singleton.ConsumeItem(bowItemData.ammoType, 1);
    }
}