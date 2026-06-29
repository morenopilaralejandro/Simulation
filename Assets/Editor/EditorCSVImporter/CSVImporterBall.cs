using UnityEngine;
using UnityEditor;
using System.IO;

public class CSVImporterBall
{
    [MenuItem("Tools/Import CSV/Ball/Ball")]
    public static void ImportBallFromCSV()
    {
        string assetFolder = "Assets/Addressables/AddressBallData";
        string csvFolder = "Csv";
        string defaultPath = Path.Combine(Application.dataPath, csvFolder);
        string path = EditorUtility.OpenFilePanel("Select Ball CSV File", defaultPath, "csv");
        
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

        int ballIdIndex = System.Array.IndexOf(headers, "BallId");
        int ballTextureAddressIndex = System.Array.IndexOf(headers, "BallTextureAddress");

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] values = lines[i].Split(',');
            BallData ballData = ScriptableObject.CreateInstance<BallData>();

            ballData.BallId = values[ballIdIndex].Trim();
            ballData.BallTextureAddress = values[ballTextureAddressIndex].Trim();

            string safeName = ballData.BallId.Replace(" ", "_").Replace("/", "_");
            string assetPath = $"{assetFolder}/{safeName}.asset";
            AssetDatabase.CreateAsset(ballData, assetPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Ball ScriptableObjects created from CSV.");
    }
}
