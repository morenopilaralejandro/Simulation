using UnityEngine;
using UnityEditor;

public static class AssetDatabaseManager 
{
    public static void CreateFolder(string parent, string folderName)
    {
        string path = $"{parent}/{folderName}";
        if (!AssetDatabase.IsValidFolder(path))
        {
            AssetDatabase.CreateFolder(parent, folderName);
        }
    }

    public static void CreateFolderFromPath(string folderPath)
    {
        string[] parts = folderPath.Split('/');
        string currentPath = parts[0]; // should start with "Assets"

        for (int i = 1; i < parts.Length; i++)
        {
            string nextPath = $"{currentPath}/{parts[i]}";
            if (!AssetDatabase.IsValidFolder(nextPath))
            {
                AssetDatabase.CreateFolder(currentPath, parts[i]);
            }
            currentPath = nextPath;
        }
    }
}
