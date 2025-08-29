using UnityEngine;

public class CharacterComponentKeeper : MonoBehaviour
{
    [SerializeField] private bool isKeeper;
    [SerializeField] private Collider keeperCollider;

    public bool IsKeeper() => isKeeper;

    public void UpdateKeeperColliderState()
    {
        if (keeperCollider != null)
            keeperCollider.enabled = isKeeper;
    }
}
