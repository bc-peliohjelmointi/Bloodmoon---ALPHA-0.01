using UnityEngine;

public class SlowTextureGenerationManager : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Terrains.TextureGenerators.Count == 0)
        {
            Destroy(gameObject);
        }
        else
        {
            Terrains.TextureGenerators[Terrains.TextureGenerators.Count - 1].MaterialChange();
            //Terrains.TextureGenerators[Terrains.TextureGenerators.Count - 1].ClearTrees();
            Terrains.TextureGenerators.RemoveAt(Terrains.TextureGenerators.Count - 1);
            Debug.Log("Tiles Left: " + Terrains.TextureGenerators.Count);
        }
    }
}
