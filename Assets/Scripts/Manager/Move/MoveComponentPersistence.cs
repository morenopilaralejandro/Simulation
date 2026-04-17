using UnityEngine;
using System.Collections.Generic;
using Aremoreno.Enums.Character;

public class MoveComponentPersistence
{
    #region Fields

    private Move move;

    #endregion        

    #region LifeCycle

    public MoveComponentPersistence(MoveData moveData, Move move)
    {
        Initialize(moveData, move);
    }

    public void Initialize(MoveData moveData, Move move)
    {
        this.move = move;
    }

    #endregion

    #region Import

    public void Import(MoveSaveData moveSaveData)
    {
        MoveData moveData = MoveManager.Instance.GetMoveData(moveSaveData.MoveId);
        move.Initialize(moveData, moveSaveData);
    }

    #endregion

    #region Export

    public MoveSaveData Export()
    {
        return new MoveSaveData
        {
            MoveId = move.MoveId,
            CurrentEvolution = move.CurrentEvolution,
            TimesUsedTotal = move.TimesUsedTotal,
            TimesUsedCurrentEvolution = move.TimesUsedCurrentEvolution
        };
    }

    #endregion

    #region Helpers

    #endregion
}
