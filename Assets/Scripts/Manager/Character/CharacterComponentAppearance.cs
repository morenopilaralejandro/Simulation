using UnityEngine;
using System;
using System.Threading.Tasks;
using Simulation.Enums.Character;
using Simulation.Enums.Kit;
using Simulation.Enums.SpriteLayer;

public class CharacterComponentAppearance : MonoBehaviour
{
    [SerializeField] private SpriteLayerRendererCharacter spriteLayerRenderer;
  
    private Character character;
    private SpriteLayerState<CharacterSpriteLayer> state;
    private Sprite portraitSprite;
    private PortraitSize portraitSize;

    public SpriteLayerState<CharacterSpriteLayer> SpriteLayerState => state;
    public Sprite PortraitSprite => portraitSprite;
    public PortraitSize PortraitSize => portraitSize;
    public Color BodyColor => state.Colors[CharacterSpriteLayer.Body];

    public void Initialize(CharacterData characterData, Character character)
    {
        this.character = character;
        state = new SpriteLayerState<CharacterSpriteLayer>();
        InitializeVisibility();
        _ = InitializeAsync(characterData);
    }

    private async Task InitializeAsync(CharacterData characterData)
    {
        await LoadSprites(characterData);
        ApplyColors(characterData);
    }

    private async Task LoadSprites(CharacterData characterData)
    {
        state.Sprites[CharacterSpriteLayer.Hair] =
            await SpriteAtlasManager.Instance.GetCharacterHair(
                characterData.HairStyle.ToString().ToLower());

        portraitSprite =
            await SpriteAtlasManager.Instance.GetCharacterPortrait(
                characterData.CharacterId);
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
        if (this.character != character) return;

        if(formationCoord.Position == Position.GK) 
        {
            state.VisibleLayers.Remove(CharacterSpriteLayer.Gloves);
            spriteLayerRenderer.SetActive(CharacterSpriteLayer.Gloves, false);
        }
        ApplyKit(team, team.Kit);
        ApplyStateToRenderer();
    }

    private void ApplyColors(CharacterData characterData)
    {
        var hairColor = ColorManager.GetHairColor(characterData.HairColor);

        state.Colors[CharacterSpriteLayer.Hair] = hairColor;
        state.Colors[CharacterSpriteLayer.EyeIris] = ColorManager.GetEyeColor(characterData.EyeColor);
        state.Colors[CharacterSpriteLayer.Body] = ColorManager.GetBodyColor(characterData.BodyColor);
    }

    private void InitializeVisibility()
    {
        foreach (CharacterSpriteLayer layer in Enum.GetValues(typeof(CharacterSpriteLayer)))
            state.VisibleLayers.Add(layer);

        state.VisibleLayers.Remove(CharacterSpriteLayer.Armor);
    }

    private void ApplyStateToRenderer()
    {
        foreach (var (layer, sprite) in state.Sprites)
            spriteLayerRenderer.SetSprite(layer, sprite);

        foreach (var (layer, color) in state.Colors)
            spriteLayerRenderer.SetColor(layer, color);

        foreach (CharacterSpriteLayer layer in Enum.GetValues(typeof(CharacterSpriteLayer)))
            spriteLayerRenderer.SetVisible(layer, state.Contains(layer));
    }

    public void ApplyKit(Team team, Kit kit)
    {
        var kitColor = kit.GetColors(GetKitVariant(team), GetKitRole());

        state.Colors[CharacterSpriteLayer.KitBase] = kitColor.Base;
        state.Colors[CharacterSpriteLayer.KitDetail] = kitColor.Detail;
        state.Colors[CharacterSpriteLayer.KitShocks] = kitColor.Shocks;
    }

    public Role GetKitRole() 
    {
        return this.character.FormationCoord.Position == Position.GK ? Role.Keeper : Role.Field;
    }

    public Variant GetKitVariant(Team team) 
    {
        return team.Variant;
    }

    public void SetCharacterVisible(bool isVisible)
    {
        foreach (CharacterSpriteLayer layer in state.VisibleLayers)
            spriteLayerRenderer.SetVisible(layer, isVisible);
    }
}
