using System;
using System.Collections.Generic;
using Aremoreno.Enums.Quest;
using Aremoreno.Enums.Story;

[Serializable]
public class QuestSystemSaveData
{
    public string CurrentMainQuestId;
    public string CurrentActiveQuestId;
    public List<SerializableKeyValue<string, QuestSaveData>> QuestSaveDataList;
}

