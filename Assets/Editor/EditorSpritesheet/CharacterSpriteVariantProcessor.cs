using UnityEngine;
using UnityEditor;
using UnityEngine.U2D.Animation;

using System.Collections.Generic;
using System.IO;
using System.Linq;

public class CharacterSpriteVariantProcessor : EditorWindow
{
    const string MarkerFile = "process-spritesheet-character";

    const int SliceSize = 64;

    SpriteLibraryAsset baseLibrary;

    [MenuItem("Tools/Spritesheet/Process Character Variant Spritesheets")]
    static void ShowWindow()
    {
        GetWindow<CharacterSpriteVariantProcessor>(
            "Character Variant Processor");
    }

    void OnGUI()
    {
        GUILayout.Space(10);

        EditorGUILayout.LabelField(
            "Character Variant Processor",
            EditorStyles.boldLabel);

        GUILayout.Space(10);

        baseLibrary = (SpriteLibraryAsset)
            EditorGUILayout.ObjectField(
                "Base Library",
                baseLibrary,
                typeof(SpriteLibraryAsset),
                false);

        GUILayout.Space(15);

        GUI.enabled = baseLibrary != null;

        if (GUILayout.Button(
            "Process Spritesheets",
            GUILayout.Height(40)))
        {
            ProcessAll();
        }

        GUI.enabled = true;
    }

