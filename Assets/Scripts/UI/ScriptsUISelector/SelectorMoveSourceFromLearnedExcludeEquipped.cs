using UnityEngine;
using System.Collections.Generic;

public class SelectorMoveSourceFromLearnedExcludeEquipped : ISelectorSource<Move>
{
    private List<Move> moveList = new();
    private Character character;

    public SelectorMoveSourceFromLearnedExcludeEquipped() {}
    public SelectorMoveSourceFromLearnedExcludeEquipped(Character character) 
    {
        this.character = character;
    }

    public void SetCharacter(Character character) 
    {
        this.character = character;
    }

    public IEnumerable<Move> Enumerate() 
    {
        moveList.Clear();
        foreach (var move in character.LearnedMoves) 
        {
            if (!character.IsMoveEquipped(move))
                moveList.Add(move);              
        }
        return moveList;
    }
}
