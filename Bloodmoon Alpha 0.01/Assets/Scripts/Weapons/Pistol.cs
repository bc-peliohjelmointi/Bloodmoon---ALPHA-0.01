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
        // 🔥 Shoot from camera center
        Ray ray = new Ray(
            Camera.main.transform.position,
            Camera.main.transform.forward
        );

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, range, hitMask))
        {
            if (hit.collider.CompareTag("Enemy") ||
                hit.collider.gameObject.layer == LayerMask.NameToLayer("Entitys"))
            {
                Destroy(hit.transform.root.gameObject);
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