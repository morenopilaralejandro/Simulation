using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Simulation.Enums.Character;
using Simulation.Enums.Move;
using Simulation.Enums.Localization;

public class Move
{
    private MoveComponentAttribute attributeComponent;
    private LocalizationComponentString localizationStringComponent;
    private MoveComponentParticipants participantsComponent;
    private MoveComponentEvolution evolutionComponent;
    //private MoveComponentRestrictionLearn restrictionLearnComponent;
    private MoveComponentRestrictionParticipants restrictionParticipantsComponent;

    public Move(MoveData moveData)
    {
        Initialize(moveData);
    }

    public void Initialize(MoveData moveData)
    {
        attributeComponent = new MoveComponentAttribute(moveData);

        localizationStringComponent = new LocalizationComponentString(
            LocalizationEntity.Move,
            moveData.MoveId,
            new [] { LocalizationField.Name, LocalizationField.Description }
        );

        participantsComponent = new MoveComponentParticipants(moveData, this);
        evolutionComponent = new MoveComponentEvolution(moveData);
        //restrictionLearnComponent = new MoveComponentRestrictionLearn(moveData);
        restrictionParticipantsComponent = new MoveComponentRestrictionParticipants(moveData, this);
    }

    #region API
    //attributeComponent
    public string MoveId => attributeComponent.MoveId;
    public Category Category => attributeComponent.Category;
    public Element Element => attributeComponent.Element;
    public Trait Trait => attributeComponent.Trait;

    public int Cost => attributeComponent.Cost;
    public int BasePower => attributeComponent.BasePower;
    public int StunDamage => attributeComponent.StunDamage;
    public int AuraDamage => attributeComponent.AuraDamage;
    public int Difficulty => attributeComponent.Difficulty;
    public int FaultRate => attributeComponent.FaultRate;
    //localizationComponent
    public string MoveName => localizationStringComponent.GetString(LocalizationField.Name);
    public string MoveDescription => localizationStringComponent.GetString(LocalizationField.Description);
    //participantsComponent
    public int TotalParticipantCount => participantsComponent.TotalParticipantCount;
    public int RequiredParticipantCount => participantsComponent.RequiredParticipantCount;
    public Character[] SelectedParticipants => participantsComponent.SelectedParticipants;
    public List<Character> FinalParticipants => participantsComponent.FinalParticipants;
    public bool TryFinalizeParticipants(Character user, List<Character> teammates) => participantsComponent.TryFinalizeParticipants(user, teammates);
    //evolutionComponent
    public GrowthType GrowthType => evolutionComponent.GrowthType;
    public GrowthRate GrowthRate => evolutionComponent.GrowthRate;
    //restrictionLearnComponent
    /*
    public List<Element> AllowedElements => restrictionLearnComponent.AllowedElements;
    public List<Position> AllowedPositions => restrictionLearnComponent.AllowedPositions;
    public List<Gender> AllowedGenders => restrictionLearnComponent.AllowedGenders;
    public List<CharacterSize> AllowedSizes => restrictionLearnComponent.AllowedSizes;
    */
    //restrictionParticipantsComponent
    public List<Element> RequiredParticipantElements => restrictionParticipantsComponent.RequiredParticipantElements;
    public List<string> RequiredParticipantMoves => restrictionParticipantsComponent.RequiredParticipantMoves;
    public bool IsCharacterValidForIndex(Character character, int index) => restrictionParticipantsComponent.IsCharacterValidForIndex(character, index);
    public bool MeetsAllParticipantRestrictions(Character[] participants) => restrictionParticipantsComponent.MeetsAllParticipantRestrictions(participants);
    #endregion
}
