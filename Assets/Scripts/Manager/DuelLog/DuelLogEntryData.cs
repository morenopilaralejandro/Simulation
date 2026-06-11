using UnityEngine;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Duel;
using Aremoreno.Enums.Log;

public record DuelLogEntryData
{
    public string EntryId { get; set; }
    public LogLevel LogLevel { get; set; }

    public Character Character { get; set; }
    public TeamSide TeamSide { get; set; }
    public Move Move { get; set; }
    public Wing Wing { get; set; }

    public object Args { get; set; }
}
