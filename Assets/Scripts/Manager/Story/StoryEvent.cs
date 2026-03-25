using UnityEngine;
using System.Collections.Generic;
using Simulation.Enums.StoryEvent;
using Simulation.Enums.Story;

public class StoryEvent
{

    #region Components

    private StoryEventComponentAttributes attributesComponent;
    private LocalizationComponentString localizationStringComponent;
    private StoryEventComponentPrerequisites prerequisitesComponent;
    private StoryEventComponentEffects effectsComponent;
    private StoryEventComponentCutscene cutsceneComponent;
    private StoryEventComponentScriptedEvent scriptedEventComponent;
    private StoryEventComponentPersistence persistenceComponent;

    #endregion

    #region Initialize

    public StoryEvent(StoryEventData storyEventData) 
    {
        Initialize(characterData, characterSaveData);
    }

    public void Initialize(StoryEventData storyEventData)
    {
        attributesComponent = new StoryEventComponentAttributes(storyEventData, this, storyEventSaveData);
        localizationStringComponent = new LocalizationComponentString(
            LocalizationEntity.StoryEvent,
            storyEventData.StoryEventId,
            new[] { LocalizationField.Name, LocalizationField.Description }
        );
        prerequisitesComponent = new StoryEventComponentPrerequisites(storyEventData, this, storyEventSaveData);
        effectsComponent = new StoryEventComponentEffects(storyEventData, this, storyEventSaveData);
        cutsceneComponent = new StoryEventComponentCutscene(storyEventData, this, storyEventSaveData);
        scriptedEventComponent = new StoryEventComponentScriptedEvent(storyEventData, this, storyEventSaveData);
        persistenceComponent = new StoryEventComponentPersistence(storyEventData, this, storyEventSaveData);
    }

    #endregion

    #region API

    // attributesComponent
    public string StoryEventId => attributesComponent.StoryEventId();

    // localizationComponent
    public LocalizationComponentString LocalizationComponent => localizationStringComponent;
    public string StoryEventName => localizationStringComponent.GetString(LocalizationField.Name);
    public string StoryEventDescription => localizationStringComponent.GetString(LocalizationField.Description);

    // prerequisitesComponent;
    public IReadOnlyList<StoryPrerequisite> Prerequisites => prerequisitesComponent.Prerequisites;

    // effectsComponent;
    public IReadOnlyList<StoryEffect> Effects => effectsComponent.Effects;

    // cutsceneComponent;
    public string CutsceneId => cutsceneComponent.CutsceneId;
    public bool HasCutscene => cutsceneComponent.HasCutscene;
    public AudioClip CutsceneBgmClip => cutsceneComponent.CutsceneBgmClip;

    // scriptedEventComponent;
    public string ScriptedEventId => scriptedEventComponent.ScriptedEventId;
    public bool HasScriptedEvent => scriptedEventComponent.HasScriptedEvent;
    public AudioClip ScriptedEventBgmClip => scriptedEventComponent.ScriptedEventBgmClip;

    // persistenceComponent;
    public bool IsCompleted => persistenceComponent.IsCompleted;

    #endregion

}
