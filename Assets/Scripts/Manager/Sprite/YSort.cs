using UnityEngine;
using UnityEngine.Rendering; 

public class YSort : MonoBehaviour
{
    [SerializeField] private Transform sortPoint;
    [SerializeField] private SortingGroup sortingGroup;
    [SerializeField] private int offset = 0;

    void LateUpdate()
    {
        sortingGroup.sortingOrder = -(int)(sortPoint.position.y * 10 + offset);
    }
}
