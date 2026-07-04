using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;
using Aremoreno.Enums.Match;

public class SelectorMatchChainNodeListItem : SelectorListItem<MatchChainNode>
{
    [Header("UI")]
    [SerializeField] private Image imageIcon;
    [SerializeField] private Image imageArrow;
    [SerializeField] private Color colorArrowEnable;
    [SerializeField] private Color colorArrowDisable;

    private readonly AddressableBinding<Sprite> _binding = new();

    protected override void OnBind(MatchChainNode n)
    {
        LogManager.Trace($"BIND | id={n.MatchChainNodeId} | hash={n.GetHashCode()} | unlocked={n.IsNodeUnlocked} | completed={n.IsNodeCompleted}");

        base.button.interactable = n.IsNodeUnlocked;
        imageArrow.color = n.IsNodeCompleted ? colorArrowEnable : colorArrowDisable;
        imageArrow.enabled = !n.IsLastNode;
        _ = SetAsync(n.IconAddress);
    }

    protected override void OnUnbind()
    {
        imageIcon.sprite = null;

        _binding.Release();
        _binding.Cancel();
    }

    private async Task SetAsync(string address)
    {
        imageIcon.enabled = false;
        var asset = await _binding.LoadAsync(address);
        imageIcon.sprite = asset;
        imageIcon.enabled = true;
    }

    #region Events

    private void OnEnable()
    {
        UIEvents.OnMatchChainNodeUpdated += HandleMatchChainNodeUpdated;
        Selected += HandleItemSelected;
    }

    private void OnDisable()
    {
        UIEvents.OnMatchChainNodeUpdated -= HandleMatchChainNodeUpdated;
        Selected -= HandleItemSelected;
    }

    private void HandleMatchChainNodeUpdated(MatchChainNode node)
    {
        if (Data.MatchChainNodeId != node.MatchChainNodeId) return;
        OnBind(node);
    }

    private void HandleItemSelected(SelectorListItem<MatchChainNode> listItem)
    {
        if (Data.MatchChainNodeId != listItem.Data.MatchChainNodeId) return;
        StorySystemManager.Instance.TrySetSelectedIndex(listItem.Data);
    }


    #endregion
}
