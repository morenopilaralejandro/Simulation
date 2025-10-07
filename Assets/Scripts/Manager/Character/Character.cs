using UnityEngine;
using Simulation.Enums.Character;
using Simulation.Enums.Localization;

public class Character : MonoBehaviour
{
    #region Components
    [SerializeField] private CharacterComponentAttributes attributesComponent;
    [SerializeField] private LocalizationComponentString localizationStringComponent;
    [SerializeField] private CharacterComponentTeamMember teamMemberComponent;
    [SerializeField] private CharacterComponentKeeper keeperComponent;
    #endregion

    /*
    [SerializeField] private CharacterStatsComponent stats;
    [SerializeField] private HealthComponent health;
    [SerializeField] private SpiritComponent spirit;

        character leveling
        character training

    character control window
    character kick window
    [SerializeField] private CharacterStatusController status; //stun
    [SerializeField] private CharacterSpeedComponent speedDebuff;
    [SerializeField] private MovementComponent movement;
    [SerializeField] private CharacterAppearanceComponent appearance; //include PortraitSize BodyId
    [SerializeField] private SecretLearning secrets; //change to character move component
    [SerializeField] private CharacterPersistenceComponent persistence; //
    [SerializeField] private CharacterAiComponent ai; //ally and opponent
    */

    #region Initialize
    public void Initialize(CharacterData characterData, bool isSave = false)
    {
        BindComponents();

        attributesComponent.Initialize(characterData);
        localizationStringComponent = new LocalizationComponentString(
            LocalizationEntity.Character,
            characterData.CharacterId,
            new [] { LocalizationField.Name, LocalizationField.Nick, LocalizationField.Description }            
        );

        /*
        stats.Initialize(def);
        health.Initialize(stats);
        spirit.Initialize(stats);
        secrets.Initialize(def);
        appearance.Initialize(def);
        namePresenter.Initialize(def.CharacterId);

        if (isSave)
            persistence.Apply(save);
        else
            stats.ApplyLevel(def.StartingLevel);        
        
        health.OnHpChanged += hp =>
        {
            speedDebuff.OnHpChanged(hp);
        };
        */
    }

    private void BindComponents()
    {
        teamMemberComponent.SetCharacter(this);
        keeperComponent.SetCharacter(this);
    }
    #endregion

    #region API
    //attributeComponent
    public string CharacterId => attributesComponent.CharacterId;
    public CharacterSize CharacterSize => attributesComponent.CharacterSize;
    public Gender Gender => attributesComponent.Gender;
    public Element Element => attributesComponent.Element;
    public Position Position => attributesComponent.Position;
    //localizationComponent
    public string CharacterName => localizationStringComponent.GetString(LocalizationField.Name);
    public string CharacterNick => localizationStringComponent.GetString(LocalizationField.Nick);
    public string CharacterDescription => localizationStringComponent.GetString(LocalizationField.Description);
    //teamMemberComponent
    public TeamSide TeamSide => teamMemberComponent.TeamSide;
    public FormationCoord FormationCoord => teamMemberComponent.FormationCoord;
    //keeperComponent
    public bool IsKeeper => keeperComponent.IsKeeper;
    

    //public void ApplyStun(float duration) => status.ApplyStun(duration);
    //public void ClearStun() => status.ClearStun();
    //public float GetSpeedPerSecond() => movement.GetSpeedPerSecond();
    #endregion

}
