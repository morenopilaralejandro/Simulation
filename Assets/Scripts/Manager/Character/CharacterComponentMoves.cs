using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Simulation.Enums.Character;
using Simulation.Enums.Move;

public class CharacterComponentMoves : MonoBehaviour
{
    private Character character;

    public const int MAX_EQUIPPED_MOVES_DEFAULT = 6;
    public const int MAX_EQUIPPED_MOVES_FUSION = 8;

    [SerializeField] private List<MoveLearnsetData> levelUpLearnset = new List<MoveLearnsetData>();
    [SerializeField] private List<Move> learnedMoves = new List<Move>();
    [SerializeField] private List<Move> equippedMoves = new List<Move>();

    public IReadOnlyList<MoveLearnsetData> LevelUpLearnset => levelUpLearnset;
    public IReadOnlyList<Move> LearnedMoves => learnedMoves;
    public IReadOnlyList<Move> EquippedMoves => equippedMoves;

    public void Initialize(CharacterData characterData, Character character)
    {
        this.character = character;
        levelUpLearnset = GetLevelUpLearnSetByCharacterData(characterData);
        learnedMoves.Clear();
        equippedMoves.Clear();
        CheckLearnMoveOnLevelUp();
    }

    private List<MoveLearnsetData> GetLevelUpLearnSetByCharacterData(CharacterData characterData)
    {
        List<MoveLearnsetData> learnset = new List<MoveLearnsetData>();
        for (int i = 0; i < characterData.MoveIds.Length; i++)
        {
            learnset.Add(new MoveLearnsetData {
                MoveId = characterData.MoveIds[i],
                Level = characterData.MoveLvs[i]
            });
        }
        return learnset;
    }

    public void CheckLearnMoveOnLevelUp()
    {
        var movesToLearn = levelUpLearnset
            .Where(m => m.Level <= this.character.Level && !IsMoveLearned(m.MoveId))
            .ToList();

        foreach (var moveData in movesToLearn)
        {
            LearnMove(moveData.MoveId);
        }
    }

    public void LearnMove(string moveId)
    {
        Move move = new Move(MoveManager.Instance.GetMoveData(moveId));
        learnedMoves.Add(move);
        LogManager.Trace($"[CharacterComponentMoves] {this.character.CharacterId} learned {move.MoveId}");
        EquipMove(move);
    }

    public void EquipMove(Move move)
    {
        if (equippedMoves.Count < MAX_EQUIPPED_MOVES_DEFAULT) equippedMoves.Add(move);
    }

    public void UnequipMove(Move move)
    {
        equippedMoves.Remove(move);
    }

    public bool IsMoveEquipped(string moveId) => equippedMoves.Any(move => move.MoveId == moveId);
    public bool IsMoveEquipped(Move move) => equippedMoves.Contains(move);


    public bool IsMoveLearned(string moveId) => learnedMoves.Any(move => move.MoveId == moveId);
    public bool IsMoveLearned(Move move) => learnedMoves.Contains(move);

    public bool CanLearnMove(string moveId)
    {
        if (IsMoveLearned(moveId)) return false;
        MoveData moveData = MoveManager.Instance.GetMoveData(moveId);
        if (moveData.AllowedElements.Count > 0  && !moveData.AllowedElements.Contains(this.character.Element)) return false;
        if (moveData.AllowedPositions.Count > 0 && !moveData.AllowedPositions.Contains(this.character.Position)) return false;
        if (moveData.AllowedGenders.Count > 0   && !moveData.AllowedGenders.Contains(this.character.Gender)) return false;
        if (moveData.AllowedSizes.Count > 0     && !moveData.AllowedSizes.Contains(this.character.CharacterSize)) return false; 
        return true;
    }

    public bool CanPerformeMove(Move move)
    {
        //include sp and other things
        return move.TryFinalizeParticipants(this.character, this.character.GetTeammates()) &&
            CanAffordMove(move);
    }

    public bool CanAffordMove(Move move) => this.character.GetBattleStat(Stat.Sp) >= move.Cost;

    public bool HasAffordableMove() => equippedMoves.Any(move => CanAffordMove(move));
 
    public bool HasAffordableMoveWithTrait(Trait trait) => equippedMoves.Any(move => move.Trait == trait && CanAffordMove(move));

    public Move GetRandomAffordableMove()
    {
        var affordableMoves = equippedMoves.Where(CanAffordMove).ToList();
        if (affordableMoves.Count == 0) return null;

        int index = UnityEngine.Random.Range(0, affordableMoves.Count);
        return affordableMoves[index];
    }

    public Move GetStrongestAffordableMove()
    {
        var affordableMoves = equippedMoves.Where(CanAffordMove).ToList();
        if (affordableMoves.Count == 0) return null;

        // Sort moves by element match priority and then by base power
        var bestMove = affordableMoves
            .OrderByDescending(move => move.Element == character.Element) // prioritize matching element
            .ThenByDescending(move => move.BasePower)
            .FirstOrDefault();

        return bestMove;
    }

}
