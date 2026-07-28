using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Aremoreno.Enums.Battle;

public class BattleResultsUI : MonoBehaviour
{
    private void Start() 
    {
        switch (BattleArgs.BattleResultsType) 
        {
            case BattleResultsType.MatchStory:
                UIEvents.RaiseResultsMatchStoryOpenRequested(BattleManager.Instance.BattleResultData);
                break;
            case BattleResultsType.MatchNode:
                UIEvents.RaiseResultsMatchNodeOpenRequested(BattleManager.Instance.BattleResultData);
                break;
            case BattleResultsType.Encounter:
                UIEvents.RaiseResultsEncounterOpenRequested(BattleManager.Instance.BattleResultData);
                break;
            case BattleResultsType.Arcade:
            default:
                UIEvents.RaiseResultsArcadeOpenRequested(BattleManager.Instance.BattleResultData);
                break;
        }
    }
}
