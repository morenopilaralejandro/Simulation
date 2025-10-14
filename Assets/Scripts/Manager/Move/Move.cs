using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Simulation.Enums.Character;
using Simulation.Enums.Move;
using Simulation.Enums.Localization;

public class Move
{
    private MoveComponentAttributes attributesComponent;
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
        attributesComponent = new MoveComponentAttributes(moveData, this);

        localizationStringComponent = new LocalizationComponentString(
            LocalizationEntity.Move,
            moveData.MoveId,
            new [] { LocalizationField.Name, LocalizationField.Description }
        );

        participantsComponent = new MoveComponentParticipants(moveData, this);
        evolutionComponent = new MoveComponentEvolution(moveData, this);
        //restrictionLearnComponent = new MoveComponentRestrictionLearn(moveData);
        restrictionParticipantsComponent = new MoveComponentRestrictionParticipants(moveData, this);
    }

    #region API
    //attributesComponent
    public string MoveId => attributesComponent.MoveId;
    public Category Category => attributesComponent.Category;
    public Element Element => attributesComponent.Element;
    public Trait Trait => attributesComponent.Trait;

    public int Cost => attributesComponent.Cost;
    public int BasePower => attributesComponent.BasePower;
    public int StunDamage => attributesComponent.StunDamage;
    public int AuraDamage => attributesComponent.AuraDamage;
    public int Difficulty => attributesComponent.Difficulty;
    public int FaultRate => attributesComponent.FaultRate;
    //localizationComponent
    public string MoveName => localizationStringComponent.GetString(LocalizationField.Name);
    public string MoveDescription => localizationStringComponent.GetString(LocalizationField.Description);
    //participantsComponent
    public int TotalParticipantCount => participantsComponent.TotalParticipantCount;
    public int RequiredParticipantCount => participantsComponent.RequiredParticipantCount;
    public Character[] SelectedParticipants => participantsComponent.SelectedParticipants;
    public List<Character> FinalParticipants => participantsComponent.FinalParticipants;
    public bool TryFinalizeParticipants(Character user, List<Character> teammates) => participantsComponent.TryFinalizeParticipants(user, teammates);
    public void SetSelectedParticipant(Character character, int index) => participantsComponent.SetSelectedParticipant(character, index);
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
