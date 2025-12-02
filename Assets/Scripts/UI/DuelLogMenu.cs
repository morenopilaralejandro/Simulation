using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Localization;
using Simulation.Enums.Log;
using Simulation.Enums.Input;

public class DuelLogMenu : Menu
{
    [SerializeField] private DuelLogPopup popupPrefab;
    [SerializeField] private Transform contentParent; // ScrollView content
    [SerializeField] private GameObject panelMenu; // ScrollView content
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private DuelLogPopupStack popupStack;

    public bool IsDuelLogMenuOpen => MenuManager.Instance.IsMenuOpen(this);

    void Awake()
    {
        BattleUIManager.Instance.RegisterDuelLogMenu(this);
    }

    void Start()
    {
        base.Hide();
        base.SetInteractable(false);
    }

    private void OnEnable()
    {
    }

    private void OnDisable()
    {

    }

    private void OnDestroy()
    {
        BattleUIManager.Instance.UnregisterDuelLogMenu(this);
    }

    void Update()
    {
        if (!IsInteractable()) return;
        HandleInput();
    }

    private void HandleInput()
    {
        if (IsDuelLogMenuOpen)
        {
            if (InputManager.Instance.GetDown(CustomAction.BattleUI_CloseBattleMenu))
                MenuManager.Instance.CloseMenu();
        }
    }

    // Call this when opening the menu
    private void PopulateLog()
    {
        // Clear previous items
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        // Add a static popup for each log entry
        foreach (DuelLogEntry entry in DuelLogManager.Instance.DuelLogEntries)
        {
            if(entry.LogLevel == LogLevel.Info) continue;

            var popup = Instantiate(popupPrefab, contentParent);

            // Use a special method or overload that doesn't start a timer
            popup.ShowStatic(entry);
        }

        StartCoroutine(ScrollToBottomNextFrame()); // <-- Fix here
        //AudioManager.Instance.PlaySfx("SfxMenuTap");
    }

    private IEnumerator ScrollToBottomNextFrame()
    {
        yield return new WaitForEndOfFrame(); // Wait one frame for UI to rebuild
        scrollRect.verticalNormalizedPosition = 0f;
    }

    public override void Show()
    {
        base.Show();
        PopulateLog();
    }

    public override void Hide()
    {
        base.Hide();
    }

    private void HandleMenuOpened(Menu menu) 
    {
        if (menu == this) return;
        if (!IsDuelLogMenuOpen) return;

        MenuManager.Instance.CloseMenu();
    }
}
