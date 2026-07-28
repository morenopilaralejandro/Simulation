using UnityEngine;

[System.Serializable]
public class BattleResultDataXp
{
    public Character Character;

    public int StartLevel;
    public int StartXp;
    public int StartXpToNextLevel;

    public int EndLevel;
    public int EndXp;
    public int EndXpToNextLevel;

    public int XPGained;

    public void Clear()
    {
        Character = null;

        StartLevel = 0;
        StartXp = 0;
        StartXpToNextLevel = 0;

        EndLevel = 0;
        EndXp = 0;
        EndXpToNextLevel = 0;

        XPGained = 0;
    }
}
