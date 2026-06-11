using UnityEngine;
using UnityEngine.Localization;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Log;
using Aremoreno.Enums.Localization;

public class DuelLogEntry
{
    private readonly LocalizationComponentString localizationStringComponent;

    public DuelLogEntryData Data { get; }

    public LogLevel LogLevel => Data.LogLevel;
    public Character Character => Data.Character;
    public TeamSide TeamSide => Data.TeamSide;
    public Move Move => Data.Move;
    public Wing Wing => Data.Wing;

    public DuelLogEntry(DuelLogEntryData data)
    {
        Data = data;

        localizationStringComponent = new LocalizationComponentString(
            LocalizationEntity.Duel_Log,
            data.EntryId,
            new[] { LocalizationField.Entry }
        );

        localizationStringComponent.SetArguments(
            LocalizationField.Entry,
            data.Args
        );
    }

    public string EntryString => localizationStringComponent.GetString(LocalizationField.Entry);
}
