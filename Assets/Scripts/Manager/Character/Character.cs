using UnityEngine;
using Simulation.Enums.Character;
using Simulation.Enums.Localization;

public class Character : MonoBehaviour
{
    #region Components
    [SerializeField] private CharacterComponentAttribute attributeComponent;
    [SerializeField] private ComponentLocalizationString stringLocalizationComponent;
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
        attributeComponent.Initialize(characterData);
        stringLocalizationComponent = new ComponentLocalizationString(
            LocalizationEntity.Character,
            characterData.CharacterId,
            new [] { LocalizationField.FullName, LocalizationField.NickName, LocalizationField.Description }            
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
    #endregion

    #region API
    //attributeComponent
    public string CharacterId() => attributeComponent.GetCharacterId();
    public CharacterSize CharacterSize => attributeComponent.GetCharacterSize();
    public Gender Gender => attributeComponent.GetGender();
    public Element Element => attributeComponent.GetElement();
    public Position Position => attributeComponent.GetPosition();
    //localizationComponent
    public string CharacterFullName => stringLocalizationComponent.GetString(LocalizationField.FullName);
    public string CharacterNickName => stringLocalizationComponent.GetString(LocalizationField.NickName);
    public string CharacterDescription => stringLocalizationComponent.GetString(LocalizationField.Description);
    //teamMemberComponent
    public int GetTeamIndex() => teamMemberComponent.GetTeamIndex();
    public FormationCoord GetFormationCoord() => teamMemberComponent.GetFormationCoord();
    public ControlType GetControlType() => teamMemberComponent.GetControlType();
    //keeperComponent
    public bool IsKeeper() => keeperComponent.IsKeeper();
    

    //public void ApplyStun(float duration) => status.ApplyStun(duration);
    //public void ClearStun() => status.ClearStun();
    //public float GetSpeedPerSecond() => movement.GetSpeedPerSecond();
    #endregion

}
