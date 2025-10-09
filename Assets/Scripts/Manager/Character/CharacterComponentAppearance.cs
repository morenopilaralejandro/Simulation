using UnityEngine;
using System.Threading.Tasks;
using Simulation.Enums.Character;
using Simulation.Enums.Kit;

public class CharacterComponentAppearance : MonoBehaviour
{
    #region SpriteRenderer
    [SerializeField] private SpriteRenderer characterHeadRenderer;    //inspector
    [SerializeField] private SpriteRenderer characterBodyRenderer;    //inspector
    [SerializeField] private SpriteRenderer kitBodyRenderer;          //inspector

    public SpriteRenderer CharacterHeadRenderer => characterHeadRenderer;
    public SpriteRenderer CharacterBodyRenderer => characterBodyRenderer;
    public SpriteRenderer KitBodyRenderer => kitBodyRenderer;
    #endregion

    #region Sprite
    [SerializeField] private Sprite characterHeadSprite;
    [SerializeField] private Sprite characterBodySprite;
    [SerializeField] private Sprite characterPortraitSprite;
    [SerializeField] private Sprite kitBodySprite;
    [SerializeField] private Sprite kitPortraitSprite;

    public Sprite CharacterHeadSprite => characterHeadSprite;
    public Sprite CharacterBodySprite => characterBodySprite;
    public Sprite CharacterPortraitSprite => characterPortraitSprite;
    public Sprite KitBodySprite => kitBodySprite;
    public Sprite KitPortraitSprite => kitPortraitSprite;
    #endregion

    #region Address
    private string _characterHeadAddress;
    private string _characterBodyAddress;
    private string _characterPortraitAddress;
    private string _kitBodyAddress;
    private string _kitPortraitAddress;
    #endregion

    #region Internal
    private Character character;
    private PortraitSize portraitSize;
    #endregion

    public void Initialize(CharacterData characterData, Character character)
    {
        this.character = character;
        this.portraitSize = characterData.PortraitSize;
        _ = InitializeAsync(characterData);
    }

    private void OnDestroy()
    {
        if (!string.IsNullOrEmpty(_characterHeadAddress)) AddressableLoader.Release(_characterHeadAddress);
        if (!string.IsNullOrEmpty(_characterBodyAddress)) AddressableLoader.Release(_characterBodyAddress);
        if (!string.IsNullOrEmpty(_characterPortraitAddress)) AddressableLoader.Release(_characterPortraitAddress);
        if (!string.IsNullOrEmpty(_kitBodyAddress)) AddressableLoader.Release(_kitBodyAddress);
        if (!string.IsNullOrEmpty(_kitPortraitAddress)) AddressableLoader.Release(_kitPortraitAddress);
    }

    private void OnEnable()
    {
        TeamEvents.OnAssignCharacterToTeamBattle += HandleAssignCharacterToTeamBattle;    
    }

    private void OnDisable()
    {
        TeamEvents.OnAssignCharacterToTeamBattle -= HandleAssignCharacterToTeamBattle;
    }

    private void HandleAssignCharacterToTeamBattle(
        Character character, 
        Team team, 
        FormationCoord formationCoord)
    {
        if (this.character == character)
            _ = SetKitAsync(team, team.Kit);
    }

    private async Task InitializeAsync(CharacterData characterData)
    {
        _characterHeadAddress = AddressableLoader.GetCharacterHeadAddress(characterData.CharacterId);
        characterHeadSprite = await AddressableLoader.LoadAsync<Sprite>(_characterHeadAddress);
        if (characterHeadSprite) 
            characterHeadRenderer.sprite = characterHeadSprite;

        _characterBodyAddress = AddressableLoader.GetCharacterBodyAddress(characterData.BodyTone.ToString().ToLower());
        characterBodySprite = await AddressableLoader.LoadAsync<Sprite>(_characterBodyAddress);
        if (characterBodySprite)
            characterBodyRenderer.sprite = characterBodySprite;

        _characterPortraitAddress = AddressableLoader.GetCharacterHeadAddress(characterData.CharacterId);
        characterPortraitSprite = await AddressableLoader.LoadAsync<Sprite>(_characterPortraitAddress);
    }

    private async Task SetKitAsync(Team team, Kit kit)
    {
        Role role = GetRole();
        Variant variant = GetVariant(team);

        _kitBodyAddress = AddressableLoader.GetKitBodyAddress(kit.KitId, variant.ToString(), role.ToString());
        kitBodySprite = await AddressableLoader.LoadAsync<Sprite>(_kitBodyAddress);
        if (kitBodySprite)
            kitBodyRenderer.sprite = kitBodySprite;

        _kitPortraitAddress = AddressableLoader.GetKitPortraitAddress(kit.KitId, variant.ToString(), role.ToString(), portraitSize.ToString());
        kitPortraitSprite = await AddressableLoader.LoadAsync<Sprite>(_kitPortraitAddress);
    }

    private Role GetRole() 
    {
        return this.character.Position == Position.GK ? Role.Keeper : Role.Field;
    }

    private Variant GetVariant(Team team) 
    {
        return team.Variant;
    }

    public void SetRenderersVisible(bool isVisible)
    {
        characterHeadRenderer.enabled = isVisible;
        characterBodyRenderer.enabled = isVisible;
        kitBodyRenderer.enabled = isVisible;
    }

}
