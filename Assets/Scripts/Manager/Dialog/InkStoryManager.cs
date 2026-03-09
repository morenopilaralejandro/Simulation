using UnityEngine;
using Ink.Runtime;
using System;
using System.Collections.Generic;

/// <summary>
/// Manages the Ink runtime story.
/// 
/// INK RUNTIME EXPLAINED:
/// - Story: The compiled ink file loaded at runtime (.json)
/// - story.Continue(): Advances to the next line of text
/// - story.canContinue: True if there's more text to show
/// - story.currentChoices: Available choices at current point
/// - story.ChooseChoiceIndex(i): Select choice i
/// - story.currentTags: Tags on the current line
/// - story.variablesState["name"]: Read/write ink variables
/// - story.BindExternalFunction(): Connect ink EXTERNAL to C#
/// - story.ChoosePathString("knot_name"): Jump to a knot
/// </summary>
public class InkStoryManager : MonoBehaviour
{
    [Header("Ink Story Files")]
    [SerializeField] private TextAsset _mainStoryJson;

    // For loading additional story files dynamically
    [SerializeField] private List<StoryMapping> _storyMappings = new List<StoryMapping>();

    private Story _currentStory;
    private Dictionary<string, Story> _storyCache = new Dictionary<string, Story>();

    // Dependencies
    private DialogLocalizationBridge _locBridge;
    private IDialogGameDataProvider _gameData;

    public bool IsActive { get; private set; }
    public bool CanContinue => _currentStory != null && _currentStory.canContinue;
    public bool HasChoices => _currentStory != null && _currentStory.currentChoices.Count > 0;

    public void Initialize(DialogLocalizationBridge locBridge, IDialogGameDataProvider gameData)
    {
        _locBridge = locBridge;
        _gameData = gameData;

        // Pre-compile main story
        if (_mainStoryJson != null)
        {
            var story = new Story(_mainStoryJson.text);
            BindExternalFunctions(story);
            _storyCache["main"] = story;
        }

        // Cache additional stories
        foreach (var mapping in _storyMappings)
        {
            if (mapping.storyJson != null)
            {
                var story = new Story(mapping.storyJson.text);
                BindExternalFunctions(story);
                _storyCache[mapping.storyId] = story;
            }
        }
    }

    /// <summary>
    /// Binds C# functions to ink EXTERNAL declarations.
    /// When ink calls GetItemName("sword"), it actually calls our C# method.
    /// </summary>
    private void BindExternalFunctions(Story story)
    {
        // Localization
        story.BindExternalFunction("GetLocalizedText", (string key) =>
        {
            return _locBridge.ResolveItemName(key); // generic localized text
        });

        // Item system
        story.BindExternalFunction("GetItemName", (string itemId) =>
        {
            string localizedName = _locBridge.ResolveItemName(itemId);
            return localizedName;
        });

        story.BindExternalFunction("HasItem", (string itemId) =>
        {
            return _gameData.HasItem(itemId);
        });

        story.BindExternalFunction("GetItemCount", (string itemId) =>
        {
            return _gameData.GetItemCount(itemId);
        });

        story.BindExternalFunction("GiveItem", (string itemId, int count) =>
        {
            _gameData.GiveItem(itemId, count);
        });

        story.BindExternalFunction("RemoveItem", (string itemId, int count) =>
        {
            _gameData.RemoveItem(itemId, count);
        });

        // Gold
        story.BindExternalFunction("GetGold", () =>
        {
            return _gameData.GetGold();
        });

        story.BindExternalFunction("GiveGold", (int amount) =>
        {
            _gameData.GiveGold(amount);
        });

        story.BindExternalFunction("RemoveGold", (int amount) =>
        {
            _gameData.RemoveGold(amount);
        });

        // Character
        story.BindExternalFunction("GetCharacterName", (string charId) =>
        {
            return _locBridge.ResolveCharacterName(charId);
        });

        // Sound
        story.BindExternalFunction("PlaySFX", (string sfxName) =>
        {
            DialogEvents.RaiseCommandExecuted(new DialogCommand
            {
                CommandName = "sfx",
                Parameters = new[] { sfxName }
            });
        });

        // Game flags
        story.BindExternalFunction("SetGameFlag", (string flagName, bool value) =>
        {
            _gameData.SetFlag(flagName, value);
        });

        // Events
        story.BindExternalFunction("TriggerEvent", (string eventName) =>
        {
            DialogEvents.RaiseCommandExecuted(new DialogCommand
            {
                CommandName = "event",
                Parameters = new[] { eventName }
            });
        });
    }

    /// <summary>
    /// Start a dialog by jumping to a specific knot in a story.
    /// storyId: which ink file to use ("main" for the main one)
    /// knotName: the === knot === to jump to
    /// </summary>
    public void StartDialog(string storyId, string knotName)
    {
        if (!_storyCache.TryGetValue(storyId, out _currentStory))
        {
            LogManager.Trace($"[InkStoryManager] Story not found: {storyId}");
            return;
        }

        IsActive = true;

        // Sync ink variables with game state before starting
        SyncVariablesFromGameState();

        // Jump to the requested knot
        _currentStory.ChoosePathString(knotName);

        // Start processing
        ContinueDialog();
    }

    /// <summary>
    /// Quick start for NPC conversations (uses main story file).
    /// </summary>
    public void StartNPCDialog(string knotName)
    {
        StartDialog("main", knotName);
    }

