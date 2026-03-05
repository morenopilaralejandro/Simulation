using UnityEngine;

public class NpcEntity : MonoBehaviour
{
    #region Components

    [SerializeField] private NpcComponentInteractable interactableComponent;

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
