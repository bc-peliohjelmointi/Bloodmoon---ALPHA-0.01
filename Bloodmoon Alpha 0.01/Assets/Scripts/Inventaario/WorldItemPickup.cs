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

        foreach (var col in colliders)
            col.isTrigger = col.isTrigger; // keep original setup
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

        // Wait for pickup delay
        if (Time.time < spawnTime + pickupDelay) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= magnetRange)
        {
            // Activate magnet once
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

        pickedUp = true;

        DisableObject();

        Inventory.Singleton.SpawnInventoryItem(item);

        Destroy(gameObject);
    }

    void DisableObject()
    {
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

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
            {
                col.enabled = false;
            }
        }
    }
}