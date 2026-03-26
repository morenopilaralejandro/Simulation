using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;

public class DataLoadManager : MonoBehaviour
{
    public static DataLoadManager Instance { get; private set; }

    public bool IsReady { get; private set; } = false;

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

    private async void Start()
    {
        LogManager.Trace("[DataLoadManager] Starting data initialization...");
        // Generic
        Task loadSpriteAtlas = SpriteAtlasManager.Instance.LoadAllSpriteAtlasAsync();
        Task loadFormationCoord = FormationCoordManager.Instance.LoadAllFormationCoordDataAsync();
        Task loadBalls = BallManager.Instance.LoadAllBallDataAsync();
        Task loadFields = FieldManager.Instance.LoadAllFieldDataAsync();
        Task loadMoves = MoveManager.Instance.LoadAllMoveDataAsync();
        Task loadMoveEvolutionGrowthProfile = MoveEvolutionGrowthProfileManager.Instance.LoadAllMoveEvolutionGrowthProfileAsync();
        Task loadMoveEvolutionPath =  MoveEvolutionPathManager.Instance.LoadAllMoveEvolutionPathAsync();
        Task loadCharacters = CharacterManager.Instance.LoadAllCharacterDataAsync();
        Task loadKits = KitManager.Instance.LoadAllKitsAsync();   
        Task loadScenes = SceneGroupRegistry.Instance.LoadAllSceneGroupAsync();
        Task loadNpcs = NpcManager.Instance.LoadAllNpcDataAsync();
        Task loadItems = ItemManager.Instance.LoadAllItemDataAsync();

        // SpriteAtlas
        await loadSpriteAtlas;
        // Formation     
        await loadFormationCoord;
        Task loadFormations = FormationManager.Instance.LoadAllFormationsAsync();
        // Team        
        await Task.WhenAll(
            loadKits, 
            loadCharacters,
            loadFormations);
        Task loadTeams = TeamManager.Instance.LoadAllTeamsAsync();
        // Misc
        await Task.WhenAll(
            loadBalls, 
            loadFields,
            loadMoves,
            loadMoveEvolutionGrowthProfile,
            loadMoveEvolutionPath,
            loadTeams, 
            loadScenes,
            loadNpcs,
            loadItems);








        //Quest
        Task loadQuestObjective = QuestObjectiveDatabase.Instance.LoadAllQuestObjectiveDataAsync();
        await loadQuestObjective;
        Task loadQuest = QuestDatabase.Instance.LoadAllQuestDataAsync();
        //Story
        Task loadStoryEvent = StoryEventDatabase.Instance.LoadAllStoryEventDataAsync();
        await loadStoryEvent;
        Task loadStoryAutoTrigger = StoryAutoTriggerDatabase.Instance.LoadAllStoryAutoTriggerDataAsync();
        Task loadStoryChapter = StoryChapterDatabase.Instance.LoadAllStoryChapterDataAsync();
        await Task.WhenAll(
            loadQuest,
            loadStoryAutoTrigger,
            loadStoryChapter);

        IsReady = true;
        LogManager.Trace("[DataLoadManager] All data loaded.");
    }
}
