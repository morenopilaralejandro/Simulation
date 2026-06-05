using UnityEngine;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Wing;

public class WingComponentAttributes
{
    //private Wing wing;

    public string WingId { get; private set; }
    public Element Element { get; private set; }

    public WingComponentAttributes(WingData wingData, Wing wing)
    {
        Initialize(wingData, wing);
    }

    public void Initialize(WingData wingData, Wing wing)
    {
        this.WingId = wingData.WingId;
        this.Element = wingData.Element;
    }
}