    /// <summary>
    /// Advance the story by one line.
    /// </summary>
    public void ContinueDialog()
    {
        if (_currentStory == null) return;

        if (_currentStory.canContinue)
        {
            // Continue() gets the next line of text
            string text = _currentStory.Continue();

            // currentTags contains all #tags from that line
            var tags = _currentStory.currentTags;

            // Parse into our structured format
            DialogLine line = InkTagParser.ParseLine(text.Trim(), tags);

            // Sync localization variables from ink state
            SyncLocalizationVariables();

            // Resolve localized text
            line.ResolvedText = _locBridge.ResolveDialogText(line);

            // Process commands (sfx, animations, etc.)
            ProcessCommands(line);

            // Notify UI
            DialogEvents.RaiseLineReady(line);
        }
        else if (_currentStory.currentChoices.Count > 0)
        {
            // We've hit a choice point
            PresentChoices();
        }
        else
        {
            // Dialog is done
            EndDialog();
        }
    }

    /// <summary>
    /// After showing a line, check if choices need to be presented.
    /// Called by DialogManager after text display completes.
    /// </summary>
    public void CheckForChoices()
    {
        if (_currentStory != null && _currentStory.currentChoices.Count > 0)
        {
            PresentChoices();
        }
    }

    private void PresentChoices()
    {
        var choices = new List<DialogChoice>();

        for (int i = 0; i < _currentStory.currentChoices.Count; i++)
        {
            var inkChoice = _currentStory.currentChoices[i];

            // Each choice can have tags too (in newer ink versions)
            // We parse tags from the choice text using our convention
            var choiceTags = inkChoice.tags;

            var dialogChoice = InkTagParser.ParseChoice(inkChoice.text, i, choiceTags);
            dialogChoice.ResolvedText = _locBridge.ResolveChoiceText(dialogChoice);
            choices.Add(dialogChoice);
        }

        DialogEvents.RaiseChoicesReady(choices);
    }

    /// <summary>
    /// Player selected a choice.
    /// </summary>
    public void SelectChoice(int choiceIndex)
    {
        if (_currentStory == null) return;

        _currentStory.ChooseChoiceIndex(choiceIndex);
        ContinueDialog();
    }

    /// <summary>
    /// Push ink variables to/from game state so both stay synchronized.
    /// </summary>
    private void SyncVariablesFromGameState()
    {
        if (_currentStory == null) return;

        try
        {
            _currentStory.variablesState["player_name"] = _gameData.GetPlayerName();
            _currentStory.variablesState["gold_amount"] = _gameData.GetGold();
            _currentStory.variablesState["has_forest_key"] = _gameData.HasItem("forest_key");
        }
        catch (Exception e)
        {
            LogManager.Trace($"[InkStoryManager] Variable sync error (may not exist in ink): {e.Message}");
        }
    }

    /// <summary>
    /// Sync ink variables into the localization system so {player_name} etc. resolve.
    /// </summary>
    private void SyncLocalizationVariables()
    {
        if (_currentStory == null) return;

        // Iterate through all ink variables and push to localization bridge
        foreach (string varName in _currentStory.variablesState)
        {
            var value = _currentStory.variablesState[varName];
            if (value != null)
            {
                _locBridge.SetVariable(varName, value.ToString());
            }
        }
    }

    private void ProcessCommands(DialogLine line)
    {
        // Process SFX tag
        if (!string.IsNullOrEmpty(line.SFX))
        {
            DialogEvents.RaiseCommandExecuted(new DialogCommand
            {
                CommandName = "sfx",
                Parameters = new[] { line.SFX }
            });
        }

        // Process animation tag
        if (!string.IsNullOrEmpty(line.Animation))
        {
            DialogEvents.RaiseCommandExecuted(new DialogCommand
            {
                CommandName = "anim",
                Parameters = new[] { line.Animation }
            });
        }

        // Process inline commands
        foreach (var cmd in line.Commands)
        {
            DialogEvents.RaiseCommandExecuted(cmd);
        }
    }

    private void EndDialog()
    {
        IsActive = false;
        SyncVariablesToGameState();
        DialogEvents.RaiseDialogComplete();
    }

    /// <summary>
    /// Push ink variable changes back to the game (quest flags, etc.)
    /// </summary>
    private void SyncVariablesToGameState()
    {
        if (_currentStory == null) return;

        try
        {
            // Read quest variables back from ink
            bool questStarted = (bool)_currentStory.variablesState["quest_elderwood_started"];
            _gameData.SetFlag("quest_elderwood_started", questStarted);

            bool questComplete = (bool)_currentStory.variablesState["quest_elderwood_complete"];
            _gameData.SetFlag("quest_elderwood_complete", questComplete);

            int reputation = (int)_currentStory.variablesState["reputation"];
            _gameData.SetReputation(reputation);
        }
        catch (Exception e)
        {
            LogManager.Trace($"[InkStoryManager] Variable writeback error: {e.Message}");
        }
    }

    /// <summary>
    /// Save ink story state for save/load system.
    /// </summary>
    public string GetStoryState(string storyId)
    {
        if (_storyCache.TryGetValue(storyId, out var story))
        {
            return story.state.ToJson();
        }
        return null;
    }

    /// <summary>
    /// Restore ink story state from save data.
    /// </summary>
    public void LoadStoryState(string storyId, string stateJson)
    {
        if (_storyCache.TryGetValue(storyId, out var story))
        {
            story.state.LoadJson(stateJson);
        }
    }
}
