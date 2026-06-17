public class DatabaseRegistry
{
    public Database<FormationCoordData> FormationCoordData;
    public Database<BallData> BallData;
    public Database<FieldData> FieldData;

    public Database<MoveData> MoveData;
    public Database<MoveEvolutionGrowthProfile> MoveEvolutionGrowthProfile;
    public Database<MoveEvolutionPath> MoveEvolutionPath;

    public Database<WingData> WingData;
    public Database<WingEvolutionGrowthProfile> WingEvolutionGrowthProfile;
    public Database<WingEvolutionPath> WingEvolutionPath;

    public Database<CharacterData> CharacterData;
    public Database<KitData> KitData;
    public Database<EmblemData> EmblemData;

    public Database<SceneGroup> SceneGroup;
    public Database<NpcData> NpcData;
    public Database<ItemData> ItemData;
    public Database<OverworldDefinition> OverworldDefinition;

    public Database<FormationData> FormationData;
    public Database<TeamData> TeamData;

    public Database<QuestObjectiveData> QuestObjectiveData;
    public Database<QuestData> QuestData;

    public Database<StoryEventData> StoryEventData;
    public Database<StoryAutoTriggerData> StoryAutoTriggerData;
    public Database<StoryChapterData> StoryChapterData;

    public DatabaseRegistry()
    {
        // Formation & Field
        FormationCoordData = new Database<FormationCoordData>("FormationCoords-Data", f => f.FormationCoordId);
        BallData = new Database<BallData>("Balls-Data", b => b.BallId);
        FieldData = new Database<FieldData>("Fields-Data", f => f.FieldId);

        // Moves
        MoveData = new Database<MoveData>("Moves-Data", m => m.MoveId);
        MoveEvolutionGrowthProfile = new Database<MoveEvolutionGrowthProfile>("Moves-Evolutions-Growth", g => $"{g.growthType}_{g.growthRate}"); //compound key
        MoveEvolutionPath = new Database<MoveEvolutionPath>("Moves-Evolutions-Path", p => p.growthType.ToString());

        // Wings
        WingData = new Database<WingData>("Wing-Data", w => w.WingId);
        WingEvolutionGrowthProfile = new Database<WingEvolutionGrowthProfile>("Wing-Evolution-Growth", g => $"{g.growthType}_{g.growthRate}");
        WingEvolutionPath = new Database<WingEvolutionPath>("Wing-Evolution-Path", p => p.growthType.ToString());

        // Characters & Equipment
        CharacterData = new Database<CharacterData>("Characters-Data", c => c.CharacterId);
        KitData = new Database<KitData>("Kits-Data", k => k.KitId);
        EmblemData = new Database<EmblemData>("Emblems-Data", e => e.EmblemId);

        // World & NPCs
        SceneGroup = new Database<SceneGroup>("SceneGroup-Data", s => s.groupName);
        NpcData = new Database<NpcData>("Npcs-Data", n => n.NpcId);
        ItemData = new Database<ItemData>("Items-Data", i => i.ItemId);
        OverworldDefinition = new Database<OverworldDefinition>("OverworldDefinition-Data", o => o.Realm.ToString());

        // Teams & Formations
        FormationData = new Database<FormationData>("Formations-Data", f => f.FormationId);
        TeamData = new Database<TeamData>("Teams-Data", t => t.TeamId);

        // Quests
        QuestObjectiveData = new Database<QuestObjectiveData>("QuestObjective-Data", q => q.QuestObjectiveId);
        QuestData = new Database<QuestData>("Quest-Data", q => q.QuestId);

        // Story
        StoryEventData = new Database<StoryEventData>("StoryEvent-Data", s => s.StoryEventId);
        StoryAutoTriggerData = new Database<StoryAutoTriggerData>("StoryAutoTrigger-Data", s => s.StoryAutoTriggerId);
        StoryChapterData = new Database<StoryChapterData>("StoryChapter-Data", s => s.StoryChapterNumber.ToString());
    }
}
