using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Simulation.Enums.Character;
using Simulation.Enums.Battle;
using Simulation.Enums.Kit;

public class FormationLayoutUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RectTransform fieldArea;
    [SerializeField] private RectTransform benchArea;
    [SerializeField] private GameObject playerSlotPrefab;
    [SerializeField] private TMP_Text textTeamName;
    //[SerializeField] private TMP_Text formationNameLabel;
    [SerializeField] private Image imageTeamCrest;

    [Header("Pitch Padding")]
    [SerializeField] private float paddingX = 50f;
    [SerializeField] private float paddingY = 40f;

    [Header("Animation")]
    [SerializeField] private bool animateTransitions = true;
    [SerializeField] private float transitionDuration = 0.4f;

    // Runtime data
    private Formation currentFormation;
    private Kit currentKit;
    private bool showDefaultSelected = true;

    // Slot tracking
    private List<FormationCharacterSlotUI> fieldSlots = new List<FormationCharacterSlotUI>();
    private List<FormationCharacterSlotUI> benchSlots = new List<FormationCharacterSlotUI>();

    // Team data (assigned externally)
    private TeamManager teamManager;
    private List<Character> teamRoster;

    private void Start()
    {

    }

    /// <summary>
    /// Call this to initialize with your loaded formations.
    /// </summary>
    public void Initialize(Team team, BattleType battleType)
    {
        teamManager = TeamManager.Instance;
        teamRoster = teamManager.ResolveCharacters(team, battleType);
        textTeamName.text = team.TeamName;
        imageTeamCrest.sprite = team.TeamCrestSprite;
        currentKit = team.Kit;
        SetFormation(team.GetFormation(battleType));
    }

    public void SetFormation(Formation formation, bool showDefaultSelected = true)
    {
        currentFormation = formation;
        this.showDefaultSelected = showDefaultSelected;
        //formationNameLabel.text = currentFormation.FormationName;

        StopAllCoroutines();

        if (animateTransitions && fieldSlots.Count == currentFormation.FormationCoords.Count)
        {
            // TODO Same team with different formation
            AnimateToFormation();
        }
        else
        {
            // TODO Different team or same team with different battleType
            RebuildLayout();
        }
    }

    public void SetKit(Kit kit, bool showDefaultSelected = true)
    {
        currentKit = kit;
        this.showDefaultSelected = showDefaultSelected;

        StopAllCoroutines();

        RebuildLayout();
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
            {
                teamRoster[i].ApplyKit(currentKit, Variant.Home, coord.Position);
                slot.SetCharacter(teamRoster[i]);
            }

            fieldSlots.Add(slot);
        }

        // --- Bench Slots ---
        int benchCount = GetBenchCount();
        for (int i = 0; i < benchCount; i++)
        {
            int rosterIndex = fieldCount + i;
            if (rosterIndex >= teamRoster.Count) break;

            GameObject slotGO = Instantiate(playerSlotPrefab, benchArea);
            FormationCharacterSlotUI slot = slotGO.GetComponent<FormationCharacterSlotUI>();

            slot.SetAsBench(rosterIndex);
            teamRoster[rosterIndex].ApplyKit(currentKit, Variant.Home, teamRoster[rosterIndex].Position);
            slot.SetCharacter(teamRoster[rosterIndex]);

            benchSlots.Add(slot);
        }

        if (showDefaultSelected)
            UIEvents.RaiseFormationCharacterSlotUISelectedDefault(fieldSlots[fieldSlots.Count - 2]);
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

        if (rt == null) yield break;

        Vector2 start = rt.anchoredPosition;
        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            if (rt == null) yield break;

            elapsed += Time.deltaTime;
            float t = elapsed / transitionDuration;
            // Ease out cubic
            t = 1f - Mathf.Pow(1f - t, 3f);
            rt.anchoredPosition = Vector2.Lerp(start, target, t);
            yield return null;
        }

        if (rt != null)
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
        StopAllCoroutines();

        foreach (var s in fieldSlots) if (s != null) Destroy(s.gameObject);
        foreach (var s in benchSlots) if (s != null) Destroy(s.gameObject);
        fieldSlots.Clear();
        benchSlots.Clear();
    }

    #region Events

    // placeholder

    #endregion
    
}
