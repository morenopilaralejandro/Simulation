using UnityEngine;
using Simulation.Enums.Character;
using Simulation.Enums.Battle;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ShootTriangle : MonoBehaviour
{
    [Header("References")]
    private Mesh triangleMesh;
    [SerializeField] private MeshFilter triangleMeshFilter;
    [SerializeField] private MeshRenderer triangleMeshRenderer;

    private BoxCollider boundTop => BoundManager.Instance?.Bounds[BoundPlacement.Top].BoxCollider;
    private BoxCollider boundBottom => BoundManager.Instance?.Bounds[BoundPlacement.Bottom].BoxCollider;

    [Header("Triangle Vertices")]
    [SerializeField] private Vector3 vertex0;
    [SerializeField] private Vector3 vertex1;
    [SerializeField] private Vector3 vertex2;

    [Header("Base Length/Range Settings")]
    private float coordY = 0.02f;
    private float medianMin = 0.1f;
    private float medianMax = 2.0f;
    private float baseLengthAtDefault = 1.2f;
    private float widenFactorDefault = 1.0f;
    private float widenFactorMax = 2.0f;
    private int controlMin = 0;
    private int controlMax = 130;
    private float narrowFactorMin = 1.0f;
    private float narrowFactorMax = 0.3f;
    private float randomPointY = 0.5f;

    private void Awake()
    {
        ShootTriangleManager.Instance?.RegisterTriangle(this);
    }

    private void OnDestroy()
    {
        ShootTriangleManager.Instance?.UnregisterTriangle(this);
    }

    private void Start()
    {
        triangleMesh = new Mesh();
        triangleMeshFilter.mesh = triangleMesh;
        UpdateMesh();
    }

    private void UpdateMesh()
    {
        vertex0.y = coordY;
        vertex1.y = coordY;
        vertex2.y = coordY;

        Vector3[] vertices = { vertex0, vertex1, vertex2 };

        // Ensure consistent winding order for culling
        if (vertex1.z < 0)
        {
            // Swap vertex1 and vertex2
            Vector3 tmp = vertex1;
            vertices[1] = vertex2;
            vertices[2] = tmp;
        }

        int[] triangles = { 0, 1, 2 };

        triangleMesh.Clear();
        triangleMesh.vertices = vertices;
        triangleMesh.triangles = triangles;
        triangleMesh.RecalculateNormals();
        triangleMesh.RecalculateBounds();
    }

    public void Hide()
    {
        triangleMeshRenderer.enabled = false;
    }

    public void Show()
    {
        triangleMeshRenderer.enabled = true;
    }

    public void SetTriangleFromCharacter(
        Character character, 
        Vector3 worldCoord)
    {
        // --- The actual triangle math ---
        vertex0 = character.transform.position;
        Vector3 dirToTarget = (worldCoord - vertex0).normalized;
        Vector3 perpendicular = Vector3.Cross(dirToTarget, Vector3.up).normalized;

        vertex1 = worldCoord + perpendicular;
        vertex2 = worldCoord - perpendicular;

        /*
        float borderZ = (worldCoord.z >= 0f) 
            ? boundTop.bounds.min.z 
            : boundBottom.bounds.max.z;
        */

        float borderZ = (worldCoord.z >= 0f) 
            ? GoalManager.Instance.Goals[character.GetOpponentSide()].GoalCollider.bounds.min.z
            : GoalManager.Instance.Goals[character.GetOpponentSide()].GoalCollider.bounds.max.z;
        vertex1.z = borderZ;
        vertex2.z = borderZ;

        AdjustBaseLengthByMedian(character);

        UpdateMesh();
    }

    /// <summary>
    /// Adjusts the base width of the triangle based on how far the tap is from the player's center (median)
    /// and the player's control stat (both serialized for inspector tuning).
    /// </summary>
    private void AdjustBaseLengthByMedian(Character character)
    {
        float baseMedianX = (vertex1.x + vertex2.x) * 0.5f;
        float median = Mathf.Abs(vertex0.x - baseMedianX);
        LogManager.Trace($"[ShootTriangle] Median distance for base adjustment: {median}", this);

        float t = Mathf.InverseLerp(medianMin, medianMax, median);
        float widenFactor = Mathf.Lerp(widenFactorDefault, widenFactorMax, t);

        float characterControl = character.GetBattleStat(Stat.Control);
        float controlT = Mathf.InverseLerp(controlMin, controlMax, characterControl);
        float narrowFactor = Mathf.Lerp(narrowFactorMin, narrowFactorMax, controlT);

        // Apply both control and widen factors:
        float baseAdjustment = baseLengthAtDefault * widenFactor * narrowFactor;
        float halfBase = baseAdjustment * 0.5f;

        // Re-calculate the "base" midpoint and set vertices symmetrically
        float baseZ = (vertex1.z + vertex2.z) * 0.5f;
        float baseY = (vertex1.y + vertex2.y) * 0.5f;
        float baseMidX = baseMedianX;
        Vector3 baseMid = new Vector3(baseMidX, baseY, baseZ);

        vertex1 = baseMid - Vector3.right * halfBase;
        vertex2 = baseMid + Vector3.right * halfBase;

        LogManager.Trace($"[ShootTriangle] Base adjusted: " +
            $"width={baseAdjustment}, " +
            $"factors (widen={widenFactor}, " +
            $"narrow={narrowFactor}), " +
            $"vertices: v1={vertex1}, v2={vertex2}", this);
    }

    public Vector3 GetRandomPoint()
    {
        float t = Random.Range(0f, 1f);
        Vector3 randomPoint = Vector3.Lerp(vertex1, vertex2, t);
        randomPoint.y = randomPointY;
        LogManager.Trace($"[ShootTriangle] Generated random point on base: {randomPoint}", this);
        return randomPoint;
    }
}
