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
        BattleEvents.OnBattleEnded += HandleBattleEnded;
    }

    private void OnDisable()
    {
        BattleEvents.OnBattleEnded -= HandleBattleEnded;
    }

    private void HandleBattleEnded()
    {
        Destroy(gameObject);
    }

    #endregion

}
