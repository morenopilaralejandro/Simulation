using System;
using System.Collections;
using UnityEngine;
using System.Threading.Tasks;
using Simulation.Enums.World;

public class WorldManagerPlayer
{
    #region Fields

    public PlayerWorldEntity PlayerWorldEntity { get; private set; }
    public PlayerWorldConfig PlayerWorldConfig { get; private set; }

    #endregion

    #region Constructor

    public WorldManagerPlayer(PlayerWorldConfig playerWorldConfig) 
    {
        PlayerWorldEntity = PlayerWorldEntity.Instance;
        PlayerWorldConfig = playerWorldConfig;

        CharacterData characterData = CharacterManager.Instance.GetCharacterData("are");
        Kit kit = KitManager.Instance.GetKit("faith");
        PlayerWorldEntity.Initialize(characterData, kit, PlayerWorldConfig);
    }

    #endregion

    #region Events

    public void Subscribe()
    {

    }

    public void Unsubscribe()
    {

    }

    #endregion

}
