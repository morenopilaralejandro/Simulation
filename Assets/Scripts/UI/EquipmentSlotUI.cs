using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;
using UnityEngine.EventSystems;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Item;

public class EquipmentSlotUI : MonoBehaviour
{
    #region Field

    [Header("UI Elements")]
    [SerializeField] private Image imageBlock;
    [SerializeField] private Image imageIconDefault;
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text textName;
    [SerializeField] private Image imageIcon;

    private ItemEquipment itemEquipment;
    private Character character;
    private int index;
    private readonly AddressableBinding<Sprite> _bindingIcon = new();

    public ItemEquipment ItemEquipment => itemEquipment;
    public Character Character => character;
    public int Index => index;
    public Button Button => button;

    #endregion

    #region Lifecycle

    private void Awake()
    {
        imageIconDefault.enabled = true;
        imageBlock.enabled = true;
    }

    #endregion

    #region Initialize

    public void Initialize(ItemEquipment itemEquipment, Character character, int index)
    {
        this.itemEquipment = itemEquipment;
        this.character = character;
        this.index = index;
    }

    #endregion

    #region Helpers

    public void SetEquipment(ItemEquipment itemEquipment)
    {
        if (itemEquipment == null) 
        {
            Clear();
            return;
        }

        this.itemEquipment = itemEquipment;
        _ = SetIconAsync(itemEquipment.IconSpriteAddress);
        imageBlock.enabled = false;
        imageIconDefault.enabled = false;
    }

    public void Clear()
    {   
        itemEquipment = null;
        //character = null;

        imageBlock.enabled = true;
        imageIconDefault.enabled = true;

        imageIcon.enabled = false;
        _bindingIcon.Release();
        _bindingIcon.Cancel();
        imageIcon.sprite = null;
    }

    public async Task SetIconAsync(string address)
    {
        imageIcon.enabled = false;
        var asset = await _bindingIcon.LoadAsync(address);
        imageIcon.sprite = asset;
        imageIcon.enabled = true;
    }

    #endregion

    #region Button Handle

    public void OnClick()
    {
        UIEvents.RaiseEquipmentSlotUIClicked(this);
    }

    #endregion

    /*
    #region Events

    private void OnEnable()
    {
        UIEvents.OnMoveSlotUIMoveStarted += HandleMoveStarted;
        UIEvents.OnMoveSlotUIMoveEnded += HandleMoveEnded;
    }

    private void OnDisable()
    {
        UIEvents.OnMoveSlotUIMoveStarted -= HandleMoveStarted;
        UIEvents.OnMoveSlotUIMoveEnded -= HandleMoveEnded;
    }

    private void HandleMoveStarted(MoveSlotUI slot)
    {
        if (this != slot) return;
        canvasGroup.alpha = 0.6f;
    }

    private void HandleMoveEnded(MoveSlotUI slot)
    {
        if (canvasGroup.alpha == 0.6f)
            canvasGroup.alpha = 1f;
    }

    #endregion

    */

    #region Helpers

    #endregion
}
