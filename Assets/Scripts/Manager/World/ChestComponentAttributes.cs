using UnityEngine;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.World;

public class ChestComponentAttributes
{
    public string ChestId { get; private set; }

    public ChestComponentAttributes(string chestId)
    {
        Initialize(chestId);
    }

    public void Initialize(string chestId)
    {
        ChestId = chestId;
    }
}
