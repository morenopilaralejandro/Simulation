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

    //[SerializeField] private PlayerWorldAppearanceComponent appearanceComponent;
    [SerializeField] private PlayerWorldControllerComponent controllerComponent;
    [SerializeField] private PlayerWorldInteractionComponent interactionComponent;
    [SerializeField] private PlayerWorldModelComponent modelComponent;
    [SerializeField] private PlayerWorldPersistenceComponent persistenceComponent;
    [SerializeField] private PlayerWorldStateMachineComponent stateMachineComponent;
    #endregion

    #region Initialize
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Initialize(CharacterData characterData, Character character = null)
    {
        PlayerWorldConfig config = WorldManagerPlayer.Instance.PlayerWorldConfig;
        if (character != null)
        {
            this.character = character;
        } else
        {
            //this.character = new Character(characterData); from data
        }

        //appearanceBattleComponent.Initialize(this);
        controllerComponent.Initialize(this, config);
        controllerComponent.Initialize(this, config);
        interactionComponent.Initialize(this, config);
        modelComponent.Initialize(this, config);
        persistenceComponent.Initialize(this, config);
        stateMachineComponent.Initialize(this, config);
    }

    #endregion

    #region API Character
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
    public void TeleportPlayer(Vector3 destination) => modelComponent.TeleportPlayer(destination);
    public void SetFacing(Vector2 input) => modelComponent.SetFacing(input);
    public void SetFacing(FacingDirection dir) => modelComponent.SetFacing(dir);
    //persistenceComponent
    //stateMachineComponent
    public PlayerWorldState PlayerWorldState => stateMachineComponent.PlayerWorldState;
    public void SetState(PlayerWorldState newState) => stateMachineComponent.SetState(newState);
    #endregion

}
