using System.Collections.Generic;
using UnityEngine;

public class EmblemComponentAttributes
{
    public string EmblemId { get; private set; }
    public string EmblemAddress { get; private set; }

    public EmblemComponentAttributes(EmblemData emblemData, Emblem emblem)
    {
        Initialize(emblemData, emblem);
    }

    public void Initialize(EmblemData emblemData, Emblem emblem)
    {
        EmblemId = emblemData.EmblemId;
        //EmblemAddress = AddressableLoader.GetTeamEmblemAddress(EmblemId);
        EmblemAddress = EmblemId;
    }
}
