using UnityEngine;

public class RocketLauncher : MonoBehaviour
{
    [Header("Item Data")]
    [SerializeField] private Item rocketItemData;

    [Header("References")]
    [SerializeField] private Transform firePoint;

    [Header("Settings")]
    [SerializeField] private float shootCooldown = 1f;

    private float lastShotTime = -Mathf.Infinity;

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

        if (rocketItemData == null || rocketItemData.projectilePrefab == null)
        {
            Debug.LogWarning("Rocket or projectile missing!");
            return;
        }

        lastShotTime = Time.time;
        Shoot();
    }

    void Shoot()
    {
        GameObject rocket = Instantiate(
            rocketItemData.projectilePrefab,
            firePoint.position,
            Quaternion.LookRotation(Camera.main.transform.forward)
        );

        RocketProjectile proj = rocket.GetComponent<RocketProjectile>();

        if (proj != null)
        {
            proj.Launch(
                Camera.main.transform.forward,
                rocketItemData.damage,
                rocketItemData.knockbackForce
            );
        }
    }
}