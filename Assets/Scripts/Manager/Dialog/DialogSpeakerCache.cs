using UnityEngine;
using System.Collections.Generic;

public class DialogSpeakerCache : MonoBehaviour
{
    private const int CACHE_SIZE = 5;
    private Dictionary<string, Speaker> speakerCacheDict = new Dictionary<string, Speaker>();

    private Speaker speaker;
    public Speaker Speaker => speaker;

    public Speaker GetSpeakerById(string speakerId)
    {
        // Try to get from cache first
        if (speakerCacheDict.TryGetValue(speakerId, out Speaker cachedSpeaker))
        {
            speaker = cachedSpeaker;
            return speaker;
        }

        // If not in cache, resolve and cache it
        speaker = ResolveSpeaker(speakerId);
        return speaker;
    }

    private Speaker ResolveSpeaker(string speakerId)
    {
        // Try Character first
        Character character = new Character(CharacterManager.Instance.GetCharacterData(speakerId));
        if (character != null)
        {
            return AddToCache(
                character.CharacterId,
                character.LocalizationComponent,
                character.AppearanceComponent
            );
        }

        // Fall back to NPC
        Npc npc = new Npc(NpcManager.Instance.GetNpcData(speakerId));
        if (npc != null)
        {
            return AddToCache(
                npc.NpcId,
                npc.LocalizationComponent,
                npc.AppearanceComponent
            );
        }

        Debug.LogWarning($"[DialogSpeakerCache] No Character or NPC found for speakerId: {speakerId}");
        return null;
    }

    private Speaker AddToCache(
        string id,
        LocalizationComponentString localizationComponent,
        CharacterComponentAppearance appearanceComponent)
    {
        // Evict oldest entry if cache is full
        if (speakerCacheDict.Count >= CACHE_SIZE)
        {
            EvictOldestEntry();
        }

        Speaker newSpeaker = new Speaker(id, localizationComponent, appearanceComponent);
        speakerCacheDict[id] = newSpeaker;
        return newSpeaker;
    }

    private void EvictOldestEntry()
    {
        // Dictionary doesn't guarantee order, so we use an enumerator to remove the first entry
        // For a proper LRU cache, consider using a LinkedList + Dictionary combo
        using (var enumerator = speakerCacheDict.GetEnumerator())
        {
            if (enumerator.MoveNext())
            {
                speakerCacheDict.Remove(enumerator.Current.Key);
            }
        }
    }

    public void ClearCache()
    {
        speakerCacheDict.Clear();
        speaker = null;
    }
}
