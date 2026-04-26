using UnityEngine;
using Aremoreno.Enums.Character;

public class NpcComponentAttributes
{
    public string NpcId  { get; private set; }
    public Gender Gender { get; private set; }

    public NpcComponentAttributes(NpcData npcData)
    {
        Initialize(npcData);
    }

    public void Initialize(NpcData npcData)
    {
        NpcId = npcData.NpcId;
        Gender = npcData.Gender;
    }
}
