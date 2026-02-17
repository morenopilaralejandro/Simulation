using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class SceneGroupRegistry : MonoBehaviour
{
    public static SceneGroupRegistry Instance { get; private set; }

    private readonly Dictionary<string, SceneGroup> sceneGroupDict = new();

    public bool IsReady { get; private set; } = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public async Task LoadAllSceneGroupAsync()
    {
        var handle = Addressables.LoadAssetsAsync<SceneGroup>(
            "SceneGroup-Data",
            data => sceneGroupDict[data.groupName] = data
        );
        await handle.Task;
        IsReady = true;
        LogManager.Trace($"[SceneGroupRegistry] All sceneGroup loaded. Total count: {sceneGroupDict.Count}", this);
    }

    public SceneGroup GetGroupByName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            LogManager.Error("[SceneGroupRegistry] Tried to GetGroupByName with null/empty name!");
            return null;
        }

        if (!sceneGroupDict.TryGetValue(name, out var sceneGroup))
        {
            LogManager.Error($"[SceneGroupRegistry] No sceneGroup found for name '{name}'.");
            return null;
        }

        return sceneGroup;
    }
}
