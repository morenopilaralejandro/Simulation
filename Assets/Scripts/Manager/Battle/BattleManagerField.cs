using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class BattleManagerField
{
    #region Fields

    private Field field;

    #endregion

    #region Constructor

    public BattleManagerField()
    {

    }

    #endregion

    #region Logic

    public void RegisterField(Field field)
    {
        this.field = field;
    }

    public void UnregisterField()
    {
        field = null;
    }

    public void InitializeField()
    {
        var fieldData = FieldManager.Instance.GetFieldData(BattleArgs.FieldId);
        field.Initialize(fieldData);
    }

    #endregion

    #region Helpers

    #endregion

    #region Events

    //public void Subscribe() { }
    //public void Unsubscribe() { }

    #endregion
}
