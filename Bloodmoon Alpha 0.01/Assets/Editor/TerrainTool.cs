// TerrainScenesCreatorWindow.cs
// Put this file under an "Editor" folder, e.g. Assets/Editor/TerrainScenesCreatorWindow.cs
// Creates ONE Unity Scene per terrain tile (e.g. Scene_Terrain-1_1.unity), each containing a single Terrain,
// positioned in a 4x5 world grid (x*tileSizeX, z*tileSizeZ).

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TerrainScenesCreatorWindow : EditorWindow
{
    // Grid
    private int gridX = 4;
    private int gridZ = 5;

    // Terrain sizing (Unity units)
    private float tileSizeX = 500f;
    private float tileSizeZ = 500f;
    private float height = 200f;

    // Terrain data
    private int heightmapResolution = 513; // (2^n) + 1
    private int alphamapResolution = 512;
    private int baseMapResolution = 1024;
    private int detailResolution = 1024;
    private int detailResolutionPerPatch = 16;
    private int terrainLayer = 0;

    // Options
    private bool addTerrainCollider = true;
    private bool addDirectionalLight = true;
    private bool addCamera = false;
    private bool markTerrainsAutoConnect = false;
    private string terrainTag = "Untagged";

    // Naming & folders
    private string terrainDataFolder = "Assets/TerrainData";
    private string scenesFolder = "Assets/TerrainScenes";
    private string scenePrefix = "Scene_"; // e.g. Scene_Terrain-1_1
    private bool promptToSaveOpenScenes = true;

    [MenuItem("Tools/Terrain/Generate Terrain Scenes (4x5)")]
    public static void ShowWindow()
    {
        var w = GetWindow<TerrainScenesCreatorWindow>("Terrain Scenes Creator");
        w.minSize = new Vector2(420, 480);
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Grid", EditorStyles.boldLabel);
        gridX = EditorGUILayout.IntField("Tiles X (columns)", Mathf.Max(1, gridX));
        gridZ = EditorGUILayout.IntField("Tiles Z (rows)", Mathf.Max(1, gridZ));

        EditorGUILayout.Space(8);
        EditorGUILayout.LabelField("Tile Size (Unity units)", EditorStyles.boldLabel);
        tileSizeX = EditorGUILayout.FloatField("Width (X)", Mathf.Max(1f, tileSizeX));
        tileSizeZ = EditorGUILayout.FloatField("Length (Z)", Mathf.Max(1f, tileSizeZ));
        height = EditorGUILayout.FloatField("Height (Y)", Mathf.Max(1f, height));

        EditorGUILayout.Space(8);
        EditorGUILayout.LabelField("Resolutions", EditorStyles.boldLabel);
        heightmapResolution = EditorGUILayout.IntField("Heightmap Resolution", heightmapResolution);
        alphamapResolution = EditorGUILayout.IntField("Alphamap Resolution", alphamapResolution);
        baseMapResolution = EditorGUILayout.IntField("Basemap Resolution", baseMapResolution);
        detailResolution = EditorGUILayout.IntField("Detail Resolution", detailResolution);
        detailResolutionPerPatch = EditorGUILayout.IntField("Detail Res / Patch", detailResolutionPerPatch);

        EditorGUILayout.Space(8);
        EditorGUILayout.LabelField("Options", EditorStyles.boldLabel);
        addTerrainCollider = EditorGUILayout.Toggle("Add TerrainCollider", addTerrainCollider);
        markTerrainsAutoConnect = EditorGUILayout.Toggle("Terrain.allowAutoConnect", markTerrainsAutoConnect);
        terrainTag = EditorGUILayout.TagField("Terrain Tag", terrainTag);
        terrainLayer = EditorGUILayout.LayerField("Terrain Layer", terrainLayer);
        addDirectionalLight = EditorGUILayout.Toggle("Add Directional Light", addDirectionalLight);
        addCamera = EditorGUILayout.Toggle("Add Camera", addCamera);
        promptToSaveOpenScenes = EditorGUILayout.Toggle("Prompt to save current scenes", promptToSaveOpenScenes);

        EditorGUILayout.Space(8);
        EditorGUILayout.LabelField("Folders", EditorStyles.boldLabel);
        terrainDataFolder = EditorGUILayout.TextField("TerrainData Folder", terrainDataFolder);
        scenesFolder = EditorGUILayout.TextField("Scenes Folder", scenesFolder);

        EditorGUILayout.Space(8);
        EditorGUILayout.LabelField("Naming", EditorStyles.boldLabel);
        scenePrefix = EditorGUILayout.TextField("Scene Prefix", scenePrefix);

        EditorGUILayout.Space(14);

        using (new EditorGUI.DisabledScope(!IsValidHeightmapRes(heightmapResolution)))
        {
            if (GUILayout.Button("Generate Scenes", GUILayout.Height(36)))
            {
                GenerateScenes();
            }
        }

        if (!IsValidHeightmapRes(heightmapResolution))
        {
            EditorGUILayout.HelpBox("Heightmap Resolution must be (2^n) + 1, e.g. 513, 1025, 2049.", MessageType.Warning);
        }

        EditorGUILayout.Space(10);
        EditorGUILayout.HelpBox(
            $"Will create {gridX * gridZ} scenes under:\n{scenesFolder}\n" +
            $"Each scene contains one terrain named Terrain-<x>_<z> (1-based) and positioned at (x*{tileSizeX}, z*{tileSizeZ}).",
            MessageType.Info
        );
    }

    private static bool IsValidHeightmapRes(int r)
    {
        int v = r - 1;
        return r >= 33 && (v & (v - 1)) == 0;
    }

    private void GenerateScenes()
    {
        if (promptToSaveOpenScenes)
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                return;
        }

        EnsureFolderExists(terrainDataFolder);
        EnsureFolderExists(scenesFolder);

        for (int z = 0; z < gridZ; z++)
        {
            for (int x = 0; x < gridX; x++)
            {
                // Create new empty scene
                Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

                // This tile's world origin (so objects are placed consistently across scenes)
                Vector3 tileOrigin = new Vector3(x * tileSizeX, 0f, z * tileSizeZ);

                // Optional basics
                if (addDirectionalLight)
                {
                    var lightGO = new GameObject("Directional Light");
                    var light = lightGO.AddComponent<Light>();
                    light.type = LightType.Directional;
                    lightGO.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
                }

                if (addCamera)
                {
                    var camGO = new GameObject("Main Camera");
                    var cam = camGO.AddComponent<Camera>();
                    cam.tag = "MainCamera";

                    camGO.transform.position = tileOrigin + new Vector3(tileSizeX * 0.5f, height * 0.75f, -tileSizeZ * 0.75f);
                    camGO.transform.rotation = Quaternion.Euler(25f, 0f, 0f);
                }

                // Create TerrainData asset for this tile
                var td = new TerrainData
                {
                    heightmapResolution = heightmapResolution,
                    alphamapResolution = alphamapResolution,
                    baseMapResolution = baseMapResolution
                };
                td.SetDetailResolution(Mathf.Max(8, detailResolution), Mathf.Clamp(detailResolutionPerPatch, 8, 128));
                td.size = new Vector3(tileSizeX, height, tileSizeZ);

                string terrainName = $"Terrain-{x + 1}_{z + 1}";
                string tdAssetPath = AssetDatabase.GenerateUniqueAssetPath($"{terrainDataFolder}/{terrainName}.asset");
                AssetDatabase.CreateAsset(td, tdAssetPath);

                // Create terrain GameObject
                var terrainGO = Terrain.CreateTerrainGameObject(td);
                terrainGO.name = terrainName;
                terrainGO.tag = terrainTag;
                terrainGO.layer = terrainLayer;

                // IMPORTANT: place this terrain into the correct world grid position
                terrainGO.transform.position = tileOrigin;

                var t = terrainGO.GetComponent<Terrain>();
                t.allowAutoConnect = markTerrainsAutoConnect;

                if (!addTerrainCollider)
                {
                    var col = terrainGO.GetComponent<TerrainCollider>();
                    if (col) DestroyImmediate(col);
                }

                // Save scene
                string scenePath = AssetDatabase.GenerateUniqueAssetPath(
                    $"{scenesFolder}/{scenePrefix}{terrainName}.unity"
                );
                EditorSceneManager.SaveScene(scene, scenePath);
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // Open the first generated scene for convenience (if it exists)
        string firstScenePath = $"{scenesFolder}/{scenePrefix}Terrain-1_1.unity";
        if (System.IO.File.Exists(firstScenePath))
            EditorSceneManager.OpenScene(firstScenePath, OpenSceneMode.Single);
    }

    private static void EnsureFolderExists(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) path = "Assets";
        path = path.Replace("\\", "/");

        if (AssetDatabase.IsValidFolder(path)) return;

        string[] parts = path.Split('/');
        string current = (parts.Length > 0 && parts[0] == "Assets") ? "Assets" : "Assets";

        for (int i = 1; i < parts.Length; i++)
        {
            string next = $"{current}/{parts[i]}";
            if (!AssetDatabase.IsValidFolder(next))
                AssetDatabase.CreateFolder(current, parts[i]);
            current = next;
        }
    }
}
#endif
