using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class BattleFieldManager : MonoBehaviour
{
    public static BattleFieldManager Instance { get; private set; }

    private Field field;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

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

}
