using UnityEngine;
using UnityEngine.Localization;
using Simulation.Enums.Character;
using Simulation.Enums.Localization;

public class CharacterComponentLocalization : MonoBehaviour
{
    [SerializeField] private LocalizedString localizedFullName;
    [SerializeField] private LocalizedString localizedNickName;
    [SerializeField] private LocalizedString localizedDescription;

    public string GetStringFullName() => localizedFullName.GetLocalizedString();
    public string GetStringNickName() => localizedNickName.GetLocalizedString();
    public string GetStringDescription() => localizedDescription.GetLocalizedString();

    public void Initialize(CharacterData characterData)
    {
        SetFullName(characterData.CharacterId);
        SetNickName(characterData.CharacterId);
        SetDescription(characterData.CharacterId);
    }

    private void SetFullName(string id)
    {
        localizedFullName = new LocalizedString(
            LocalizationManager.Instance.GetTableReference(LocalizationEntity.Character, LocalizationField.FullName),
            id
        );
    }

    private void SetNickName(string id)
    {
        localizedNickName = new LocalizedString(
            LocalizationManager.Instance.GetTableReference(LocalizationEntity.Character, LocalizationField.NickName),
            id
        );
    }

    private void SetDescription(string id)
    {
        localizedDescription = new LocalizedString(
            LocalizationManager.Instance.GetTableReference(LocalizationEntity.Character, LocalizationField.Description),
            id
        );
    }
}
