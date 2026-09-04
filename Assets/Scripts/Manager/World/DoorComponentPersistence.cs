using UnityEngine;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.World;

public class DoorComponentPersistence
{
    private Door door;

    public bool IsPersistent { get; private set; }

    public DoorComponentPersistence(Door door, bool isPersistent)
    {
        Initialize(door, isPersistent);
    }

    public void Initialize(Door door, bool isPersistent)
    {
        this.door = door;
        IsPersistent = isPersistent;
    }

    public bool IsOpenedPersistent => IsPersistent && StorySystemManager.Instance.GetFlag(door.DoorId);
    public void OpenPersistent() => StorySystemManager.Instance.SetFlag(door.DoorId, true);
}
