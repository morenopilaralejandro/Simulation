using UnityEngine;
using System.Collections.Generic;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Wing;

public class WingManager : MonoBehaviour
{
    public static WingManager Instance { get; private set; }

    #region Fields

    private WingManagerStorage storageSystem;
    private WingManagerEquip equipSystem;
    //inheritance system
    private WingManagerPersistance persistanceSystem;

    #endregion

    #region Lifecycle

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

    private void OnDestroy() 
    {
        //encounterSystem.Unsubscribe();
    }


    private void Start()
    {
        storageSystem = new WingManagerStorage();
        equipSystem = new WingManagerEquip();
        persistanceSystem = new WingManagerPersistance();
        
        //encounterSystem.Subscribe();
        //InitializeAsync();
    }

    //private async void InitializeAsync() { }

    #endregion

    #region API

    // storageSystem
    public IReadOnlyDictionary<string, Wing> Wings => storageSystem.Wings;
    public int Count => storageSystem.Count;
    public void FirstTimeInitialize() => storageSystem.FirstTimeInitialize();
    public Wing AddWing(Wing wing) => storageSystem.AddWing(wing);
    public Wing AddWing(WingData wingData) => storageSystem.AddWing(wingData);
    public bool RemoveWing(string wingGuid) => storageSystem.RemoveWing(wingGuid);
    public Wing GetWing(string wingGuid) => storageSystem.GetWing(wingGuid);
    public bool HasWing(string wingGuid) => storageSystem.HasWing(wingGuid);
    public List<Wing> GetAllWings() => storageSystem.GetAllWings();
    public List<Wing> GetWingsByElement(Element element) => storageSystem.GetWingsByElement(element);
    public List<Wing> GetWingsByElementMatchingInheritance(Element[] elements) => storageSystem.GetWingsByElementMatchingInheritance(elements);
    public WingStorageSaveData ExportStorageSystem() => storageSystem.Export();
    public void ImportStorageSystem(WingStorageSaveData saveData) => storageSystem.Import(saveData);
    public void Clear() => storageSystem.Clear();

    //equipSystem
    public void EquipWing(Character character, Wing wing) => equipSystem.EquipWing(character, wing);
    public void UnequipWing(Character character) => equipSystem.UnequipWing(character);
    public void UnequipWing(Wing wing) => equipSystem.UnequipWing(wing);

    // persistanceSystem
    public WingSystemSaveData Export() => persistanceSystem.Export();
    public void Import(WingSystemSaveData saveData) => persistanceSystem.Import(saveData);

    #endregion

    /*

    #region Event

    //TODO placeholder

    private void OnEnable() 
    {
        MoveEvents.OnMoveUsed += HandleMoveUsed;
    }

    private void OnDisable() 
    {
        MoveEvents.OnMoveUsed -= HandleMoveUsed;
    }

    private void HandleMoveUsed(Move move, CharacterEntityBattle character)
    {
        if(!HasCharacter(character.Character.CharacterGuid)) return;

        move.ProgressEvolution();
        LogManager.Trace($"[CharacterStorage] {character.Character.CharacterName} used {move.MoveName}");
    }

    #endregion

    */
}
