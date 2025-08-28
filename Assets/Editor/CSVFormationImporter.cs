using UnityEngine;
using UnityEditor;
using System.IO;
using Simulation.Enums.Character;

public class CSVFormationImporter
{
    [MenuItem("Tools/Import CSV/Formation")]
    public static void ImportFormationsFromCSV()
    {
        string assetFolder = "Assets/Resources/ScriptableObjects/Formation";
        string csvFolder = "Csv";
        string defaultPath = Path.Combine(Application.dataPath, csvFolder);
        string path = EditorUtility.OpenFilePanel("Select Formation CSV File", defaultPath, "csv");
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

        int formationIdIndex    = System.Array.IndexOf(headers, "FormationId");
        int c0Index             = System.Array.IndexOf(headers, "C0");
        int p0Index             = System.Array.IndexOf(headers, "P0");
        int c1Index             = System.Array.IndexOf(headers, "C1");
        int p1Index             = System.Array.IndexOf(headers, "P1");
        int c2Index             = System.Array.IndexOf(headers, "C2");
        int p2Index             = System.Array.IndexOf(headers, "P2");
        int c3Index             = System.Array.IndexOf(headers, "C3");
        int p3Index             = System.Array.IndexOf(headers, "P3");
        int c4Index             = System.Array.IndexOf(headers, "C4");
        int p4Index             = System.Array.IndexOf(headers, "P4");
        int c5Index             = System.Array.IndexOf(headers, "C5");
        int p5Index             = System.Array.IndexOf(headers, "P5");
        int c6Index             = System.Array.IndexOf(headers, "C6");
        int p6Index             = System.Array.IndexOf(headers, "P6");
        int c7Index             = System.Array.IndexOf(headers, "C7");
        int p7Index             = System.Array.IndexOf(headers, "P7");
        int c8Index             = System.Array.IndexOf(headers, "C8");
        int p8Index             = System.Array.IndexOf(headers, "P8");
        int c9Index             = System.Array.IndexOf(headers, "C9");
        int p9Index             = System.Array.IndexOf(headers, "P9");
        int c10Index            = System.Array.IndexOf(headers, "C10");
        int p10Index            = System.Array.IndexOf(headers, "P10");
        int Kickoff0Index       = System.Array.IndexOf(headers, "Kickoff0");
        int Kickoff1Index       = System.Array.IndexOf(headers, "Kickoff1");

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] values = lines[i].Split(',');
            FormationData formationData = ScriptableObject.CreateInstance<FormationData>();

            formationData.FormationId   = values[formationIdIndex].Trim();
            formationData.CoordIds = new string[11]
            {
                values[c0Index].Trim(),
                values[c1Index].Trim(),
                values[c2Index].Trim(),
                values[c3Index].Trim(),
                values[c4Index].Trim(),
                values[c5Index].Trim(),
                values[c6Index].Trim(),
                values[c7Index].Trim(),
                values[c8Index].Trim(),
                values[c9Index].Trim(),
                values[c10Index].Trim()
            };
            formationData.Positions = new Position[11]
            {
                EnumManager.StringToEnum<Position>(values[p0Index].Trim()),
                EnumManager.StringToEnum<Position>(values[p1Index].Trim()),
                EnumManager.StringToEnum<Position>(values[p2Index].Trim()),
                EnumManager.StringToEnum<Position>(values[p3Index].Trim()),
                EnumManager.StringToEnum<Position>(values[p4Index].Trim()),
                EnumManager.StringToEnum<Position>(values[p5Index].Trim()),
                EnumManager.StringToEnum<Position>(values[p6Index].Trim()),
                EnumManager.StringToEnum<Position>(values[p7Index].Trim()),
                EnumManager.StringToEnum<Position>(values[p8Index].Trim()),
                EnumManager.StringToEnum<Position>(values[p9Index].Trim()),
                EnumManager.StringToEnum<Position>(values[p10Index].Trim())
            };
            formationData.Kickoff0 = int.Parse(values[Kickoff0Index].Trim());
            formationData.Kickoff1 = int.Parse(values[Kickoff1Index].Trim());

            string safeName = formationData.FormationId.Replace(" ", "_").Replace("/", "_");
            string assetPath = $"{assetFolder}/{safeName}.asset";
            AssetDatabase.CreateAsset(formationData, assetPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Formation ScriptableObjects created from CSV.");
    }
}
