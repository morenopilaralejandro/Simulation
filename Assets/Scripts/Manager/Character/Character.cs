using UnityEngine;
using UnityEngine.Localization;
using Simulation.Enums.Character;

public class Character : MonoBehaviour
{
    #region Components
    [SerializeField] private string characterId;
    public string CharacterId => characterId;

    [SerializeField] private CharacterComponentAttribute attributeComponent;
    [SerializeField] private CharacterComponentLocalization localizationComponent;
    [SerializeField] private CharacterComponentTeamMember teamMemberComponent;
    #endregion



    /*
    character atrribute : element, ControlType; ... 
    character team member component
    character keeper component
    character control window
    character kick window
    [SerializeField] private CharacterStatsComponent stats;
    [SerializeField] private HealthComponent health;
    [SerializeField] private SpiritComponent spirit;

        character leveling
        character training

    [SerializeField] private CharacterStatusController status; //stun
    [SerializeField] private MovementComponent movement;
    [SerializeField] private CharacterSpeedComponent speedDebuff;
    [SerializeField] private CharacterAppearanceComponent appearance; //include PortraitSize BodyId
    [SerializeField] private SecretLearning secrets; //change to character move component
    [SerializeField] private CharacterPersistenceComponent persistence; //
    [SerializeField] private CharacterAiComponent ai; //ally and opponent
    */

    #region Initialize
    public void Initialize(CharacterData characterData, bool isSave = false)
    {
        attributeComponent.Initialize(characterData);
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
    //api-attributeComponent
    public string GetCharacterId() => attributeComponent.GetCharacterId();
    public CharacterSize GetCharacterSize() => attributeComponent.GetCharacterSize();
    public Gender GetGender() => attributeComponent.GetGender();
    public Element GetElement() => attributeComponent.GetElement();
    public Position GetPosition() => attributeComponent.GetPosition();
    public ControlType GetControlType() => attributeComponent.GetControlType();
    //api-localizationComponent
    public LocalizedString GetLocalizedName() => localizationComponent.GetLocalizedName();
    public LocalizedString GetLocalizedDescription() => localizationComponent.GetLocalizedDescription();
    //api-teamMemberComponent
    public int GetTeamIndex() => teamMemberComponent.GetTeamIndex();
    public FormationCoord GetFormationCoord() => teamMemberComponent.GetFormationCoord();



    //public void ApplyStun(float duration) => status.ApplyStun(duration);
    //public void ClearStun() => status.ClearStun();
    //public float GetSpeedPerSecond() => movement.GetSpeedPerSecond();
    #endregion

}
