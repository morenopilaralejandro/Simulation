using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCharacterSpawnPoint : MonoBehaviour
{
    private void Awake()
    {
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.RegisterSpawnPointCharacter(this.transform);
        }
    }

    private void OnDestroy()
    {
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.UnregisterSpawnPointCharacter();
        }
    }

    private void Start() 
    {
        BattleManager.Instance.StartBattle();
    }
}
