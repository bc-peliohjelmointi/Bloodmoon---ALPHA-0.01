using UnityEngine;
using UnityEngine.InputSystem;

public class RocketLauncher : MonoBehaviour
{
    [Header("Item Data")]
    [SerializeField] private Item rocketItemData;

    [Header("References")]
    [SerializeField] private Transform firePoint;

    [Header("Settings")]
    [SerializeField] private float shootCooldown = 1f;

    private float lastShotTime = -Mathf.Infinity;

    private PlayerInput input;

    private bool readytoshoot = true;

    private void Start()
    {
        input = GameObject.Find("Character").GetComponent<PlayerInput>();
    }

    private void Update()
    {
        if (input.actions.FindAction("Attack").IsPressed() && readytoshoot)
        {
            TryShoot();
        }
        else if (!input.actions.FindAction("Attack").IsPressed())
        {
            readytoshoot = true;
        }
    }

    void TryShoot()
    {
        if (PauseMenu.IsPaused) return;
        if (Time.time < lastShotTime + shootCooldown)
            return;

        if (rocketItemData == null || rocketItemData.projectilePrefab == null)
        {
            Debug.LogWarning("Rocket or projectile missing!");
            return;
        }

        lastShotTime = Time.time;
        Shoot();
        readytoshoot = false;
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