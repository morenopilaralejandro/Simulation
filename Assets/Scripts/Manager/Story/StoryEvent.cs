using UnityEngine;
using System.Collections.Generic;
using Aremoreno.Enums.Localization;
using Aremoreno.Enums.Quest;
using Aremoreno.Enums.Story;

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
        Initialize(storyEventData);
    }

    public void Initialize(StoryEventData storyEventData)
    {
        attributesComponent = new StoryEventComponentAttributes(storyEventData, this);
        localizationStringComponent = new LocalizationComponentString(
            LocalizationEntity.Story_Event,
            storyEventData.StoryEventId,
            new[] { LocalizationField.Name, LocalizationField.Description }
        );
        prerequisitesComponent = new StoryEventComponentPrerequisites(storyEventData, this);
        effectsComponent = new StoryEventComponentEffects(storyEventData, this);
        cutsceneComponent = new StoryEventComponentCutscene(storyEventData, this);
        scriptedEventComponent = new StoryEventComponentScriptedEvent(storyEventData, this);
        persistenceComponent = new StoryEventComponentPersistence(storyEventData, this);
    }

    #endregion

    #region API

    // attributesComponent
    public string StoryEventId => attributesComponent.StoryEventId;

    // localizationComponent
    public LocalizationComponentString LocalizationComponent => localizationStringComponent;
    public string StoryEventName => localizationStringComponent.GetString(LocalizationField.Name);
    public string StoryEventDescription => localizationStringComponent.GetString(LocalizationField.Description);

    // prerequisitesComponent;
    public IReadOnlyList<StoryPrerequisite> StoryPrerequisites => prerequisitesComponent.StoryPrerequisites;

    // effectsComponent;
    public IReadOnlyList<StoryEffect> StoryEffects => effectsComponent.StoryEffects;

    // cutsceneComponent;
    public string CutsceneId => cutsceneComponent.CutsceneId;
    public bool HasCutscene => cutsceneComponent.HasCutscene;
    public string CutsceneBgmId => cutsceneComponent.CutsceneBgmId;

    // scriptedEventComponent;
    public string ScriptedEventId => scriptedEventComponent.ScriptedEventId;
    public bool HasScriptedEvent => scriptedEventComponent.HasScriptedEvent;
    public string ScriptedEventBgmId => scriptedEventComponent.ScriptedEventBgmId;

    // persistenceComponent;
    public bool IsCompleted => persistenceComponent.IsCompleted;

    #endregion

}
