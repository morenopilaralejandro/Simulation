using UnityEngine;
using System.Collections.Generic;
using Aremoreno.Enums.Quest;
using Aremoreno.Enums.Story;

public class StoryAutoTrigger
{
    #region Components

    private StoryAutoTriggerComponentAttributes attributesComponent;
    private StoryAutoTriggerComponentStoryEvent storyEventComponent;
    private StoryAutoTriggerComponentPrerequisites prerequisitesComponent;
    private StoryAutoTriggerComponentPersistence persistenceComponent;

    #endregion

    #region Initialize

    public StoryAutoTrigger(StoryAutoTriggerData storyAutoTriggerData) 
    {
        Initialize(storyAutoTriggerData);
    }

    public void Initialize(StoryAutoTriggerData storyAutoTriggerData)
    {
        attributesComponent = new StoryAutoTriggerComponentAttributes(storyAutoTriggerData, this);
        storyEventComponent = new StoryAutoTriggerComponentStoryEvent(storyAutoTriggerData, this);
        prerequisitesComponent = new StoryAutoTriggerComponentPrerequisites(storyAutoTriggerData, this);
        persistenceComponent = new StoryAutoTriggerComponentPersistence(storyAutoTriggerData, this);
    }

    #endregion

    #region API

    // attributesComponent
    public string StoryAutoTriggerId => attributesComponent.StoryAutoTriggerId;

    // storyEventComponent
    public StoryEvent StoryEvent => storyEventComponent.StoryEvent;

    // prerequisitesComponent
    public IReadOnlyList<StoryPrerequisite> Prerequisites => prerequisitesComponent.Prerequisites;

    // persistenceComponent
    public bool HasTriggered  => persistenceComponent.HasTriggered;

    #endregion
}
