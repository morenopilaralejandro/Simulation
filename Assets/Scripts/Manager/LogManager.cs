using UnityEngine;
using Simulation.Enums.Log;

public static class LogManager
{
    // Minimum log level for printing messages
    public static Level MinimumLevel = Level.Trace;

    public static void Log(string message, Level level, UnityEngine.Object context = null)
    {
    #if UNITY_EDITOR || DEVELOPMENT_BUILD
        if (level >= MinimumLevel)
        {
            switch (level)
            {
                case Level.Trace:
                case Level.Debug:
                case Level.Info:
                    UnityEngine.Debug.Log(message, context);
                    break;
                case Level.Warning:
                    UnityEngine.Debug.LogWarning(message, context);
                    break;
                case Level.Error:
                case Level.Fatal:
                    UnityEngine.Debug.LogError(message, context);
                    break;
            }
        }
    #endif
    }

    // Optional Helper Methods for each log level
    public static void Trace(string message, UnityEngine.Object context = null)
        => Log(message, Level.Trace, context);

    public static void Debug(string message, UnityEngine.Object context = null)
        => Log(message, Level.Debug, context);

    public static void Info(string message, UnityEngine.Object context = null)
        => Log(message, Level.Info, context);

    public static void Warning(string message, UnityEngine.Object context = null)
        => Log(message, Level.Warning, context);

    public static void Error(string message, UnityEngine.Object context = null)
        => Log(message, Level.Error, context);

    public static void Fatal(string message, UnityEngine.Object context = null)
        => Log(message, Level.Fatal, context);
}

/*
Usage: Log with the appropriate Level.
LogManager.Trace("A very detailed message.", this);
LogManager.Debug("Debug info", this);
LogManager.Info("Something happened.", this);
LogManager.Warning("Potential issue.", this);
LogManager.Error("An error occurred.", this);
LogManager.Fatal("Critical failure!", this);
*/
