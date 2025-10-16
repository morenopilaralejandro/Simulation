using UnityEngine;
using System.Collections.Generic;
using Simulation.Enums.Battle;

public class BoundManager : MonoBehaviour
{
    public static BoundManager Instance { get; private set; }

    private float topOffsetCharacter = 0f;
    private float bottomOffsetCharacter = 0f;
    private float leftOffsetCharacter = 0f;
    private float rightOffsetCharacter = 0f;
    private float touchAreaOffset = 0f;

    private float topOffsetCamera = 0f;
    private float bottomOffsetCamera = 0f;
    private float leftOffsetCamera = 0f;
    private float rightOffsetCamera = 0f;

    private Dictionary<BoundPlacement, Bound> bounds = new();
    
    #region Bound Accessors
    private Bound left => bounds.ContainsKey(BoundPlacement.Left) ? bounds[BoundPlacement.Left] : null;
    private Bound right => bounds.ContainsKey(BoundPlacement.Right) ? bounds[BoundPlacement.Right] : null;
    private Bound top => bounds.ContainsKey(BoundPlacement.Top) ? bounds[BoundPlacement.Top] : null;
    private Bound bottom => bounds.ContainsKey(BoundPlacement.Bottom) ? bounds[BoundPlacement.Bottom] : null;
    #endregion

    private float characterMinX;
    private float characterMaxX;
    private float characterMinZ;
    private float characterMaxZ;

    private float cameraMinX;
    private float cameraMaxX;
    private float cameraMinZ;
    private float cameraMaxZ;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Setup()
    {
        characterMinX = left.BoxCollider.bounds.min.x + leftOffsetCharacter;
        characterMaxX = right.BoxCollider.bounds.max.x - rightOffsetCharacter;
        characterMinZ = bottom.BoxCollider.bounds.min.z + bottomOffsetCharacter - touchAreaOffset;
        characterMaxZ = top.BoxCollider.bounds.max.z - topOffsetCharacter;

        cameraMinX = left.BoxCollider.bounds.min.x + leftOffsetCamera;
        cameraMaxX = right.BoxCollider.bounds.max.x - rightOffsetCamera;
        cameraMinZ = bottom.BoxCollider.bounds.min.z + bottomOffsetCamera;
        cameraMaxZ = top.BoxCollider.bounds.max.z - topOffsetCamera;
    }

    public void RegisterBound(Bound bound)
    {
        if (!bounds.ContainsKey(bound.BoundPlacement)) bounds.Add(bound.BoundPlacement, bound);
        if (AreAllBoundsReady()) Setup();
    }

    public void UnregisterBound(Bound bound)
    {
        if (bounds.ContainsKey(bound.BoundPlacement)) bounds.Remove(bound.BoundPlacement);
    }

    public Vector3 ClampCharacter(Vector3 point)
    {
        return new Vector3(
            Mathf.Clamp(point.x, characterMinX, characterMaxX),
            point.y,
            Mathf.Clamp(point.z, characterMinZ, characterMaxZ)
        );
    }

    public Vector3 ClampCamera(Vector3 point)
    {
        return new Vector3(
            Mathf.Clamp(point.x, cameraMinX, cameraMaxX),
            point.y,
            Mathf.Clamp(point.z, cameraMinZ, cameraMaxZ)
        );
    }

    private bool AreAllBoundsReady() => bounds.Count == 4;

}
