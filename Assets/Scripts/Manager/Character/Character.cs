using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Simulation.Enums.Character;
using Simulation.Enums.Localization;

public class Character : MonoBehaviour
{
    #region Components
    [SerializeField] private CharacterComponentAttributes attributesComponent;
    [SerializeField] private LocalizationComponentString localizationStringComponent;
    [SerializeField] private CharacterComponentTeamMember teamMemberComponent;
    [SerializeField] private CharacterComponentAppearance appearanceComponent;
    [SerializeField] private CharacterComponentKeeper keeperComponent;
    [SerializeField] private CharacterComponentLevels levelsComponent;
    [SerializeField] private CharacterComponentStats statsComponent;
    [SerializeField] private CharacterComponentTraining trainingComponent;
    [SerializeField] private CharacterComponentFatigue fatigueComponent;
    [SerializeField] private CharacterComponentSpeed speedComponent;
    [SerializeField] private CharacterComponentStatusEffects statusEffectsComponent;

    [SerializeField] private SpeechBubble speechBubble;
    #endregion

    /*
    character control window
    character kick window
    [SerializeField] private CharacterStatusController status; //stun

    [SerializeField] private SecretLearning secrets; //change to character move component
    [SerializeField] private CharacterPersistenceComponent persistence; //
    [SerializeField] private CharacterAiComponent ai; //ally and opponent
    */

    #region Initialize
    public void Initialize(CharacterData characterData)
    {
        attributesComponent.Initialize(characterData, this);
        localizationStringComponent = new LocalizationComponentString(
            LocalizationEntity.Character,
            characterData.CharacterId,
            new [] { LocalizationField.Name, LocalizationField.Nick, LocalizationField.Description }            
        );
        teamMemberComponent.Initialize(characterData, this);
        appearanceComponent.Initialize(characterData, this);
        keeperComponent.Initialize(characterData, this);
        levelsComponent.Initialize(characterData, this);
        statsComponent.Initialize(characterData, this);
        trainingComponent.Initialize(characterData, this);
        fatigueComponent.Initialize(characterData, this);
        speedComponent.Initialize(characterData, this);
        statusEffectsComponent.Initialize(characterData, this);

        /*
        if (isSave)
            persistence.Apply(save);
        else
            stats.ApplyLevel(def.StartingLevel);        
        
        health.OnHpChanged += hp =>
        {
            speedDebuff.OnHpChanged(hp);
        };
        */
    }
    #endregion

    #region API
    //attributeComponent
    public string CharacterId => attributesComponent.CharacterId;
    public CharacterSize CharacterSize => attributesComponent.CharacterSize;
    public Gender Gender => attributesComponent.Gender;
    public Element Element => attributesComponent.Element;
    public Position Position => attributesComponent.Position;
    //localizationComponent
    public string CharacterName => localizationStringComponent.GetString(LocalizationField.Name);
    public string CharacterNick => localizationStringComponent.GetString(LocalizationField.Nick);
    public string CharacterDescription => localizationStringComponent.GetString(LocalizationField.Description);
    //teamMemberComponent
    public TeamSide TeamSide => teamMemberComponent.TeamSide;
    public FormationCoord FormationCoord => teamMemberComponent.FormationCoord;
    public bool IsOnUsersTeam() => teamMemberComponent.IsOnUsersTeam();
    public bool IsSameTeam(Character otherCharacter) => teamMemberComponent.IsSameTeam(otherCharacter);
    public TeamSide GetOpponentSide() => teamMemberComponent.GetOpponentSide();
    //appearanceComponent
    public Sprite CharacterPortraitSprite => appearanceComponent.CharacterPortraitSprite;
    public Sprite KitPortraitSprite => appearanceComponent.KitPortraitSprite;
    public void SetRenderersVisible(bool isVisible) => appearanceComponent.SetRenderersVisible(isVisible);
    //keeperComponent
    public bool IsKeeper => keeperComponent.IsKeeper;
    //levelsComponent
    public int Level => levelsComponent.Level;
    public int MaxLevel => CharacterComponentLevels.MAX_LEVEL;
    public void LevelUp() => levelsComponent.LevelUp();
    //statsComponent    
    public int GetTrainedStat(Stat stat) => statsComponent.GetTrainedStat(stat);
    public int GetTrueStat(Stat stat) => statsComponent.GetTrueStat(stat);
    public int GetBattleStat(Stat stat) => statsComponent.GetBattleStat(stat);
    public void ResetBattleStats() => statsComponent.ResetBattleStats();
    public void ResetTrainedStats() => statsComponent.ResetTrainedStats();
    public void ModifyBattleStat(Stat stat, int amount) => statsComponent.ModifyBattleStat(stat, amount);
    public void ModifyTrainedStat(Stat stat, int amount) => statsComponent.ModifyTrainedStat(stat, amount);
    public void UpdateStats() => statsComponent.UpdateStats();
    //trainingComponent
    public int MaxTrainingPerStat => CharacterComponentTraining.MAX_TRAINING_PER_STAT;
    public int BaseFreedom => trainingComponent.BaseFreedom;
    public int TrueFreedom => trainingComponent.TrueFreedom;
    public void TrainStat(Stat stat, int amount) => trainingComponent.TrainStat(stat, amount);
    public bool IsCharacterTrainable(Stat stat) => trainingComponent.IsCharacterTrainable(stat);
    public bool IsStatTrainable(Stat stat) => trainingComponent.IsStatTrainable(stat);
    public int GetRemainingTrainingByStat(Stat stat) => trainingComponent.GetRemainingTrainingByStat(stat);
    public void ResetTraining() => trainingComponent.ResetTraining();
    //fatigueComponent
    public FatigueState FatigueState => fatigueComponent.FatigueState;
    public float FatigueSpeedMultiplier => fatigueComponent.FatigueSpeedMultiplier;
    public void UpdateFatigue() => fatigueComponent.UpdateFatigue();
    //speedComponent
    public float GetMovementSpeed() => speedComponent.GetMovementSpeed();
    //statusEffectsComponent
    public HashSet<StatusEffect> ActiveStatusEffects => statusEffectsComponent.ActiveStatusEffects;
    public float StatusSpeedMultiplier => statusEffectsComponent.StatusSpeedMultiplier;
    public void ApplyStatus(StatusEffect effect) => statusEffectsComponent.ApplyStatus(effect);
    public void ClearStatus(StatusEffect effect) => statusEffectsComponent.ClearStatus(effect);
    public void ClearAllStatus() => statusEffectsComponent.ClearAllStatus();
    public bool HasStatusEffect() => ActiveStatusEffects.Count != 0;

    //speechBubble
    public void ShowMessage(BubbleMessage bubbleMessage) => speechBubble.ShowMessage(bubbleMessage);
    public void HideSpeechBubbleImmediate() => speechBubble.HideImmediate();


    //misc
    #endregion

}
