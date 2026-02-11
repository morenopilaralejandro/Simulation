[System.Serializable]
public class DataTransferPacket
{
    public int protocolVersion = 1;
    public string deviceId;
    public string timestamp;
    public SaveData saveData;
}
