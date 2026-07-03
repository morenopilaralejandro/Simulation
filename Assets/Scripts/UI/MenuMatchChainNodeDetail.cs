using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;
using Aremoreno.Enums.Input;

public class MenuMatchChainNodeDetail : Menu
{
    [Header("Panels")]
    [SerializeField] private CanvasGroup panelText;
    [SerializeField] private CanvasGroup panelImage;
    [SerializeField] private CanvasGroup panelChest;

    [SerializeField] private TMP_Text textDisplayNode;
    [SerializeField] private Image imageDisplayNode;
    [SerializeField] private Button buttonOpenChest;

    private readonly AddressableBinding<Sprite> _bindingImageDisplay = new();
    private Item item;
    private MatchChainNode node;
    private MatchChainNodeChest nodeChest;

    protected override void OnEnable()
    {
        base.OnEnable();

        UIEvents.OnMatchChainNodeDetailOpened += HandleMatchChainNodeDetailOpened;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        UIEvents.OnMatchChainNodeDetailOpened -= HandleMatchChainNodeDetailOpened;
    }

    protected override void OnGainedInput()
    {
        InputManager.Instance.SubscribeDown(CustomAction.Navigation_Back, OnButtonCloseClicked);
    }

    protected override void OnLostInput()
    {
        InputManager.Instance.UnsubscribeDown(CustomAction.Navigation_Back, OnButtonCloseClicked);
    }

    private void HandleMatchChainNodeDetailOpened(MatchChainNode node)
    {
        HideAllPanels();
        
        this.node = node;

        switch (node)
        {
            case MatchChainNodeText textNode:
                textDisplayNode.text = textNode.MatchChainNodeDisplayText;
                ShowPanel(panelText);
                break;

            case MatchChainNodeImage imageNode:
                _ = SetImageDisplayAsync(imageNode.ImageAddress);
                ShowPanel(panelImage);
                break;

            case MatchChainNodeChest chestNode:
                this.nodeChest = chestNode;
                item = ItemFactory.CreateById(chestNode.ItemId);
                buttonOpenChest.interactable = !chestNode.IsChestOpen;
                ShowPanel(panelChest);
                break;

            default:
                LogManager.Trace($"Unsupported MatchChainNode type: {node.GetType().Name}");
                return;
        }

        MenuManager.Instance.OpenMenu(this);
    }

    public void OnButtonCloseClicked()
    {
        RequestClose();
        Clear();
    }

    private void HideAllPanels()
    {
        SetCanvasGroup(panelText, false);
        SetCanvasGroup(panelImage, false);
        SetCanvasGroup(panelChest, false);
    }

    private void ShowPanel(CanvasGroup group)
    {
        SetCanvasGroup(group, true);
    }

    private void SetCanvasGroup(CanvasGroup group, bool visible)
    {
        group.alpha = visible ? 1f : 0f;
        group.interactable = visible;
        group.blocksRaycasts = visible;
    }

    private void Clear()
    {
        imageDisplayNode.sprite = null;
        textDisplayNode = null;

        _bindingImageDisplay.Release();
        _bindingImageDisplay.Cancel();
    }

    public async Task SetImageDisplayAsync(string address)
    {
        imageDisplayNode.enabled = false;
        var asset = await _bindingImageDisplay.LoadAsync(address);
        imageDisplayNode.sprite = asset;
        imageDisplayNode.enabled = true;
    }

    public void OnButtonOpenChestClicked()
    {
        nodeChest.Open();
        RequestClose();
    }

}
