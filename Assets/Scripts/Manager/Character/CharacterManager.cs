using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance { get; private set; }

    private readonly Dictionary<string, CharacterData> characterDataDict = new();

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

    public async Task LoadAllCharacterDataAsync()
    {
        var handle = Addressables.LoadAssetsAsync<CharacterData>(
            "Characters-Data",
            data => characterDataDict[data.CharacterId] = data
        );
        await handle.Task;
        IsReady = true;
        LogManager.Trace($"[CharacterManager] All characters loaded. Total count: {characterDataDict.Count}", this);
    }

    public CharacterData GetCharacterData(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            LogManager.Error("[CharacterManager] Tried to GetCharacterData with null/empty id!");
            return null;
        }

        if (!characterDataDict.TryGetValue(id, out var characterData))
        {
            LogManager.Error($"[CharacterManager] No characterData found for id '{id}'.");
            return null;
        }

        return characterData;
    }
}
