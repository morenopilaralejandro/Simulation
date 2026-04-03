using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Simulation.Enums.Localization;
using Simulation.Enums.World;

public class SaveFileCard : MonoBehaviour
{
    [SerializeField] private TMP_Text textZoneName;
    [SerializeField] private TMP_Text textTime;
    // [SerializeField] private TMP_Text textBuddy;

    private LocalizationComponentString localizationStringComponent;
    private string zoneName => localizationStringComponent.GetString(LocalizationField.Name);
    private int maxHours = 999;

    public void SetData(SaveData saveData)
    {
        localizationStringComponent = new LocalizationComponentString(
            LocalizationEntity.Zone,
            saveData.SaveDataWorldSystem.ZoneId,
            new [] { LocalizationField.Name }
        );

        textZoneName.text = zoneName;
        textTime.text = GetTimeFormated(saveData);
    }

    private string GetTimeFormated(SaveData saveData) 
    {
        long elapsedSeconds = saveData.TimestampSave - saveData.TimestampCreation;

        if (elapsedSeconds < 0)
            elapsedSeconds = 0;

        int totalMinutes = (int)(elapsedSeconds / 60);
        int hours = totalMinutes / 60;
        int minutes = totalMinutes % 60;

        // Clamp hours to 999 max
        if (hours > maxHours) hours = maxHours;

        return string.Format("{0}:{1:D2}", hours, minutes);
    }

}
