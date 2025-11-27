using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Simulation.Enums.Log;

public class DuelLogPopupStack : MonoBehaviour
{
    [Header("Popup Lifetime")]
    [SerializeField] private float minDisplayTime = 2f;           // Minimum on screen time
    [SerializeField] private float autoHideTime = 2f;             // Disappear after this
    [Header("Stagger Intervals")]
    [SerializeField] private float addStaggerInterval = 0.3f;     // Delay between shows
    [SerializeField] private float removeStaggerInterval = 1f;    // Delay between removals
    [Header("Setup")]
    [SerializeField] private DuelLogPopup popupPrefab;
    [SerializeField] private Transform popupParent;
    [SerializeField] private GameObject panelStack;
    [SerializeField] private int maxPopups = 3;

    private List<DuelLogPopUpData> activePopups = new List<DuelLogPopUpData>();
    private Queue<DuelLogEntry> pendingEntries = new Queue<DuelLogEntry>();
    private bool isAdding = false;
    private bool isRemoving = false;
    private bool isOpen;

    void OnEnable()
    {
        BattleEvents.OnBattleStart += HandleBattleStart;
        SettingsEvents.OnAutoBattleToggled += HandleAutoBattleToggled;
        DuelLogEvents.OnNewEntry += EnqueuePopup;
    }

    void OnDisable()
    { 
        BattleEvents.OnBattleStart -= HandleBattleStart;
        SettingsEvents.OnAutoBattleToggled -= HandleAutoBattleToggled;
        DuelLogEvents.OnNewEntry -= EnqueuePopup; 
    }

    public void Toggle()
    {
        if (!isOpen && SettingsManager.Instance.IsAutoBattleEnabled) return;

        isOpen = !isOpen;
        SetActive(isOpen);
    }

    public void SetActive(bool active) 
    {
        panelStack.SetActive(active);
        if (active)
            ClearAllPopups();
    }

    private void HandleBattleStart() => SetActive(!SettingsManager.Instance.IsAutoBattleEnabled);
    private void HandleAutoBattleToggled(bool enable) => SetActive(!enable);

    void EnqueuePopup(DuelLogEntry entry)
    {
        if (entry.LogLevel != LogLevel.Info) return;

        pendingEntries.Enqueue(entry);

        if (!isAdding)
            StartCoroutine(AddPopupsStaggered());
    }

    IEnumerator AddPopupsStaggered()
    {
        isAdding = true;

        while (pendingEntries.Count > 0 && activePopups.Count < maxPopups)
        {
            DuelLogEntry entry = pendingEntries.Dequeue();
            DuelLogPopup popup = Instantiate(popupPrefab, popupParent);
            popup.Show(entry);
            activePopups.Add(new DuelLogPopUpData { Popup = popup, SpawnTime = popup.SpawnTime, RemovalScheduled = false });

            if (activePopups.Count < maxPopups)
                yield return new WaitForSecondsRealtime(addStaggerInterval);
        }

        isAdding = false;

        // Start removal process if any popups need to be removed (either new popups can't fit, or need auto-hide)
        if (!isRemoving)
            StartCoroutine(RemoveDuePopupsStaggered());
    }

    IEnumerator RemoveDuePopupsStaggered()
    {
        isRemoving = true;
        while (true)
        {
            float now = Time.unscaledTime;
            bool removed = false;

            // --- 1. Remove for overflow (when at or over maxPopups) ---
            while (activePopups.Count > maxPopups)
            {
                var oldest = activePopups[0];
                float alive = now - oldest.SpawnTime;
                float wait = Mathf.Max(minDisplayTime - alive, 0f);
                if (wait > 0f)
                    yield return new WaitForSecondsRealtime(wait);

                RemovePopup(oldest);
                removed = true;

                // Wait between removals for stagger
                yield return new WaitForSecondsRealtime(removeStaggerInterval);

                // Update now in case time elapsed
                now = Time.unscaledTime;
            }

            // --- 2. Remove for auto-hide (popups that reached their 5s) ---
            foreach (var pd in new List<DuelLogPopUpData>(activePopups))
            {
                float alive = now - pd.SpawnTime;
                // Only remove if at least autoHideTime elapsed
                if (!pd.RemovalScheduled && alive >= autoHideTime)
                {
                    RemovePopup(pd);
                    removed = true;
                    yield return new WaitForSecondsRealtime(removeStaggerInterval);
                    break; // Only one removal per interval for staggering
                }
            }

            if (!removed)
                yield return null;

            // End if there’s nothing left to remove or add
            if (activePopups.Count == 0 && pendingEntries.Count == 0)
                break;
        }
        isRemoving = false;
    }

    void RemovePopup(DuelLogPopUpData pd)
    {
        pd.RemovalScheduled = true;
        activePopups.Remove(pd);
        Destroy(pd.Popup.gameObject);
    }

    private void ClearAllPopups()
    {
        // Destroy existing popup GameObjects
        foreach (var pd in activePopups)
        {
            if (pd.Popup != null)
                Destroy(pd.Popup.gameObject);
        }

        activePopups.Clear();
        pendingEntries.Clear();

        isAdding = false;
        isRemoving = false;
    }

    void Update()
    {
        // For robustness: if popups on screen and neither stagger is running, restart
        if (!isAdding && pendingEntries.Count > 0 && activePopups.Count < maxPopups)
            StartCoroutine(AddPopupsStaggered());

        if (!isRemoving && activePopups.Count > 0)
            StartCoroutine(RemoveDuePopupsStaggered());
    }

    
}
