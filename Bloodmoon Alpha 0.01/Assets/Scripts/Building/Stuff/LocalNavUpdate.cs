using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using Unity.AI.Navigation;

public class LocalNavUpdate : MonoBehaviour
{
    public NavMeshData navMeshData;
    public NavMeshBuildSettings buildSettings;
    public List<NavMeshBuildSource> sourses = new List<NavMeshBuildSource>();
    public float worldSizeX;
    public float worldSizeY;
    public float worldSizeZ;
    private Bounds bounds;
    public LayerMask mask;
    private NavMeshSurface surface;
    private GameObject Player;
    private GameObject Enemymanager;

    AsyncOperation navUpdate;

    private void Start()
    {
        Enemymanager = GameObject.Find("EnemyManager");
        Player = GameObject.Find("Character");
        bounds = new Bounds(new Vector3(2000/2, 250/2, 2500/2), new Vector3(2000, 250, 2500));
        NavMeshBuilder.CollectSources(bounds, mask, NavMeshCollectGeometry.RenderMeshes, 0, new List<NavMeshBuildMarkup>(), sourses);
        bounds = new Bounds(new Vector3(Player.transform.position.x, worldSizeY/2+1, Player.transform.position.z), new Vector3(worldSizeX, worldSizeY, worldSizeZ));
        surface = GetComponent<NavMeshSurface>();
        buildSettings = surface.GetBuildSettings();
        navMeshData = surface.navMeshData;
        NavUpdate();
    }

    private void Update()
    {
        if (Mathf.Abs(Player.transform.position.x - bounds.center.x) > worldSizeX / 5f || Mathf.Abs(Player.transform.position.z - bounds.center.z) > worldSizeZ / 5f) 
        {
            bounds = new Bounds(new Vector3(Player.transform.position.x, worldSizeY / 2 + 1, Player.transform.position.z), new Vector3(worldSizeX, worldSizeY, worldSizeZ));
            FinalizedUpdate();
        }
    }

    public void NavUpdate(GameObject go = null)
    {
        if (go != null)
        {
            if (TryCreateSource(go, out NavMeshBuildSource source))
            {
                sourses.Add(source);
            }
        }
        else
        {
            bounds = new Bounds(new Vector3(2000 / 2, 250 / 2, 2500 / 2), new Vector3(2000, 250, 2500));
            NavMeshBuilder.CollectSources(bounds, mask, NavMeshCollectGeometry.RenderMeshes, 0, new List<NavMeshBuildMarkup>(), sourses);
            bounds = new Bounds(new Vector3(Player.transform.position.x, worldSizeY / 2 + 1, Player.transform.position.z), new Vector3(worldSizeX, worldSizeY, worldSizeZ));
            Debug.LogWarning("Navmesh updated when go = null");
        }
        FinalizedUpdate();
    }

    public void FinalizedUpdate()
    {
        if (Enemymanager != null)
        {
            foreach (Transform child in Enemymanager.transform)
            {
                if (child.name != "EnemyManager")
                {
                    if(Mathf.Abs(child.transform.position.x - bounds.center.x) > worldSizeX/2 || Mathf.Abs(child.transform.position.y - bounds.center.y) > worldSizeY/2)
                    {
                        child.gameObject.SetActive(false);
                    }
                }
            }
        }
        NavMeshBuilder.Cancel(navMeshData);
        navUpdate = NavMeshBuilder.UpdateNavMeshDataAsync(navMeshData, buildSettings, sourses, bounds);

        navUpdate.completed += OnNavMeshUpdateFinished;
    }

    public static bool TryCreateSource(GameObject go, out NavMeshBuildSource source)
    {
        source = new NavMeshBuildSource();

        // MeshRenderer + MeshFilter
        if (go.TryGetComponent<MeshFilter>(out var meshFilter) &&
            go.TryGetComponent<MeshRenderer>(out var renderer))
        {
            source.shape = NavMeshBuildSourceShape.Mesh;
            source.sourceObject = meshFilter.sharedMesh;
            source.transform = go.transform.localToWorldMatrix;
            source.area = 0;

            return true;
        }

        // Terrain
        if (go.TryGetComponent<Terrain>(out var terrain))
        {
            source.shape = NavMeshBuildSourceShape.Terrain;
            source.sourceObject = terrain.terrainData;
            source.transform = go.transform.localToWorldMatrix;
            source.area = 0;

            return true;
        }

        return false;
    }

    void OnNavMeshUpdateFinished(AsyncOperation op)
    {
        foreach (Transform child in Enemymanager.transform)
        {
            if (child.name != "EnemyManager")
            {
                if (Mathf.Abs(child.transform.position.x - bounds.center.x) < worldSizeX/2 && Mathf.Abs(child.transform.position.z - bounds.center.z) < worldSizeZ/2)
                {
                    child.gameObject.SetActive(true);
                }
            }
        }
    }
}
