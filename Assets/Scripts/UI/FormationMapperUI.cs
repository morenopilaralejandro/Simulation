using UnityEngine;

public static class FormationMapperUI
{
    // Your world coordinate bounds
    private const float WORLD_X_MIN = -6f;  // A column
    private const float WORLD_X_MAX =  6f;  // K column
    private const float WORLD_Z_MIN = -7.5f;  // Row 9 (GK)
    private const float WORLD_Z_MAX = -1.5f;  // Row 1 (FW)

    /// <summary>
    /// Converts a FormationCoord's 3D world position to a normalized (0-1, 0-1) UI position.
    /// (0,0) = bottom-left of pitch UI,  (1,1) = top-right
    /// GK at bottom, FW at top.
    /// </summary>
    public static Vector2 WorldToNormalized(Vector3 worldPos)
    {
        float normalizedX = Mathf.InverseLerp(WORLD_X_MIN, WORLD_X_MAX, worldPos.x);
        // Z is negative and inverted: -7.5 (GK) → bottom (0), -1.5 (FW) → top (1)
        float normalizedY = Mathf.InverseLerp(WORLD_Z_MIN, WORLD_Z_MAX, worldPos.z);
        return new Vector2(normalizedX, normalizedY);
    }

    /// <summary>
    /// Maps a FormationCoord directly to an anchored position within a RectTransform.
    /// </summary>
    public static Vector2 WorldToUIPosition(Vector3 worldPos, RectTransform pitchRect, 
        float paddingX = 50f, float paddingY = 40f)
    {
        Vector2 normalized = WorldToNormalized(worldPos);

        float usableWidth  = pitchRect.rect.width  - paddingX * 2;
        float usableHeight = pitchRect.rect.height - paddingY * 2;

        float uiX = paddingX + normalized.x * usableWidth  - pitchRect.rect.width  * 0.5f;
        float uiY = paddingY + normalized.y * usableHeight - pitchRect.rect.height * 0.5f;

        return new Vector2(uiX, uiY);
    }

    /// <summary>
    /// For mini battle (4 players), the pitch is smaller.
    /// Coordinates still work — just different Z range.
    /// </summary>
    public static Vector2 WorldToUIPositionMini(Vector3 worldPos, RectTransform pitchRect)
    {
        // Mini uses F5(0,-4.5), F3(0,-3), C3/I3, C1/I1, F1
        // Z range: -4.5 to -1.5
        float normalizedX = Mathf.InverseLerp(-4.5f, 4.5f, worldPos.x);
        float normalizedY = Mathf.InverseLerp(-5.0f, -1.0f, worldPos.z);

        float padX = 50f, padY = 40f;
        float w = pitchRect.rect.width  - padX * 2;
        float h = pitchRect.rect.height - padY * 2;

        return new Vector2(
            padX + normalizedX * w - pitchRect.rect.width  * 0.5f,
            padY + normalizedY * h - pitchRect.rect.height * 0.5f
        );
    }
}
