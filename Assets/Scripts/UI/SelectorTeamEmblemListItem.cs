using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SelectorTeamEmblemListItem : MonoBehaviour
{
    #region Fields

    [Header("UI Elements")]
    [SerializeField] private Image emblemImage;

    private string emblemId;
    private Sprite emblemSprite;

    #endregion

    #region Lifecycle

    private void Awake()
    {

    }

    #endregion

    #region Initialize

    public void Initialize(string emblemId, Sprite emblemSprite)
    {
        this.emblemId = emblemId;
        this.emblemSprite = emblemSprite;
        emblemImage.sprite = emblemSprite;
    }

    #endregion

    #region Button

    public void OnListItemClicked() 
    {
        UIEvents.RaiseTeamEmblemSelected(emblemId, emblemSprite);
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
