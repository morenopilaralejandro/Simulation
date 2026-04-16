using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class SelectorItemSideListItem : MonoBehaviour
{
    #region Fields

    [Header("UI Elements")]
    [SerializeField] private TMP_Text textName;

    private ItemStorageSlot slot;
    public ItemStorageSlot ItemStorageSlot => slot;

    #endregion

    #region Lifecycle

    private void Awake()
    {

    }

    #endregion

    #region Initialize

    public void Initialize(ItemStorageSlot slot)
    {
        this.slot = slot;

        textName.text = slot.Item.ItemName;
    }

    #endregion

    #region Button

    public void OnListItemClicked() 
    {
        UIEvents.RaiseItemSelected(slot.Item);
    }

    public void OnListItemHighlighted() 
    {
        UIEvents.RaiseSelectorItemSideListItemHighlighted(this);
    }

    public void OnListItemSelected() 
    {
        UIEvents.RaiseSelectorItemSideListItemSelected(this);
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
