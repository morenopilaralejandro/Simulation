#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using Aremoreno.Enums.World;

public class ChunkCreatorTool : EditorWindow
{
    private OverworldDefinition _overworld;
    private ZoneDefinition _zone;
    private Vector2Int _newChunkCoord;
    private string _sceneSavePath = "Assets/Addressables/AddressScene/AddressSceneWorld";
    private GameObject _gridPrefab;

    [MenuItem("Tools/World/Create New Chunk")]
    static void Open()
    {
        GetWindow<ChunkCreatorTool>("Chunk Creator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Create New Chunk", EditorStyles.boldLabel);

        _overworld = (OverworldDefinition)EditorGUILayout.ObjectField(
            "Overworld", _overworld, typeof(OverworldDefinition), false
        );
        _zone = (ZoneDefinition)EditorGUILayout.ObjectField(
            "Zone", _zone, typeof(ZoneDefinition), false
        );
        _newChunkCoord = EditorGUILayout.Vector2IntField(
            "Chunk Coordinate", _newChunkCoord
        );
        _sceneSavePath = EditorGUILayout.TextField(
            "Save Path", _sceneSavePath
        );
        _gridPrefab = (GameObject)EditorGUILayout.ObjectField(
            "Grid Prefab", _gridPrefab, typeof(GameObject), false
        );

        // Validate that the selected prefab has a Grid component
        if (_gridPrefab != null && _gridPrefab.GetComponent<Grid>() == null)
        {
            EditorGUILayout.HelpBox(
                "Selected prefab does not have a Grid component!",
                MessageType.Error
            );
        }

        // Validate zone belongs to overworld
        if (_overworld != null && _zone != null
            && !_overworld.allZones.Contains(_zone))
        {
            EditorGUILayout.HelpBox(
                "Selected zone is not part of the overworld definition!",
                MessageType.Warning
            );
        }

        // Show where this chunk will appear in world space
        Vector2 worldPos = new Vector2(
            _newChunkCoord.x * WorldConstants.CHUNK_SIZE,
            _newChunkCoord.y * WorldConstants.CHUNK_SIZE
        );
        EditorGUILayout.HelpBox(
            $"World position: ({worldPos.x}, {worldPos.y})\n" +
            $"Covers: ({worldPos.x}, {worldPos.y}) to " +
            $"({worldPos.x + WorldConstants.CHUNK_SIZE}, " +
            $"{worldPos.y + WorldConstants.CHUNK_SIZE})",
            MessageType.Info
        );

        // Check if chunk already exists in the overworld
        if (_overworld != null)
        {
            bool exists = _overworld.allChunks.Exists(
                c => c.chunkCoord == _newChunkCoord
            );
            if (exists)
            {
                EditorGUILayout.HelpBox(
                    "A chunk at this coordinate already exists in the overworld!",
                    MessageType.Warning
                );
            }

            // Show existing neighbors
            GUILayout.Label("Neighbors:", EditorStyles.miniLabel);
            ShowNeighborStatus(Vector2Int.up, "North");
            ShowNeighborStatus(Vector2Int.down, "South");
            ShowNeighborStatus(Vector2Int.left, "West");
            ShowNeighborStatus(Vector2Int.right, "East");
        }

        GUILayout.Space(10);

        GUI.enabled = _gridPrefab != null
            && _gridPrefab.GetComponent<Grid>() != null
            && _overworld != null;
        if (GUILayout.Button("Create Chunk Scene"))
        {
            CreateChunkScene();
        }
        GUI.enabled = true;
    }

    private void ShowNeighborStatus(Vector2Int dir, string label)
    {
        Vector2Int neighborCoord = _newChunkCoord + dir;
        bool exists = _overworld.allChunks.Exists(
            c => c.chunkCoord == neighborCoord
        );
        string status = exists ? "exists" : "empty";
        EditorGUILayout.LabelField(
            $"  {label} ({neighborCoord.x},{neighborCoord.y})",
            status
        );
    }

    private void CreateChunkScene()
    {
        if (_overworld == null)
        {
            EditorUtility.DisplayDialog(
                "Error", "Assign an overworld definition first!", "OK"
            );
            return;
        }

        if (_zone == null)
        {
            EditorUtility.DisplayDialog("Error", "Assign a zone first!", "OK");
            return;
        }

        if (_gridPrefab == null || _gridPrefab.GetComponent<Grid>() == null)
        {
            EditorUtility.DisplayDialog(
                "Error",
                "Assign a valid Grid prefab first!",
                "OK"
            );
            return;
        }

        string chunkId = $"chunk_{_newChunkCoord.x}_{_newChunkCoord.y}";
        string sceneName = $"{_zone.zoneId}_{chunkId}";
        string scenePath = $"{_sceneSavePath}/{sceneName}.unity";

        // Create new scene
        var newScene = EditorSceneManager.NewScene(
            NewSceneSetup.EmptyScene,
            NewSceneMode.Additive
        );

        // Create root object
        GameObject root = new GameObject("ChunkRoot");
        var chunkRoot = root.AddComponent<ChunkSceneRoot>();
        chunkRoot.chunkId = sceneName;
        chunkRoot.zoneId = _zone.zoneId;
        chunkRoot.chunkCoord = _newChunkCoord;

        // Instantiate the selected Grid prefab
        GameObject gridInstance = (GameObject)PrefabUtility.InstantiatePrefab(
            _gridPrefab, newScene
        );
        gridInstance.transform.SetParent(root.transform);

        // Create SpawnPoints container
        GameObject spawnContainer = new GameObject("SpawnPoints");
        spawnContainer.transform.SetParent(root.transform);

        // Create NPCs container
        GameObject npcContainer = new GameObject("NPCs");
        npcContainer.transform.SetParent(root.transform);

        // Create Interactables container
        GameObject interactablesContainer = new GameObject("Interactables");
        interactablesContainer.transform.SetParent(root.transform);

        // Create Transitions container
        GameObject transContainer = new GameObject("Transitions");
        transContainer.transform.SetParent(root.transform);

        // Save scene
        EditorSceneManager.SaveScene(newScene, scenePath);
        EditorSceneManager.CloseScene(newScene, true);

        // Add chunk to overworld definition
        var newChunkDef = new ChunkDefinition
        {
            chunkId = sceneName,
            parentZone = _zone,
            chunkCoord = _newChunkCoord,
            sceneAddress = sceneName,
            containedSpawnIds = new List<string>()
        };
        _overworld.allChunks.Add(newChunkDef);
        EditorUtility.SetDirty(_overworld);

        AssetDatabase.SaveAssets();

        // Mark scene as addressable
        // (You may need to do this manually or use
        //  Addressables API to add to a group)

        EditorUtility.DisplayDialog(
            "Success",
            $"Created chunk scene at:\n{scenePath}\n\n" +
            $"Added to overworld: {_overworld.name}\n" +
            $"Zone: {_zone.zoneId}\n\n" +
            $"Remember to mark it as Addressable\n" +
            $"with address: {sceneName}",
            "OK"
        );

        Debug.Log(
            $"Created chunk: {chunkId} at coord {_newChunkCoord} " +
            $"(overworld: {_overworld.name}, zone: {_zone.zoneId})"
        );
    }
}
#endif
