using UnityEngine;

public class Bow : MonoBehaviour
{
    [Header("Item Data")]
    [SerializeField] private Item bowItemData;   // Bow ScriptableObject

    [Header("References")]
    [SerializeField] private Transform firePoint;

    [Header("Settings")]
    [SerializeField] private float shootForce = 20f;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            Shoot();
    }

    public void Shoot()
    {
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

        GameObject arrow = Instantiate(
            bowItemData.ammoType.projectilePrefab,
            firePoint.position,
            firePoint.rotation
        );

        ArrowProjectile projectile = arrow.GetComponent<ArrowProjectile>();
        if (projectile != null)
            projectile.Launch(firePoint.forward);
    }

    bool ConsumeAmmo()
    {
        return Inventory.Singleton.ConsumeItem(bowItemData.ammoType, 1);
    }
}