using UnityEngine;
using Aremoreno.Enums.Animation;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Kit;
using Aremoreno.Enums.World;

public class NpcEntityCharacter : NpcEntity
{
    #region Fields
    [SerializeField] private NpcType npcType = NpcType.Character;
    [SerializeField] private CharacterData characterData;
    [SerializeField] private KitData kitData;
    [SerializeField] private Variant variant;
    [SerializeField] private Role role;

    private CharacterComponentAppearance appearanceComponent;

    #endregion

    #region Components

    [SerializeField] private CharacterComponentAppearanceBattle appearanceComponentBattle;
    [SerializeField] private CharacterComponentAnimationController animationControllerComponent;
    [SerializeField] private YSort ySortComponent;
    [SerializeField] private NpcComponentInteractableDialog interactableDialogComponent;

    #endregion

    #region Initialize

    public void Start() 
    {
        Initialize(characterData);
    }

    public void Initialize(CharacterData characterData)
    {
        base.SetNpc(new Npc(null, characterData, npcType));

        appearanceComponent = new CharacterComponentAppearance(characterData, null, null);
        appearanceComponentBattle.Initialize(appearanceComponent);
        appearanceComponent.SetKit(KitManager.Instance.GetKit(kitData.KitId), variant, role);
        _ = appearanceComponentBattle.LoadKitAsync();

        interactableDialogComponent?.Initialize(this);
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
