using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Simulation.Enums.Character;
using Simulation.Enums.SpriteLayer;
using Simulation.Enums.Kit;
using Simulation.Enums.Move;
using Simulation.Enums.Duel;
using Simulation.Enums.Battle;
using Simulation.Enums.Localization;

public class Character : MonoBehaviour
{
    #region Components
    [SerializeField] private CharacterComponentAttributes attributesComponent;
    [SerializeField] private LocalizationComponentString localizationStringComponent;
    [SerializeField] private CharacterComponentTeamMember teamMemberComponent;
    [SerializeField] private CharacterComponentAppearance appearanceComponent;
    [SerializeField] private CharacterComponentModel modelComponent;
    [SerializeField] private CharacterComponentKeeper keeperComponent;
    [SerializeField] private CharacterComponentLevels levelsComponent;
    [SerializeField] private CharacterComponentStats statsComponent;
    [SerializeField] private CharacterComponentTraining trainingComponent;
    [SerializeField] private CharacterComponentFatigue fatigueComponent;
    [SerializeField] private CharacterComponentSpeed speedComponent;
    [SerializeField] private CharacterComponentStatusEffects statusEffectsComponent;
    [SerializeField] private CharacterComponentStatusIndicator statusIndicatorComponent;
    [SerializeField] private CharacterComponentMoves movesComponent;
    [SerializeField] private CharacterComponentController controllerComponent;
    [SerializeField] private CharacterComponentAI aiComponent;
    [SerializeField] private CharacterComponentStateLock stateLockComponent;
    [SerializeField] private CharacterComponentStateMachine stateMachineComponent;

    [SerializeField] private CharacterComponentTeamIndicator teamIndicatorComponent;
    [SerializeField] private CharacterComponentElementIndicator elementIndicatorComponent;
    [SerializeField] private CharacterComponentSpeechBubble speechBubbleComponent;
    #endregion

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
        statusIndicatorComponent.Initialize(characterData, this);
        movesComponent.Initialize(characterData, this);
        controllerComponent.Initialize(characterData, this);
        aiComponent.Initialize(characterData, this);
        stateLockComponent.Initialize(characterData, this);
        stateMachineComponent.Initialize(characterData, this);

