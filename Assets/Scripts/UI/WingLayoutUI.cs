using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;
using TMPro;
using Aremoreno.Enums.Wing;

public class WingLayoutUI : MonoBehaviour
{
    #region Field

    [Header("UI References")]
    [SerializeField] private WingSlotUI wingSlot;
    [SerializeField] private TMP_Text textName;
    [SerializeField] private LocalizedString defaultWingName;

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
        textName.text = defaultWingName.GetLocalizedString();
    }

    #endregion

    #region Helpers

    public void Populate()
    {
        wingSlot.Initialize(character);
        wingSlot.SetWing(character.Wing);
        if (character.HasWingEquipped) 
        {
            textName.text = character.Wing.WingName;
        } else 
        {
            textName.text = defaultWingName.GetLocalizedString();
        }
    }

    #endregion
}
