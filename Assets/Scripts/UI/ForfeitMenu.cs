using UnityEngine;
using UnityEngine.EventSystems;
using Aremoreno.Enums.Input;

public class ForfeitMenu : Menu
{
    //[Header("UI References")]
    //[SerializeField] private TMPInputFieldNoAutoActivate inputFieldName;

    public bool IsForfeitMenuOpen => MenuManager.Instance.IsMenuOpen(this);

    protected override void Awake()
    {
        base.Awake();
        BattleUIManager.Instance.RegisterForfeitMenu(this);
    }

    private void Destroy()
    {
        BattleUIManager.Instance.UnregisterForfeitMenu(this);
    }

    protected override void OnGainedInput()
        => InputManager.Instance.SubscribeDown(CustomAction.Navigation_Back, OnButtonCancelTapped);

    protected override void OnLostInput()
        => InputManager.Instance.UnsubscribeDown(CustomAction.Navigation_Back, OnButtonCancelTapped);

    public void OnButtonConfimTapped()
    {
        AudioManager.Instance.PlaySfxUI("sfx-menu_tap");
        RequestClose();
        BattleManager.Instance.ForfeitBattle();
    }

    public void OnButtonCancelTapped()
    {
        RequestClose();
    }

    /*
    protected override void OnEnable()
    {
        base.OnEnable();
        UIEvents.OnTeamPanelNameOpened += HandleTeamPanelNameOpened;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        UIEvents.OnTeamPanelNameOpened -= HandleTeamPanelNameOpened;
    }

    private void HandleTeamPanelNameOpened(string teamName)
    {
        inputFieldName.text = teamName;
        MenuManager.Instance.OpenMenu(this);
    }
    */
}
