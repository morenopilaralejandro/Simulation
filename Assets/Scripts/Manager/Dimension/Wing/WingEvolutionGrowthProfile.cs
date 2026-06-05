using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Wing;

[CreateAssetMenu(menuName = "ScriptableObject/Dimension/Wing/WingEvolutionGrowthProfile")]
public class WingEvolutionGrowthProfile : ScriptableObject
{
    public WingGrowthType growthType;
    public WingGrowthRate growthRate;

    public List<WingEvolutionUsageThresholdEntry> listUsageThreshold;
    public List<WingRefinementEntry> listRefinementThreshold;

    private Dictionary<WingEvolution, int> _usageThresholdMap;
    private Dictionary<WingRefinementRank, int> _refinementThresholdMap;

    public void Initialize()
    {
        _usageThresholdMap = listUsageThreshold.ToDictionary(t => t.Evolution, t => t.UsageThreshold);
        _refinementThresholdMap = listRefinementThreshold.ToDictionary(t => t.Rank, t => t.RefinementThreshold);
    }

    public int GetUsageThreshold(WingEvolution evo)
        => _usageThresholdMap.TryGetValue(evo, out int v) ? v : int.MaxValue;

    public int GetRefinementThreshold(WingRefinementRank rank)
        => _refinementThresholdMap.TryGetValue(rank, out int r) ? r : int.MaxValue;
}
