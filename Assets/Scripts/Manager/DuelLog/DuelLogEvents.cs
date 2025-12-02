using System;
using UnityEngine;

public static class DuelLogEvents
{
    public static event Action<DuelLogEntry> OnNewEntry;
    public static void RaiseNewEntry(DuelLogEntry entry)
    {
        OnNewEntry?.Invoke(entry);
    }

    public static event Action<DuelLogPopup> OnHideCallback;
    public static void RaiseHideCallback(DuelLogPopup popup)
    {
        OnHideCallback?.Invoke(popup);
    }
}
