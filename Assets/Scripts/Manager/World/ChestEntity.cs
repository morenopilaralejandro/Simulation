using UnityEngine;

public class ChestEntity : MonoBehaviour
{
    #region Components

    [SerializeField] private ChestComponentInteractable interactableComponent;

    #endregion

    #region Initialize

    public void Awake() 
    {
        Initialize();
    }

    public void Initialize()
    {
        interactableComponent.Initialize(this);
    }

    #endregion

    #region API

    #endregion
}
