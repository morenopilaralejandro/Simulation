#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class WorldMapVisualizer : EditorWindow
{
    private OverworldDefinition _overworld;
    private ZoneDefinition _filterZone;
    private bool _filterByZone;
    private float _cellSize = 40f;
    private Vector2 _scrollPos;

    [MenuItem("Tools/World/World Map Visualizer")]
    static void Open()
    {
        GetWindow<WorldMapVisualizer>("World Map");
    }

    private void OnGUI()
    {
        _overworld = (OverworldDefinition)EditorGUILayout.ObjectField(
            "Overworld", _overworld, typeof(OverworldDefinition), false
        );

        _filterByZone = EditorGUILayout.Toggle("Filter by Zone", _filterByZone);
        if (_filterByZone)
        {
            _filterZone = (ZoneDefinition)EditorGUILayout.ObjectField(
                "Zone Filter", _filterZone, typeof(ZoneDefinition), false
            );

            if (_overworld != null && _filterZone != null
                && !_overworld.allZones.Contains(_filterZone))
            {
                EditorGUILayout.HelpBox(
                    "Selected zone is not part of this overworld!",
                    MessageType.Warning
                );
            }
        }

        _cellSize = EditorGUILayout.Slider("Zoom", _cellSize, 20f, 100f);

        if (_overworld == null || _overworld.allChunks.Count == 0) return;

        // Determine which chunks to visualize
        List<ChunkDefinition> chunks;
        if (_filterByZone && _filterZone != null)
        {
            chunks = _overworld.allChunks
                .Where(c => c.parentZone == _filterZone)
                .ToList();
        }
        else
        {
            chunks = _overworld.allChunks;
        }

        if (chunks.Count == 0)
        {
            EditorGUILayout.HelpBox(
                "No chunks found for the selected filter.",
                MessageType.Info
            );
            return;
        }

        // Find bounds of all chunks
        int minX = int.MaxValue, maxX = int.MinValue;
        int minY = int.MaxValue, maxY = int.MinValue;

        foreach (var chunk in chunks)
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

                Rect cellRect = new Rect(
                    drawX, drawY, _cellSize - 2, _cellSize - 2
                );

                var coord = new Vector2Int(x, y);

                // Always look up in the full overworld for drawing context
                var chunk = _overworld.allChunks.Find(
                    c => c.chunkCoord == coord
                );

                // Check if the chunk belongs to the filtered zone
                bool inFilterSet = !_filterByZone
                    || _filterZone == null
                    || (chunk != null && chunk.parentZone == _filterZone);

                if (chunk != null)
                {
                    Color color;

                    if (x == 0 && y == 0)
                    {
                        // Green = origin
                        color = new Color(0.2f, 0.8f, 0.2f);
                    }
                    else if (_filterByZone && !inFilterSet)
                    {
                        // Dimmed = belongs to another zone
                        color = new Color(0.25f, 0.25f, 0.35f);
                    }
                    else
                    {
                        // Blue = normal / in-filter chunk
                        color = new Color(0.3f, 0.5f, 0.8f);
                    }

                    EditorGUI.DrawRect(cellRect, color);
                    GUI.Label(cellRect,
                        $"({x},{y})\n{chunk.chunkId}",
                        EditorStyles.miniLabel
                    );
                }
                else
                {
                    // Empty slot
                    EditorGUI.DrawRect(
                        cellRect, new Color(0.15f, 0.15f, 0.15f)
                    );

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
