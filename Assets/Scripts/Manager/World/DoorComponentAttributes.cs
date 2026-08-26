using UnityEngine;
using Aremoreno.Enums.World;

public class DoorComponentAttributes
{
    public string DoorId { get; private set; }

    public DoorComponentAttributes(string doorId)
    {
        Initialize(doorId);
    }

    public void Initialize(string doorId)
    {
        DoorId = doorId;
    }
}
