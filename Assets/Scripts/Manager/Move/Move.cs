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
    private ComponentLocalizationString stringLocalizationComponent;
    private MoveComponentParticipants participantsComponent;
    private MoveComponentEvolution evolutionComponent;
    //private MoveComponentRestrictionLearn restrictionLearnComponent;
    private MoveComponentRestrictionParticipants restrictionParticipantsComponent;

    public void Initialize(MoveData moveData)
    {
        attributeComponent = new MoveComponentAttribute(moveData);

        stringLocalizationComponent = new ComponentLocalizationString(
            LocalizationEntity.Move,
            moveData.MoveId,
            new [] { LocalizationField.Name, LocalizationField.Description }
        );

        participantsComponent = new MoveComponentParticipants(moveData);
        evolutionComponent = new MoveComponentEvolution(moveData);
        //restrictionLearnComponent = new MoveComponentRestrictionLearn(moveData);
        restrictionParticipantsComponent = new MoveComponentRestrictionParticipants(moveData);
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
    public string MoveName => stringLocalizationComponent.GetString(LocalizationField.Name);
    public string MoveDescription => stringLocalizationComponent.GetString(LocalizationField.Description);
    //participantsComponent
    public int TotalParticipantCount => participantsComponent.TotalParticipantCount;
    public int RequiredParticipantCount => participantsComponent.RequiredParticipantCount;
    public Character[] SelectedParticipants => participantsComponent.SelectedParticipants;
    public void SetParticipant(int participantIndex, Character character) => participantsComponent.SetParticipant(participantIndex, character);
    public bool IsParticipantSelected(int participantIndex) => participantsComponent.IsParticipantSelected(participantIndex);
    public List<Character> GetFinalParticipants(List<Character> teammates) => participantsComponent.GetFinalParticipants(teammates);
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
    public bool HasParticipantElementRestriction => restrictionParticipantsComponent.HasParticipantElementRestriction;
    public bool HasParticipantMoveRestriction => restrictionParticipantsComponent.HasParticipantMoveRestriction;
    public bool HasValidParticipantElements() => restrictionParticipantsComponent.HasValidParticipantElements(participantsComponent.SelectedParticipants);
    public bool HasValidParticipantMoves() => restrictionParticipantsComponent.HasValidParticipantElements(participantsComponent.SelectedParticipants);
    #endregion
}
