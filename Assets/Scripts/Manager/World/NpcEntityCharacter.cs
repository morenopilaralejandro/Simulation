using UnityEngine;
using System.Threading.Tasks;
using Aremoreno.Enums.Animation;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Kit;
using Aremoreno.Enums.World;

public class NpcEntityCharacter : NpcEntity, IAsyncSceneLoader
{
    #region Fields
    [SerializeField] private NpcType npcType = NpcType.Character;
    [SerializeField] private CharacterData characterData;
    [SerializeField] private KitData kitData;
    [SerializeField] private Variant variant;
    [SerializeField] private Role role;
    [SerializeField] private CharacterDirection defaultFacingDirection = CharacterDirection.Down;

    private CharacterComponentAppearance appearanceComponent;

    #endregion

    #region Components

    [SerializeField] private CharacterComponentAppearanceBattle appearanceComponentBattle;
    [SerializeField] private CharacterComponentAnimationController animationControllerComponent;
    [SerializeField] private YSort ySortComponent;
    [SerializeField] private NpcComponentInteractableDialog interactableDialogComponent;

    #endregion

    #region Initialize

    public void Awake() 
    {
        animationControllerComponent.RefreshAnimation();
        Play(CharacterAnimationState.Idle, defaultFacingDirection);
    }

    public async Task LoadAsync()
    {
        Initialize(characterData);
        await appearanceComponentBattle.LoadKitAsync();
    }

    public void Initialize(CharacterData characterData)
    {
        base.SetNpc(new Npc(null, characterData, npcType));

        appearanceComponent = new CharacterComponentAppearance(characterData, null, null);
        appearanceComponentBattle.Initialize(appearanceComponent);
        appearanceComponent.SetKit(DatabaseManager.Instance.GetKit(kitData.KitId), variant, role);

        interactableDialogComponent?.Initialize(this);

        animationControllerComponent.RefreshAnimation();
        Play(CharacterAnimationState.Idle, defaultFacingDirection);
    }

    #endregion

    #region Update

    private void LateUpdate()
    {
        ySortComponent.OnLateUpdate();
        animationControllerComponent.OnLateUpdate();
    }

    #endregion

    #region Methods

    public override void FacePlayer()
    {
        CharacterDirection direction = WorldManager.Instance.PlayerWorldEntity.GetOppositeFacingDirection();
        SetFacing(direction);
        Play(CharacterAnimationState.Idle, direction);
        animationControllerComponent.RefreshAnimation();
    }

    #endregion

    #region API
    // appearanceComponent
    public PortraitSize PortraitSize => appearanceComponent.PortraitSize;

    // animationControllerComponent
    public void Play(CharacterAnimationState state, CharacterDirection direction) => animationControllerComponent.Play(state, direction);
    public void RefreshAnimation() => animationControllerComponent.RefreshAnimation();

    #endregion

}
