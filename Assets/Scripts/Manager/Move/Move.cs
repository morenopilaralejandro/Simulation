using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using Simulation.Enums.Character;
using Simulation.Enums.Move;

public class Move
{
    #region Identification
    [Header("Identification")]
    [SerializeField] private string moveId;
    public string MoveId => moveId;

    [SerializeField] private LocalizedString localizedName;
    public LocalizedString LocalizedName => localizedName;

    [SerializeField] private LocalizedString localizedDescription;
    public LocalizedString LocalizedDescription => localizedDescription;
    #endregion

    #region Core Attributes
    [Header("Core Attributes")]
    [SerializeField] private Category category;
    public Category Category => category;

    [SerializeField] private Element element;
    public Element Element => element;

    [SerializeField] private Trait trait;
    public Trait Trait => trait;

    [SerializeField] private GrowthType growthType;
    public GrowthType GrowthType => growthType;

    [SerializeField] private GrowthRate growthRate;
    public GrowthRate GrowthRate => growthRate;

    [SerializeField] private int cost;
    public int Cost => cost;

    [SerializeField] private int basePower;
    public int BasePower => basePower;

    [SerializeField] private int stunDamage;
    public int StunDamage => stunDamage;

    [SerializeField] private int auraDamage;
    public int AuraDamage => auraDamage;

    [SerializeField] private int difficulty;
    public int Difficulty => difficulty;

    [SerializeField] private int faultRate;
    public int FaultRate => faultRate;

    [SerializeField] private int participants;
    public int Participants => participants;
    #endregion

    #region Restrictions
    [SerializeField] private List<Element> allowedElements = new();
    public IReadOnlyList<Element> AllowedElements => allowedElements;

    [SerializeField] private List<Position> allowedPositions = new();
    public IReadOnlyList<Position> AllowedPositions => allowedPositions;

    [SerializeField] private List<Gender> allowedGenders = new();
    public IReadOnlyList<Gender> AllowedGenders => allowedGenders;

    [SerializeField] private List<CharacterSize> allowedSizes = new();
    public IReadOnlyList<CharacterSize> AllowedSizes => allowedSizes;

    [SerializeField] private List<Element> requiredParticipantElements = new();
    public IReadOnlyList<Element> RequiredParticipantElements => requiredParticipantElements;

    [SerializeField] private List<string> requiredParticipantMoves = new();
    public IReadOnlyList<string> RequiredParticipantMoves => requiredParticipantMoves;

    public bool HasParticipantElementRestriction => RequiredParticipantElements.Count > 0;
    public bool HasParticipantMoveRestriction => RequiredParticipantMoves.Count > 0;
    #endregion

    #region Localization
    [Header("Localization")]
    [SerializeField] private bool isRomazed = false;
    [SerializeField] private string stringTableNameLocalized = "MoveNamesLocalized";
    [SerializeField] private string stringTableNameRomanized = "MoveNamesRomanized";
    [SerializeField] private string stringTableDescriptionLocalized = "MoveDescriptionsLocalized";
    [SerializeField] private string stringTableDescriptionRomanized = "MoveDescriptionsRomanized";
    #endregion

    public void Initialize(MoveData moveData)
    {
        moveId = moveData.MoveId;

        category = moveData.Category;
        element = moveData.Element;
        trait = moveData.Trait;
        growthType = moveData.GrowthType;
        growthRate = moveData.GrowthRate;

        cost = moveData.Cost;
        basePower = moveData.BasePower;
        stunDamage = moveData.StunDamage;
        auraDamage = moveData.AuraDamage;
        difficulty = moveData.Difficulty;
        faultRate = moveData.FaultRate;
        participants = moveData.Participants;

        allowedElements = new List<Element>(moveData.AllowedElements);
        allowedPositions = new List<Position>(moveData.AllowedPositions);
        allowedGenders = new List<Gender>(moveData.AllowedGenders);
        allowedSizes = new List<CharacterSize>(moveData.AllowedSizes);

        requiredParticipantElements = new List<Element>(moveData.RequiredParticipantElements);
        requiredParticipantMoves = new List<string>(moveData.RequiredParticipantMoves);

        SetName();
        SetDescription();
    }

    private void SetName()
    {
        localizedName = new LocalizedString(
            isRomazed ? stringTableNameRomanized : stringTableNameLocalized,
            moveId
        );
    }

    private void SetDescription()
    {
        localizedDescription = new LocalizedString(
            isRomazed ? stringTableDescriptionRomanized : stringTableDescriptionLocalized,
            moveId
        );
    }

}
