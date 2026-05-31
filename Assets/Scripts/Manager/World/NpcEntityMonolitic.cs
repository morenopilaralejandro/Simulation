using UnityEngine;
using Aremoreno.Enums.Animation;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Kit;
using Aremoreno.Enums.World;

public class NpcEntityMonolitic : NpcEntity
{
    #region Fields

    [SerializeField] private NpcType npcType = NpcType.Monolitic;
    [SerializeField] private NpcData npcData;

    // npc appearance private CharacterComponentAppearance appearanceComponent;

    #endregion

    #region Components

    [SerializeField] private CharacterComponentAnimationController animationControllerComponent;
    [SerializeField] private YSort ySortComponent;
    [SerializeField] private NpcComponentInteractableDialog interactableDialogComponent;

    #endregion

    #region Initialize

    public void Start() 
    {
        Initialize(npcData);
    }

    public void Initialize(NpcData npcData)
    {
        base.SetNpc(new Npc(npcData, null, npcType));

        interactableDialogComponent?.Initialize(this);
        /*
        appearanceComponent.Initialize(characterData, null, null);
        appearanceComponent.SetKit(KitManager.Instance.GetKit(kitData.KitId), variant, role);
        _ = appearanceComponentBattle.LoadKitAsync();
        */
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

    //public Sprite PortraitSize => appearanceComponent.PortraitSize;

    // animationControllerComponent
    public void Play(CharacterAnimationState state, CharacterDirection direction) => animationControllerComponent.Play(state, direction);
    public void RefreshAnimation() => animationControllerComponent.RefreshAnimation();

    #endregion

}
