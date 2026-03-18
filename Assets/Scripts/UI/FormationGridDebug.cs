using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class FormationGridDebug : MonoBehaviour
{
    [SerializeField] private RectTransform pitchArea;
    [SerializeField] private Formation currentFormation;
    [SerializeField] private bool showGrid = true;
    [SerializeField] private bool showCoordLabels = true;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!showGrid || pitchArea == null) return;

        // Draw all possible grid positions
        string[] cols = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K" };
        float[] xVals = { -4.5f, -3.75f, -3f, -2.25f, -1.5f, 0f, 1.5f, 2.25f, 3f, 3.75f, 4.5f };
        float[] zVals = { -1.5f, -2.25f, -3f, -3.75f, -4.5f, -5.25f, -6f, -6.75f };

        Gizmos.color = new Color(1, 1, 1, 0.2f);
        for (int c = 0; c < cols.Length; c++)
        {
            for (int r = 0; r < zVals.Length; r++)
            {
                Vector3 worldPos = new Vector3(xVals[c], 0.34f, zVals[r]);
                Vector2 uiPos = FormationMapperUI.WorldToUIPosition(worldPos, pitchArea);
                Vector3 screenPos = pitchArea.TransformPoint(uiPos);

                Gizmos.DrawWireSphere(screenPos, 5f);

                if (showCoordLabels)
                {
                    Handles.Label(screenPos, $"{cols[c]}{r + 1}");
                }
            }
        }

        // Highlight current formation positions
        if (currentFormation != null)
        {
            Gizmos.color = Color.yellow;
            foreach (var coord in currentFormation.FormationCoords)
            {
                Vector2 uiPos = FormationMapperUI.WorldToUIPosition(
                    coord.DefaultPosition, pitchArea);
                Vector3 screenPos = pitchArea.TransformPoint(uiPos);
                Gizmos.DrawSphere(screenPos, 8f);
            }
        }
    }
#endif
}
