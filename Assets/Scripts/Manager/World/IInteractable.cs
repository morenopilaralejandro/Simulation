public interface IInteractable
{
    void Interact();
    float GetInteractionRange();
}

/*

Example implementation

using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    [SerializeField] private float interactionRange = 1.5f;
    private bool opened = false;

    public void Interact()
    {
        if (!opened)
        {
            opened = true;
            Debug.Log("Chest Opened!");
        }
    }

    public float GetInteractionRange() => interactionRange;
}

*/

/*

Example input

using UnityEngine;
using UnityEngine.InputSystem;

public class TouchInputManager : MonoBehaviour
{
    private PlayerInputActions inputActions;
    private GameObject player;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnEnable()
    {
        inputActions.Enable();
        inputActions.Touch.Tap.performed += OnTap;
    }

    private void OnDisable()
    {
        inputActions.Touch.Tap.performed -= OnTap;
        inputActions.Disable();
    }

    private void OnTap(InputAction.CallbackContext context)
    {
        Vector2 screenPosition = inputActions.Touch.TouchPosition.ReadValue<Vector2>();
        DetectInteraction(screenPosition);
    }




    use specific layer
    [SerializeField] private LayerMask interactableLayer;
    RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero, Mathf.Infinity, interactableLayer);


    private void DetectInteraction(Vector2 screenPosition)
    {
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);

        RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);

        if (hit.collider == null) return;

        IInteractable interactable = hit.collider.GetComponent<IInteractable>();

        if (interactable != null)
        {
            float distance = Vector2.Distance(player.transform.position, hit.collider.transform.position);

            if (distance <= interactable.GetInteractionRange())
            {
                interactable.Interact();
            }
            else
            {
                Debug.Log("Too far away");
            }
        }
    }
}


Better flow:

Player taps chest
If far → player auto-walks to it
When within range → interact automatically


*/


