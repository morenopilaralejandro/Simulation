[System.Serializable]
public class SaveDataHeader
{
    public uint FileSignature;
    public string GameIdentifier;
    public int SaveFormatVersion;
    public string GameVersion;
}
