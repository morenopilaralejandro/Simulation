using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public void Interact()
    {
        InteractInternal();
    }

    protected abstract void InteractInternal();
}


