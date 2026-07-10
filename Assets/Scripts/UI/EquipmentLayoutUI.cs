using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Item;

public class EquipmentLayoutUI : MonoBehaviour
{
    #region Field

    [Header("UI References")]
    [SerializeField] private EquipmentSlotUI equipmentSlot0;
    [SerializeField] private EquipmentSlotUI equipmentSlot1;
    [SerializeField] private EquipmentSlotUI equipmentSlot2;
    [SerializeField] private EquipmentSlotUI equipmentSlot3;

    private EquipmentSlotUI[] equipmentSlots;
    private Character character;

    #endregion

    #region Lifecycle

    private void Awake()
    {
        equipmentSlots = new EquipmentSlotUI[]
        {
            equipmentSlot0,
            equipmentSlot1,
            equipmentSlot2,
            equipmentSlot3
        };
    }

    private void Start()
    {

    }

    private void OnDestroy()
    {

    }

    #endregion

    #region Initialize

    public void Initialize(Character character)
    {
        this.character = character;

        Clear();

        if (character == null) return;
    }

    public void Clear()
    {
        for (int i = 0; i < equipmentSlots.Length; i++)
        {
            equipmentSlots[i].Clear();
        }
    }

    #endregion

    #region Helpers

    /// <summary>
    /// Populates the fixed UI slots from the character's equipped equipment list.
    /// The equipped list is compact (no nulls/gaps), so we fill slots
    /// sequentially and reset any remaining slots beyond the equipped count.
    /// </summary>
    public void Populate()
    {
        for (int i = 0; i < 4; i++)
        {
            equipmentSlots[i].Initialize(character.GetEquipment(i), character, i);
            equipmentSlots[i].SetEquipment(character.GetEquipment(i));
        }
    }

    /*

    /// <summary>
    /// Finds the slot index currently displaying the given equipment.
    /// Returns -1 if no slot holds that equipment.
    /// </summary>
    private int GetSlotIndexByEquipment(Equipment equipment)
    {
        if (equipment == null) return -1;

        for (int i = 0; i < equipmentSlots.Length; i++)
        {
            if (equipmentSlots[i].Equipment != null && equipmentSlots[i].Equipment == equipment)
                return i;
        }

        return -1;
    }

    */

    #endregion

    /*

    #region Events

    private void OnEnable()
    {

    }

    private void OnDisable()
    {

    }

    #endregion

    */
}
