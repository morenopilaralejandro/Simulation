using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.UI;
using Aremoreno.Enums.Input;

public class MenuCredits : Menu
{
    [Header("UI References")]
    [SerializeField] private SceneGroup sceneMainMenu;
    [SerializeField] private float scrollSpeed = 30f;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Vector2 startPosition;
    private float endY = 5964f;

    private void Start() 
    {
        MenuManager.Instance.OpenMenu(this);
    }


    public override void Show() 
    {
        rectTransform.anchoredPosition = startPosition;

        base.Show();

        AudioManager.Instance.PlayBgm("bgm-ending_0_shining_in_the_dark");    
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
        RequestClose();
        SceneLoader.Instance.LoadGroup(sceneMainMenu);
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
