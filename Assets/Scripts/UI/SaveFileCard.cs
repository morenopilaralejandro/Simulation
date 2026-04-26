using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Aremoreno.Enums.Localization;
using Aremoreno.Enums.World;

public class SaveFileCard : MonoBehaviour
{
    #region Fields

    [SerializeField] private bool initializeFromSaveData;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text textZoneName;
    [SerializeField] private TMP_Text textTime;
    // [SerializeField] private TMP_Text textBuddy;

    private LocalizationComponentString localizationStringComponent;
    private int maxHours = 999;
    private PersistenceManager persistenceManager;
    private WorldManager worldManager;

    #endregion

    #region Lifecycle

    private void Awake() 
    {
        persistenceManager = PersistenceManager.Instance;
        worldManager = WorldManager.Instance;
    }

    private void Start() 
    {
        if (initializeFromSaveData)
            SetFromSaveData(persistenceManager.GetLastSaveData());
        else
            SetFromRuntime();
    }

    #endregion

    #region SetData

    public void SetFromRuntime() 
    {
        if (persistenceManager == null) persistenceManager = PersistenceManager.Instance;
        if (worldManager == null) worldManager = WorldManager.Instance;

        SetData(
            worldManager.ZoneName,
            GetTimeFormated(
                DateTimeOffset.UtcNow.ToUnixTimeSeconds(), 
                persistenceManager.TimestampCreation
            )
        );
    }

    public void SetFromSaveData(SaveData saveData)
    {
        if (saveData == null) 
        {
            SetVisible(false);
            return;
        }

        localizationStringComponent = new LocalizationComponentString(
            LocalizationEntity.Zone,
            saveData.SaveDataWorldSystem.ZoneId,
            new [] { LocalizationField.Name }
        );

        SetData(
            localizationStringComponent.GetString(LocalizationField.Name),
            GetTimeFormated(
                saveData.TimestampCreation, 
                saveData.TimestampSave
            )
        );
    }

    public void SetData(
        string zoneName,
        string timeFormated)
    {
        textZoneName.text = zoneName;
        textTime.text = timeFormated;

        SetVisible(true);
    }

    #endregion

    #region Helpers

    private string GetTimeFormated(
        long timestampCreation,
        long timestampSave
    ) 
    {
        long elapsedSeconds = timestampSave - timestampCreation;

        if (elapsedSeconds < 0)
            elapsedSeconds = 0;

        int totalMinutes = (int)(elapsedSeconds / 60);
        int hours = totalMinutes / 60;
        int minutes = totalMinutes % 60;

        // Clamp hours to 999 max
        if (hours > maxHours) hours = maxHours;

        return string.Format("{0}:{1:D2}", hours, minutes);
    }

    #endregion

    #region SetVisible

    public void SetVisible(bool boolValue)
    {
        canvasGroup.alpha = boolValue ? 1f : 0f;
        canvasGroup.interactable = boolValue;
        canvasGroup.blocksRaycasts = boolValue;
    }

    #endregion

    #region Events

    private void OnEnable()
    {
        PersistenceEvents.OnGameSaved += SetFromSaveData;
    }

    private void OnDisable()
    {
        PersistenceEvents.OnGameSaved -= SetFromSaveData;
    }

    #endregion

}
