using System;
using System.Collections.Generic;
using UnityEngine;
using Simulation.Enums.Quest;
using Simulation.Enums.Story;

public class StoryChapterComponentStoryEvents
{
    public StoryEvents IntroEvent { get; private set; }

    public StoryChapterComponentStoryEvents(StoryChapterData storyChapterData, StoryChapter storyChapter)
    {
        IntroEvent = StoryEventFactory.CreateById(storyChapterData.IntroEventId);
    }
}
