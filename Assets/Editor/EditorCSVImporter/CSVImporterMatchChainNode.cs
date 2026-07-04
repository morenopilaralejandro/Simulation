using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using Aremoreno.Enums.Match;

public class CSVImporterMatchChainNode
{
    [MenuItem("Tools/Import CSV/Match/Match Chain Nodes")]
    public static void ImportMatchChainNodesFromCSV()
    {
        string nodeAssetFolder = "Assets/Addressables/AddressMatchChainNodeData";
        string chainAssetFolder = "Assets/Addressables/AddressMatchChainData";
        string csvFolder = "Csv";

        string defaultPath = Path.Combine(Application.dataPath, csvFolder);
        string path = EditorUtility.OpenFilePanel("Select Match Chain Node CSV File", defaultPath, "csv");

        if (string.IsNullOrEmpty(path))
        {
            Debug.LogWarning("[CSVImporterMatchChainNode] No CSV file selected.");
            return;
        }

        AssetDatabaseManager.CreateFolderFromPath(nodeAssetFolder);

        string[] lines = File.ReadAllLines(path);
        if (lines.Length < 2)
        {
            Debug.LogWarning("[CSVImporterMatchChainNode] CSV file does not contain enough lines.");
            return;
        }

        string[] headers = lines[0].Split(',');

        // Base
        int matchChainNodeIdIndex = System.Array.IndexOf(headers, "MatchChainNodeId");
        int matchChainIdIndex = System.Array.IndexOf(headers, "MatchChainId");
        int nodeIndexIndex = System.Array.IndexOf(headers, "NodeIndex");
        int nodeCategoryIndex = System.Array.IndexOf(headers, "NodeCategory");

        // Chest
        int itemIdIndex = System.Array.IndexOf(headers, "ItemId");

        // Image
        int imageAddressIndex = System.Array.IndexOf(headers, "ImageAddress");

        // Match
        int matchIdIndex = System.Array.IndexOf(headers, "MatchId");
        int dropIdBIndex = System.Array.IndexOf(headers, "DropIdB");
        int dropIdAIndex = System.Array.IndexOf(headers, "DropIdA");
        int dropIdSIndex = System.Array.IndexOf(headers, "DropIdS");

        // Text
        int textLocalizationKeyIndex = System.Array.IndexOf(headers, "TextLocalizationKey");

        Dictionary<string, List<MatchChainNodeData>> nodesByChain = new();

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i]))
                continue;

            string[] values = lines[i].Split(',');

            string matchChainId = values[matchChainIdIndex].Trim();
            string matchChainNodeId = values[matchChainNodeIdIndex].Trim();

            MatchChainNodeCategory category = EnumManager.StringToEnum<MatchChainNodeCategory>(values[nodeCategoryIndex].Trim());
            MatchChainNodeData nodeData = MatchChainNodeDataFactory.CreateByCategory(category);

            nodeData.MatchChainNodeId = matchChainNodeId;
            nodeData.MatchChainId = matchChainId;
            nodeData.NodeIndex = int.Parse(values[nodeIndexIndex].Trim());
            nodeData.NodeCategory = category;

            switch (nodeData)
            {
                case MatchChainNodeDataChest chest:
                    chest.ItemId = values[itemIdIndex].Trim();
                    break;

                case MatchChainNodeDataImage image:
                    image.ImageAddress = values[imageAddressIndex].Trim();
                    break;

                case MatchChainNodeDataMatch match:
                    match.MatchId = values[matchIdIndex].Trim();
                    match.DropIdB = values[dropIdBIndex].Trim();
                    match.DropIdA = values[dropIdAIndex].Trim();
                    match.DropIdS = values[dropIdSIndex].Trim();
                    break;

                case MatchChainNodeDataText text:
                    text.TextLocalizationKey = values[textLocalizationKeyIndex].Trim();
                    break;
            }

            string safeName = $"{matchChainNodeId.Replace(" ", "_").Replace("/", "_")}";
            string assetPath = $"{nodeAssetFolder}/{safeName}.asset";

            AssetDatabase.CreateAsset(nodeData, assetPath);

            if (!nodesByChain.TryGetValue(matchChainId, out var list))
            {
                list = new List<MatchChainNodeData>();
                nodesByChain.Add(matchChainId, list);
            }

            list.Add(nodeData);
        }

        // Sort nodes by index
        foreach (var pair in nodesByChain)
        {
            pair.Value.Sort((a, b) => a.NodeIndex.CompareTo(b.NodeIndex));
        }

        // Find MatchChainData assets
        string[] guids = AssetDatabase.FindAssets("t:MatchChainData", new[] { chainAssetFolder });

        int updatedCount = 0;
        HashSet<string> matchedChains = new();

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            MatchChainData chain = AssetDatabase.LoadAssetAtPath<MatchChainData>(assetPath);

            if (chain == null)
                continue;

            if (nodesByChain.TryGetValue(chain.MatchChainId, out List<MatchChainNodeData> nodes))
            {
                chain.NodeDataList = new List<MatchChainNodeData>(nodes);

                EditorUtility.SetDirty(chain);
                matchedChains.Add(chain.MatchChainId);
                updatedCount++;

                Debug.Log($"[CSVImporterMatchChainNode] Updated MatchChain '{chain.MatchChainId}' with {nodes.Count} node(s).");
            }
        }

        foreach (string chainId in nodesByChain.Keys)
        {
            if (!matchedChains.Contains(chainId))
            {
                Debug.LogWarning($"[CSVImporterMatchChainNode] No MatchChainData asset found for MatchChainId '{chainId}'.");
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"[CSVImporterMatchChainNode] Import complete. Updated {updatedCount} MatchChainData asset(s).");
    }
}
