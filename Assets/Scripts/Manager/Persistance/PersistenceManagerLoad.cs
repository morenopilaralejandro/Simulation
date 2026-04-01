using UnityEngine;
using System;
using System.IO;

public class PersistenceManagerLoad : MonoBehaviour
{
    #region Fields

    private PersistenceManager persistenceManager;

    #endregion

    #region Constructor

    public PersistenceManagerLoad() 
    {
        persistenceManager = PersistenceManager.Instance;
    }

    #endregion

    #region Logic

    public void LoadGame()
    {
        /*
            GetLastSaveData();
            initialize the characters etc.
        */
    }

    #endregion

    #region Helpers

    #endregion

    #region Events
    /*    
    public void Subscribe() { }
    public void Unsubscribe() { }
    */
    #endregion

}
