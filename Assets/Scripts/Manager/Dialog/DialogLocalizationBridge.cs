using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Aremoreno.Enums.Localization;

/// <summary>
/// Bridges Ink localization keys to your LocalizationComponentString system.
/// 
/// Each unique localization key gets a cached LocalizationComponentString.
/// Variables are injected via SetArguments using anonymous objects,
/// which Unity Smart Strings resolve automatically.
/// 
/// SMART STRING FORMAT IN YOUR TABLES:
///   "Hello, {player_name}! You have {gold_amount} gold."
/// These get resolved when SetArguments passes the matching object.
/// </summary>
public class DialogLocalizationBridge : MonoBehaviour
{
    // ============ CACHES ============
    // We cache LocalizationComponentString per key to avoid
    // recreating them every line. Dialog keys are reusable
    // (e.g., revisiting an NPC shows the same keys).

    private Dictionary<string, LocalizationComponentString> _dialogCache
        = new Dictionary<string, LocalizationComponentString>();

    private Dictionary<string, LocalizationComponentString> _choiceCache
        = new Dictionary<string, LocalizationComponentString>();

    private Dictionary<string, LocalizationComponentString> _characterNameCache
        = new Dictionary<string, LocalizationComponentString>();

    private Dictionary<string, LocalizationComponentString> _itemNameCache
        = new Dictionary<string, LocalizationComponentString>();

    // ============ VARIABLES ============
    // Collected from ink state, passed as arguments to Smart Strings.

    private Dictionary<string, object> _variables = new Dictionary<string, object>();

    // ============ VARIABLE MANAGEMENT ============

    public void SetVariable(string key, string value)
    {
        _variables[key] = value;
    }

    public void SetVariable(string key, int value)
    {
        _variables[key] = value;
    }

    public void SetVariable(string key, float value)
    {
        _variables[key] = value;
    }

    public void SetVariable(string key, bool value)
    {
        _variables[key] = value;
    }

    public void ClearVariables()
    {
        _variables.Clear();
    }

    // ============ RESOLVE METHODS ============

    /// <summary>
    /// Resolve a dialog line's text.
    /// Looks up #loc: key in DialogTable, injects variables, falls back to raw text.
    /// </summary>
    public string ResolveDialogText(DialogLine line)
    {
        if (!line.HasLocalization)
            return SubstituteVariablesFallback(line.RawText);

        var locComponent = GetOrCreateDialog(line.LocalizationKey);
        InjectArguments(locComponent, LocalizationField.Text);

        string result = locComponent.GetString(LocalizationField.Text);

        if (string.IsNullOrEmpty(result))
        {
            Debug.LogWarning(
                $"[DialogLocBridge] Key not found: {line.LocalizationKey}. Using raw text.");
            return SubstituteVariablesFallback(line.RawText);
        }

        return result;
    }

    /// <summary>
    /// Resolve a choice's display text.
    /// </summary>
    public string ResolveChoiceText(DialogChoice choice)
    {
        if (string.IsNullOrEmpty(choice.LocalizationKey))
            return SubstituteVariablesFallback(choice.RawText);

        var locComponent = GetOrCreateChoice(choice.LocalizationKey);
        InjectArguments(locComponent, LocalizationField.Text);

        string result = locComponent.GetString(LocalizationField.Text);

        if (string.IsNullOrEmpty(result))
            return SubstituteVariablesFallback(choice.RawText);

        return result;
    }

    /// <summary>
    /// Resolve a character's display name.
    /// </summary>
    public string ResolveCharacterName(string characterId)
    {
        if (string.IsNullOrEmpty(characterId))
            return string.Empty;

        var locComponent = GetOrCreateCharacterName(characterId);
        string result = locComponent.GetString(LocalizationField.Name);

        // Fallback to raw ID if not found
        return string.IsNullOrEmpty(result) ? characterId : result;
    }

    /// <summary>
    /// Resolve an item's display name.
    /// </summary>
    public string ResolveItemName(string itemId)
    {
        if (string.IsNullOrEmpty(itemId))
            return string.Empty;

        var locComponent = GetOrCreateItemName(itemId);
        string result = locComponent.GetString(LocalizationField.Name);

        return string.IsNullOrEmpty(result) ? itemId : result;
    }

    // ============ CACHE MANAGEMENT ============

    private LocalizationComponentString GetOrCreateDialog(string locKey)
    {
        if (!_dialogCache.TryGetValue(locKey, out var component))
        {
            component = new LocalizationComponentString(
                LocalizationEntity.Dialog,
                locKey,
                new[] { LocalizationField.Text }
            );
            _dialogCache[locKey] = component;
        }
        return component;
    }

    private LocalizationComponentString GetOrCreateChoice(string locKey)
    {
        if (!_choiceCache.TryGetValue(locKey, out var component))
        {
            component = new LocalizationComponentString(
                LocalizationEntity.Dialog,
                locKey,
                new[] { LocalizationField.Text }
            );
            _choiceCache[locKey] = component;
        }
        return component;
    }

    private LocalizationComponentString GetOrCreateCharacterName(string characterId)
    {
        if (!_characterNameCache.TryGetValue(characterId, out var component))
        {
            component = new LocalizationComponentString(
                LocalizationEntity.Character,
                characterId,
                new[] { LocalizationField.Name }
            );
            _characterNameCache[characterId] = component;
        }
        return component;
    }

    private LocalizationComponentString GetOrCreateItemName(string itemId)
    {
        if (!_itemNameCache.TryGetValue(itemId, out var component))
        {
            component = new LocalizationComponentString(
                LocalizationEntity.Item,
                itemId,
                new[] { LocalizationField.Name }
            );
            _itemNameCache[itemId] = component;
        }
        return component;
    }

    // ============ ARGUMENT INJECTION ============

    /// <summary>
    /// Injects all current variables into a LocalizationComponentString
    /// as a dynamic object for Unity Smart String resolution.
    /// 
    /// Your localization table entry:
    ///   "Hello, {player_name}! You got {item_name} x{item_count}!"
    /// 
    /// We build an anonymous object with all variables so Smart Strings
    /// can resolve any of them.
    /// </summary>
    private void InjectArguments(LocalizationComponentString component, LocalizationField field)
    {
        if (_variables.Count == 0) return;

        // Build a Dictionary that Unity Smart Strings can read.
        // Smart Strings support Dictionary<string, object> as arguments.
        var args = new Dictionary<string, object>(_variables);

        component.SetArguments(field, args);
    }

    /// <summary>
    /// Fallback regex substitution for raw ink text (when no loc key exists).
    /// Replaces {variable_name} with values from our variable dictionary.
    /// </summary>
    private string SubstituteVariablesFallback(string text)
    {
        if (string.IsNullOrEmpty(text) || _variables.Count == 0)
            return text;

        return Regex.Replace(text, @"\{(\w+)\}", match =>
        {
            string varName = match.Groups[1].Value;
            if (_variables.TryGetValue(varName, out object value))
                return value.ToString();
            return match.Value;
        });
    }

    // ============ CLEANUP ============

    /// <summary>
    /// Clear all caches. Call on scene change or when memory is a concern.
    /// Usually not needed since cached entries are lightweight.
    /// </summary>
    public void ClearCaches()
    {
        _dialogCache.Clear();
        _choiceCache.Clear();
        _characterNameCache.Clear();
        _itemNameCache.Clear();
    }
}
