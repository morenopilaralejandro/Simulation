using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using TMPro;
using Aremoreno.Enums.UI;
using Aremoreno.Enums.Item;

public class PanelBagDescription : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text textName;
    [SerializeField] private TMP_Text textDescription;
    [SerializeField] private Image imageIcon;
    [SerializeField] private CanvasGroup canvasName;
    [SerializeField] private CanvasGroup canvasMove;
    [SerializeField] private MoveUI moveUI;
    [SerializeField] private CanvasGroup canvasGroup;

    private Move auxMove;
    private MoveData auxMoveData;
    private readonly AddressableBinding<Sprite> _bindingIcon = new();

    private int _setVersion;

    private void Start() 
    {
        Clear();
    }

    private void SetData(Item item)
    {
        int version = ++_setVersion;

        textName.text = item.ItemName;
        //textDescription.text = item.ItemDescription;
        textDescription.text = "";

        imageIcon.color = item.IconColor;
        _ = SetIconAsync(item.IconSpriteAddress, version);

        if (item is ItemMove itemMove)
        {
            auxMoveData = DatabaseManager.Instance.GetMoveData(itemMove.MoveId);
            auxMove = new Move(auxMoveData);
            moveUI.SetMoveAsync(auxMove);

            SetCanvasVisible(canvasMove, true);
            SetCanvasVisible(canvasName, false);
        }
        else
        {
            SetCanvasVisible(canvasMove, false);
            SetCanvasVisible(canvasName, true);
        }

        if (item is ItemRecovery itemRecovery)
        { 
            textDescription.text = itemRecovery.ItemDescription;    
        } else 
        {
            textDescription.text = "";
        }
    }

    private static void SetCanvasVisible(CanvasGroup canvas, bool visible)
    {
        canvas.alpha = visible ? 1f : 0f;
        canvas.interactable = visible;
        canvas.blocksRaycasts = visible;
    }

    private void Clear()
    {
        _setVersion++;

        textName.text = "";
        textDescription.text = "";

        _bindingIcon.Release();
        _bindingIcon.Cancel();

        imageIcon.enabled = false;
        imageIcon.sprite = null;

        SetCanvasVisible(canvasMove, false);
        SetCanvasVisible(canvasName, true);
    }

    private async Task SetIconAsync(string address, int version)
    {
        imageIcon.enabled = false;

        var sprite = await _bindingIcon.LoadAsync(address);

        if (version != _setVersion) return;

        imageIcon.sprite = sprite;
        imageIcon.enabled = true;
    }

    #region Events

    private void OnEnable()
    {
        //UIEvents.OnBagCategoryChanged += HandleBagCategoryChanged;
        UIEvents.OnBagDescriptionUpdated += HandleBagDescriptionUpdated;
    }

    private void OnDisable()
    {
        //UIEvents.OnBagCategoryChanged -= HandleBagCategoryChanged;
        UIEvents.OnBagDescriptionUpdated -= HandleBagDescriptionUpdated;
    }

    /*
    private void HandleBagCategoryChanged(ItemCategory itemCategory)
    {
        Clear();
    }
    */

    private void HandleBagDescriptionUpdated(Item item)
    {
        if(!canvasGroup.interactable) return;

        if (item == null) 
        {
            Clear();
            return;
        }

        SetData(item);
    }

    #endregion
}
