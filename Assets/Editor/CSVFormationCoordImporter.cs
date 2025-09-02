using UnityEngine;
using UnityEditor;
using System.IO;

public class CSVFormationCoordImporter
{
    [MenuItem("Tools/Import CSV/FormationCord")]
    public static void ImportFormationCoordsFromCSV()
    {
        string assetFolder = "Assets/ScriptableObjects/FormationCord";
        string csvFolder = "Csv";
        string defaultPath = Path.Combine(Application.dataPath, csvFolder);
        string path = EditorUtility.OpenFilePanel("Select FormationCord CSV File", defaultPath, "csv");
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

        int formationCoordIdIndex   = System.Array.IndexOf(headers, "FormationCoordId");
        int xIndex                  = System.Array.IndexOf(headers, "X");
        int yIndex                  = System.Array.IndexOf(headers, "Y");
        int zIndex                  = System.Array.IndexOf(headers, "Z");

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] values = lines[i].Split(',');
            FormationCoordData formationCoordData = ScriptableObject.CreateInstance<FormationCoordData>();

            formationCoordData.FormationCoordId     = values[formationCoordIdIndex].Trim();
            formationCoordData.X                    = float.Parse(values[xIndex]);
            formationCoordData.Y                    = float.Parse(values[yIndex]);
            formationCoordData.Z                    = float.Parse(values[zIndex]);

            string safeName = formationCoordData.FormationCoordId.Replace(" ", "_").Replace("/", "_");
            string assetPath = $"{assetFolder}/{safeName}.asset";
            AssetDatabase.CreateAsset(formationCoordData, assetPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("FormationCord ScriptableObjects created from CSV.");
    }
}
