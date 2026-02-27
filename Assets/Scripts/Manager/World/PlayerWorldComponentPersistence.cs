// PlayerMovementController.cs
using UnityEngine;
using Simulation.Enums.World;

public class PlayerWorldComponentPersistence : MonoBehaviour
{
    //pull from manager
    private PlayerWorldEntity playerWorldEntity;
    private PlayerWorldConfig config;

    private bool isPersistent = false;

    public void Initialize(PlayerWorldEntity playerWorldEntity, PlayerWorldConfig cfg)
    {
        this.playerWorldEntity = playerWorldEntity;
        config = cfg;
    }

    public void MakePersistent()
    {
        if (isPersistent) return;
        DontDestroyOnLoad(playerWorldEntity);
        isPersistent = true;
    }

    /*
    public PlayerWorldSaveData GetSaveData()
    {
        _saveData.lastPosition = PlayerPosition;
        _saveData.lastFacing = PlayerFacing;
        //_saveData.lastScene = SceneManager.GetActiveScene().name;
        return _saveData;
    }

    public void LoadFromSaveData(PlayerWorldSaveData data)
    {
        _saveData = data;
        //TransitionToZone(data.lastScene, data.lastPosition, data.lastFacing);
    }
    */

}
