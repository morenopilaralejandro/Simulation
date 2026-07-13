using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Move;

public class WingSlotUI : MonoBehaviour
{
    #region Field

    [Header("UI Elements")]
    [SerializeField] private Image imageIcon;

    private Character character;
    public Character Character => character;

    private Wing wing;
    public Wing Wing => wing;

    #endregion

    #region Lifecycle

    #endregion

    #region Initialize

    public void Initialize(Character character)
    {
        this.character = character;
    }

    #endregion

    #region Helpers

    public void SetWing(Wing wing)
    {
        this.wing = wing;
        if (wing == null) return;

        imageIcon.color = ColorManager.GetWingColor(wing.WingColorType);
    }

    public void Clear()
    {   
        wing = null;
        imageIcon.color = Color.white;
    }

    #endregion

    #region Button Handle

    public void OnClick()
    {
        UIEvents.RaiseWingSlotUIClicked(this);
    }

    #endregion

}
