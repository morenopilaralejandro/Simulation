using UnityEngine;
using UnityEditor;
using System.IO;
using Simulation.Enums.Quest;

public class CSVImporterQuest
{
    [MenuItem("Tools/Import CSV/Quest/Quest")]
    public static void ImportQuestFromCSV()
    {
        string assetFolder = "Assets/Addressables/AddressQuestData";
        string csvFolder = "Csv";
        string defaultPath = Path.Combine(Application.dataPath, csvFolder);
        string path = EditorUtility.OpenFilePanel("Select Quest CSV File", defaultPath, "csv");
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogWarning("No CSV file selected.");
            return;
        }

        AssetDatabaseManager.CreateFolderFromPath(assetFolder);

        string[] lines = File.ReadAllLines(path);
        if (lines.Length < 2)
        {
            Debug.LogWarning("CSV file does not contain enough lines.");
            return;
        }

        // Get CSV header index mapping
        string[] headers = lines[0].Split(',');

        int questIdIndex = System.Array.IndexOf(headers, "QuestId");
        int questTypeIndex = System.Array.IndexOf(headers, "QuestType");
        int recommendedLevelIndex = System.Array.IndexOf(headers, "RecommendedLevel");
        int objectiveIdsIndex = System.Array.IndexOf(headers, "ObjectiveIds");
        int storyPrerequisitesIndex = System.Array.IndexOf(headers, "StoryPrerequisites");
        int rewardExpIndex = System.Array.IndexOf(headers, "RewardExp");
        int rewardGoldIndex = System.Array.IndexOf(headers, "RewardGold");
        int itemRewardsIndex = System.Array.IndexOf(headers, "ItemRewards");
        int storyEffectsIndex = System.Array.IndexOf(headers, "StoryEffects");
        int followUpQuestIdsIndex = System.Array.IndexOf(headers, "FollowUpQuestIds");

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] values = lines[i].Split(',');
            QuestData questData = ScriptableObject.CreateInstance<QuestData>();

            questData.QuestId = values[questIdIndex].Trim();
            questData.QuestType = EnumManager.StringToEnum<QuestType>(values[questTypeIndex].Trim());
            questData.RecommendedLevel = int.Parse(values[recommendedLevelIndex].Trim());
            questData.ObjectiveIds = CSVImporterParser.ParseListString(values[objectiveIdsIndex].Trim());
            questData.StoryPrerequisites = CSVImporterParser.ParseListStoryPrerequisite(values[storyPrerequisitesIndex].Trim());
            questData.RewardExp = int.Parse(values[rewardExpIndex].Trim());
            questData.RewardGold = int.Parse(values[rewardGoldIndex].Trim());
            questData.ItemRewards = CSVImporterParser.ParseListItemReward(values[itemRewardsIndex].Trim());
            questData.StoryEffects = CSVImporterParser.ParseListStoryEffect(values[storyEffectsIndex].Trim());
            questData.FollowUpQuestIds = CSVImporterParser.ParseListString(values[followUpQuestIdsIndex].Trim());

            string safeName = questData.QuestId.Replace(" ", "_").Replace("/", "_");
            string assetPath = $"{assetFolder}/{safeName}.asset";
            AssetDatabase.CreateAsset(questData, assetPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Quest ScriptableObjects created from CSV.");
    }
}
