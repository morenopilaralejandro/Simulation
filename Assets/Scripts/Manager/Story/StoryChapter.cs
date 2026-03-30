using UnityEngine;
using System.Collections.Generic;
using Simulation.Enums.Localization;
using Simulation.Enums.Quest;
using Simulation.Enums.Story;

public class StoryChapter
{

    #region Components

    private StoryChapterComponentAttributes attributesComponent;
    private LocalizationComponentString localizationStringComponent;
    private StoryChapterComponentStoryEvents storyEventsComponent;
    private StoryChapterComponentQuests questsComponent;

    #endregion

    #region Initialize

    public StoryChapter(StoryChapterData storyChapterData) 
    {
        Initialize(storyChapterData);
    }

    public void Initialize(StoryChapterData storyChapterData)
    {
        attributesComponent = new StoryChapterComponentAttributes(storyChapterData, this);
        localizationStringComponent = new LocalizationComponentString(
            LocalizationEntity.Story_Chapter,
            storyChapterData.StoryChapterId,
            new[] { LocalizationField.Title, LocalizationField.Description }
        );
        storyEventsComponent = new StoryChapterComponentStoryEvents(storyChapterData, this);
        questsComponent = new StoryChapterComponentQuests(storyChapterData, this);
    }

    #endregion

    #region API

    // attributesComponent
    public string StoryChapterId { get; private set; }
    public int StoryChapterNumber { get; private set; }

    // localizationComponent
    public LocalizationComponentString LocalizationComponent => localizationStringComponent;
    public string StoryChapterTitle => localizationStringComponent.GetString(LocalizationField.Title);
    public string StoryChapterDescription => localizationStringComponent.GetString(LocalizationField.Description);

    // storyEventsComponent
    public StoryEvent IntroEvent => storyEventsComponent.IntroEvent;

    // questsComponent
    public IReadOnlyList<string> ChapterQuestIds => questsComponent.ChapterQuestIds;

    #endregion

}
