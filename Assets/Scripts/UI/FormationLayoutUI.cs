using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Kit;
using Aremoreno.Enums.UI;

public class FormationLayoutUI : MonoBehaviour
{
    #region Field

    [Header("UI References")]
    [SerializeField] private FormationCharacterSlotPool pool;
    [SerializeField] private RectTransform fieldArea;
    [SerializeField] private RectTransform benchArea;
    [SerializeField] private RectTransform dragLayer;
    [SerializeField] private TMP_Text textTeamName;
    [SerializeField] private Image imageTeamCrest;
    [SerializeField] private bool canDrag = true;

    [Header("Pitch Padding")]
    [SerializeField] private float paddingX = 50f;
    [SerializeField] private float paddingY = 40f;

    private Formation currentFormation;
    private Kit currentKit;
    private Variant currentVariant;
    private bool showDefaultSelected = true;
    private MenuTeamMode menuTeamMode = MenuTeamMode.Edit;

    private List<FormationCharacterSlotUI> fieldSlots = new List<FormationCharacterSlotUI>();
    private List<FormationCharacterSlotUI> benchSlots = new List<FormationCharacterSlotUI>();

    private TeamManager teamManager;
    private List<Character> teamRoster;

    #endregion

    #region Lifecycle

    private void Start()
    {
        teamManager = TeamManager.Instance;
    }

    private void OnDestroy()
    {
        pool?.Clear();
    }

    #endregion

    #region Initialize

    public void Initialize(Team team, BattleType battleType, MenuTeamMode menuTeamMode, bool showDefaultSelected = true)
    {
        this.menuTeamMode = menuTeamMode;

        if (menuTeamMode == MenuTeamMode.Edit) 
            teamRoster = teamManager.ResolveCharactersFromStorage(team, battleType);
        else 
            teamRoster = teamManager.ResolveCharactersFromBattle(team, battleType);

        textTeamName.text = team.TeamName;
        imageTeamCrest.sprite = team.TeamCrestSprite;
        currentKit = team.Kit;
        currentVariant = team.Variant;
        this.showDefaultSelected = showDefaultSelected;
        currentFormation = team.GetFormation(battleType);

        RebuildLayout();
    }

    #endregion

    #region Public Setters

    public void SetFormation(Formation formation, bool showDefaultSelected = true)
    {
        currentFormation = formation;
        this.showDefaultSelected = showDefaultSelected;
        RebuildLayout();
    }

    public void SetKit(Kit kit, bool showDefaultSelected = true)
    {
        currentKit = kit;
        this.showDefaultSelected = showDefaultSelected;
        RebuildLayout();
    }

    #endregion

    #region Core Layout

    private void RebuildLayout()
    {
        bool isMini = currentFormation.BattleType == BattleType.Mini;
        int neededFieldCount = Mathf.Min(
            currentFormation.FormationCoords.Count, 
            teamRoster.Count
        );
        int neededBenchCount = GetBenchCount();

        // --- Reconcile Field Slots ---
        ReconcileSlots(
            fieldSlots, 
            neededFieldCount, 
            isBench: false
        );

        for (int i = 0; i < neededFieldCount; i++)
        {
            FormationCoord coord = currentFormation.FormationCoords[i];
            FormationCharacterSlotUI slot = fieldSlots[i];
            RectTransform rt = slot.GetComponent<RectTransform>();

            // Reposition
            rt.anchoredPosition = isMini
                ? FormationMapperUI.WorldToUIPositionMini(coord.DefaultPosition, fieldArea)
                : FormationMapperUI.WorldToUIPosition(
                    coord.DefaultPosition, fieldArea, paddingX, paddingY);

            // Re-initialize data
            slot.SetDragLayer(dragLayer);
            slot.Initialize(i, coord);
            slot.SetCanDrag(canDrag);
            teamRoster[i].ApplyKit(currentKit, currentVariant, coord.Position);
            slot.SetCharacter(teamRoster[i]);
        }

        // --- Reconcile Bench Slots ---
        ReconcileSlots(
            benchSlots, 
            neededBenchCount, 
            isBench: true
        );

        int fieldCount = currentFormation.FormationCoords.Count;
        for (int i = 0; i < neededBenchCount; i++)
        {
            int rosterIndex = fieldCount + i;
            if (rosterIndex >= teamRoster.Count) break;

            FormationCharacterSlotUI slot = benchSlots[i];
            slot.SetDragLayer(dragLayer);
            slot.SetAsBench(rosterIndex);
            slot.SetCanDrag(canDrag);
            teamRoster[rosterIndex].ApplyKit(
                currentKit, currentVariant, teamRoster[rosterIndex].Position);
            slot.SetCharacter(teamRoster[rosterIndex]);
        }

        // Default selection
        if (showDefaultSelected && fieldSlots.Count >= 2)
        {
            /*
            UIEvents.RaiseFormationCharacterSlotUISelectedDefault(
                fieldSlots[fieldSlots.Count - 2]);
            */

            UIEvents.RaiseFormationCharacterSlotUISelectedDefault(
                fieldSlots[0]);
        }
    }

    /// <summary>
    /// Grows or shrinks the slot list to match the needed count.
    /// Existing slots are kept and reused in place.
    /// </summary>
    private void ReconcileSlots(
        List<FormationCharacterSlotUI> slots, 
        int neededCount, 
        bool isBench)
    {
        // Too many → return extras to pool
        while (slots.Count > neededCount)
        {
            int last = slots.Count - 1;
            pool.Return(slots[last], isBench);
            slots.RemoveAt(last);
        }

        // Too few → get new ones from pool
        while (slots.Count < neededCount)
        {
            FormationCharacterSlotUI slot = pool.Get(isBench);
            slots.Add(slot);
        }
    }

    public void ReleaseAll()
    {
        // Return all field slots to the pool
        for (int i = fieldSlots.Count - 1; i >= 0; i--)
        {
            pool.Return(fieldSlots[i], isBench: false);
        }
        fieldSlots.Clear();

        // Return all bench slots to the pool
        for (int i = benchSlots.Count - 1; i >= 0; i--)
        {
            pool.Return(benchSlots[i], isBench: true);
        }
        benchSlots.Clear();

        // Null out references to allow GC collection
        teamRoster = null;
        currentFormation = null;
        currentKit = null;
        teamManager = null;

        // Clear UI references that hold asset data
        if (textTeamName != null) textTeamName.text = string.Empty;
        if (imageTeamCrest != null) imageTeamCrest.sprite = null;
    }

    #endregion

    #region Helpers

    private int GetBenchCount()
    {
        if (currentFormation.BattleType == BattleType.Mini) return 0;
        return Mathf.Max(0, teamRoster.Count - currentFormation.FormationCoords.Count);
    }

    #endregion

    #region Public Queries

    /// <summary>
    /// Searches both field and bench slots for a slot whose current character
    /// matches the given GUID. Returns null if no match is found.
    /// </summary>
    public FormationCharacterSlotUI FindSlotByCharacterGuid(string characterGuid)
    {
        if (string.IsNullOrEmpty(characterGuid)) return null;

        for (int i = 0; i < fieldSlots.Count; i++)
        {
            Character character = fieldSlots[i].GetCharacter();
            if (character != null && character.CharacterGuid == characterGuid)
                return fieldSlots[i];
        }

        for (int i = 0; i < benchSlots.Count; i++)
        {
            Character character = benchSlots[i].GetCharacter();
            if (character != null && character.CharacterGuid == characterGuid)
                return benchSlots[i];
        }

        return null;
    }

    #endregion
}
