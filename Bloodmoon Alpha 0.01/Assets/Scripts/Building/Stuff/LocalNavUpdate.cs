using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using Unity.AI.Navigation;

public class LocalNavUpdate : MonoBehaviour
{
    public NavMeshData navMeshData;
    public NavMeshBuildSettings buildSettings;
    public List<NavMeshBuildSource> sourse = new List<NavMeshBuildSource>();
    public float worldSizeX;
    public float worldSizeY;
    public float worldSizeZ;
    private Bounds bounds;
    public LayerMask mask;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        bounds = new Bounds(new Vector3(worldSizeX/2, worldSizeY/2+1, worldSizeZ/2), new Vector3(worldSizeX, worldSizeY, worldSizeZ));
        buildSettings = GetComponent<NavMeshSurface>().GetBuildSettings();
        navMeshData = GetComponent<NavMeshSurface>().navMeshData;
        NavUpdate();
    }

    public void NavUpdate()
    {
        NavMeshBuilder.CollectSources(bounds, mask, NavMeshCollectGeometry.RenderMeshes, 0, new List<NavMeshBuildMarkup>(), sourse);
        NavMeshBuilder.UpdateNavMeshData(navMeshData, buildSettings, sourse, bounds);
    }
}
