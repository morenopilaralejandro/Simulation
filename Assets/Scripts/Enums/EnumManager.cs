using System;
using System.Collections.Generic;
using Simulation.Enums.Character;
using Simulation.Enums.Move;

public static class EnumManager
{
    /// <summary>
    /// Converts a string to the given enum type (case-insensitive by default).
    /// Throws if parsing fails.
    /// </summary>
    public static T StringToEnum<T>(string value, bool ignoreCase = true) where T : struct, Enum
    {
        if (Enum.TryParse<T>(value, ignoreCase, out var result))
            return result;

        throw new ArgumentException($"'{value}' is not a valid {typeof(T).Name}");
    }

    /// <summary>
    /// Tries to convert a string to the given enum type (case-insensitive by default).
    /// Returns true/false instead of throwing.
    /// </summary>
    public static bool TryStringToEnum<T>(string value, out T result, bool ignoreCase = true) where T : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            result = default;
            return false;
        }

        return Enum.TryParse<T>(value.Trim(), ignoreCase, out result);
    }

    /// <summary>
    /// Converts an enum to its string representation.
    /// </summary>
    public static string EnumToString<T>(T enumValue) where T : Enum
    {
        return enumValue.ToString();
    }

    /// <summary>
    /// Converts a delimited string (like "FW|MF|DF") to a list of enums.
    /// Skips any invalid entries safely.
    /// </summary>
    public static List<T> ParseEnumList<T>(string value, char delimiter = '|') where T : struct, Enum
    {
        var list = new List<T>();
        if (string.IsNullOrWhiteSpace(value))
            return list;

        string[] parts = value.Split(delimiter);
        foreach (string part in parts)
        {
            if (TryStringToEnum<T>(part, out var parsed))
                list.Add(parsed);
            else
                UnityEngine.Debug.LogWarning(
                    $"EnumManager: Skipped invalid enum '{part}' for type {typeof(T).Name}.");
        }

        return list;
    }

    /// <summary>
    /// Converts a delimited string (like "Move1|Move2") to a list of plain strings.
    /// </summary>
    public static List<string> ParseStringList(string value, char delimiter = '|')
    {
        var list = new List<string>();
        if (string.IsNullOrWhiteSpace(value))
            return list;

        string[] parts = value.Split(delimiter);
        foreach (string part in parts)
        {
            string trimmed = part.Trim();
            if (!string.IsNullOrEmpty(trimmed))
                list.Add(trimmed);
        }

        return list;
    }

    /// <summary>
    /// Gets the number of defined values in the specified enum type.
    /// </summary>
    /// <typeparam name="T">The enum type.</typeparam>
    /// <returns>The number of values defined in the enum.</returns>
    public static int GetLength<T>() where T : System.Enum
    {
        return System.Enum.GetValues(typeof(T)).Length;
    }

    /// <summary>
    /// Gets an array containing all values of the specified enum type.
    /// </summary>
    /// <typeparam name="T">The enum type.</typeparam>
    /// <returns>An array of the enum’s values.</returns>
    public static T[] GetValues<T>() where T : System.Enum
    {
        return (T[])System.Enum.GetValues(typeof(T));
    }

    /// <summary>
    /// Gets an array containing the names of all values in the specified enum type.
    /// </summary>
    /// <typeparam name="T">The enum type.</typeparam>
    /// <returns>An array of strings representing the enum’s value names.</returns>
    public static string[] GetNames<T>() where T : System.Enum
    {
        return System.Enum.GetNames(typeof(T));
    }
}