        teamIndicatorComponent.Initialize(characterData, this);
        elementIndicatorComponent.Initialize(characterData, this);
        speechBubbleComponent.Initialize(characterData, this);
        /*
        if (isSave)
            persistence.Apply(save);
        else
            stats.ApplyLevel(def.StartingLevel);               
        */
    }
    #endregion

    #region API
    //attributesComponent
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
    public Team GetTeam() => teamMemberComponent.GetTeam();
    public Team GetOpponentTeam() => teamMemberComponent.GetOpponentTeam();
    public List<Character> GetTeammates() => teamMemberComponent.GetTeammates();
    public List<Character> GetOpponents() => teamMemberComponent.GetOpponents();
    //appearanceComponent
    public void SetCharacterVisible(bool isVisible) => appearanceComponent.SetCharacterVisible(isVisible);
    public Role GetKitRole() => appearanceComponent.GetKitRole();
    public Variant GetKitVariant() => appearanceComponent.GetKitVariant();
    public Sprite PortraitSprite => appearanceComponent.PortraitSprite;
    public PortraitSize PortraitSize => appearanceComponent.PortraitSize;
    public SpriteLayerState<CharacterSpriteLayer> SpriteLayerState => appearanceComponent.SpriteLayerState;
    //modelComponent
    public Transform Model => modelComponent.Model;
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
    public bool IsStunned() => ActiveStatusEffects.Contains(StatusEffect.Stunned);
    //statusIndicatorComponent
    public void UpdateStatusIndicator(StatusEffect? newStatus) => statusIndicatorComponent.UpdateStatusIndicator(newStatus);
    // movesComponent
    public int MaxEquippedMovesDefault => CharacterComponentMoves.MAX_EQUIPPED_MOVES_DEFAULT;
    public int MaxEquippedMovesFusion => CharacterComponentMoves.MAX_EQUIPPED_MOVES_FUSION;
    public IReadOnlyList<MoveLearnsetData> LevelUpLearnset => movesComponent.LevelUpLearnset;
    public IReadOnlyList<Move> LearnedMoves => movesComponent.LearnedMoves;
    public IReadOnlyList<Move> EquippedMoves => movesComponent.EquippedMoves;
    public void CheckLearnMoveOnLevelUp() => movesComponent.CheckLearnMoveOnLevelUp();
    public void LearnMove(string moveId) => movesComponent.LearnMove(moveId);
    public void EquipMove(Move move) => movesComponent.EquipMove(move);
    public void UnequipMove(Move move) => movesComponent.UnequipMove(move);
    public bool IsMoveEquipped(string moveId) => movesComponent.IsMoveEquipped(moveId);
    public bool IsMoveEquipped(Move move) => movesComponent.IsMoveEquipped(move);
    public bool IsMoveLearned(string moveId) => movesComponent.IsMoveLearned(moveId);
    public bool IsMoveLearned(Move move) => movesComponent.IsMoveLearned(move);
    public bool CanLearnMove(string moveId) => movesComponent.CanLearnMove(moveId);
    public bool CanPerformeMove(Move move) => movesComponent.CanPerformeMove(move);
    public bool CanAffordMove(Move move) => movesComponent.CanAffordMove(move);
    public bool HasAffordableMove() => movesComponent.HasAffordableMove();
    public bool HasAffordableMoveWithCategory(Category category) => movesComponent.HasAffordableMoveWithCategory(category);
    public bool HasAffordableMoveWithTrait(Trait trait) => movesComponent.HasAffordableMoveWithTrait(trait);
    public Move GetRandomAffordableMoveByCategory(Category category) => movesComponent.GetRandomAffordableMoveByCategory(category);
    public Move GetStrongestAffordableMoveByCategory(Category category) => movesComponent.GetStrongestAffordableMoveByCategory(category);
    public Move GetRandomAffordableMoveByTrait(Trait trait) => movesComponent.GetRandomAffordableMoveByTrait(trait);
    public Move GetStrongestAffordableMoveByTrait(Trait trait) => movesComponent.GetStrongestAffordableMoveByTrait(trait);
    public List<Move> GetEquippedMovesByCategory(Category category) => movesComponent.GetEquippedMovesByCategory(category);
    public List<Move> GetEquippedMovesByTrait(Trait trait) => movesComponent.GetEquippedMovesByTrait(trait);
    //controllerComponent
    public bool IsControlled => controllerComponent.IsControlled;
    //aiComponent
    public bool IsEnemyAI => aiComponent.IsEnemyAI;
    public bool IsAIEnabled => aiComponent.IsAIEnabled;
    public bool IsAutoBattleEnabled => aiComponent.IsAutoBattleEnabled;
    public void EnableAI() => aiComponent.EnableAI();
    public void EnableAI(bool isAIEnabled) => aiComponent.EnableAI(isAIEnabled);
    public void DisableAI() => aiComponent.DisableAI();
    public AIDifficulty AIDifficulty => aiComponent.AIDifficulty;
    public AIState AIState => aiComponent.AIState;
    public DuelCommand GetRegularCommand() => aiComponent.GetRegularCommand();
    public DuelCommand GetCommandByCategory(Category category) => aiComponent.GetCommandByCategory(category);
    public DuelCommand GetCommandByTrait(Trait trait) => aiComponent.GetCommandByTrait(trait);
    public Move GetMoveByCommandAndCategory(DuelCommand command, Category category) => aiComponent.GetMoveByCommandAndCategory(command, category);
    public Move GetMoveByCommandAndTrait(DuelCommand command, Trait trait) => aiComponent.GetMoveByCommandAndTrait(command, trait);
    //stateLockComponent
    public bool IsStateLocked => stateLockComponent.IsStateLocked;
    public void StartStateLock(CharacterState state) => stateLockComponent.StartStateLock(state);
    public void ReleaseStateLock() => stateLockComponent.ReleaseStateLock();
    //stateMachineComponent
    public CharacterState CurrentState => stateMachineComponent.CurrentState;
    public void SetCharacterState(CharacterState state) => stateMachineComponent.SetCharacterState(state);
    public void StartKick() => stateMachineComponent.StartKick();
    public void StartControl() => stateMachineComponent.StartControl();
    public void StartMove() => stateMachineComponent.StartMove();
    public void OnKickAnimationEnd() => stateMachineComponent.OnKickAnimationEnd();
    public void OnControlAnimationEnd() => stateMachineComponent.OnControlAnimationEnd();
    public void OnMoveAnimationEnd() => stateMachineComponent.OnMoveAnimationEnd();




    //elementIndicatorComponent
    public void SetElementIndicatorEnabled(bool enabled) => elementIndicatorComponent.SetEnabled(enabled);
    public void SetElementIndicatorActive(bool active) => elementIndicatorComponent.SetActive(active);
    //speechBubbleComponent
    public void ShowSpeechBubble(SpeechMessage speechMessage) => speechBubbleComponent.ShowSpeechBubble(speechMessage);
    public void HideSpeechBubble() => speechBubbleComponent.HideSpeechBubble();

    //ball
    public void KickBallTo(Vector3 targetPos) 
    {
        BattleManager.Instance.Ball.KickBallTo(targetPos);
        BattleEvents.RaisePassPerformed(this);
        StartKick();
    }
    public bool HasBall() => PossessionManager.Instance.CurrentCharacter == this;
    public bool CanGainBall() => BattleManager.Instance.Ball.IsFree() && !PossessionManager.Instance.IsOnCooldown(this) && !IsStunned();
    public bool CanShoot() => GoalManager.Instance.IsInShootDistance(this) || HasAffordableMoveWithTrait(Trait.Long);
    public bool IsInOwnPenaltyArea() => GoalManager.Instance.IsInOwnPenaltyArea(this);
    //misc
    public bool CanMove() => !IsStunned();
    public bool CanDuel() => !IsStunned();
    
    #endregion

}
