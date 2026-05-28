using UnityEngine;
using UnityEngine.Rendering; 

public class YSort : MonoBehaviour
{
    [SerializeField] private Transform sortPoint;
    [SerializeField] private SortingGroup sortingGroup;
    [SerializeField] private int offset = 0;

    public void OnLateUpdate()
    {
        sortingGroup.sortingOrder = -(int)(sortPoint.position.y * 10 + offset);
    }
}
