using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using Aremoreno.Enums.Character;

public class SelectorCharacterListItem : MonoBehaviour
{
    #region Fields

    [Header("UI Elements")]
    [SerializeField] private Button button;
    [SerializeField] private CharacterCard characterCard;
    [SerializeField] private BarHPSP barHp;
    [SerializeField] private BarHPSP barSp;
    [SerializeField] private TMP_Text textLv;

    private Character character;

    public Button Button => button;

    #endregion

    #region Lifecycle

    private void Awake()
    {

    }

    #endregion

    #region Initialize

    public void Initialize(Character character)
    {
        this.character = character;
        characterCard.SetCharacter(character, character.Position);
        barHp.SetCharacter(character, Stat.Hp);
        barSp.SetCharacter(character, Stat.Sp);
        textLv.text = $"{character.Level}";
    }

    public void Clear() 
    {
        characterCard.Clear();
        textLv.text = "";
    }

    #endregion

    #region Button

    public void OnListItemClicked() 
    {
        UIEvents.RaiseCharacterSelected(character);
    }

    #endregion

    #region Events
    
    /*

    private void OnEnable()
    {
        UIEvents.OnFormationCharacterSlotUIReplaced += HandleFormationCharacterSlotUIReplaced;
    }

    private void OnDisable()
    {
        UIEvents.OnFormationCharacterSlotUIReplaced -= HandleFormationCharacterSlotUIReplaced;
    }

    private void HandleFormationCharacterSlotUIReplaced(FormationCharacterSlotUI slot, Character character) 
    {
        if(this != slot) return;
        SetCharacter(character);
    }

    */

    #endregion
}
