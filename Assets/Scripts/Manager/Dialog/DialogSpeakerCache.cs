using UnityEngine;
using System.Collections.Generic;
using Aremoreno.Enums.Localization;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Kit;

public class DialogSpeakerCache : MonoBehaviour
{
    private const int CACHE_SIZE = 5;

    // LRU cache: LinkedList for order tracking + Dictionary for O(1) lookup
    private readonly Dictionary<string, LinkedListNode<KeyValuePair<string, Speaker>>> speakerCacheDict
        = new Dictionary<string, LinkedListNode<KeyValuePair<string, Speaker>>>(CACHE_SIZE);
    private readonly LinkedList<KeyValuePair<string, Speaker>> lruList
        = new LinkedList<KeyValuePair<string, Speaker>>();

    // Reusable objects to avoid per-resolve allocations
    private CharacterData reusableCharacterData;
    private NpcData reusableNpcData;

    private string id;
    private string portraitCharacterAddress;
    private string portraitKitAddress;
    private LocalizationEntity localizationEntity;
    private LocalizationField localizationField;
    private bool hasKit;
    private string kitId;
    private Variant variant;
    private Role role;
    private bool hasData;

    private Speaker speaker;
    public Speaker Speaker => speaker;
    private DatabaseManager database;

    private void Start() 
    {
        database = DatabaseManager.Instance;
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
        // shared
        hasData = false;
        hasKit = dialogLine.DialogKit != null && dialogLine.DialogKit.KitId != "none";

        // character path
        reusableCharacterData = database.GetCharacterData(dialogLine.SpeakerId);
        if (reusableCharacterData != null)
        {
            hasData = true;
            id = reusableCharacterData.CharacterId;
            localizationEntity = LocalizationEntity.Character;
            localizationField = LocalizationField.Nick;
            portraitCharacterAddress = AddressableLoader.GetCharacterPortraitAddress(id);

            if (hasKit) 
            {
                if (dialogLine.DialogKit.KitId == "default")
                {
                    kitId = TeamManager.Instance.ActiveLoadout.Kit.KitId;
                    variant = Variant.Home;
                    role = Role.Field;
                } else {
                    kitId = dialogLine.DialogKit.KitId;
                    variant = EnumManager.StringToEnum<Variant>(dialogLine.DialogKit.VariantId);
                    role = EnumManager.StringToEnum<Role>(dialogLine.DialogKit.RoleId);
                }
                portraitKitAddress = AddressableLoader.GetKitPortraitAddress(kitId, variant, role, reusableCharacterData.PortraitSize);
            }
        }

        // npc path
        if (!hasData) {
            reusableNpcData = database.GetNpcData(dialogLine.SpeakerId);
            if (reusableNpcData != null)
            {
                hasData = true;
                id = reusableNpcData.NpcId;
                localizationEntity = LocalizationEntity.Npc;
                localizationField = LocalizationField.Name;
                portraitCharacterAddress = AddressableLoader.GetNpcPortraitAddress(id);
            }
        }

        if (hasData)
            return AddToCache(id, localizationEntity, localizationField, portraitCharacterAddress, portraitKitAddress, hasKit);
        else
            LogManager.Error(string.Concat("[DialogSpeakerCache] No Character or NPC found for speakerId: ", dialogLine.SpeakerId));

        return null;
    }

    private Speaker AddToCache(
        string id, 
        LocalizationEntity localizationEntity,
        LocalizationField localizationField,
        string portraitCharacterAddress,
        string portraitKitAddress,
        bool hasKit
    )
    {
        // Evict LRU entry if cache is full
        if (speakerCacheDict.Count >= CACHE_SIZE)
        {
            EvictOldestEntry();
        }

        Speaker newSpeaker = new Speaker(id, localizationEntity, localizationField, portraitCharacterAddress, portraitKitAddress, hasKit);
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
