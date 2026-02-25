// PlayerMovementController.cs
using UnityEngine;
using Simulation.Enums.World;

[RequireComponent(typeof(CharacterController))]
public class PlayerWorldPersistenceComponent : MonoBehaviour
{
    //pull from manager
    private PlayerWorldEntity playerWorldEntity;
    private PlayerWorldConfig config;

    public void Initialize(PlayerWorldEntity playerWorldEntity, PlayerWorldConfig cfg)
    {
        this.playerWorldEntity = playerWorldEntity;
        config = cfg;
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
