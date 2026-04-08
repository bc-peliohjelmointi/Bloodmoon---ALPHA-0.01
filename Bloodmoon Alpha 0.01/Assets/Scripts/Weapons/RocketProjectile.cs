using UnityEngine;

public class RocketProjectile : MonoBehaviour
{
    public float speed = 30f;
    public float lifeTime = 5f;

    [Header("Explosion")]
    public float explosionRadius = 5f;
    public float explosionForce = 10f;

    private Rigidbody rb;

    private float damage;
    private float knockback;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void Launch(Vector3 direction, float dmg, float kb)
    {
        damage = dmg;
        knockback = kb;

        rb.useGravity = false; // 🚀 no drop
        rb.linearVelocity = direction * speed;
    }

    private void Update()
    {
        if (rb.linearVelocity.sqrMagnitude > 0.1f)
            transform.forward = rb.linearVelocity.normalized;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Explode(collision.contacts[0].point, collision.collider);
    }

    void Explode(Vector3 position, Collider directHit)
    {
        Collider[] hits = Physics.OverlapSphere(position, explosionRadius);

        foreach (Collider hit in hits)
        {
            IDamageable dmgTarget = hit.GetComponentInParent<IDamageable>();
            if (dmgTarget == null) continue;

            float distance = Vector3.Distance(position, hit.transform.position);

            // Full damage on direct hit
            float finalDamage = damage;

            if (hit != directHit)
            {
                // Falloff damage for AOE
                float falloff = 1f - (distance / explosionRadius);
                finalDamage *= Mathf.Clamp01(falloff);
            }

            Vector3 dir = (hit.transform.position - position).normalized;
            Vector3 knock = dir * knockback;

            dmgTarget.TakeDamage(finalDamage, knock);
        }

        // Optional: explosion force
        foreach (Collider hit in hits)
        {
            Rigidbody r = hit.GetComponent<Rigidbody>();
            if (r != null)
            {
                r.AddExplosionForce(explosionForce, position, explosionRadius);
            }
        }

        // TODO: add explosion VFX here if you want

        Destroy(gameObject);
    }
}