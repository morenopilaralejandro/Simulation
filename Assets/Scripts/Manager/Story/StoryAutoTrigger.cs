using UnityEngine;
using System.Collections.Generic;
using Simulation.Enums.StoryAutoTrigger;
using Simulation.Enums.Story;

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
        Initialize(characterData, characterSaveData);
    }

    public void Initialize(StoryAutoTriggerData storyAutoTriggerData)
    {
        attributesComponent = new StoryAutoTriggerComponentAttributes(storyAutoTriggerData, this, storyAutoTriggerSaveData);

        StoryAutoTriggerComponentStoryEvent
        StoryAutoTriggerComponentPrerequisites
        private StoryAutoTriggerComponentPersistence persistenceComponent;
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
