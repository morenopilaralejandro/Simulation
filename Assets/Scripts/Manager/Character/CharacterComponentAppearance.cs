using UnityEngine;
using System.Threading.Tasks;
using Simulation.Enums.Character;
using Simulation.Enums.Kit;

public class CharacterComponentAppearance : MonoBehaviour
{
    #region SpriteRenderer
    [Header("SpriteRenderer")]
    [Header("Hair")]
    [SerializeField] private SpriteRenderer hairRenderer;
    [Header("Body")]
    [SerializeField] private SpriteRenderer bodyRenderer;
    [Header("Eye")]
    [SerializeField] private SpriteRenderer eyeBrowsRenderer;
    [SerializeField] private SpriteRenderer eyeIrisRenderer;
    [SerializeField] private SpriteRenderer eyeBaseRenderer;
    [Header("Kit")]
    [SerializeField] private SpriteRenderer kitDetailRenderer;
    [SerializeField] private SpriteRenderer kitBaseRenderer;
    [SerializeField] private SpriteRenderer kitShocksRenderer;
    [Header("Gloves")]
    [SerializeField] private SpriteRenderer glovesWristRenderer;
    [SerializeField] private SpriteRenderer glovesFingersRenderer;
    [Header("Spikes")]
    [SerializeField] private SpriteRenderer spikesLacesRenderer;
    [SerializeField] private SpriteRenderer spikesTongueRenderer;
    [SerializeField] private SpriteRenderer spikesSoleRenderer;
    #endregion

    #region Sprite
    [Header("Sprite")]
    [Header("Portrait")]
    [SerializeField] private Sprite portraitSprite;
    [Header("Hair")]
    [SerializeField] private Sprite hairSprite;
    [Header("Body")]
    [SerializeField] private Sprite bodySprite;
    [Header("Eye")]
    [SerializeField] private Sprite eyeBrowsSprite;
    [SerializeField] private Sprite eyeIrisSprite;
    [SerializeField] private Sprite eyeBaseSprite;
    [Header("Kit")]
    [SerializeField] private Sprite kitDetailSprite;
    [SerializeField] private Sprite kitBaseSprite;
    [SerializeField] private Sprite kitShocksSprite;
    [Header("Gloves")]
    [SerializeField] private Sprite glovesWristSprite;
    [SerializeField] private Sprite glovesFingersSprite;
    [Header("Spikes")]
    [SerializeField] private Sprite spikesLacesSprite;
    [SerializeField] private Sprite spikesTongueSprite;
    [SerializeField] private Sprite spikesSoleSprite;

    public Sprite PortraitSprite => portraitSprite;
    public Sprite HairSprite => hairSprite;
    public Sprite BodySprite => bodySprite;
    public Sprite EyeBrowsSprite => eyeBrowsSprite;
    public Sprite EyeIrisSprite => eyeIrisSprite;
    public Sprite EyeBaseSprite => eyeBaseSprite;
    public Sprite KitDetailSprite => kitDetailSprite;
    public Sprite KitBaseSprite => kitBaseSprite;
    public Sprite KitShocksSprite => kitShocksSprite;
    public Sprite GlovesWristSprite => glovesWristSprite;
    public Sprite GlovesFingersSprite => glovesFingersSprite;
    public Sprite SpikesLacesSprite => spikesLacesSprite;
    public Sprite SpikesTongueSprite => spikesTongueSprite;
    public Sprite SpikesSoleSprite => spikesSoleSprite;
    #endregion

    #region Color
    [Header("Color")]
    [Header("Hair")]
    [SerializeField] private Color hairColor;
    [Header("Body")]
    [SerializeField] private Color bodyColor;
    [Header("Eye")]
    [SerializeField] private Color eyeBrowsColor;
    [SerializeField] private Color eyeIrisColor;
    //[SerializeField] private Color eyeBaseColor;
    [Header("Kit")]
    [SerializeField] private Color kitDetailColor;
    [SerializeField] private Color kitBaseColor;
    [SerializeField] private Color kitShocksColor;
    [Header("Gloves")]
    [SerializeField] private Color glovesWristColor;
    [SerializeField] private Color glovesFingersColor;
    [Header("Spikes")]
    [SerializeField] private Color spikesLacesColor;
    [SerializeField] private Color spikesTongueColor;
    [SerializeField] private Color spikesSoleColor;

