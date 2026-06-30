using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Move;

[CreateAssetMenu(menuName = "ScriptableObject/MoveEvolutionPath")]
public class MoveEvolutionPath : ScriptableObject
{
    public GrowthType growthType;

    public List<MoveEvolutionPathEntry> evolutionPath;

    private Dictionary<MoveEvolution, MoveEvolution> _pathMap;

    private List<MoveEvolution> _orderedEvolutions;

    public void OnEnable()    
    {
        _pathMap = evolutionPath.ToDictionary(p => p.Previous, p => p.Next);

        _orderedEvolutions = new List<MoveEvolution>();
        if (evolutionPath.Count == 0) return;
        //_orderedEvolutions.Add(evolutionPath[0].Previous);
        foreach (var entry in evolutionPath)
            _orderedEvolutions.Add(entry.Next);
    }

    public bool TryGetNextEvolution(MoveEvolution current, out MoveEvolution next) => _pathMap.TryGetValue(current, out next);

    public bool IsLastEvolution(MoveEvolution evo) => !_pathMap.ContainsKey(evo);

    public MoveEvolution GetLastEvolution() => evolutionPath[evolutionPath.Count - 1].Next;

    public int GetEvolutionIndex(MoveEvolution evolution) => _orderedEvolutions.IndexOf(evolution);
}
