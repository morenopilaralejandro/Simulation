using UnityEngine;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.World;

public class NpcComponentAttributes
{
    public string NpcId { get; private set; }
    public Gender Gender { get; private set; }
    public NpcType NpcType { get; private set; }

    public NpcComponentAttributes(NpcData npcData, CharacterData characterData, NpcType npcType)
    {
        Initialize(npcData, characterData, npcType);
    }

    public void Initialize(NpcData npcData, CharacterData characterData, NpcType npcType)
    {
        NpcType = npcType;

        if (npcData != null)
        {
            NpcId = npcData.NpcId;
            Gender = npcData.Gender;
        } else 
        {
            NpcId = characterData.CharacterId;
            Gender = characterData.Gender;
        }
    }
}
