using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aremoreno.Enums.Character;

public class BattleManagerCharacter
{
    #region Fields

    private GameObject characterEntityBattlePrefab;    //inspector
    private int initialPoolSize = 22;
    private Queue<CharacterEntityBattle> characterPool = new Queue<CharacterEntityBattle>();
    private Transform spawnPoint;

    #endregion

    #region Constructor

    public BattleManagerCharacter(GameObject characterEntityBattlePrefab)
    {
        this.characterEntityBattlePrefab = characterEntityBattlePrefab;
    }

    #endregion

    #region Logic

    public void RegisterSpawnPoint(Transform spawner)
    {
        spawnPoint = spawner;

        ClearPool();
        PrewarmCharacterPool();
    }

    public void UnregisterSpawnPoint()
    {
        spawnPoint = null;
    }

    private void PrewarmCharacterPool()
    {
        if (characterEntityBattlePrefab == null)
        {
            LogManager.Error("[BattleCharacterManager] Character prefab missing!");
            return;
        }

        for (int i = 0; i < initialPoolSize; i++)
        {
            var go = BattleManager.Instance.InstantiateCharacter(characterEntityBattlePrefab, spawnPoint);
            var characterEntityBattle = go.GetComponent<CharacterEntityBattle>();
            go.SetActive(false);
            characterPool.Enqueue(characterEntityBattle);
        }

        LogManager.Trace($"[BattleCharacterManager] Prewarmed {initialPoolSize} pooled characters");
    }

    public void GetPooledCharacter(Action<CharacterEntityBattle> onCharacterReady)
    {
        if (characterPool.Count == 0)
        {
            var go = BattleManager.Instance.InstantiateCharacter(characterEntityBattlePrefab, spawnPoint);
            var characterEntityBattle = go.GetComponent<CharacterEntityBattle>();
            characterPool.Enqueue(characterEntityBattle);
        }

        var pooledCharacter = characterPool.Dequeue();
        pooledCharacter.gameObject.SetActive(true);
        onCharacterReady?.Invoke(pooledCharacter);
    }

    public void ReturnCharacterToPool(CharacterEntityBattle character)
    {
        if (character == null) return;
        character.gameObject.SetActive(false);
        characterPool.Enqueue(character);
    }

    public void AssignCharacterToTeamBattle(CharacterEntityBattle character, Team team, int characterIndex)
    {
        FormationCoord formationCoord = team.GetFormation(BattleManager.Instance.CurrentType).FormationCoords[characterIndex];
        TeamEvents.RaiseAssignCharacterToTeamBattle(character, team, formationCoord);
    }

    public void ResetCharacterPosition(CharacterEntityBattle character)
    {
        character.ResetPhysics();
        character.Teleport(character.FormationCoord.DefaultPosition);
        character.Model.transform.rotation = character.FormationCoord.DefaultRotation;

        character.ClearAllStatus();
        character.ReleaseStateLock();
        character.SetLocomotion(Aremoreno.Enums.Animation.CharacterAnimationState.Idle);
    }

    public void ClearPool()
    {
        foreach (var character in characterPool) 
        {
            if (character != null)
                BattleManager.Instance.DestroyGameObject(character.gameObject);
        }
        characterPool.Clear();
    }

    #endregion

    #region Helpers

    #endregion

    #region Events

    //public void Subscribe() { }
    //public void Unsubscribe() { }

    #endregion

}
