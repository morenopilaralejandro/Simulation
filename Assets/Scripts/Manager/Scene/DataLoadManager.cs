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

        var databases = DatabaseManager.Instance.DatabaseRegistry;
        var deps = new DatabaseDependencies();

        // Formation dependencies
        deps.Register(databases.FormationData,
            databases.FormationCoordData);

        // Team dependencies
        deps.Register(databases.TeamData,
            databases.FormationData,
            databases.CharacterData,
            databases.KitData);

        // Move evolution dependencies
        deps.Register(databases.MoveEvolutionPath,
            databases.MoveData);

        deps.Register(databases.MoveEvolutionGrowthProfile,
            databases.MoveData);

        // Wing evolution dependencies
        deps.Register(databases.WingEvolutionPath,
            databases.WingData);

        deps.Register(databases.WingEvolutionGrowthProfile,
            databases.WingData);

        // Quest dependencies
        deps.Register(databases.QuestData,
            databases.QuestObjectiveData);

        // Story dependencies
        deps.Register(databases.StoryAutoTriggerData,
            databases.StoryEventData);

        deps.Register(databases.StoryChapterData,
            databases.StoryEventData);

        // -------------------------
        // LOAD EVERYTHING
        // -------------------------

        var loader = new DatabaseLoader(databases, deps);
        await loader.LoadAllAsync();

        IsReady = true;

        LogManager.Trace("[DataLoadManager] All data loaded.");
    }
}
