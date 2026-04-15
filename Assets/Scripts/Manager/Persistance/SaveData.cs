[System.Serializable]
public class SaveData
{
    public SaveDataHeader Header;
    public long TimestampSave;
    public long TimestampCreation; //DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    public CharacterSystemSaveData CharacterSystemSaveData;
    public SaveDataItemSystem SaveDataItemSystem;
    public QuestSystemSaveData QuestSystemSaveData;
    public StorySystemSaveData StorySystemSaveData;
    public ChestStateSaveData ChestStateSaveData;
    public SaveDataWorldSystem SaveDataWorldSystem;
    public SaveDataTeamSystem SaveDataTeamSystem;
}
