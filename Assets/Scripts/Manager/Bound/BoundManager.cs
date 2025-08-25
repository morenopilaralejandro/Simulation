using UnityEngine;

public static class BoundManager
{
    public static BoxCollider Top { get; set; }
    public static BoxCollider Bottom { get; set; }
    public static BoxCollider Left { get; set; }
    public static BoxCollider Right { get; set; }

    public static float TopOffset { get; set; }
    public static float BottomOffset { get; set; }
    public static float LeftOffset { get; set; }
    public static float RightOffset { get; set; }
    public static float TouchAreaOffset { get; set; }

    public static float MinX { get; private set; }
    public static float MaxX { get; private set; }
    public static float MinZ { get; private set; }
    public static float MaxZ { get; private set; }

    /// <summary>
    /// Calculate bounds based on current colliders and offsets.
    /// Call this after assigning colliders and offsets.
    /// </summary>
    public static void Setup()
    {
        if (Top == null || Bottom == null || Left == null || Right == null)
        {
            LogManager.Error("[BoundManager] Please assign all 4 colliders before calling Setup().");
            return;
        }

        MinX = Left.bounds.min.x + LeftOffset;
        MaxX = Right.bounds.max.x - RightOffset;
        MinZ = Bottom.bounds.min.z + BottomOffset - TouchAreaOffset;
        MaxZ = Top.bounds.max.z - TopOffset;
    }

    public static Vector3 Clamp(Vector3 point)
    {
        return new Vector3(
            Mathf.Clamp(point.x, MinX, MaxX),
            point.y,
            Mathf.Clamp(point.z, MinZ, MaxZ)
        );
    }
}
