using UnityEngine;
using Aremoreno.Enums.Match;

public static class MatchChainNodeDataFactory
{
    public static MatchChainNodeData CreateByCategory(MatchChainNodeCategory category)
    {
        switch (category)
        {
            case MatchChainNodeCategory.Match:
                return ScriptableObject.CreateInstance<MatchChainNodeDataMatch>();

            case MatchChainNodeCategory.Text:
                return ScriptableObject.CreateInstance<MatchChainNodeDataText>();

            case MatchChainNodeCategory.Image:
                return ScriptableObject.CreateInstance<MatchChainNodeDataImage>();

            case MatchChainNodeCategory.Chest:
                return ScriptableObject.CreateInstance<MatchChainNodeDataChest>();

            default:
                LogManager.Error($"[MatchChainNodeDataFactory] No data class for {category.ToString()}.");
                return null;
        }
    }
}
