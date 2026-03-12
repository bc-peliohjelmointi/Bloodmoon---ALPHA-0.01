using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DecalFade : MonoBehaviour
{
    public float LifeTime;
    public float FadeTime;
    float StartTime;
    DecalProjector projector;
    private void Awake()
    {
        StartTime = Time.time;
        projector = GetComponent<DecalProjector>();
    }
    private void Update()
    {
        Fade();
    }
    void Fade()
    {
        if (LifeTime - Time.time + StartTime < FadeTime)
        {
            projector.fadeFactor -= Time.deltaTime / FadeTime;
        }
        if (projector.fadeFactor == 0)
        {
            Destroy(gameObject);
        }
    }
}
