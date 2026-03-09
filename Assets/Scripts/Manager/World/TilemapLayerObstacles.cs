using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapLayerObstacles : MonoBehaviour
{
    [SerializeField]
    private Tilemap tilemap;

    private void Awake()
    {
        Color color = tilemap.color;
        color.a = 0f;
        tilemap.color = color;
    }
}
