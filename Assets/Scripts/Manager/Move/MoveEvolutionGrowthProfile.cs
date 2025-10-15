using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Simulation.Enums.Character;
using Simulation.Enums.Move;

[CreateAssetMenu(menuName = "ScriptableObject/MoveEvolutionGrowthProfile")]
public class MoveEvolutionGrowthProfile : ScriptableObject
{
    public GrowthType growthType;
    public GrowthRate growthRate;

    public List<MoveEvolutionBonusEntry> bonuses;
    public List<MoveEvolutionThresholdEntry> thresholds;

    private Dictionary<MoveEvolution, int> _bonusMap;
    private Dictionary<MoveEvolution, int> _thresholdMap;

    public void Initialize()
    {
        _bonusMap = bonuses.ToDictionary(b => b.Evolution, b => b.ExtraPower);
        _thresholdMap = thresholds.ToDictionary(t => t.Evolution, t => t.UsageThreshold);
    }

    public int GetBonus(MoveEvolution evo)
        => _bonusMap.TryGetValue(evo, out int v) ? v : 0;

    public int GetThreshold(MoveEvolution evo)
        => _thresholdMap.TryGetValue(evo, out int v) ? v : int.MaxValue;
}
