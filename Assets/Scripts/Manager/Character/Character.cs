using UnityEngine;
using System.Collections.Generic;
using Simulation.Enums.Character;
using Simulation.Enums.Kit;
using Simulation.Enums.Move;
using Simulation.Enums.Localization;
using Simulation.Enums.SpriteLayer;

public class Character
{
    #region Components

    private CharacterComponentAttributes attributesComponent;
    private LocalizationComponentString localizationStringComponent;
    private CharacterComponentLevels levelsComponent;
    private CharacterComponentStats statsComponent;
    private CharacterComponentTraining trainingComponent;
    private CharacterComponentMoves movesComponent;
    private CharacterComponentPersistence persistenceComponent;
    private CharacterComponentAppearance appearanceComponent;

    #endregion

    #region Initialize

    public Character(CharacterData characterData, CharacterSaveData characterSaveData = null) 
    {
        Initialize(characterData, characterSaveData);
    }

    public void Initialize(CharacterData characterData, CharacterSaveData characterSaveData = null)
    {
        attributesComponent = new CharacterComponentAttributes(characterData, this, characterSaveData);
        localizationStringComponent = new LocalizationComponentString(
            LocalizationEntity.Character,
            characterData.CharacterId,
            new[] { LocalizationField.Name, LocalizationField.Nick, LocalizationField.Description }
        );
        levelsComponent = new CharacterComponentLevels(characterData, this, characterSaveData);
        statsComponent = new CharacterComponentStats(characterData, this, characterSaveData);
        trainingComponent = new CharacterComponentTraining(characterData, this, characterSaveData);
        movesComponent = new CharacterComponentMoves(characterData, this, characterSaveData);
        persistenceComponent = new CharacterComponentPersistence(characterData, this);
        appearanceComponent = new CharacterComponentAppearance(characterData, this, characterSaveData);
    }

    #endregion

    #region API
    // attributesComponent
    public string CharacterId => attributesComponent.CharacterId;
    public string CharacterGuid => attributesComponent.CharacterGuid;
    public CharacterSize CharacterSize => attributesComponent.CharacterSize;
    public PortraitSize PortraitSize => attributesComponent.PortraitSize;
    public Gender Gender => attributesComponent.Gender;
    public Element Element => attributesComponent.Element;
    public Position Position => attributesComponent.Position;

    // localizationComponent
    public string CharacterName => localizationStringComponent.GetString(LocalizationField.Name);
    public string CharacterNick => localizationStringComponent.GetString(LocalizationField.Nick);
    public string CharacterDescription => localizationStringComponent.GetString(LocalizationField.Description);

    // levelsComponent
    public int Level => levelsComponent.Level;
    public int CurrentExp => levelsComponent.CurrentExp;
    public int ExpToNextLevel => levelsComponent.ExpToNextLevel;
    public int MaxLevel => CharacterComponentLevels.MAX_LEVEL;
    public void SetLevel(int level) => levelsComponent.SetLevel(level);

    // statsComponent
    public int GetTrainedStat(Stat stat) => statsComponent.GetTrainedStat(stat);
    public int GetTrueStat(Stat stat) => statsComponent.GetTrueStat(stat);
    public int GetBattleStat(Stat stat) => statsComponent.GetBattleStat(stat);
    public void ResetBattleStats() => statsComponent.ResetBattleStats();
    public void ResetTrainedStats() => statsComponent.ResetTrainedStats();
    public void ModifyBattleStat(Stat stat, int amount) => statsComponent.ModifyBattleStat(stat, amount);
    public void ModifyTrainedStat(Stat stat, int amount) => statsComponent.ModifyTrainedStat(stat, amount);
    public void UpdateStats() => statsComponent.UpdateStats();

    // trainingComponent
    public int MaxTrainingPerStat => CharacterComponentTraining.MAX_TRAINING_PER_STAT;
    public int BaseFreedom => trainingComponent.BaseFreedom;
    public int TrueFreedom => trainingComponent.TrueFreedom;
    public void TrainStat(Stat stat, int amount) => trainingComponent.TrainStat(stat, amount);
    public bool IsCharacterTrainable(Stat stat) => trainingComponent.IsCharacterTrainable(stat);
    public bool IsStatTrainable(Stat stat) => trainingComponent.IsStatTrainable(stat);
    public int GetRemainingTrainingByStat(Stat stat) => trainingComponent.GetRemainingTrainingByStat(stat);
    public void ResetTraining() => trainingComponent.ResetTraining();
    public int TrainingResetCount => trainingComponent.TrainingResetCount;

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
    //public bool CanPerformeMove(Move move) => movesComponent.CanPerformeMove(move);
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
    public void ForceMaxEvolutionOnEquippedMoves() => movesComponent.ForceMaxEvolutionOnEquippedMoves();

    // persistenceComponent
    public void Import(CharacterSaveData characterSaveData) => persistenceComponent.Import(characterSaveData);
    public CharacterSaveData Export() => persistenceComponent.Export();

    // appearanceComponent
    public CharacterComponentAppearance AppearanceComponent => appearanceComponent;
    public Sprite PortraitSprite => appearanceComponent.PortraitSprite;
    public string PortraitSpriteId => appearanceComponent.PortraitSpriteId;
    public string HairStyleId => appearanceComponent.HairStyleId;
    public HairColorType HairColorType => appearanceComponent.HairColorType;
    public EyeColorType EyeColorType => appearanceComponent.EyeColorType;
    public BodyColorType BodyColorType => appearanceComponent.BodyColorType;
    public SpriteLayerState<CharacterSpriteLayer> SpriteLayerState
    {
        get => appearanceComponent.State;
        set => appearanceComponent.State = value;
    }
    public void ApplyKit(Kit kit, Variant variant, Position position) => appearanceComponent.ApplyKit(kit, variant, position);
    public void InitializeVisibility() => appearanceComponent.InitializeVisibility();
    public Variant GetKitVariant(Team team) => appearanceComponent.GetKitVariant(team);
    public Role GetKitRole(Position position) => appearanceComponent.GetKitRole(position);
    #endregion
}
