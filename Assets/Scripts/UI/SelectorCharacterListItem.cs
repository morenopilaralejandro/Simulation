using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.UI;

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
    private CharacterSelectorModeClick modeClick;

    public Button Button => button;
    public Character Character => character;

    #endregion

    #region Lifecycle

    private void Awake()
    {

    }

    #endregion

    #region Initialize

    public void Initialize(Character character, CharacterSelectorModeClick modeClick)
    {
        this.character = character;
        this.modeClick = modeClick;
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
        if (modeClick == CharacterSelectorModeClick.SelectCharacter)
            UIEvents.RaiseCharacterSelected(character);
        else 
            UIEvents.RaiseCharacterDetailOpenRequested(character);
    }

    public void OnListItemSelected() 
    {
        UIEvents.RaiseCharacterSelectedListItemSelected(this);
    }

    public void OnListItemPointerEnter() 
    {
        UIEvents.RaiseCharacterSelectedListItemPointerEnter(this);
    }

    public void OnListItemScroll(BaseEventData eventData) 
    {
        UIEvents.RaiseGenericScroll(eventData);
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
