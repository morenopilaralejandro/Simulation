using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;
using Simulation.Enums.Localization;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance { get; private set; }

    [SerializeField] private StringTableConfig stringTableConfig;
    [SerializeField] private bool isRomazed = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        stringTableConfig.Initialize();
    }

    public TableReference GetTableReference(LocalizationEntity entity, LocalizationField field)
    {
        LocalizationStyle style = isRomazed ? LocalizationStyle.Romanized : LocalizationStyle.Localized; 
        return stringTableConfig.GetTableReference(entity, field, style);
    }

}
