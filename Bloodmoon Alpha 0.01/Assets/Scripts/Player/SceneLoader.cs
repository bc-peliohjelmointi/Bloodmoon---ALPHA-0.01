using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SceneLoader : MonoBehaviour
{
    GameObject player;

    int chunkSize = 500;
    int radius = 1;

    // WORLD SIZE (5 x 4)
    int minX = 1;
    int maxX = 4;
    int minZ = 1;
    int maxZ = 5;

    HashSet<string> loadingScenes = new HashSet<string>();
    HashSet<string> loadedScenes = new HashSet<string>();

    void Start()
    {
        player = GameObject.Find("Character");
    }

    void Update()
    {
        UpdateChunks();
        UpdateVisibleChunks();
    }

    void UpdateChunks()
    {
        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camPos = Camera.main.transform.position - camForward * 200f;

        int playerChunkX = Mathf.FloorToInt(player.transform.position.x / chunkSize) + 1;
        int playerChunkZ = Mathf.FloorToInt(player.transform.position.z / chunkSize) + 1;

        Debug.Log(playerChunkX);
        Debug.Log(playerChunkZ);

        HashSet<string> requiredScenes = new HashSet<string>();

        // Determine which chunks SHOULD be loaded
        for (int x = -radius; x <= radius; x++)
        {
            for (int z = -radius; z <= radius; z++)
            {
                int chunkX = playerChunkX + x;
                int chunkZ = playerChunkZ + z;
                bool isPlayerChunk = (chunkX == playerChunkX && chunkZ == playerChunkZ);

                if (chunkX < minX || chunkX > maxX || chunkZ < minZ || chunkZ > maxZ)
                    continue;

                Vector3 chunkCenter = new Vector3((chunkX - 0.5f) * chunkSize, camPos.y, (chunkZ - 0.5f) * chunkSize);

                Vector3 dirToChunk = (chunkCenter - camPos).normalized;

                float dot = Vector3.Dot(camForward, dirToChunk);

                // Skip chunks mostly behind the camera
                float unloadThreshold = -0.4f;

                if (!isPlayerChunk && dot < unloadThreshold)
                    continue;

                string sceneName = $"Scene_Terrain-{chunkX}_{chunkZ}";
                requiredScenes.Add(sceneName);

                Scene scene = SceneManager.GetSceneByName(sceneName);

                if (!scene.isLoaded && !loadingScenes.Contains(sceneName))
                {
                    loadingScenes.Add(sceneName);
                    StartCoroutine(LoadScene(sceneName));
                }
            }
        }

        // Unload scenes outside radius
        List<string> scenesToRemove = new List<string>();

        foreach (string sceneName in loadedScenes)
        {
            if (!requiredScenes.Contains(sceneName))
            {
                SceneManager.UnloadSceneAsync(sceneName);
                scenesToRemove.Add(sceneName);
            }
        }

        foreach (string scene in scenesToRemove)
        {
            loadedScenes.Remove(scene);
        }
    }

    System.Collections.IEnumerator LoadScene(string sceneName)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        yield return op;

        loadingScenes.Remove(sceneName);
        loadedScenes.Add(sceneName);
    }

    void UpdateVisibleChunks()
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);

        foreach (string sceneName in loadedScenes)
        {
            Scene scene = SceneManager.GetSceneByName(sceneName);

            foreach (GameObject root in scene.GetRootGameObjects())
            {
                Renderer[] renderers = root.GetComponentsInChildren<Renderer>();

                foreach (Renderer r in renderers)
                {
                    r.enabled = GeometryUtility.TestPlanesAABB(planes, r.bounds);
                }
            }
        }
    }
}