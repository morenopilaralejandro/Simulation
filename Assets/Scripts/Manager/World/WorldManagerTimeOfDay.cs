using System;
using System.Collections;
using UnityEngine;
using System.Threading.Tasks;
using Aremoreno.Enums.World;

public class WorldManagerTimeOfDay
{
    #region Fields

    public int CurrentHour { get; private set; }
    public TimeOfDay CurrentTimeOfDay { get; private set; }
    public TimeOfDay PreviousTimeOfDay { get; private set; }

    private int morningStart = 6;
    private int morningEnd = 11;
    private int dayStart = 12;
    private int dayEnd = 17;
    private int eveningStart = 18;
    private int eveningEnd = 20;

    #endregion

    #region Constructor

    public WorldManagerTimeOfDay() 
    {
        CurrentHour = 12;
    }

    #endregion

    #region Logic

    /// <summary>
    /// Advance time by one hour
    /// </summary>
    public void AdvanceHour()
    {
        CurrentHour = (CurrentHour + 1) % 24;
        UpdateTimeOfDay();
        
        LogManager.Trace($"[WorldManagerTimeOfDay] Time advanced to: {CurrentHour:D2}:00 ({CurrentTimeOfDay})");
    }

    /// <summary>
    /// Advance time by multiple hours
    /// </summary>
    public void AdvanceHours(int hours)
    {
        for (int i = 0; i < hours; i++) AdvanceHour();
    }

    /// <summary>
    /// Set the hour directly (0-23)
    /// </summary>
    public void SetHour(int hour)
    {
        if (hour < 0 || hour > 23) return;

        CurrentHour = hour;
        UpdateTimeOfDay();
        
        LogManager.Trace($"[WorldManagerTimeOfDay] Hour set to: {CurrentHour:D2}:00 ({CurrentTimeOfDay})");
    }

    /// <summary>
    /// Directly change to a specific time of day (sets appropriate hour)
    /// </summary>
    public void ChangeTime(TimeOfDay newTime)
    {
        if (CurrentTimeOfDay == newTime) return;

        CurrentHour = newTime switch
        {
            TimeOfDay.Morning => morningStart,
            TimeOfDay.Day => dayStart,
            TimeOfDay.Evening => eveningStart,
            TimeOfDay.Night => 21,
            _ => 12
        };

        UpdateTimeOfDay();
        LogManager.Trace($"[WorldManagerTimeOfDay] Time changed to: {newTime} ({CurrentHour:D2}:00)");
    }

    /// <summary>
    /// Update the current time of day based on the hour
    /// </summary>
    private void UpdateTimeOfDay()
    {
        TimeOfDay newTime = GetTimeOfDayFromHour(CurrentHour);
        if (newTime == CurrentTimeOfDay) return;
        
        PreviousTimeOfDay = CurrentTimeOfDay;
        CurrentTimeOfDay = newTime;
        WorldEvents.RaiseTimeOfDayChanged(CurrentTimeOfDay);
    }

    /// <summary>
    /// Determine time of day based on current hour
    /// </summary>
    private TimeOfDay GetTimeOfDayFromHour(int hour)
    {
        if (hour >= morningStart && hour <= morningEnd)
            return TimeOfDay.Morning;
        
        if (hour >= dayStart && hour <= dayEnd)
            return TimeOfDay.Day;
        
        if (hour >= eveningStart && hour <= eveningEnd)
            return TimeOfDay.Evening;
        
        return TimeOfDay.Night;
    }

    public string GetTimeAsString() => $"{CurrentHour:D2}:00";

    #endregion

    #region Events

    public void Subscribe()
    {
        WorldEvents.OnZoneChanged += HandleZoneChanged;
        BattleEvents.OnBattleEnd += HandleBattleEnd;
    }

    public void Unsubscribe()
    {
        WorldEvents.OnZoneChanged -= HandleZoneChanged;
        BattleEvents.OnBattleEnd -= HandleBattleEnd;
    }

    private void HandleZoneChanged(ZoneDefinition zoneCurrent, ZoneDefinition zonePrevious, string zoneName)
    {
        AdvanceHour();
    }

    private void HandleBattleEnd()
    {
        AdvanceHour();
    }
    
    #endregion
}
