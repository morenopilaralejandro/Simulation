using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.UI;
using Aremoreno.Enums.Input;

public class MenuMainPanelCredits : Menu
{
    [Header("UI References")]
    [SerializeField] private float scrollSpeed = 30f;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Vector2 startPosition;
    private float endY = 4964f;

    public override void Show() 
    {
        rectTransform.anchoredPosition = startPosition;

        base.Show();

        AudioManager.Instance.PlayBgm("bgm-boru_arrival");    
    }

    void Update()
    {
        if (!IsInteractable()) return; 
        if (rectTransform.anchoredPosition.y >= endY) return;
        rectTransform.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;
    }

    protected override void OnGainedInput()
        => InputManager.Instance.SubscribeDown(CustomAction.Navigation_Back, OnButtonBackClicked);

    protected override void OnLostInput()
        => InputManager.Instance.UnsubscribeDown(CustomAction.Navigation_Back, OnButtonBackClicked);

    public void OnButtonBackClicked()
    {
        AudioManager.Instance.PlayBgm("bgm-simulation");
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
