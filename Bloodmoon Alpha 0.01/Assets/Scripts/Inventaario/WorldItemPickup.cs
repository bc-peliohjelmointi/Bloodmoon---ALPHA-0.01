using UnityEngine;

[RequireComponent(typeof(Collider))]
public class WorldItemPickup : MonoBehaviour
{
    public Item item;

    [Header("Pickup Settings")]
    [SerializeField] private float pickupDelay = 0.5f;

    [Header("Magnet Settings")]
    [SerializeField] private float magnetRange = 2f;
    [SerializeField] private float magnetSpeed = 10f;

    private Collider[] colliders;
    private bool magnetActive = false;
    private bool pickedUp = false;
    private float spawnTime;
    private Transform player;

    private void Awake()
    {
        colliders = GetComponents<Collider>();
    }

    private void Start()
    {
        spawnTime = Time.time;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    private void Update()
    {
        if (pickedUp || player == null) return;
        if (Time.time < spawnTime + pickupDelay) return;

        // Stop magnetism if inventory is full
        if (Inventory.Singleton != null && Inventory.Singleton.IsFull())
        {
            // If we were already flying toward the player, reset
            if (magnetActive)
            {
                magnetActive = false;
                ReEnableSolidColliders();
            }
            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= magnetRange)
        {
            if (!magnetActive)
            {
                magnetActive = true;
                DisableSolidColliders();
            }

            transform.position = Vector3.MoveTowards(
                transform.position,
                player.position,
                magnetSpeed * Time.deltaTime
            );
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (pickedUp) return;
        if (Time.time < spawnTime + pickupDelay) return;
        if (!other.CompareTag("Player")) return;

        if (item == null)
        {
            Debug.LogError("WorldItemPickup has no Item assigned!", this);
            return;
        }

        // Try to add to inventory — if full, item stays in the world
        bool success = Inventory.Singleton.SpawnInventoryItem(item);
        if (!success) return;

        pickedUp = true;
        DisableObject();
        Destroy(gameObject);
    }

    void DisableObject()
    {
        foreach (var col in colliders)
            col.enabled = false;

        Renderer rend = GetComponent<Renderer>();
        if (rend != null) rend.enabled = false;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) rb.linearVelocity = Vector3.zero;
    }

    void DisableSolidColliders()
    {
        foreach (var col in colliders)
        {
            if (!col.isTrigger)
                col.enabled = false;
        }
    }

    void ReEnableSolidColliders()
    {
        foreach (var col in colliders)
        {
            if (!col.isTrigger)
                col.enabled = true;
        }
    }
}