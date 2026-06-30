using Aremoreno.Enums.Match;

public static class MatchChainNodeFactory
{
    private static MatchChainNodeData auxData;

    public static MatchChainNode Create(MatchChainNodeData data)
    {
        switch (data)
        {
            case MatchChainNodeDataMatch matchData:
                return new MatchChainNodeMatch(matchData);

            case MatchChainNodeDataText textData:
                return new MatchChainNodeText(textData);

            case MatchChainNodeDataImage imageData:
                return new MatchChainNodeImage(imageData);

            case MatchChainNodeDataChest chestData:
                return new MatchChainNodeChest(chestData);

            default:
                LogManager.Warning($"[MatchChainNodeFactory] No runtime class for {data.GetType().Name}. Using base MatchChainNode.");
                return new MatchChainNode(data);
        }
    }

    /*
    public static MatchChainNode Create(MatchChainNodeSaveData saveData)
    {
        return new MatchChainNode(null, saveData);
    }
    */

    public static MatchChainNode CreateById(string nodeId)
    {
        auxData = DatabaseManager.Instance.GetMatchChainNodeData(nodeId);
        return Create(auxData);
    }

    public static MatchChainNode CreateByIdAndCategory(string nodeId, MatchChainNodeCategory category)
    {
        auxData = category switch
        {
            MatchChainNodeType.Match => DatabaseManager.Instance.GetMatchChainNodeData<MatchChainNodeDataMatch>(nodeId),
            MatchChainNodeType.Text  => DatabaseManager.Instance.GetMatchChainNodeData<MatchChainNodeDataText>(nodeId),
            MatchChainNodeType.Image => DatabaseManager.Instance.GetMatchChainNodeData<MatchChainNodeDataImage>(nodeId),
            MatchChainNodeType.Chest => DatabaseManager.Instance.GetMatchChainNodeData<MatchChainNodeDataChest>(nodeId),
            _ => null
        };

        return Create(auxData);
    }
}
