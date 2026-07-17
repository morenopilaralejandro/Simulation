using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Aremoreno.Enums.UI;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Item;

public class EquipmentStatLayout : MonoBehaviour
{
    #region Fields

    [Header("UI References - Kick")]
    [SerializeField] private TMP_Text textValueTrueKick;

    [Header("UI References - Body")]
    [SerializeField] private TMP_Text textValueTrueBody;

    [Header("UI References - Control")]
    [SerializeField] private TMP_Text textValueTrueControl;

    [Header("UI References - Guard")]
    [SerializeField] private TMP_Text textValueTrueGuard;

    [Header("UI References - Speed")]
    [SerializeField] private TMP_Text textValueTrueSpeed;

    [Header("UI References - Stamina")]
    [SerializeField] private TMP_Text textValueTrueStamina;

    [Header("UI References - Courage")]
    [SerializeField] private TMP_Text textValueTrueCourage;

    #endregion

    #region Lifecycle

    private void Start() 
    {
        Clear();
    }

    #endregion

    #region Initialize

    public void Clear()
    {
        textValueTrueKick.text      = "";
        textValueTrueBody.text      = "";
        textValueTrueControl.text   = "";
        textValueTrueGuard.text     = "";
        textValueTrueSpeed.text     = "";
        textValueTrueStamina.text   = "";
        textValueTrueCourage.text   = "";
    }

    #endregion

    #region Helpers

    public void Populate(ItemEquipment equipment)
    {
        textValueTrueKick.text      = equipment.EquipmentStats[Stat.Kick].ToString();
        textValueTrueBody.text      = equipment.EquipmentStats[Stat.Body].ToString();
        textValueTrueControl.text   = equipment.EquipmentStats[Stat.Control].ToString();
        textValueTrueGuard.text     = equipment.EquipmentStats[Stat.Guard].ToString();
        textValueTrueSpeed.text     = equipment.EquipmentStats[Stat.Speed].ToString();
        textValueTrueStamina.text   = equipment.EquipmentStats[Stat.Stamina].ToString();
        textValueTrueCourage.text   = equipment.EquipmentStats[Stat.Courage].ToString();
    }

    #endregion

    #region Events

    private void OnEnable()
    {
        UIEvents.OnEquipmentStatLayoutUpdateRequested += HandleEquipmentStatLayoutUpdateRequested;
    }

    private void OnDisable()
    {
        UIEvents.OnEquipmentStatLayoutUpdateRequested -= HandleEquipmentStatLayoutUpdateRequested;
    }

    private void HandleEquipmentStatLayoutUpdateRequested(ItemEquipment equipment) 
    {
        Populate(equipment);
    }

    #endregion
}
