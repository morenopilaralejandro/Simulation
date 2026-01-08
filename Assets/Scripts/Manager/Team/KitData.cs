using UnityEngine;

[CreateAssetMenu(fileName = "KitData", menuName = "ScriptableObject/KitData")]
public class KitData : ScriptableObject
{
    public string KitId;

    public Color BaseColorHomeField;
    public Color DetailColorHomeField;
    public Color ShockColorHomeField;

    public Color BaseColorHomeKeeper;
    public Color DetailColorHomeKeeper;
    public Color ShockColorHomeKeeper;

    public Color BaseColorAwayField;
    public Color DetailColorAwayField;
    public Color ShockColorAwayField;

    public Color BaseColorAwayKeeper;
    public Color DetailColorAwayKeeper;
    public Color ShockColorAwayKeeper;
}
