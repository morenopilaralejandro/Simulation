using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.SpriteLayer;
using Aremoreno.Enums.Kit;
using Aremoreno.Enums.Move;
using Aremoreno.Enums.Duel;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Localization;

public class CharacterEntityBattle : MonoBehaviour
{
    #region Components
    [SerializeField] private Character character;
    [SerializeField] private CharacterComponentTeamMember teamMemberComponent;
    [SerializeField] private CharacterComponentAppearanceBattle appearanceBattleComponent;
    [SerializeField] private CharacterComponentModel modelComponent;
    [SerializeField] private CharacterComponentKeeper keeperComponent;
    [SerializeField] private CharacterComponentFatigue fatigueComponent;
    [SerializeField] private CharacterComponentStatusEffects statusEffectsComponent;
    [SerializeField] private CharacterComponentStatusIndicator statusIndicatorComponent;
    [SerializeField] private CharacterComponentSpeed speedComponent;
    [SerializeField] private CharacterComponentController controllerComponent;
    [SerializeField] private CharacterComponentAI aiComponent;
    [SerializeField] private CharacterComponentStateLock stateLockComponent;
    [SerializeField] private CharacterComponentStateMachine stateMachineComponent;
    [SerializeField] private CharacterComponentRigidbody rigidbodyComponent;

    [SerializeField] private CharacterComponentTeamIndicator teamIndicatorComponent;
    [SerializeField] private CharacterComponentElementIndicator elementIndicatorComponent;
    [SerializeField] private CharacterComponentSpeechBubble speechBubbleComponent;
    #endregion

    #region Initialize
    public void Initialize(CharacterData characterData, Character character = null)
    {
        if (character != null) 
        {
            this.character = character;
        } else 
        {
            this.character = new Character(characterData);
        }

        teamMemberComponent.Initialize(this);
        appearanceBattleComponent.Initialize(this);
        keeperComponent.Initialize(this);
        fatigueComponent.Initialize(this);
        statusEffectsComponent.Initialize(this);
        statusIndicatorComponent.Initialize(this);
        speedComponent.Initialize(this);
        controllerComponent.Initialize(this);
        aiComponent.Initialize(this);
        stateLockComponent.Initialize(this);
        stateMachineComponent.Initialize(this);
        teamIndicatorComponent.Initialize(this);
        elementIndicatorComponent.Initialize(characterData, this);
        speechBubbleComponent.Initialize(this.character);
    }

    #endregion

    #region API Character
    //character
    public Character Character => character;
    // attributesComponent
    public string CharacterId => character.CharacterId;
    public string CharacterGuid => character.CharacterGuid;
    public CharacterSize CharacterSize => character.CharacterSize;
    public PortraitSize PortraitSize => character.PortraitSize;
    public Gender Gender => character.Gender;
    public Element Element => character.Element;
    public Position Position => character.Position;

    // localizationComponent
    public string CharacterName => character.CharacterName;
    public string CharacterNick => character.CharacterNick;
    public string CharacterDescription => character.CharacterDescription;

    // levelsComponent
    public int Level => character.Level;
    public int CurrentExp => character.CurrentExp;
    public int ExpToNextLevel => character.ExpToNextLevel;
    public int MaxLevel => character.MaxLevel;
    public void SetLevel(int level) 
    {
        character.SetLevel(level);
        CalculateSpeed();
    }

    // statsComponent
    public int GetTrainedStat(Stat stat) => character.GetTrainedStat(stat);
    public int GetTrueStat(Stat stat) => character.GetTrueStat(stat);
    public int GetBattleStat(Stat stat) => character.GetBattleStat(stat);
    public void ResetBattleStats() => character.ResetBattleStats();
    public void ResetTrainedStats() => character.ResetTrainedStats();
    public void ModifyBattleStat(Stat stat, int amount) 
    {
        character.ModifyBattleStat(stat, amount);
        if (stat == Stat.Hp) UpdateFatigue();
    }
    public void ModifyTrainedStat(Stat stat, int amount) => character.ModifyTrainedStat(stat, amount);
    public void UpdateStats() => character.UpdateStats();

    // trainingComponent
    public int MaxTrainingPerStat => character.MaxTrainingPerStat;
    public int BaseFreedom => character.BaseFreedom;
    public int TrueFreedom => character.TrueFreedom;
    public void TrainStat(Stat stat, int amount) => character.TrainStat(stat, amount);
    public bool IsCharacterTrainable(Stat stat) => character.IsCharacterTrainable(stat);
    public bool IsStatTrainable(Stat stat) => character.IsStatTrainable(stat);
    public int GetRemainingTrainingByStat(Stat stat) => character.GetRemainingTrainingByStat(stat);
    public void ResetTraining() => character.ResetTraining();
    public int TrainingResetCount => character.TrainingResetCount;

