using System.IO;
using UnityEditor;
using UnityEngine;

public class TileAtlasPaddingGenerator
{
    private const int Padding = 2;

    [MenuItem("Tools/Spritesheet/Generate Padded Tile Atlas")]
    private static void Generate()
    {
        Texture2D source = Selection.activeObject as Texture2D;

        if (source == null)
        {
            EditorUtility.DisplayDialog("Error", "Select a Texture2D in the Project window.", "OK");
            return;
        }

        string path = AssetDatabase.GetAssetPath(source);
        var importer = (TextureImporter)AssetImporter.GetAtPath(path);

        if (!importer.isReadable)
        {
            importer.isReadable = true;
            importer.SaveAndReimport();
        }

        int tileWidth = EditorUtility.DisplayDialogComplex(
            "Tile Size",
            "Use 32x32 tiles?\n\nYes = 32\nNo = 16\nCancel = Custom (edit script)",
            "32",
            "16",
            "Cancel") == 0 ? 32 : 16;

        int cols = source.width / tileWidth;
        int rows = source.height / tileWidth;

        int paddedTile = tileWidth + Padding * 2;

        Texture2D output = new Texture2D(
            cols * paddedTile,
            rows * paddedTile,
            TextureFormat.RGBA32,
            false);

        output.filterMode = FilterMode.Point;

        Color32[] pixels = source.GetPixels32();

        Color32 GetPixel(int x, int y)
        {
            return pixels[y * source.width + x];
        }

        for (int ty = 0; ty < rows; ty++)
        {
            for (int tx = 0; tx < cols; tx++)
            {
                int srcX = tx * tileWidth;
                int srcY = ty * tileWidth;

                int dstX = tx * paddedTile + Padding;
                int dstY = ty * paddedTile + Padding;

                // Copy tile
                for (int y = 0; y < tileWidth; y++)
                {
                    for (int x = 0; x < tileWidth; x++)
                    {
                        output.SetPixel(
                            dstX + x,
                            dstY + y,
                            GetPixel(srcX + x, srcY + y));
                    }
                }

                // Extrude left/right
                for (int y = 0; y < tileWidth; y++)
                {
                    Color left = GetPixel(srcX, srcY + y);
                    Color right = GetPixel(srcX + tileWidth - 1, srcY + y);

                    for (int p = 1; p <= Padding; p++)
                    {
                        output.SetPixel(dstX - p, dstY + y, left);
                        output.SetPixel(dstX + tileWidth - 1 + p, dstY + y, right);
                    }
                }

                // Extrude top/bottom
                for (int x = 0; x < tileWidth; x++)
                {
                    Color bottom = GetPixel(srcX + x, srcY);
                    Color top = GetPixel(srcX + x, srcY + tileWidth - 1);

                    for (int p = 1; p <= Padding; p++)
                    {
                        output.SetPixel(dstX + x, dstY - p, bottom);
                        output.SetPixel(dstX + x, dstY + tileWidth - 1 + p, top);
                    }
                }

                // Corners
                Color bl = GetPixel(srcX, srcY);
                Color br = GetPixel(srcX + tileWidth - 1, srcY);
                Color tl = GetPixel(srcX, srcY + tileWidth - 1);
                Color tr = GetPixel(srcX + tileWidth - 1, srcY + tileWidth - 1);

                for (int py = 1; py <= Padding; py++)
                {
                    for (int px = 1; px <= Padding; px++)
                    {
                        output.SetPixel(dstX - px, dstY - py, bl);
                        output.SetPixel(dstX + tileWidth - 1 + px, dstY - py, br);
                        output.SetPixel(dstX - px, dstY + tileWidth - 1 + py, tl);
                        output.SetPixel(dstX + tileWidth - 1 + px, dstY + tileWidth - 1 + py, tr);
                    }
                }
            }
        }

        output.Apply();

        string outputPath = Path.Combine(
            Path.GetDirectoryName(path),
            Path.GetFileNameWithoutExtension(path) + "_Padded.png");

        File.WriteAllBytes(outputPath, output.EncodeToPNG());

        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog(
            "Done",
            "Saved:\n" + outputPath,
            "OK");
    }
}