    void ProcessAll()
    {
        string[] markerGuids =
            AssetDatabase.FindAssets(MarkerFile);

        int processedFolders = 0;

        foreach (string guid in markerGuids)
        {
            string markerPath =
                AssetDatabase.GUIDToAssetPath(guid);

            string rootFolder =
                Path.GetDirectoryName(markerPath)
                    .Replace("\\", "/");

            ProcessFolderRecursive(rootFolder);

            processedFolders++;
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log(
            $"Processed {processedFolders} marker folders.");
    }

    void ProcessFolderRecursive(string rootFolder)
    {
        string[] textureGuids =
            AssetDatabase.FindAssets(
                "t:Texture2D",
                new[] { rootFolder });

        Dictionary<string, SpriteLibraryAsset>
            loadedVariants = new();

        foreach (string guid in textureGuids)
        {
            string texturePath =
                AssetDatabase.GUIDToAssetPath(guid);

            string fileName =
                Path.GetFileNameWithoutExtension(
                    texturePath);

            if (!fileName.StartsWith(
                "sprite-character-"))
            {
                continue;
            }

            // Skip already processed
            if (HasSubSprites(texturePath))
            {
                continue;
            }

            Debug.Log($"Processing {fileName}");

            SliceTexture(texturePath);

            AssetDatabase.ImportAsset(
                texturePath,
                ImportAssetOptions.ForceUpdate);

            string[] parts =
                fileName.Split('-');

            if (parts.Length < 6)
            {
                Debug.LogWarning(
                    $"Invalid naming format:\n{fileName}");

                continue;
            }





            string categoryRaw;
            string variantName;
            string animation;

            // STANDARD
            // sprite-character-body-teen-amber-1h_backslash
            //
            // KIT
            // sprite-character-kit-crimson-away-field-1h_backslash

            if (parts[2] == "kit")
            {
                categoryRaw = "kit";

                string teamName = parts[3];

                string kitVariant = parts[4];

                string kitRole = parts[5];

                variantName =
                    $"{teamName}-{kitVariant}-{kitRole}";

                animation =
                    string.Join("-",
                        parts.Skip(6));
            }
            else
            {
                categoryRaw = parts[2];

                variantName = parts[4];

                animation =
                    string.Join("-",
                        parts.Skip(5));
            }






            string category =
                FirstUpper(categoryRaw);

            string variantKey =
                $"{categoryRaw}-{variantName}";

            if (!loadedVariants.TryGetValue(
                variantKey,
                out SpriteLibraryAsset variantLibrary))
            {
                variantLibrary =
                    GetOrCreateVariantLibrary(
                        rootFolder,
                        categoryRaw,
                        variantName);

                loadedVariants.Add(
                    variantKey,
                    variantLibrary);
            }

            AddSpritesToVariant(
                variantLibrary,
                texturePath,
                category,
                animation);
        }
    }

    bool HasSubSprites(string texturePath)
    {
        Object[] assets =
            AssetDatabase.LoadAllAssetsAtPath(
                texturePath);

        return assets.Count(x => x is Sprite) > 1;
    }

    void SliceTexture(string texturePath)
    {
        TextureImporter importer =
            AssetImporter.GetAtPath(texturePath)
            as TextureImporter;

        if (importer == null)
            return;

        importer.textureType =
            TextureImporterType.Sprite;

        importer.spriteImportMode =
            SpriteImportMode.Multiple;

        importer.filterMode =
            FilterMode.Point;

        importer.mipmapEnabled = false;

        importer.spritePixelsPerUnit = 64;

        Texture2D tex =
            AssetDatabase.LoadAssetAtPath<Texture2D>(
                texturePath);

        int columns = tex.width / SliceSize;

        int rows = tex.height / SliceSize;

        List<SpriteMetaData> metas =
            new();

        string baseName =
            Path.GetFileNameWithoutExtension(
                texturePath);

        int index = 0;

        for (int y = rows - 1; y >= 0; y--)
        {
            for (int x = 0; x < columns; x++)
            {
                SpriteMetaData meta =
                    new SpriteMetaData();

                meta.name =
                    $"{baseName}_{index}";

                meta.rect =
                    new Rect(
                        x * SliceSize,
                        y * SliceSize,
                        SliceSize,
                        SliceSize);

                meta.alignment =
                    (int)SpriteAlignment.Center;

                metas.Add(meta);

                index++;
            }
        }

        importer.spritesheet =
            metas.ToArray();

        EditorUtility.SetDirty(importer);

        importer.SaveAndReimport();
    }

    SpriteLibraryAsset GetOrCreateVariantLibrary(
        string folder,
        string category,
        string variant)
    {
        string assetName =
            $"sprite-library-character-{category}-{variant}.asset";

        string assetPath =
            Path.Combine(folder, assetName)
                .Replace("\\", "/");

        SpriteLibraryAsset existing =
            AssetDatabase.LoadAssetAtPath<SpriteLibraryAsset>(
                assetPath);

        if (existing != null)
        {
            return existing;
        }

        // Create new sprite library asset
        SpriteLibraryAsset variantLibrary =
            ScriptableObject.CreateInstance<SpriteLibraryAsset>();

        AssetDatabase.CreateAsset(
            variantLibrary,
            assetPath);

        // Copy all categories/labels from base library
        if (baseLibrary != null)
        {
            foreach (var categoryAux in baseLibrary.GetCategoryNames())
            {
                foreach (var label in baseLibrary.GetCategoryLabelNames(categoryAux))
                {
                    Sprite sprite =
                        baseLibrary.GetSprite(category, label);

                    if (sprite != null)
                    {
                        variantLibrary.AddCategoryLabel(
                            sprite,
                            categoryAux,
                            label);
                    }
                }
            }
        }

        EditorUtility.SetDirty(variantLibrary);

        return variantLibrary;
    }

    void AddSpritesToVariant(
        SpriteLibraryAsset variantLibrary,
        string texturePath,
        string category,
        string animation)
    {
        Object[] assets =
            AssetDatabase.LoadAllAssetsAtPath(
                texturePath);

        List<Sprite> sprites =
            assets
                .OfType<Sprite>()
                .OrderBy(x => x.name)
                .ToList();

        if (sprites.Count == 0)
        {
            Debug.LogWarning(
                $"No sprites found:\n{texturePath}");

            return;
        }

        Texture2D tex =
            AssetDatabase.LoadAssetAtPath<Texture2D>(
                texturePath);

        int rows =
            tex.height / SliceSize;

        int columns =
            tex.width / SliceSize;

        bool directional =
            rows == 4;

        string[] directions =
        {
            "up",
            "left",
            "down",
            "right"
        };

        int spriteIndex = 0;

        for (int row = 0; row < rows; row++)
        {
            string dir =
                directional
                    ? directions[row]
                    : "down";

            for (int col = 0; col < columns; col++)
            {
                if (spriteIndex >= sprites.Count)
                    break;

                Sprite sprite =
                    sprites[spriteIndex];

                string label =
                    $"{animation}_{dir}_{col}";

                variantLibrary.AddCategoryLabel(
                    sprite,
                    category,
                    label);

                spriteIndex++;
            }
        }

        EditorUtility.SetDirty(
            variantLibrary);
    }

    static string FirstUpper(string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        return char.ToUpper(value[0]) +
               value.Substring(1);
    }
}
