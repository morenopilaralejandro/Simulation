using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Simulation.Enums.Character;
using Simulation.Enums.SpriteLayer;
using Simulation.Enums.Kit;
using Simulation.Enums.Move;
using Simulation.Enums.Duel;
using Simulation.Enums.Battle;
using Simulation.Enums.Localization;
using Simulation.Enums.World;

public class PlayerWorldEntity : MonoBehaviour
{
    private static PlayerWorldEntity instance;

    #region Components

    [SerializeField] private Character character;
    [SerializeField] private PlayerWorldComponentAppearance appearanceComponent;
    [SerializeField] private PlayerWorldComponentController controllerComponent;
    [SerializeField] private PlayerWorldComponentInteraction interactionComponent;
    [SerializeField] private PlayerWorldComponentModel modelComponent;
    [SerializeField] private PlayerWorldComponentPersistence persistenceComponent;
    [SerializeField] private PlayerWorldComponentRigidbody rigidbodyComponent;
    [SerializeField] private PlayerWorldComponentStateMachine stateMachineComponent;

    #endregion

    #region Initialize

    public void Initialize(CharacterData characterData, Kit kit)
    {
        PlayerWorldConfig config = WorldManagerPlayer.Instance.PlayerWorldConfig;
        character = new Character(characterData);
        character.ApplyKit(kit, Variant.Home, Position.FW);

        appearanceComponent.Initialize(this);
        controllerComponent.Initialize(this, config);
        interactionComponent.Initialize(this, config);
        modelComponent.Initialize(this, config);
        persistenceComponent.Initialize(this, config);
        //rigidbodyComponent.Initialize(this, config);
        stateMachineComponent.Initialize(this, config);
    }

    #endregion

    #region API Character
    public Character Character => character;
    #endregion

    #region API PlayerWorldEntity
    //appearanceComponent
    //controllerComponent
    public bool IsMoving => controllerComponent.IsMoving;
    public Vector2 MoveInput => controllerComponent.MoveInput;
    public bool IsRunning => controllerComponent.IsRunning;
    public float DistanceTravelledSinceReset => controllerComponent.DistanceTravelledSinceReset;
    public void SetControlEnabled(bool value) => controllerComponent.SetControlEnabled(value);
    public void StopMovement() => controllerComponent.StopMovement();
    public void ResetDistance() => controllerComponent.ResetDistance();
    //interactionComponent
    //modelComponent
    public FacingDirection FacingDirection => modelComponent.FacingDirection;
    public void SetFacing(Vector2 input) => modelComponent.SetFacing(input);
    public void SetFacing(FacingDirection dir) => modelComponent.SetFacing(dir);
    //persistenceComponent
    public void MakePersistent() => persistenceComponent.MakePersistent();
    //rigidbodyComponent
    public Rigidbody2D Rb => rigidbodyComponent.Rb;
    public void Teleport(Vector3 position) => rigidbodyComponent.Teleport(position);
    //stateMachineComponent
    public PlayerWorldState PlayerWorldState => stateMachineComponent.PlayerWorldState;
    public void SetState(PlayerWorldState newState) => stateMachineComponent.SetState(newState);
    #endregion

}
