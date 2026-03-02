using UnityEngine;

public class ArrowProjectile : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 5f;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void Launch(Vector3 direction)
    {
        rb.linearVelocity = direction * speed;
    }

    private void Update()
    {
        if (rb.linearVelocity.sqrMagnitude > 0.1f)
            transform.forward = rb.linearVelocity.normalized;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}