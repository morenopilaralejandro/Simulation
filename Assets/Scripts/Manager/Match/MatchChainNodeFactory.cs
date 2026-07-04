using Aremoreno.Enums.Match;

public static class MatchChainNodeFactory
{
    private static MatchChainNodeData auxData;

    public static MatchChainNode Create(MatchChainNodeData data, MatchChainNodeSaveData saveData = null)
    {
        switch (data)
        {
            case MatchChainNodeDataMatch matchData:
                return new MatchChainNodeMatch(matchData, saveData);

            case MatchChainNodeDataText textData:
                return new MatchChainNodeText(textData, saveData);

            case MatchChainNodeDataImage imageData:
                return new MatchChainNodeImage(imageData, saveData);

            case MatchChainNodeDataChest chestData:
                return new MatchChainNodeChest(chestData, saveData);

            default:
                LogManager.Warning($"[MatchChainNodeFactory] No runtime class for {data.GetType().Name}. Using base MatchChainNode.");
                return new MatchChainNode(data);
        }
    }

    public static MatchChainNode CreateById(string nodeId)
    {
        auxData = DatabaseManager.Instance.GetMatchChainNodeData(nodeId);
        return CreateByIdAndCategory(auxData.MatchChainNodeId, auxData.NodeCategory);
    }

    public static MatchChainNode CreateByIdAndCategory(string nodeId, MatchChainNodeCategory category, MatchChainNodeSaveData saveData = null)
    {
        auxData = category switch
        {
            MatchChainNodeCategory.Match => DatabaseManager.Instance.GetMatchChainNodeData<MatchChainNodeDataMatch>(nodeId),
            MatchChainNodeCategory.Text  => DatabaseManager.Instance.GetMatchChainNodeData<MatchChainNodeDataText>(nodeId),
            MatchChainNodeCategory.Image => DatabaseManager.Instance.GetMatchChainNodeData<MatchChainNodeDataImage>(nodeId),
            MatchChainNodeCategory.Chest => DatabaseManager.Instance.GetMatchChainNodeData<MatchChainNodeDataChest>(nodeId),
            _ => null
        };

        return Create(auxData, saveData);
    }
}
