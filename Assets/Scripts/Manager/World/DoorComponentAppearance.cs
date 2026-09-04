using UnityEngine;
using System;
using System.Threading.Tasks;
using Aremoreno.Enums.World;

public class DoorComponentAppearance : MonoBehaviour
{
    #region Fields

    [SerializeField] private GameObject spriteClosed;

    #endregion

    #region Initialization

    public void Initialize(DoorEntity doorEntity)
    {

    }

    #endregion

    #region Helpers

    public void SetSpriteOpened() 
    {
        spriteClosed.SetActive(false);
    }

    public void SetSpriteClosed() 
    {
        spriteClosed.SetActive(true);
    }

    #endregion
}
