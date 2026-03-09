using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Simulation.Enums.Localization;

/// <summary>
/// Bridges Ink's localization keys to Unity's Localization system
/// using LocalizationComponentString as the underlying lookup mechanism.
/// 
/// HOW IT WORKS:
/// 1. Ink outputs a line with tag #loc:elder_oak_greeting_01
/// 2. This bridge creates/caches a LocalizationComponentString for that key
/// 3. The localized string is returned (with smart string + variable substitution)
/// 4. If the key isn't found, falls back to the raw ink text
/// </summary>
public class DialogLocalizationBridge : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // Cached localization components, keyed by their localization ID.
    // We lazily create and cache these so we don't re-allocate
    // a LocalizationComponentString every frame.
    // ─────────────────────────────────────────────
    private readonly Dictionary<string, LocalizationComponentString> _dialogCache    
        = new Dictionary<string, LocalizationComponentString>();

    private readonly Dictionary<string, LocalizationComponentString> _choiceCache    
        = new Dictionary<string, LocalizationComponentString>();

    private readonly Dictionary<string, LocalizationComponentString> _charNameCache  
        = new Dictionary<string, LocalizationComponentString>();

    private readonly Dictionary<string, LocalizationComponentString> _itemNameCache  
        = new Dictionary<string, LocalizationComponentString>();

    // Variable sources for manual {variable} substitution
    // (for variables coming from Ink, not from Smart Strings)
    private readonly Dictionary<string, string> _variableOverrides 
        = new Dictionary<string, string>();

    // ─────────────────────────────────────────────
    // Cache helpers
    // ─────────────────────────────────────────────

    /// <summary>
    /// Gets or creates a cached LocalizationComponentString for the given
    /// entity type, ID, field, and cache dictionary.
    /// </summary>
    private LocalizationComponentString GetOrCreate(
        Dictionary<string, LocalizationComponentString> cache,
        LocalizationEntity entity,
        string id,
        LocalizationField field)
    {
        if (string.IsNullOrEmpty(id)) return null;

        if (!cache.TryGetValue(id, out var component))
        {
            component = new LocalizationComponentString(
                entity,
                id,
                new[] { field }
            );
            cache[id] = component;
        }

        return component;
    }

    // ─────────────────────────────────────────────
    // Variable management (for Ink-driven variables)
    // ─────────────────────────────────────────────

    /// <summary>
    /// Set a variable that will be substituted in localized strings.
    /// For variables that come from Ink's runtime (player_name, etc.)
    /// </summary>
    public void SetVariable(string key, string value)
    {
        _variableOverrides[key] = value;
    }

    public void SetVariable(string key, int value)
    {
        _variableOverrides[key] = value.ToString();
    }

    public void ClearVariables()
    {
        _variableOverrides.Clear();
    }

    // ─────────────────────────────────────────────
    // Smart String argument helpers
    // ─────────────────────────────────────────────

    /// <summary>
    /// Sets Smart String arguments on a dialog line's localization component.
    /// Use this when your Unity Localization table entries use Smart String
    /// syntax like "{0}" or named references.
    /// 
    /// Example:
    ///   SetDialogSmartArgs("elder_oak_greeting_01", new { player_name = "Ash", badge_count = 3 });
    /// </summary>
    public void SetDialogSmartArgs(string localizationKey, object args)
    {
        var component = GetOrCreate(
            _dialogCache, 
            LocalizationEntity.Dialog, 
            localizationKey, 
            LocalizationField.Text
        );
        component?.SetArguments(LocalizationField.Text, args);
    }

    public void SetChoiceSmartArgs(string localizationKey, object args)
    {
        var component = GetOrCreate(
            _choiceCache, 
            LocalizationEntity.Dialog, 
            localizationKey, 
            LocalizationField.Text
        );
        component?.SetArguments(LocalizationField.Text, args);
    }

    // ─────────────────────────────────────────────
    // Resolution methods
    // ─────────────────────────────────────────────

    /// <summary>
    /// Resolve a dialog line: look up localization key, substitute variables.
    /// </summary>
    public string ResolveDialogText(DialogLine line)
    {
        string text = line.RawText; // default fallback

        if (line.HasLocalization)
        {
            var component = GetOrCreate(
                _dialogCache,
                LocalizationEntity.Dialog,
                line.LocalizationKey,
                LocalizationField.Text
            );

            if (component != null)
            {
                string localized = component.GetString(LocalizationField.Text);

                if (!string.IsNullOrEmpty(localized))
                {
                    text = localized;
                }
                else
                {
                    LogManager.Trace(
                        $"[DialogLocalizationBridge] Localization key not found: " +
                        $"{line.LocalizationKey}. Using raw text.");
                }
            }
        }

        return SubstituteVariables(text);
    }

    /// <summary>
    /// Resolve a choice option's display text.
    /// </summary>
    public string ResolveChoiceText(DialogChoice choice)
    {
        string text = choice.RawText;

        if (!string.IsNullOrEmpty(choice.LocalizationKey))
        {
            var component = GetOrCreate(
                _choiceCache,
                LocalizationEntity.Dialog,
                choice.LocalizationKey,
                LocalizationField.Text
            );

            if (component != null)
            {
                string localized = component.GetString(LocalizationField.Text);
                if (!string.IsNullOrEmpty(localized))
                    text = localized;
            }
        }

        return SubstituteVariables(text);
    }

    /// <summary>
    /// Resolve a character's display name from their ID.
    /// </summary>
    public string ResolveCharacterName(string characterId)
    {
        if (string.IsNullOrEmpty(characterId)) return characterId;

        var component = GetOrCreate(
            _charNameCache,
            LocalizationEntity.Character,
            characterId,
            LocalizationField.Name
        );

        if (component != null)
        {
            string localized = component.GetString(LocalizationField.Name);
            if (!string.IsNullOrEmpty(localized))
                return localized;
        }

        return characterId; // fallback to raw ID
    }

    /// <summary>
    /// Resolve an item's display name from its ID.
    /// </summary>
    public string ResolveItemName(string itemId)
    {
        return null;
        /*
        if (string.IsNullOrEmpty(itemId)) return itemId;

        var component = GetOrCreate(
            _itemNameCache,
            LocalizationEntity.Item,
            itemId,
            LocalizationField.Name
        );

        if (component != null)
        {
            string localized = component.GetString(LocalizationField.Name);
            if (!string.IsNullOrEmpty(localized))
                return localized;
        }

        return itemId;
        */
    }

    // ─────────────────────────────────────────────
    // Variable substitution (for Ink-driven {var} patterns)
    // ─────────────────────────────────────────────

    /// <summary>
    /// Replaces {variable_name} patterns with values from _variableOverrides.
    /// This handles Ink-sourced variables that aren't part of Unity's 
    /// Smart String system.
    /// </summary>
    private string SubstituteVariables(string text)
    {
        if (string.IsNullOrEmpty(text) || _variableOverrides.Count == 0) 
            return text;

        return Regex.Replace(text, @"\{(\w+)\}", match =>
        {
            string varName = match.Groups[1].Value;
            if (_variableOverrides.TryGetValue(varName, out string value))
                return value;
            return match.Value; // leave as-is if not found
        });
    }

    // ─────────────────────────────────────────────
    // Cache management
    // ─────────────────────────────────────────────

    /// <summary>
    /// Clear all caches. Call when changing scenes or conversation contexts
    /// to free memory from accumulated localization components.
    /// </summary>
    public void ClearCaches()
    {
        _dialogCache.Clear();
        _choiceCache.Clear();
        _charNameCache.Clear();
        _itemNameCache.Clear();
        _variableOverrides.Clear();
    }

    /// <summary>
    /// Pre-warm the dialog cache for a set of known localization keys.
    /// Call before a conversation starts to avoid allocation during dialogue.
    /// </summary>
    public void PrewarmDialogKeys(IEnumerable<string> localizationKeys)
    {
        foreach (var key in localizationKeys)
        {
            GetOrCreate(
                _dialogCache, 
                LocalizationEntity.Dialog, 
                key, 
                LocalizationField.Text
            );
        }
    }

    public void PrewarmChoiceKeys(IEnumerable<string> localizationKeys)
    {
        foreach (var key in localizationKeys)
        {
            GetOrCreate(
                _choiceCache, 
                LocalizationEntity.Dialog, 
                key, 
                LocalizationField.Text
            );
        }
    }

    private void OnDestroy()
    {
        ClearCaches();
    }
}
