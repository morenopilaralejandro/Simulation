using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Kit;
using Aremoreno.Enums.UI;
using Aremoreno.Enums.Move;

public class MoveLayoutUI : MonoBehaviour
{
    #region Field

    [Header("UI References")]
    [SerializeField] private MoveSlotUI moveSlot0;
    [SerializeField] private MoveSlotUI moveSlot1;
    [SerializeField] private MoveSlotUI moveSlot2;
    [SerializeField] private MoveSlotUI moveSlot3;
    [SerializeField] private MoveSlotUI moveSlot4;
    [SerializeField] private MoveSlotUI moveSlot5;

    private MoveSlotUI[] moveSlots;
    private Character character;

    #endregion

    #region Lifecycle

    private void Awake()
    {
        moveSlots = new MoveSlotUI[]
        {
            moveSlot0,
            moveSlot1,
            moveSlot2,
            moveSlot3,
            moveSlot4,
            moveSlot5
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
        for (int i = 0; i < moveSlots.Length; i++)
        {
            moveSlots[i].Clear();
        }
    }

    #endregion

    #region Helpers

    /// <summary>
    /// Populates the fixed UI slots from the character's equipped moves list.
    /// The equipped list is compact (no nulls/gaps), so we fill slots
    /// sequentially and reset any remaining slots beyond the equipped count.
    /// </summary>
    public void Populate()
    {
        IReadOnlyList<Move> equippedMoves = character.EquippedMoves;

        for (int i = 0; i < moveSlots.Length; i++)
        {
            if (i < equippedMoves.Count)
            {
                moveSlots[i].Initialize(equippedMoves[i], character);
                moveSlots[i].SetMove(equippedMoves[i]);
            }
            else
            {
                moveSlots[i].Clear();
            }
        }
    }

    /// <summary>
    /// Finds the slot index currently displaying the given move.
    /// Returns -1 if no slot holds that move.
    /// </summary>
    private int GetSlotIndexByMove(Move move)
    {
        if (move == null) return -1;

        for (int i = 0; i < moveSlots.Length; i++)
        {
            if (moveSlots[i].Move != null && moveSlots[i].Move == move)
                return i;
        }

        return -1;
    }

    #endregion

    #region Events

    private void OnEnable()
    {

    }

    private void OnDisable()
    {

    }

    #endregion
}
