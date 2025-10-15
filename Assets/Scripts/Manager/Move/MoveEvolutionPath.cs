using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Simulation.Enums.Character;
using Simulation.Enums.Move;

[CreateAssetMenu(menuName = "ScriptableObject/MoveEvolutionPath")]
public class MoveEvolutionPath : ScriptableObject
{
    public GrowthType growthType;

    public List<MoveEvolutionPathEntry> evolutionPath;

    private Dictionary<MoveEvolution, MoveEvolution> _pathMap;

    public void Initialize()
    {
        _pathMap = evolutionPath.ToDictionary(p => p.Previous, p => p.Next);
    }

    public bool TryGetNextEvolution(MoveEvolution current, out MoveEvolution next) => _pathMap.TryGetValue(current, out next);

    public bool IsLastEvolution(MoveEvolution evo) => !_pathMap.ContainsKey(evo);

    public MoveEvolution GetLastEvolution() => evolutionPath[evolutionPath.Count - 1].Next;
}
