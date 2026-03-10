using System;
using System.Collections.Generic;
using Simulation.Enums.Dialog;

public static class DialogEvents
{
    // Manager
    public static event Action OnDialogStarted;
    public static void RaiseDialogStarted()
    {
        OnDialogStarted?.Invoke();
    }

    public static event Action OnDialogEnded;
    public static void RaiseDialogEnded()
    {
        OnDialogEnded?.Invoke();
    }

    public static event Action<string> OnGameEvent;
    public static void RaiseGameEvent(string eventName)
    {
        OnGameEvent?.Invoke(eventName);
    }

    // UI
    public static event Action OnTextDisplayComplete;
    public static void RaiseTextDisplayComplete()
    {
        OnTextDisplayComplete?.Invoke();
    }

    public static event Action<int> OnChoiceSelected;
    public static void RaiseChoiceSelected(int choiceIndex)
    {
        OnChoiceSelected?.Invoke(choiceIndex);
    }

    public static event Action OnContinueRequested;
    public static void RaiseContinueRequested()
    {
        OnContinueRequested?.Invoke();
    }

    // Story
    public static event Action<DialogLine> OnLineReady;
    public static void RaiseLineReady(DialogLine line)
    {
        OnLineReady?.Invoke(line);
    }

    public static event Action<List<DialogChoice>> OnChoicesReady;
    public static void RaiseChoicesReady(List<DialogChoice> choices)
    {
        OnChoicesReady?.Invoke(choices);
    }

    public static event Action OnDialogComplete;
    public static void RaiseDialogComplete()
    {
        OnDialogComplete?.Invoke();
    }

    public static event Action<DialogCommand> OnCommandExecuted;
    public static void RaiseCommandExecuted(DialogCommand command)
    {
        OnCommandExecuted?.Invoke(command);
    }

    public static event Action OnDialogMenuClosed;
    public static void RaiseDialogMenuClosed()
    {
        OnDialogMenuClosed?.Invoke();
    }
}
