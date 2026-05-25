using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Move;

public class SelectorLoadoutListItem : SelectorListItem<Team>
{
    [Header("UI Elements")]
    [SerializeField] private Image imageEmblem;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private GameObject activeIndicator;
    private readonly AddressableBinding<Sprite> _binding = new();

    protected override void OnBind(Team data)
    {
        _ = SetEmblemAsync(data.Emblem.EmblemAddress);
        nameText.text = data.TeamName;

        bool isActive = TeamManager.Instance.ActiveLoadoutGuid == data.TeamGuid;
        activeIndicator.SetActive(isActive);
    }

    protected override void OnUnbind()
    {
        imageEmblem.sprite = null;
        nameText.text = "";

        _binding.Release();
        _binding.Cancel();
    }

    private async System.Threading.Tasks.Task SetEmblemAsync(string teamEmblemAddress)
    {
        var task = _binding.LoadAsync(teamEmblemAddress);
        imageEmblem.sprite = await task;
    }
}
