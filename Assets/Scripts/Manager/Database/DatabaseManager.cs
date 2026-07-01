using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;
using Aremoreno.Enums.World;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager Instance { get; private set; }

    public DatabaseRegistry DatabaseRegistry = new DatabaseRegistry();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    #region API

    public CharacterData GetCharacterData(string id) => DatabaseRegistry.CharacterData.Get(id);
    public BallData GetBallData(string id) => DatabaseRegistry.BallData.Get(id);

    public WingData GetWingData(string id) => DatabaseRegistry.WingData.Get(id);
    public WingEvolutionGrowthProfile GetWingEvolutionGrowthProfile(string id) => DatabaseRegistry.WingEvolutionGrowthProfile.Get(id);
    public WingEvolutionPath GetWingEvolutionPath(string id) => DatabaseRegistry.WingEvolutionPath.Get(id);

    public TeamData GetTeamData(string id) => DatabaseRegistry.TeamData.Get(id);
    public Team GetTeam(string id) 
    {
        var data = GetTeamData(id);
        return TeamFactory.Create(data);
    }

    public StoryEventData GetStoryEventData(string id) => DatabaseRegistry.StoryEventData.Get(id);
    public StoryChapterData GetStoryChapterData(string id) => DatabaseRegistry.StoryChapterData.Get(id);
    public StoryChapter GetStoryChapter(string id) 
    {
        var data = GetStoryChapterData(id);
        return new StoryChapter(data);
    }
    public StoryAutoTriggerData GetStoryAutoTriggerData(string id) => DatabaseRegistry.StoryAutoTriggerData.Get(id);

    public QuestObjectiveData GetQuestObjectiveData(string id) => DatabaseRegistry.QuestObjectiveData.Get(id);
    public QuestData GetQuestData(string id) => DatabaseRegistry.QuestData.Get(id);
    public OverworldDefinition GetOverworldDefinitionByRealm(Realm realm) => DatabaseRegistry.OverworldDefinition.Get(realm.ToString());

    public ItemData GetItemData(string id) => DatabaseRegistry.ItemData.Get(id);
    public T GetItemData<T>(string id) where T : ItemData
    {
        var data = GetItemData(id);
        if (data == null) return null;

        if (data is T typed) return typed;

        LogManager.Error($"[DatabaseManager] Item '{id}' is {data.GetType().Name}, not {typeof(T).Name}.");
        return null;
    }

    public EmblemData GetEmblemData(string id) => DatabaseRegistry.EmblemData.Get(id);
    public Emblem GetEmblem(string id) 
    {
        var data = GetEmblemData(id);
        return new Emblem(data);
    }

    public FieldData GetFieldData(string id) => DatabaseRegistry.FieldData.Get(id);

    public FormationCoordData GetFormationCoordData(string id) => DatabaseRegistry.FormationCoordData.Get(id);
    public FormationData GetFormationData(string id) => DatabaseRegistry.FormationData.Get(id);
    public Formation GetFormation(string id) 
    {
        var data = GetFormationData(id);
        return new Formation(data);
    }

    public KitData GetKitData(string id) => DatabaseRegistry.KitData.Get(id);
    public Kit GetKit(string id) 
    {
        var data = GetKitData(id);
        return new Kit(data);
    }

    public MoveEvolutionGrowthProfile GetMoveEvolutionGrowthProfile(string id) => DatabaseRegistry.MoveEvolutionGrowthProfile.Get(id);
    public MoveEvolutionPath GetMoveEvolutionPath(string id) => DatabaseRegistry.MoveEvolutionPath.Get(id);
    public MoveData GetMoveData(string id) => DatabaseRegistry.MoveData.Get(id);

    public NpcData GetNpcData(string id) => DatabaseRegistry.NpcData.Get(id);

    public MatchData GetMatchData(string id) => DatabaseRegistry.MatchData.Get(id);
    public MatchChainData GetMatchChainData(string id) => DatabaseRegistry.MatchChainData.Get(id);
    public MatchChainNodeData GetMatchChainNodeData(string id) => DatabaseRegistry.MatchChainNodeData.Get(id);
    public T GetMatchChainNodeData<T>(string id) where T : MatchChainNodeData
    {
        var data = GetMatchChainNodeData(id);
        if (data == null) return null;

        if (data is T typed) return typed;

        LogManager.Error($"[DatabaseManager] MatchChainNodeData '{id}' is {data.GetType().Name}, not {typeof(T).Name}.");
        return null;
    }

    #endregion
}