    public Color HairColor => hairColor;
    public Color BodyColor => bodyColor;
    public Color EyeBrowsColor => eyeBrowsColor;
    public Color EyeIrisColor => eyeIrisColor;
    //public Color EyeBaseColor => eyeBaseColor;
    public Color KitDetailColor => kitDetailColor;
    public Color KitBaseColor => kitBaseColor;
    public Color KitShocksColor => kitShocksColor;
    public Color GlovesWristColor => glovesWristColor;
    public Color GlovesFingersColor => glovesFingersColor;
    public Color SpikesLacesColor => spikesLacesColor;
    public Color SpikesTongueColor => spikesTongueColor;
    public Color SpikesSoleColor => spikesSoleColor;
    #endregion

    #region Internal
    private Character character;
    private PortraitSize portraitSize;

    public PortraitSize PortraitSize => portraitSize;
    #endregion

    public void Initialize(CharacterData characterData, Character character)
    {
        this.character = character;
        this.portraitSize = characterData.PortraitSize;
        _ = InitializeAsync(characterData);
    }

    private void OnDestroy()
    {

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
        SetKitColor(team, team.Kit);
        SetGlovesVisible(formationCoord.Position == Position.GK);
    }

    private async Task InitializeAsync(CharacterData characterData)
    {
        portraitSprite = await SpriteAtlasManager.Instance.GetCharacterPortrait(characterData.CharacterId);
        if (characterData.HairStyle == HairStyle.Bald) 
            hairSprite = null;
        else
            hairSprite = await SpriteAtlasManager.Instance.GetCharacterHair(EnumManager.EnumToString(characterData.HairStyle).ToLower());

        hairRenderer.sprite = hairSprite;

        SetBodyColor(characterData);
        SetHairColor(characterData);
        SetEyeColor(characterData);
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
        SetHairVisible(isVisible);
        SetBodyVisible(isVisible);
        SetFaceVisible(isVisible);
        SetKitVisible(isVisible);
        SetSpikesVisible(isVisible);
        if(character.IsKeeper)
            SetGlovesVisible(isVisible);
    }

    private void SetGlovesVisible(bool isVisible)
    {
        glovesWristRenderer.enabled = isVisible;
        glovesFingersRenderer.enabled = isVisible;
    }

    private void SetSpikesVisible(bool isVisible)
    {
        spikesLacesRenderer.enabled = isVisible;
        spikesTongueRenderer.enabled = isVisible;
        spikesSoleRenderer.enabled = isVisible;
    }

    private void SetFaceVisible(bool isVisible)
    {
        SetEyeVisible(isVisible);
    }

    private void SetEyeVisible(bool isVisible)
    {
        eyeBaseRenderer.enabled = isVisible;
        eyeBrowsRenderer.enabled = isVisible;
        eyeIrisRenderer.enabled = isVisible;
    }

    private void SetKitVisible(bool isVisible)
    {
        kitBaseRenderer.enabled = isVisible;
        kitDetailRenderer.enabled = isVisible;
        kitShocksRenderer.enabled = isVisible;
    }

    private void SetBodyVisible(bool isVisible)
    {
        bodyRenderer.enabled = isVisible;
    }

    private void SetHairVisible(bool isVisible)
    {
        hairRenderer.enabled = isVisible;
    }


    private void SetGlovesColor()
    {
        //glovesWristColor
        //glovesFingersColor

        glovesFingersRenderer.color = glovesWristColor;
        glovesFingersRenderer.color = glovesFingersColor;
    }

    private void SetSpikesColor()
    {
        //spikesLacesColor
        //spikesTongueColor
        //spikesSoleColor

        spikesLacesRenderer.color = spikesLacesColor;
        spikesTongueRenderer.color = spikesTongueColor;
        spikesSoleRenderer.color = spikesSoleColor;
    }

    private void SetBodyColor(CharacterData characterData)
    {
        bodyColor = ColorManager.GetBodyColor(characterData.BodyColor);

        bodyRenderer.color = bodyColor;
    }

    private void SetKitColor(Team team, Kit kit) 
    {
        var kitColor = kit.GetColors(GetKitVariant(team), GetKitRole());

        kitBaseColor = kitColor.Base;
        kitDetailColor = kitColor.Detail;
        kitShocksColor = kitColor.Shocks;

        kitBaseRenderer.color = kitBaseColor;
        kitDetailRenderer.color = kitDetailColor;
        kitShocksRenderer.color = kitShocksColor;
    }

    private void SetHairColor(CharacterData characterData)
    {
        hairColor = ColorManager.GetHairColor(characterData.HairColor);
        eyeBrowsColor = hairColor;
        hairRenderer.color = hairColor;
        eyeBrowsRenderer.color = eyeBrowsColor;
    }

    private void SetEyeColor(CharacterData characterData)
    {
        eyeIrisColor = ColorManager.GetEyeColor(characterData.EyeColor);
        eyeIrisRenderer.color = eyeIrisColor;        
    }

}
