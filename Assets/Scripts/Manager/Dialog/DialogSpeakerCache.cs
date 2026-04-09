using UnityEngine;
using System.Collections.Generic;

public class DialogSpeakerCache : MonoBehaviour
{
    private const int CACHE_SIZE = 5;

    // LRU cache: LinkedList for order tracking + Dictionary for O(1) lookup
    private readonly Dictionary<string, LinkedListNode<KeyValuePair<string, Speaker>>> speakerCacheDict
        = new Dictionary<string, LinkedListNode<KeyValuePair<string, Speaker>>>(CACHE_SIZE);
    private readonly LinkedList<KeyValuePair<string, Speaker>> lruList
        = new LinkedList<KeyValuePair<string, Speaker>>();

    // Reusable objects to avoid per-resolve allocations
    private Character reusableCharacter;
    private CharacterData reusableCharacterData;
    private Npc reusableNpc;

    private Speaker speaker;
    public Speaker Speaker => speaker;
    private CharacterDatabase characterDatabase;

    private void Start() 
    {
        characterDatabase = CharacterDatabase.Instance;
    }

    public Speaker GetSpeaker(DialogLine dialogLine)
    {
        if (speakerCacheDict.TryGetValue(dialogLine.SpeakerId, out var node))
        {
            // Move to front (most recently used)
            lruList.Remove(node);
            lruList.AddFirst(node);
            speaker = node.Value.Value;
            return speaker;
        }

        speaker = ResolveSpeaker(dialogLine);
        return speaker;
    }

    private Speaker ResolveSpeaker(DialogLine dialogLine)
    {
        reusableCharacterData = characterDatabase.GetCharacterData(dialogLine.SpeakerId);
        if (reusableCharacterData != null)
        {
            reusableCharacter = new Character(reusableCharacterData);

            return AddToCache(
                reusableCharacter.CharacterId,
                reusableCharacter.LocalizationComponent,
                reusableCharacter.AppearanceComponent,
                dialogLine.DialogKit
            );
        }

        NpcData npcData = NpcManager.Instance.GetNpcData(dialogLine.SpeakerId);
        if (npcData != null)
        {
            reusableNpc = new Npc(npcData);

            return AddToCache(
                reusableNpc.NpcId,
                reusableNpc.LocalizationComponent,
                reusableNpc.AppearanceComponent,
                dialogLine.DialogKit
            );
        }

        // Avoid string interpolation allocation in non-error path
        LogManager.Error(string.Concat("[DialogSpeakerCache] No Character or NPC found for speakerId: ", dialogLine.SpeakerId));
        return null;
    }

    private Speaker AddToCache(
        string id,
        LocalizationComponentString localizationComponent,
        CharacterComponentAppearance appearanceComponent,
        DialogKit dialogKit)
    {
        // Evict LRU entry if cache is full
        if (speakerCacheDict.Count >= CACHE_SIZE)
        {
            EvictOldestEntry();
        }

        Speaker newSpeaker = new Speaker(id, localizationComponent, appearanceComponent, dialogKit);
        var kvp = new KeyValuePair<string, Speaker>(id, newSpeaker);
        var node = lruList.AddFirst(kvp);
        speakerCacheDict[id] = node;
        return newSpeaker;
    }

    private void EvictOldestEntry()
    {
        var lastNode = lruList.Last;
        if (lastNode != null)
        {
            speakerCacheDict.Remove(lastNode.Value.Key);
            lruList.RemoveLast();
        }
    }

    public void ClearCache()
    {
        speakerCacheDict.Clear();
        lruList.Clear();
        speaker = null;
    }
}
