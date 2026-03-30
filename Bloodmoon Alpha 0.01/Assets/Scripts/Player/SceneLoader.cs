using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class SceneLoader : MonoBehaviour
{
    GameObject player;

    [Header("Chunk Settings")]
    public int chunkSize = 250;
    public int radius = 2;

    [Header("World Bounds")]
    public int minX = 1;
    public int maxX = 16;
    public int minZ = 1;
    public int maxZ = 20;

    [Header("Camera Filter")]
    public float cameraBackOffset = 400f;
    public float unloadDotThreshold = -0.6f;

    HashSet<string> loadingScenes = new HashSet<string>();
    HashSet<string> loadedScenes = new HashSet<string>();

    private float chunkUpdateTimer = 0f;
    private float chunkUpdateInterval = 0.25f;

    void Start()
    {
        player = GameObject.Find("Character");
    }

    void Update()
    {
        chunkUpdateTimer += Time.deltaTime;
        if (chunkUpdateTimer >= chunkUpdateInterval)
        {
            chunkUpdateTimer = 0f;
            UpdateChunks();
        }
    }

    void UpdateChunks()
    {
        Camera cam = Camera.main;

        Vector3 camForward = cam.transform.forward;
        Vector3 camPos = cam.transform.position;

        int playerChunkX = Mathf.FloorToInt(player.transform.position.x / chunkSize) + 1;
        int playerChunkZ = Mathf.FloorToInt(player.transform.position.z / chunkSize) + 1;

        HashSet<string> requiredScenes = new HashSet<string>();

        List<Vector2Int> offsets = new List<Vector2Int>();
        for (int x = -radius; x <= radius; x++)
        {
            for (int z = -radius; z <= radius; z++)
            {
                offsets.Add(new Vector2Int(x, z));
            }
        }

        offsets.Sort((a, b) =>
        {
            float distA = a.sqrMagnitude; // distance squared
            float distB = b.sqrMagnitude;
            return distA.CompareTo(distB); // closest first
        });

        foreach (Vector2Int offset in offsets)
        {
            int chunkX = playerChunkX + offset.x;
            int chunkZ = playerChunkZ + offset.y;

            if (chunkX < minX || chunkX > maxX || chunkZ < minZ || chunkZ > maxZ)
                continue;

            bool nearPlayer = Mathf.Abs(offset.x) <= 1 && Mathf.Abs(offset.y) <= 1;

            Vector3 chunkCenter = new Vector3((chunkX - 0.5f) * chunkSize, 0f, (chunkZ - 0.5f) * chunkSize);

            Vector3 referencePos = new Vector3(player.transform.position.x, 0f, player.transform.position.z);
            Vector3 camForwardXZ = new Vector3(Camera.main.transform.forward.x, 0f, Camera.main.transform.forward.z);
            float camForwardXZLength = camForwardXZ.magnitude;

            if (camForwardXZLength > 0.001f)
            {
                camForwardXZ.Normalize();
                Vector3 dirToChunkXZ = (chunkCenter - referencePos).normalized;
                float dot = Vector3.Dot(camForwardXZ, dirToChunkXZ);

                if (!nearPlayer && dot < unloadDotThreshold)
                    continue; // skip chunks behind
            }
            else
            {
                if (!nearPlayer)
                    continue; // looking straight up/down
            }

            string sceneName = $"Scene_Terrain-{chunkX}_{chunkZ}";
            requiredScenes.Add(sceneName);

            if (!SceneManager.GetSceneByName(sceneName).isLoaded && !loadingScenes.Contains(sceneName))
            {
                loadingScenes.Add(sceneName);
                StartCoroutine(LoadScene(sceneName));
            }
        }

        // Unload scenes not required
        List<string> toRemove = new List<string>();

        foreach (string sceneName in loadedScenes)
        {
            if (!requiredScenes.Contains(sceneName))
            {
                SceneManager.UnloadSceneAsync(sceneName);
                toRemove.Add(sceneName);
            }
        }

        foreach (string s in toRemove)
            loadedScenes.Remove(s);
    }

    IEnumerator LoadScene(string sceneName)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        yield return op;

        loadingScenes.Remove(sceneName);
        loadedScenes.Add(sceneName);
    }
}