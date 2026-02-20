#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class WorldMapVisualizer : EditorWindow
{
    private ZoneDefinition _zone;
    private float _cellSize = 40f;
    private Vector2 _scrollPos;

    [MenuItem("Tools/World/World Map Visualizer")]
    static void Open()
    {
        GetWindow<WorldMapVisualizer>("World Map");
    }

    private void OnGUI()
    {
        _zone = (ZoneDefinition)EditorGUILayout.ObjectField(
            "Zone", _zone, typeof(ZoneDefinition), false
        );
        _cellSize = EditorGUILayout.Slider("Zoom", _cellSize, 20f, 100f);

        if (_zone == null || _zone.chunks.Count == 0) return;

        // Find bounds of all chunks
        int minX = int.MaxValue, maxX = int.MinValue;
        int minY = int.MaxValue, maxY = int.MinValue;

        foreach (var chunk in _zone.chunks)
        {
            minX = Mathf.Min(minX, chunk.chunkCoord.x);
            maxX = Mathf.Max(maxX, chunk.chunkCoord.x);
            minY = Mathf.Min(minY, chunk.chunkCoord.y);
            maxY = Mathf.Max(maxY, chunk.chunkCoord.y);
        }

        // Add padding
        minX -= 2; maxX += 2;
        minY -= 2; maxY += 2;

        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

        float totalWidth = (maxX - minX + 1) * _cellSize;
        float totalHeight = (maxY - minY + 1) * _cellSize;

        GUILayout.Space(totalHeight + 50);

        Rect area = GUILayoutUtility.GetLastRect();

        // Draw grid
        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                float drawX = (x - minX) * _cellSize + area.x;
                // Flip Y so positive is up
                float drawY = (maxY - y) * _cellSize + area.y;

                Rect cellRect = new Rect(drawX, drawY, _cellSize - 2, _cellSize - 2);

                var chunk = _zone.chunks.Find(
                    c => c.chunkCoord == new Vector2Int(x, y)
                );

                if (chunk != null)
                {
                    // Existing chunk
                    Color color = (x == 0 && y == 0)
                        ? new Color(0.2f, 0.8f, 0.2f)  // green = origin
                        : new Color(0.3f, 0.5f, 0.8f);  // blue = normal

                    EditorGUI.DrawRect(cellRect, color);
                    GUI.Label(cellRect,
                        $"({x},{y})\n{chunk.chunkId}",
                        EditorStyles.miniLabel
                    );
                }
                else
                {
                    // Empty slot
                    EditorGUI.DrawRect(cellRect,new Color(0.15f,0.15f,0.15f));
                    
                    if (GUI.Button(cellRect, $"+\n({x},{y})",
                        EditorStyles.miniButton))
                    {
                        // Quick-create chunk at this coord
                        Debug.Log($"Create chunk at ({x},{y})");
                    }
                }
            }
        }

        EditorGUILayout.EndScrollView();
    }
}
#endif
