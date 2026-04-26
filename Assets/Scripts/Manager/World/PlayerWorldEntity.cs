using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.SpriteLayer;
using Aremoreno.Enums.Kit;
using Aremoreno.Enums.Move;
using Aremoreno.Enums.Duel;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Localization;
using Aremoreno.Enums.World;

public class PlayerWorldEntity : MonoBehaviour
{
    public static PlayerWorldEntity Instance;

    #region Components

    [SerializeField] private Character character;
    [SerializeField] private CharacterComponentAppearanceWorld appearanceComponent;
    [SerializeField] private PlayerWorldComponentController controllerComponent;
    [SerializeField] private PlayerWorldComponentInteraction interactionComponent;
    [SerializeField] private PlayerWorldComponentDialog dialogComponent;
    [SerializeField] private PlayerWorldComponentModel modelComponent;
    [SerializeField] private PlayerWorldComponentPersistence persistenceComponent;
    [SerializeField] private PlayerWorldComponentRigidbody rigidbodyComponent;
    [SerializeField] private PlayerWorldComponentStateMachine stateMachineComponent;

    [SerializeField] private GameObject modelObject;
    [SerializeField] private GameObject collidersObject;

    #endregion

    #region Initialize

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void Initialize(CharacterData characterData, Kit kit, PlayerWorldConfig config)
    {
        character = new Character(characterData);
        character.ApplyKit(kit, Variant.Home, Position.FW);

        appearanceComponent.Initialize(character.AppearanceComponent, this);
        controllerComponent.Initialize(this, config);
        interactionComponent.Initialize(this, config);
        dialogComponent.Initialize(this, config);
        SetDialogEnabled(false);
        modelComponent.Initialize(this, config);
        persistenceComponent.Initialize(this, config);
        rigidbodyComponent.Initialize(this);
        stateMachineComponent.Initialize(this, config);

        BattleEvents.OnBattleStart += HandleBattleStart;
        BattleEvents.OnBattleEnd += HandleBattleEnd;

        DialogEvents.OnDialogStarted += HandleDialogStarted;
        DialogEvents.OnDialogEnded += HandleDialogEnded;
    }

    private void OnDestroy()
    {
        BattleEvents.OnBattleStart -= HandleBattleStart;
        BattleEvents.OnBattleEnd -= HandleBattleEnd;

        DialogEvents.OnDialogStarted -= HandleDialogStarted;
        DialogEvents.OnDialogEnded -= HandleDialogEnded;
    }

    #endregion

    #region API Character

    public Character Character => character;

    #endregion

    #region API PlayerWorldEntity

    //appearanceComponent
    //controllerComponent
    private bool isControlEnabled;
    public bool IsControlEnabled => isControlEnabled;
    public void SetControlEnabled(bool enable) => isControlEnabled = enable;
    public bool IsMoving => controllerComponent.IsMoving;
    public Vector2 MoveInput => controllerComponent.MoveInput;
    public bool IsRunning => controllerComponent.IsRunning;
    public float DistanceTravelledSinceReset => controllerComponent.DistanceTravelledSinceReset;
    public void StopMovement() => controllerComponent.StopMovement();
    public void ResetDistance() => controllerComponent.ResetDistance();
    public Vector2 CurrentTilePosition => controllerComponent.CurrentTilePosition;
    public Vector3 CurrentTilePosition3d() => controllerComponent.CurrentTilePosition3d();
    //interactionComponent
    public Interactable CurrentInteractionTarget => interactionComponent.CurrentTarget;
    //dialogComponent
    public void SetDialogEnabled(bool enable) => dialogComponent.enabled = enable;
    //modelComponent
    public FacingDirection FacingDirection => modelComponent.FacingDirection;
    public void SetFacing(Vector2 input) => modelComponent.SetFacing(input);
    public void SetFacing(FacingDirection dir) => modelComponent.SetFacing(dir);
    public Vector3 VectorToFacing() => modelComponent.VectorToFacing();
    public Vector2 FacingToVector(FacingDirection dir) => modelComponent.FacingToVector(dir);
    //persistenceComponent
    public void MakePersistent() => persistenceComponent.MakePersistent();
    //rigidbodyComponent
    public Rigidbody2D Rb => rigidbodyComponent.Rb;
    public void Teleport(Vector3 position) => rigidbodyComponent.Teleport(position);
    public Vector2 GetPosition2d() => rigidbodyComponent.GetPosition2d();
    public Vector3 GetPosition3d() => rigidbodyComponent.GetPosition3d();
    //stateMachineComponent
    public PlayerWorldState PlayerWorldState => stateMachineComponent.PlayerWorldState;
    public void SetState(PlayerWorldState newState) => stateMachineComponent.SetState(newState);

    #endregion

    #region API Misc
    public bool CanInteract => PlayerWorldState == PlayerWorldState.FreeRoam && !IsMoving && IsControlEnabled;
    public bool CanOpenMenu => PlayerWorldState == PlayerWorldState.FreeRoam && !IsMoving && IsControlEnabled;
    #endregion

    #region Events

    private void SetEnable(bool enable) 
    {
        modelObject.SetActive(enable);
        collidersObject.SetActive(enable);
        controllerComponent.enabled = enable;
        interactionComponent.enabled = enable;
    }

    private void HandleBattleStart(BattleType battleType) => SetEnable(false);
    private void HandleBattleEnd() => SetEnable(true);

    private void HandleDialogStarted() => SetDialogEnabled(true);
    private void HandleDialogEnded() => SetDialogEnabled(false);

    #endregion
}
