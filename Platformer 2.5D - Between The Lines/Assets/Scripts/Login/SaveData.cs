[System.Serializable]
public partial class SaveData
{
    public string username;
    public int currentLevel = 0;
    public int lastCheckpoint = 0;
    public int collectibles = 0;
}

[System.Serializable]
public class JsonArrayWrapper
{
    public SaveData[] entries;
}