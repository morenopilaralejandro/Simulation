using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.UI;
using Aremoreno.Enums.Input;

public class SelectorTeamEmblemData
{
    public string EmblemId { get; private set; }
    public Sprite EmblemSprite { get; private set; }

    public SelectorTeamEmblemData(string emblemId, Sprite emblemSprite) 
    {
        EmblemId = emblemId;
        EmblemSprite = emblemSprite;
    }
}
