using System;
using System.Collections.Generic;

public class MenuStateMachine<TState> where TState : struct, Enum
{
    private readonly Dictionary<TState, Action> onEnter = new();
    private readonly Dictionary<TState, Action> onExit  = new();

    public TState Current { get; private set; }
    public event Action<TState, TState> OnChanged; // (from, to)

    public MenuStateMachine(TState initial) { Current = initial; }

    public MenuStateMachine<TState> OnEnter(TState s, Action a) { onEnter[s] = a; return this; }
    public MenuStateMachine<TState> OnExit (TState s, Action a) { onExit [s] = a; return this; }

    public void Set(TState next)
    {
        if (EqualityComparer<TState>.Default.Equals(Current, next)) return;
        var prev = Current;
        if (onExit.TryGetValue(prev, out var ex))  ex();
        Current = next;
        if (onEnter.TryGetValue(next, out var en)) en();
        OnChanged?.Invoke(prev, next);
    }

    public bool Is(TState s) => EqualityComparer<TState>.Default.Equals(Current, s);
}
