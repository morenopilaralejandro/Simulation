using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance { get; private set; }

    private readonly Dictionary<string, CharacterData> characterDict = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadAllCharacters();
    }

    private void LoadAllCharacters()
    {
        Addressables.LoadAssetsAsync<CharacterData>("Characters", data =>
        {
            if (!characterDict.ContainsKey(data.CharacterId))
                characterDict.Add(data.CharacterId, data);
        }).Completed += handle =>
        {
            LogManager.Trace($"[CharacterManager] All characters loaded. Total count: {characterDict.Count}", this);
        };
    }

    public CharacterData GetCharacterData(string id)
    {
        if (characterDict.TryGetValue(id, out var cd))
            return cd;
        return null;
    }
}
