[System.Serializable]
public class SaveData
{
    public int SaveVersion;
    public long SaveTimestamp; //DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    /*
    public PlayerData PlayerSaveData; //TODO position in overworld, current area, money, etc
    public StorySaveData StorySaveData; //TODO story event progresion, etc,
    public List<CharacterSaveData> CharacterStorage;
    public List<ItemSaveData> ItemStorage; //TODO item id and amount
    public List<TeamSaveData> TeamStorage;
    //TODO include current active team



    */
}
