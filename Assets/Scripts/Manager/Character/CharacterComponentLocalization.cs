using UnityEngine;
using UnityEngine.Localization;
using Simulation.Enums.Character;

public class CharacterComponentLocalization : MonoBehaviour
{
    [SerializeField] private CharacterComponentAttribute attributeComponent;
    [SerializeField] private LocalizedString localizedName;
    [SerializeField] private LocalizedString localizedDescription;
    [SerializeField] private bool isRomazed = false;
    [SerializeField] private string stringTableNameLocalized = "CharacterNamesLocalized";
    [SerializeField] private string stringTableNameRomanized = "CharacterNamesRomanized";
    [SerializeField] private string stringTableDescriptionLocalized = "CharacterDescriptionsLocalized";
    [SerializeField] private string stringTableDescriptionRomanized = "CharacterDescriptionsRomanized";

    public LocalizedString GetLocalizedName() => localizedName;
    public LocalizedString GetLocalizedDescription() => localizedDescription;

    /*
    public void Initialize(CharacterData characterData)
    {

    }
    */

    private void SetName()
    {
        localizedName = new LocalizedString(
            isRomazed ? stringTableNameRomanized : stringTableNameLocalized,
            attributeComponent.GetCharacterId()
        );
    }

    private void SetDescription()
    {
        localizedDescription = new LocalizedString(
            isRomazed ? stringTableDescriptionRomanized : stringTableDescriptionLocalized,
            attributeComponent.GetCharacterId()
        );
    }
}
