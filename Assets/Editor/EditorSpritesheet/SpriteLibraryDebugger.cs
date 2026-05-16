using UnityEngine;
using UnityEditor;
using UnityEngine.U2D.Animation;

public class SpriteLibraryDebugger
{
    [MenuItem("Tools/Spritesheet/Debug SpriteLibraryAsset")]
    static void DebugAsset()
    {
        var asset = Selection.activeObject as SpriteLibraryAsset;

        if (asset == null)
        {
            Debug.LogError("Select a SpriteLibraryAsset.");
            return;
        }

        SerializedObject so = new SerializedObject(asset);

        var iterator = so.GetIterator();

        while (iterator.NextVisible(true))
        {
            Debug.Log(iterator.propertyPath);
        }
    }
}
