using System;
using System.Collections;
using UnityEngine;
using System.Threading.Tasks;
using Aremoreno.Enums.World;

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

        CharacterData characterData = DatabaseManager.Instance.GetCharacterData("chara-00001-are");
        Kit kit = DatabaseManager.Instance.GetKit("kit-00001-faith");
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
