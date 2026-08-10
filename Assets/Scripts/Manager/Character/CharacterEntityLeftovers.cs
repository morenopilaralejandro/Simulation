using UnityEngine;
using Aremoreno.Enums.Character;

public class CharacterEntityLeftovers : MonoBehaviour
{
    #region Components

    #endregion

    #region Initialize

    #endregion

    #region Update

    #endregion

    #region API
      
    #endregion

    #region Events

    private void OnEnable()
    {
        BattleEvents.OnBattleEnd += HandleBattleEnd;
    }

    private void OnDisable()
    {
        BattleEvents.OnBattleEnd -= HandleBattleEnd;
    }

    private void HandleBattleEnd()
    {
        Destroy(gameObject);
    }

    #endregion

}
