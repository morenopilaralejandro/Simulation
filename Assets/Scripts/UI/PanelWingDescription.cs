using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using TMPro;
using Aremoreno.Enums.UI;
using Aremoreno.Enums.Item;

public class PanelWingDescription : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private WingUI wingUI;
    [SerializeField] private StatLayoutUI statLayoutUI;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private CanvasGroup canvasGroupWing;

    //private readonly AddressableBinding<Sprite> _bindingIcon = new();
    //private int _setVersion;

    private void Start() 
    {
        Clear();
    }

    private void SetData(Wing wing)
    {
        if(wing == null) 
        {
            Clear();
            return;
        }

        wingUI.SetData(wing);
        statLayoutUI.Populate(wing);
        SetWingCanvasVisible(true);
    }

    private void Clear()
    {
        SetWingCanvasVisible(false);
        wingUI.Clear();
        statLayoutUI.Clear();
    }

    #region Events

    private void OnEnable()
    {
        UIEvents.OnWingDescriptionUpdateRequested += HandleWingDescriptionUpdateRequested;
    }

    private void OnDisable()
    {
        UIEvents.OnWingDescriptionUpdateRequested -= HandleWingDescriptionUpdateRequested;
    }

    private void HandleWingDescriptionUpdateRequested(Wing wing)
    {
        if(!canvasGroup.interactable) return;

        if (wing == null) 
        {
            Clear();
            return;
        }

        SetData(wing);
    }

    public void SetWingCanvasVisible(bool visible)
    {
        canvasGroupWing.alpha = visible ? 1f : 0f;
        canvasGroupWing.interactable = visible;
        canvasGroupWing.blocksRaycasts = visible;
    }

    #endregion
}
