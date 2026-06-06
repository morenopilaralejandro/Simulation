using UnityEngine;
using System.Collections.Generic;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Wing;

public class WingComponentEquip
{
    private Wing wing;

    public Character EquippedCharacter { get; private set; }

    public WingComponentEquip(WingData wingData, Wing wing, WingSaveData wingSaveData = null)
    {
        Initialize(wingData, wing, wingSaveData);
    }

    public void Initialize(WingData wingData, Wing wing, WingSaveData wingSaveData = null)
    {
        this.wing = wing;
        EquippedCharacter = null;

        //if (wingSaveData != null) 
    }

    public void SetEquippedCharacter(Character character) 
    {
        EquippedCharacter = character;
    }

    public bool IsEquipped() => EquippedCharacter != null;
}
