using UnityEngine;
using UnityEngine.Localization;
using Simulation.Enums.Log;
using Simulation.Enums.Localization;

public class DuelLogEntry
{
    private LocalizationComponentString localizationStringComponent;

    public LogLevel LogLevel { get; private set; }
    public CharacterEntityBattle CharacterEntityBattle { get; private set; }
    public Move Move { get; private set; }

    public DuelLogEntry(string entryId, LogLevel logLevel, CharacterEntityBattle character, Move move, object args)
    {
        LogLevel = logLevel;
        CharacterEntityBattle = character;
        Move = move;

        localizationStringComponent = new LocalizationComponentString(
            LocalizationEntity.Duel_Log,
            entryId,
            new [] {  LocalizationField.Entry }
        );

        localizationStringComponent.SetArguments(LocalizationField.Entry, args);
    }

    public string EntryString => localizationStringComponent.GetString(LocalizationField.Entry);
}
