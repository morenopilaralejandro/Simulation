using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using TMPro;
using Aremoreno.Enums.UI;
using Aremoreno.Enums.Item;

public class PanelWingDescription : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text textName;
    [SerializeField] private Image imageIcon;
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
        //int version = ++_setVersion;

        textName.text = wing.WingName;
        imageIcon.color = ColorManager.GetWingColor(wing.WingColorType);
        statLayoutUI.Populate(wing);

        //_ = SetIconAsync(item.IconSpriteAddress, version);
    }

    private void Clear()
    {
        textName.text = "";
        imageIcon.color = Color.white;        
        statLayoutUI.Clear();

        /*
        _setVersion++;

        textName.text = "";
        textDescription.text = "";

        _bindingIcon.Release();
        _bindingIcon.Cancel();

        imageIcon.enabled = false;
        imageIcon.sprite = null;
        */
    }

    /*

    private async Task SetIconAsync(string address, int version)
    {
        imageIcon.enabled = false;

        var sprite = await _bindingIcon.LoadAsync(address);

        if (version != _setVersion) return;

        imageIcon.sprite = sprite;
        imageIcon.enabled = true;
    }

    */

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
