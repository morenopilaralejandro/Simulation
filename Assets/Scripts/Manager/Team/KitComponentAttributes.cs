using System.Collections.Generic;
using UnityEngine;

public class KitComponentAttributes
{
    public string KitId { get; private set; }

    public KitComponentAttributes(KitData kitData, Kit kit)
    {
        Initialize(kitData, kit);
    }

    public void Initialize(KitData kitData, Kit kit)
    {
        this.KitId = kitData.KitId;
    }
}
