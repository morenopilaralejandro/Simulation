using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance { get; private set; }

    private readonly Dictionary<string, CharacterData> characterDataDict = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadAllCharacterData();
    }

    private void LoadAllCharacterData()
    {
        Addressables.LoadAssetsAsync<CharacterData>("Characters", data =>
        {
            if (!characterDataDict.ContainsKey(data.CharacterId))
                characterDataDict.Add(data.CharacterId, data);
        }).Completed += handle =>
        {
            LogManager.Trace($"[CharacterManager] All characters loaded. Total count: {characterDataDict.Count}", this);
            TeamManager.Instance.LoadAllTeams();
        };
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
