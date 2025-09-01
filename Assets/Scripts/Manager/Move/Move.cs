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
    private ComponentLocalization localizationComponent;
    private MoveComponentParticipant participantComponent;
    private MoveComponentEvolution evolutionComponent;
    private MoveComponentRestrictionLearn restrictionLearnComponent;
    private MoveComponentRestrictionParticipant restrictionParticipantComponent;




    public void Initialize(MoveData moveData)
    {
        attributeComponent = new MoveComponentAttribute(moveData);

        localizationComponent = new ComponentLocalization(
            LocalizationEntity.Move,
            moveData.MoveId,
            new [] { LocalizationField.Name, LocalizationField.Description }
        );

        participantComponent = new MoveComponentParticipant(moveData);
        evolutionComponent = new MoveComponentEvolution(moveData);
        restrictionLearnComponent = new MoveComponentRestrictionLearn(moveData);
        restrictionParticipantComponent = new MoveComponentRestrictionParticipant();



        requiredParticipantElements = new List<Element>(moveData.RequiredParticipantElements);
        requiredParticipantMoves = new List<string>(moveData.RequiredParticipantMoves);
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
    public string MoveName => localizationComponent.GetString(LocalizationField.Name);
    public string MoveDescription => localizationComponent.GetString(LocalizationField.Description);
    //participantComponent
    public int Participants => participantComponent.Participants;
    public Character[] SelectedParticipants => participantComponent.SelectedParticipants;
    public void SetParticipant(int participantIndex, Character character) => participantComponent.SetParticipant(participantIndex, character);
    public bool IsParticipantSelected(int participantIndex) => participantComponent.IsParticipantSelected(participantIndex);
    public List<Character> GetFinalParticipants(List<Character> teammates) => participantComponent.GetFinalParticipants(teammates);
    //evolutionComponent
    public GrowthType GrowthType => evolutionComponent.GrowthType;
    public GrowthRate GrowthRate => evolutionComponent.GrowthRate;
    //restrictionLearnComponent
    public List<Element> AllowedElements => restrictionLearnComponent.AllowedElements;
    public List<Position> AllowedPositions => restrictionLearnComponent.AllowedPositions;
    public List<Gender> AllowedGenders => restrictionLearnComponent.AllowedGenders;
    public List<CharacterSize> AllowedSizes => restrictionLearnComponent.AllowedSizes;
    //restrictionParticipantComponent

    #endregion
}