    // movesComponent
    public int MaxEquippedMovesDefault => character.MaxEquippedMovesDefault;
    public int MaxEquippedMovesFusion => character.MaxEquippedMovesFusion;
    public IReadOnlyList<MoveLearnsetData> LevelUpLearnset => character.LevelUpLearnset;
    public IReadOnlyList<Move> LearnedMoves => character.LearnedMoves;
    public IReadOnlyList<Move> EquippedMoves => character.EquippedMoves;
    public void CheckLearnMoveOnLevelUp() => character.CheckLearnMoveOnLevelUp();
    public void LearnMove(string moveId) => character.LearnMove(moveId);
    public void EquipMove(Move move) => character.EquipMove(move);
    public void UnequipMove(Move move) => character.UnequipMove(move);
    public bool IsMoveEquipped(string moveId) => character.IsMoveEquipped(moveId);
    public bool IsMoveEquipped(Move move) => character.IsMoveEquipped(move);
    public bool IsMoveLearned(string moveId) => character.IsMoveLearned(moveId);
    public bool IsMoveLearned(Move move) => character.IsMoveLearned(move);
    public bool CanLearnMove(string moveId) => character.CanLearnMove(moveId);
    //public bool CanPerformeMove(Move move) => movesComponent.CanPerformeMove(move);
    public bool CanAffordMove(Move move) => character.CanAffordMove(move);
    public bool HasAffordableMove() => character.HasAffordableMove();
    public bool HasAffordableMoveWithCategory(Category category) => character.HasAffordableMoveWithCategory(category);
    public bool HasAffordableMoveWithTrait(Trait trait) => character.HasAffordableMoveWithTrait(trait);
    public Move GetRandomAffordableMoveByCategory(Category category) => character.GetRandomAffordableMoveByCategory(category);
    public Move GetStrongestAffordableMoveByCategory(Category category) => character.GetStrongestAffordableMoveByCategory(category);
    public Move GetRandomAffordableMoveByTrait(Trait trait) => character.GetRandomAffordableMoveByTrait(trait);
    public Move GetStrongestAffordableMoveByTrait(Trait trait) => character.GetStrongestAffordableMoveByTrait(trait);
    public List<Move> GetEquippedMovesByCategory(Category category) => character.GetEquippedMovesByCategory(category);
    public List<Move> GetEquippedMovesByTrait(Trait trait) => character.GetEquippedMovesByTrait(trait);
    public void ForceMaxEvolutionOnEquippedMoves() => character.ForceMaxEvolutionOnEquippedMoves();

    // persistenceComponent
    public void Import(CharacterSaveData characterSaveData) => character.Import(characterSaveData);
    public CharacterSaveData Export() => character.Export();

    // appearanceComponent
    public Sprite PortraitSprite => character.PortraitSprite;
    public string PortraitSpriteId => character.PortraitSpriteId;
    public HairStyle HairStyle => character.HairStyle;
    public HairColorType HairColorType => character.HairColorType;
    public EyeColorType EyeColorType => character.EyeColorType;
    public BodyColorType BodyColorType => character.BodyColorType;
    public SpriteLayerState<CharacterSpriteLayer> SpriteLayerState
    {
        get => character.SpriteLayerState;
        set => character.SpriteLayerState = value;
    }
    public void ApplyKit(Kit kit, Variant variant, Position position) => character.ApplyKit(kit, variant, position);
    public void InitializeVisibility() => character.InitializeVisibility();
    public Variant GetKitVariant(Team team) => character.GetKitVariant(team);
    public Role GetKitRole(Position position) => character.GetKitRole(position);
    #endregion

    #region API CharacterEntityBattle
    //teamMemberComponent
    public TeamSide TeamSide => teamMemberComponent.TeamSide;
    public FormationCoord FormationCoord => teamMemberComponent.FormationCoord;
    public bool IsOnUsersTeam() => teamMemberComponent.IsOnUsersTeam();
    public bool IsSameTeam(CharacterEntityBattle otherCharacter) => teamMemberComponent.IsSameTeam(otherCharacter);
    public TeamSide GetOpponentSide() => teamMemberComponent.GetOpponentSide();
    public Team GetTeam() => teamMemberComponent.GetTeam();
    public Team GetOpponentTeam() => teamMemberComponent.GetOpponentTeam();
    public List<CharacterEntityBattle> GetTeammates() => teamMemberComponent.GetTeammates();
    public List<CharacterEntityBattle> GetOpponents() => teamMemberComponent.GetOpponents();
    //appearanceBattleComponent
    public void SetCharacterVisible(bool isVisible) => appearanceBattleComponent.SetCharacterVisible(isVisible);
    //modelComponent
    public Transform Model => modelComponent.Model;
    //keeperComponent
    public bool IsKeeper => keeperComponent.IsKeeper;
    public bool HasBallInHand => keeperComponent.HasBallInHand;
    public void UpdateKeeperColliderState() => keeperComponent.UpdateKeeperColliderState();
    public void PunchBall(Trait trait) => keeperComponent.PunchBall(trait);
    public void ActivateBallInHand() => keeperComponent.ActivateBallInHand();
    //fatigueComponent
    public FatigueState FatigueState => fatigueComponent.FatigueState;
    public float FatigueSpeedMultiplier => fatigueComponent.FatigueSpeedMultiplier;
    public void UpdateFatigue() => fatigueComponent.UpdateFatigue();
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
    //speedComponent
    public float MovementSpeed => speedComponent.MovementSpeed;
    public void CalculateSpeed() => speedComponent.CalculateSpeed();

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
    public CharacterEntityBattle GetBestPassTeammate() => aiComponent.GetBestPassTeammate();
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
    //rigidbodyComponent
    public void ResetPhysics() => rigidbodyComponent.ResetPhysics();
    public void StopVelocity() => rigidbodyComponent.StopVelocity();
    public void Teleport(Vector3 position) => rigidbodyComponent.Teleport(position);

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
    public bool CanGainBall() => BattleManager.Instance.Ball.IsFree() && !PossessionManager.Instance.IsOnCooldown(this, Time.time) && !IsStunned();
    public bool CanShoot() => GoalManager.Instance.IsInShootDistance(this) || HasAffordableMoveWithTrait(Trait.Long);
    public bool IsInOwnPenaltyArea() => GoalManager.Instance.IsInOwnPenaltyArea(this);
    public bool HasBallInHandThrowIn;

    //misc
    public bool CanMove() => !IsStunned();
    public bool CanDuel() => !IsStunned() && !HasBallInHand;
    
    #endregion

}
