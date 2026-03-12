using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class BloodSplater : MonoBehaviour
{
    public GameObject BloodDecal;
    ParticleSystem system;
    ParticleSystem.Particle[] particles;
    float StartTime;
    private void Start()
    {
        StartTime = Time.time;
    }
    private void Update()
    {
        if (system == null)
        {
            system = GetComponent<ParticleSystem>();
            particles = new ParticleSystem.Particle[system.maxParticles];
        }
        int numparticle= system.GetParticles(particles);
        for (int i = 0; i < numparticle; i++)
        {
            if (particles[i].velocity == Vector3.zero)
            {
                GameObject instanse = Instantiate(BloodDecal, particles[i].position + transform.position + new Vector3(0,0.1f,0), new Quaternion());
                instanse.transform.Rotate(90,0,Random.Range(-180, 180));
            }
        }
        if (Time.time > StartTime + system.duration) 
        {
            Destroy(gameObject);
        }
    }
    
}
