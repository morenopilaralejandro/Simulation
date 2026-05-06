using UnityEngine;
using System.Collections.Generic;

public class SelectorLoadoutSource : ISelectorSource<Team>
{
    public IEnumerable<Team> Enumerate() 
        => TeamManager.Instance.GetAllLoadouts();
}
