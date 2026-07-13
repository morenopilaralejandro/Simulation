using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Aremoreno.Enums.Wing;

public class WingLayoutUI : MonoBehaviour
{
    #region Field

    [Header("UI References")]
    [SerializeField] private WingSlotUI wingSlot;

    private Character character;

    #endregion

    #region Lifecycle

    #endregion

    #region Initialize

    public void Initialize(Character character)
    {
        this.character = character;
        Clear();
    }

    public void Clear()
    {
        wingSlot.Clear();
    }

    #endregion

    #region Helpers

    public void Populate()
    {
        wingSlot.Initialize(character);
        wingSlot.SetWing(character.Wing);
    }

    #endregion
}
