using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Wing;

[CreateAssetMenu(menuName = "ScriptableObject/Dimension/Wing/WingEvolutionPath")]
public class WingEvolutionPath : ScriptableObject
{
    public WingGrowthType growthType;

    public List<WingEvolutionPathEntry> evolutionPath;

    private Dictionary<WingEvolution, WingEvolution> _pathMap;

    private List<WingEvolution> _orderedEvolutions;


    public void OnEnable()    
    {
        _pathMap = evolutionPath.ToDictionary(p => p.Previous, p => p.Next);

        _orderedEvolutions = new List<WingEvolution>();
        if (evolutionPath.Count == 0) return;
        //_orderedEvolutions.Add(evolutionPath[0].Previous);
        foreach (var entry in evolutionPath)
            _orderedEvolutions.Add(entry.Next);
    }

    public bool TryGetNextEvolution(WingEvolution current, out WingEvolution next) => _pathMap.TryGetValue(current, out next);

    public bool IsLastEvolution(WingEvolution evo) => !_pathMap.ContainsKey(evo);

    public WingEvolution GetLastEvolution() => evolutionPath[evolutionPath.Count - 1].Next;

    public int GetEvolutionIndex(WingEvolution evolution) => _orderedEvolutions.IndexOf(evolution);
}
