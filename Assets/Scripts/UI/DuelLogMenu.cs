using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Input;
using Aremoreno.Enums.Log;

public class DuelLogMenu : Menu
{
    [Header("UI References")]
    [SerializeField] private DuelLogPopup popupPrefab;
    [SerializeField] private Transform contentParent; 
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private DuelLogPopupStack popupStack;

    // Pooling & performance settings
    private readonly List<DuelLogPopup> popupPool = new List<DuelLogPopup>();
    private const int MAX_LOGS_VISIBLE = 30;
    private Coroutine populateCoroutine;

    public bool IsDuelLogMenuOpen => MenuManager.Instance.IsMenuOpen(this);

    protected override void Awake()
    {
        base.Awake();
        BattleUIManager.Instance.RegisterDuelLogMenu(this);
    }

    private void OnDestroy()
    {
        BattleUIManager.Instance.UnregisterDuelLogMenu(this);
    }

    public override void Show()
    {
        if (populateCoroutine != null)
            StopCoroutine(populateCoroutine);

        populateCoroutine = StartCoroutine(PopulateLogCoroutine());

        base.Show();
    }

    protected override void OnGainedInput()
        => InputManager.Instance.SubscribeDown(CustomAction.Navigation_Back, OnButtonCloseClicked);

    protected override void OnLostInput()
        => InputManager.Instance.UnsubscribeDown(CustomAction.Navigation_Back, OnButtonCloseClicked);

    public void OnButtonCloseClicked()
    {
        EventSystem.current.SetSelectedGameObject(null);
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

    // Efficient coroutine-based population with pooling
    private IEnumerator PopulateLogCoroutine()
    {
        var layoutGroup = contentParent.GetComponent<VerticalLayoutGroup>();
        var fitter = contentParent.GetComponent<ContentSizeFitter>();

        // Temporarily disable layouts for performance
        if (layoutGroup) layoutGroup.enabled = false;
        if (fitter) fitter.enabled = false;

        // Deactivate all pooled popups
        foreach (var popup in popupPool)
        {
            popup.gameObject.SetActive(false);
        }

        // Retrieve recent logs
        var entries = DuelLogManager.Instance.DuelLogEntries;
        int startIndex = Mathf.Max(0, entries.Count - MAX_LOGS_VISIBLE);

        int index = 0;
        for (int i = startIndex; i < entries.Count; i++)
        {
            var entry = entries[i];
            if (entry.LogLevel == LogLevel.Info) continue;

            DuelLogPopup popup;
            if (index < popupPool.Count)
            {
                popup = popupPool[index];
            }
            else
            {
                popup = Instantiate(popupPrefab, contentParent);
                popupPool.Add(popup);
            }

            popup.ShowStatic(entry);
            popup.gameObject.SetActive(true);
            index++;

            // Yield periodically to avoid frame hitching
            if (index % 10 == 0)
                yield return null;
        }

        // Re-enable layout for final rebuild
        if (layoutGroup) layoutGroup.enabled = true;
        if (fitter) fitter.enabled = true;

        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)contentParent);

        // Scroll to bottom next frame
        yield return new WaitForEndOfFrame();
        scrollRect.verticalNormalizedPosition = 0f;
        //EventSystem.current.SetSelectedGameObject(null);
    }
}
