using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectorTeamEmblemListItem : SelectorListItem<Emblem>
{
    [Header("UI Elements")] 
    [SerializeField] private Image emblemImage;
    private readonly AddressableBinding<Sprite> _binding = new();

    protected override void OnBind(Emblem data)
    {
        _ = SetEmblemAsync(data.EmblemAddress);
    }

    protected override void OnUnbind()
    {
        emblemImage.sprite = null;
        _binding.Release();
        _binding.Cancel();
    }

    private async System.Threading.Tasks.Task SetEmblemAsync(string teamEmblemAddress)
    {
        var task = _binding.LoadAsync(teamEmblemAddress);
        emblemImage.sprite = await task;
    }
}
