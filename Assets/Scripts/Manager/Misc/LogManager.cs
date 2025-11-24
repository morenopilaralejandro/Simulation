using UnityEngine;
using Simulation.Enums.Log;

public static class LogManager
{
    // Minimum log level for printing messages
    public static LogLevel MinimumLogLevel = LogLevel.Trace;

    public static void Log(string message, LogLevel level, UnityEngine.Object context = null)
    {
    #if UNITY_EDITOR || DEVELOPMENT_BUILD
        if (level >= MinimumLogLevel)
        {
            switch (level)
            {
                case LogLevel.Trace:
                case LogLevel.Debug:
                case LogLevel.Info:
                    UnityEngine.Debug.Log(message, context);
                    break;
                case LogLevel.Warning:
                    UnityEngine.Debug.LogWarning(message, context);
                    break;
                case LogLevel.Error:
                case LogLevel.Fatal:
                    UnityEngine.Debug.LogError(message, context);
                    break;
            }
        }
    #endif
    }

    // Optional Helper Methods for each log level
    public static void Trace(string message, UnityEngine.Object context = null)
        => Log(message, LogLevel.Trace, context);

    public static void Debug(string message, UnityEngine.Object context = null)
        => Log(message, LogLevel.Debug, context);

    public static void Info(string message, UnityEngine.Object context = null)
        => Log(message, LogLevel.Info, context);

    public static void Warning(string message, UnityEngine.Object context = null)
        => Log(message, LogLevel.Warning, context);

    public static void Error(string message, UnityEngine.Object context = null)
        => Log(message, LogLevel.Error, context);

    public static void Fatal(string message, UnityEngine.Object context = null)
        => Log(message, LogLevel.Fatal, context);
}

/*
Usage: Log with the appropriate LogLevel.
LogManager.Trace("A very detailed message.", this);
LogManager.Debug("Debug info", this);
LogManager.Info("Something happened.", this);
LogManager.Warning("Potential issue.", this);
LogManager.Error("An error occurred.", this);
LogManager.Fatal("Critical failure!", this);
*/
