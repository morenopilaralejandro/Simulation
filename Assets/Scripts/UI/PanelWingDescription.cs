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

    //private readonly AddressableBinding<Sprite> _bindingIcon = new();
    //private int _setVersion;

    private void Start() 
    {
        Clear();
    }

    private void SetData(Wing wing)
    {
        wingUI.SetData(wing);
        statLayoutUI.Populate(wing);
    }

    private void Clear()
    {
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

    #endregion
}
