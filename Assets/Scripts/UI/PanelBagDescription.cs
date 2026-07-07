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

    private Move auxMove;
    private MoveData auxMoveData;
    private readonly AddressableBinding<Sprite> _bindingIcon = new();

    private void SetData(Item item)
    {
        textName.text = item.ItemName;
        //textDescription.text = item.ItemDescription;
        textDescription.text = "";
        imageIcon.color = item.IconColor;
        _ = SetIconAsync(item.IconSpriteAddress);

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
    }

    private static void SetCanvasVisible(CanvasGroup canvas, bool visible)
    {
        canvas.alpha = visible ? 1f : 0f;
        canvas.interactable = visible;
        canvas.blocksRaycasts = visible;
    }

    private void Clear()
    {
        textName.text = "";
        textDescription.text = "";
        _bindingIcon.Release();
        _bindingIcon.Cancel();
        imageIcon.enabled = false;
        imageIcon.sprite = null;

        SetCanvasVisible(canvasMove, false);
        SetCanvasVisible(canvasName, true);
    }

    public async Task SetIconAsync(string address)
    {
        imageIcon.enabled = false;
        var asset = await _bindingIcon.LoadAsync(address);
        imageIcon.sprite = asset;
        imageIcon.enabled = true;
    }

    #region Events

    private void OnEnable()
    {
        UIEvents.OnBagCategoryChanged += HandleBagCategoryChanged;
        UIEvents.OnBagDescriptionUpdated += HandleBagDescriptionUpdated;
    }

    private void OnDisable()
    {
        UIEvents.OnBagCategoryChanged -= HandleBagCategoryChanged;
        UIEvents.OnBagDescriptionUpdated -= HandleBagDescriptionUpdated;
    }

    private void HandleBagCategoryChanged(ItemCategory itemCategory)
    {
        Clear();
    }

    private void HandleBagDescriptionUpdated(Item item)
    {
        SetData(item);
    }

    #endregion
}
