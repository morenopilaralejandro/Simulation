using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Simulation.Enums.Character;

public class SpeechBubble : MonoBehaviour
{
    [Header("Assign Panels for Each Message")]
    [SerializeField] private GameObject panelWin;
    [SerializeField] private GameObject panelLose;
    [SerializeField] private GameObject panelDribble;
    [SerializeField] private GameObject panelBlock;
    [SerializeField] private GameObject panelPass;
    [SerializeField] private GameObject panelShoot;
    [SerializeField] private GameObject panelNice;
    [SerializeField] private GameObject panelDirect;

    private Coroutine hideRoutine;
    private Dictionary<BubbleMessage, GameObject> panelMap;
    private GameObject activePanel;
    private float defaultDuration = 1f;

    private void Awake()
    {
        // Build map for convenient access
        panelMap = new Dictionary<BubbleMessage, GameObject>
        {
            { BubbleMessage.Win, panelWin },
            { BubbleMessage.Lose, panelLose },
            { BubbleMessage.Dribble, panelDribble },
            { BubbleMessage.Block, panelBlock },
            { BubbleMessage.Pass, panelPass },
            { BubbleMessage.Shoot, panelShoot },
            { BubbleMessage.Nice, panelNice },
            { BubbleMessage.Direct, panelDirect }
        };

        HideImmediate();
    }

    /// <summary>
    /// Shows a specific bubble message and auto-hides after the set duration.
    /// </summary>
    public void ShowMessage(BubbleMessage bubbleMessage)
    {
        // Stop any running routine
        if (hideRoutine != null)
            StopCoroutine(hideRoutine);

        // Hide all panels first — only one can show at once
        HideAllPanels();

        // Find the correct panel
        if (panelMap.TryGetValue(bubbleMessage, out GameObject targetPanel) && targetPanel != null)
        {
            targetPanel.SetActive(true);
            gameObject.SetActive(true);
            activePanel = targetPanel;
            hideRoutine = StartCoroutine(HideAfterSeconds(defaultDuration));
        }
        else
        {
            Debug.LogWarning($"[SpeechBubble] No panel assigned for BubbleMessage: {bubbleMessage}");
        }
    }

    /// <summary>
    /// Instantly hides any visible bubble.
    /// </summary>
    public void HideImmediate()
    {
        if (hideRoutine != null)
        {
            StopCoroutine(hideRoutine);
            hideRoutine = null;
        }

        HideAllPanels();
        gameObject.SetActive(false);
    }

    private void HideAllPanels()
    {
        foreach (var kvp in panelMap)
        {
            if (kvp.Value != null)
                kvp.Value.SetActive(false);
        }

        activePanel = null;
    }

    private IEnumerator HideAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        HideImmediate();
    }
}
