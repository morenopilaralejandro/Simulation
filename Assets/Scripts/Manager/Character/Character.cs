using UnityEngine;
using System.Collections.Generic;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Kit;
using Aremoreno.Enums.Move;
using Aremoreno.Enums.Localization;
using Aremoreno.Enums.SpriteLayer;
using Aremoreno.Enums.Wing;

public class Character
{
    #region Components

    private CharacterComponentAttributes attributesComponent;
    private CharacterComponentCustomAvatar customAvatarComponent;
    private LocalizationComponentString localizationStringComponent;
    private CharacterComponentLevels levelsComponent;
    private CharacterComponentStats statsComponent;
    private CharacterComponentTraining trainingComponent;
    private CharacterComponentMoves movesComponent;
    private CharacterComponentPersistence persistenceComponent;
    private CharacterComponentAppearance appearanceComponent;
    private CharacterComponentWing wingComponent;
    private CharacterComponentAIDifficulty aiDifficultyComponent;

    #endregion

    #region Initialize

    public Character(CharacterData characterData, CharacterSaveData characterSaveData = null) 
    {
        Initialize(characterData, characterSaveData);
    }

    public void Initialize(CharacterData characterData, CharacterSaveData characterSaveData = null)
    {
        attributesComponent = new CharacterComponentAttributes(characterData, this, characterSaveData);
        customAvatarComponent = new CharacterComponentCustomAvatar(characterData, this, characterSaveData);
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
        wingComponent = new CharacterComponentWing(characterData, this, characterSaveData);
        aiDifficultyComponent = new CharacterComponentAIDifficulty(characterData, this, characterSaveData);
    }

    #endregion

    #region API
    // attributesComponent
    public string CharacterId => attributesComponent.CharacterId;
    public string CharacterGuid => attributesComponent.CharacterGuid;
    public CharacterSize CharacterSize => attributesComponent.CharacterSize;
    public Gender Gender => attributesComponent.Gender;
    public Element Element => attributesComponent.Element;
    public Position Position => attributesComponent.Position;

    // customAvatarComponent
    public bool IsCustomAvatar => customAvatarComponent.IsCustomAvatar;
    public string CustomAvatarId => customAvatarComponent.CustomAvatarId;
    public string CustomName => customAvatarComponent.CustomName;
    public CharacterSize CustomCharacterSize => customAvatarComponent.CustomCharacterSize;
    public Gender CustomGender => customAvatarComponent.CustomGender;
    public Element CustomElement => customAvatarComponent.CustomElement;
    public Position CustomPosition => customAvatarComponent.CustomPosition;
    public HairStyle CustomHairStyle => customAvatarComponent.CustomHairStyle;
    public HairColorType CustomHairColorType => customAvatarComponent.CustomHairColorType;
    public EyeColorType CustomEyeColorType => customAvatarComponent.CustomEyeColorType;
    public BodyColorType CustomBodyColorType => customAvatarComponent.CustomBodyColorType;
    public PortraitSize CustomPortraitSize => customAvatarComponent.CustomPortraitSize;

    // localizationComponent
    public LocalizationComponentString LocalizationComponent => localizationStringComponent;
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
    public bool ReplaceEquippedMove(Move oldMove, Move newMove) => movesComponent.ReplaceEquippedMove(oldMove, newMove);
    public bool SwapEquippedMoves(int indexA, int indexB) => movesComponent.SwapEquippedMoves(indexA, indexB);
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
    public PortraitSize PortraitSize => appearanceComponent.PortraitSize;
    public HairStyle HairStyle => appearanceComponent.HairStyle;
    public HairColorType HairColorType => appearanceComponent.HairColorType;
    public EyeColorType EyeColorType => appearanceComponent.EyeColorType;
    public BodyColorType BodyColorType => appearanceComponent.BodyColorType;
    public void SetKitId(Kit kit) => appearanceComponent.SetKitId(kit);
    public void SetKit(Kit kit, Variant variant, Role role) => appearanceComponent.SetKit(kit, variant, role);
    public void SetKit(Team team, Position position) => appearanceComponent.SetKit(team, position);
    public Variant GetKitVariant(Team team) => appearanceComponent.GetKitVariant(team);
    public Role GetKitRole(Position position) => appearanceComponent.GetKitRole(position);
    public string KitId => appearanceComponent.KitId;
    public Variant KitVariant => appearanceComponent.KitVariant;
    public Role KitRole => appearanceComponent.KitRole;
    public string KitAddress => appearanceComponent.KitAddress;
    public string PortraitCharacterAddress => appearanceComponent.PortraitCharacterAddress;
    public string PortraitKitAddress => appearanceComponent.PortraitKitAddress;
    public string HairFrontAddress => appearanceComponent.HairFrontAddress;
    public string HairBackAddress => appearanceComponent.HairBackAddress;
    public Color ColorBody => appearanceComponent.ColorBody;
    public Color ColorHair => appearanceComponent.ColorHair;
    public void SetWingType(WingType wingType) => appearanceComponent.SetWingType(wingType);
    public void SetWingColorType(WingColorType wingColorType) => appearanceComponent.SetWingColorType(wingColorType);
    public void SetWing(Wing wing) => appearanceComponent.SetWing(wing);
    public void SetWingAddress() => appearanceComponent.SetWingAddress();
    public void SetWingColor() => appearanceComponent.SetWingColor();

    //wingComponent
    public Wing Wing => wingComponent.Wing;
    public bool HasWingActivated => wingComponent.HasWingActivated;
    public bool HasWingEquipped => wingComponent.HasWingEquipped;
    public void EquipWing(Wing wing) => wingComponent.EquipWing(wing);
    public void UnequipWing() => wingComponent.UnequipWing();
    public void SetWingEquipped(Wing wing) => wingComponent.SetWingEquipped(wing);
    public void SetWingActive(bool boolValue) => wingComponent.SetWingActive(boolValue);
    public void ForceMaxEvolutionOnEquippedWing() => wingComponent.ForceMaxEvolutionOnEquippedWing();
    public void ForceEquipWing(Wing wing) => wingComponent.ForceEquipWing(wing);
    public int WingTimesUsed => wingComponent.WingTimesUsed;
    public void IncreaseWingTimesUsed() => wingComponent.IncreaseWingTimesUsed();
    public void ResetWingTimesUsed() => wingComponent.ResetWingTimesUsed();
    public bool CanApplyWingElementMatchBonus(Element element) => wingComponent.CanApplyWingElementMatchBonus(element);
    public void TryEquipWingDefault() => wingComponent.TryEquipWingDefault();

    //aiDifficultyComponent
    public AIDifficulty AIDifficulty => aiDifficultyComponent.AIDifficulty;
    public void ScaleDifficultySystem()  => aiDifficultyComponent.ScaleDifficultySystem();

    #endregion
}
