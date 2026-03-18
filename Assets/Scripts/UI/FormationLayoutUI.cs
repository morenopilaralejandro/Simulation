using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Simulation.Enums.Character;
using Simulation.Enums.Battle;

public class FormationLayoutUI : MonoBehaviour
{
    // TODO This must support full and mini battle
    // TODO toggle between edit mode and view only mode
    // Pass the formation

    [Header("UI References")]
    [SerializeField] private RectTransform fieldArea;
    [SerializeField] private RectTransform benchArea;
    [SerializeField] private Text formationNameLabel;
    [SerializeField] private GameObject playerSlotPrefab;

    [Header("Pitch Padding")]
    [SerializeField] private float paddingX = 50f;
    [SerializeField] private float paddingY = 40f;

    [Header("Animation")]
    [SerializeField] private bool animateTransitions = true;
    [SerializeField] private float transitionDuration = 0.4f;

    // Runtime data
    private Formation currentFormation;

    // Slot tracking
    private List<FormationCharacterSlotUI> fieldSlots = new List<FormationCharacterSlotUI>();
    private List<FormationCharacterSlotUI> benchSlots = new List<FormationCharacterSlotUI>();

    // Team data (assigned externally)
    private TeamLoadoutManager loadoutManager;
    private List<Character> teamRoster;

    private void Start()
    {

    }

    /// <summary>
    /// Call this to initialize with your loaded formations.
    /// </summary>
    public void Initialize(Team team, BattleType battleType)
    {
        loadoutManager = TeamLoadoutManager.Instance;
        teamRoster = loadoutManager.ResolveCharacters(team, battleType);
        SetFormation(team.GetFormation(battleType));
    }

    public void SetFormation(Formation formation)
    {
        currentFormation = formation;
        //formationNameLabel.text = currentFormation.FormationName;

        if (animateTransitions && fieldSlots.Count == currentFormation.FormationCoords.Count)
        {
            AnimateToFormation();
        }
        else
        {
            RebuildLayout();
        }
    }

    // ============================================================
    //  BUILD LAYOUT
    // ============================================================

    private void RebuildLayout()
    {
        ClearAllSlots();

        bool isMini = currentFormation.BattleType == BattleType.Mini;
        int fieldCount = currentFormation.FormationCoords.Count;

        // --- Field Slots ---
        for (int i = 0; i < fieldCount; i++)
        {
            FormationCoord coord = currentFormation.FormationCoords[i];
            GameObject slotGO = Instantiate(playerSlotPrefab, fieldArea);
            FormationCharacterSlotUI slot = slotGO.GetComponent<FormationCharacterSlotUI>();

            // Position
            Vector2 uiPos = isMini
                ? FormationMapperUI.WorldToUIPositionMini(coord.DefaultPosition, fieldArea)
                : FormationMapperUI.WorldToUIPosition(coord.DefaultPosition, fieldArea, 
                    paddingX, paddingY);

            RectTransform rt = slotGO.GetComponent<RectTransform>();
            rt.anchoredPosition = uiPos;

            // Data
            slot.Initialize(i, coord);
            if (i < teamRoster.Count)
                slot.SetCharacter(teamRoster[i]);

            fieldSlots.Add(slot);
        }

        // --- Bench Slots ---
        int benchCount = GetBenchCount();
        for (int i = 0; i < benchCount; i++)
        {
            GameObject slotGO = Instantiate(playerSlotPrefab, benchArea);
            FormationCharacterSlotUI slot = slotGO.GetComponent<FormationCharacterSlotUI>();

            int rosterIndex = fieldCount + i;
            if (rosterIndex < teamRoster.Count)
                slot.SetCharacter(teamRoster[rosterIndex]);

            slot.SetAsBench(i);
            benchSlots.Add(slot);
        }
    }

    private void AnimateToFormation()
    {
        bool isMini = currentFormation.BattleType == BattleType.Mini;

        for (int i = 0; i < fieldSlots.Count && i < currentFormation.FormationCoords.Count; i++)
        {
            FormationCoord coord = currentFormation.FormationCoords[i];
            Vector2 targetPos = isMini
                ? FormationMapperUI.WorldToUIPositionMini(coord.DefaultPosition, fieldArea)
                : FormationMapperUI.WorldToUIPosition(coord.DefaultPosition, fieldArea, 
                    paddingX, paddingY);

            RectTransform rt = fieldSlots[i].GetComponent<RectTransform>();
            fieldSlots[i].UpdateCoord(coord);

            // Simple coroutine-based lerp (or use DOTween)
            StartCoroutine(AnimateSlot(rt, targetPos, i * 0.02f));
        }
    }

    private System.Collections.IEnumerator AnimateSlot(RectTransform rt, 
        Vector2 target, float delay)
    {
        yield return new WaitForSeconds(delay);

        Vector2 start = rt.anchoredPosition;
        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / transitionDuration;
            // Ease out cubic
            t = 1f - Mathf.Pow(1f - t, 3f);
            rt.anchoredPosition = Vector2.Lerp(start, target, t);
            yield return null;
        }

        rt.anchoredPosition = target;
    }

    private int GetBenchCount()
    {
        // Full = 16 total (11 field + 5 bench), Mini = 4 field + 0 bench (adjust as needed)
        if (currentFormation.BattleType == BattleType.Mini) return 0;
        return Mathf.Max(0, teamRoster.Count - currentFormation.FormationCoords.Count);
    }

    private void ClearAllSlots()
    {
        foreach (var s in fieldSlots) if (s != null) Destroy(s.gameObject);
        foreach (var s in benchSlots) if (s != null) Destroy(s.gameObject);
        fieldSlots.Clear();
        benchSlots.Clear();
    }
}
