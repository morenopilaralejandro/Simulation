using UnityEngine;
using UnityEngine.Localization;
using Simulation.Enums.Log;
using Simulation.Enums.Localization;

public class DuelLogEntry
{
    private LocalizationComponentString localizationStringComponent;

    public LogLevel LogLevel { get; private set; }
    public Character Character { get; private set; }
    public Move Move { get; private set; }

    public DuelLogEntry(string entryId, LogLevel logLevel)
    {
        LogLevel = logLevel;
        Character = null;
        Move = null;

        localizationStringComponent = new LocalizationComponentString(
            LocalizationEntity.DuelLog,
            entryId,
            new [] {  LocalizationField.Entry }
        );
    }

    public DuelLogEntry(string entryId, LogLevel logLevel, Character character, Move move)
    {
        LogLevel = logLevel;
        Character = character;
        Move = move;

        localizationStringComponent = new LocalizationComponentString(
            LocalizationEntity.DuelLog,
            entryId,
            new [] {  LocalizationField.Entry }
        );
    }

    public string EntryString => localizationStringComponent.GetString(LocalizationField.Entry);
}
