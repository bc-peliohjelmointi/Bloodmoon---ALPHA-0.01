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
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        bounds = new Bounds(new Vector3(worldSizeX/2, worldSizeY/2+1, worldSizeZ/2), new Vector3(worldSizeX, worldSizeY, worldSizeZ));
        surface = GetComponent<NavMeshSurface>();
        buildSettings = surface.GetBuildSettings();
        navMeshData = surface.navMeshData;
        NavUpdate();
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
            NavMeshBuilder.CollectSources(bounds, mask, NavMeshCollectGeometry.RenderMeshes, 0, new List<NavMeshBuildMarkup>(), sourses);
            Debug.LogWarning("Navmesh updated when go = null");
        }
        NavMeshBuilder.Cancel(navMeshData);
        NavMeshBuilder.UpdateNavMeshDataAsync(navMeshData, buildSettings, sourses, bounds);
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
}
