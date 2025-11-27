using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Localization;
using Simulation.Enums.Log;

public class DuelLogMenu : MonoBehaviour
{
    [SerializeField] private DuelLogPopup popupPrefab;
    [SerializeField] private Transform contentParent; // ScrollView content
    [SerializeField] private GameObject panelMenu; // ScrollView content
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private DuelLogPopupStack popupStack;

    private bool isOpen = false;
    public bool IsOpen => isOpen;
    
    void Awake()
    {
        BattleUIManager.Instance.RegisterDuelLogMenu(this);
    }

    void Start()
    {
        SetActive(false);
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

    public void Toggle()
    {
        isOpen = !isOpen;
        SetActive(isOpen);
    }

    public void SetActive(bool active) 
    {
        panelMenu.SetActive(active);
        popupStack.Toggle();
        if(active)
            PopulateLog();
    }
}
