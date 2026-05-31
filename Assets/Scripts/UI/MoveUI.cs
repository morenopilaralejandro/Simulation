using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Move;

public class MoveUI : MonoBehaviour
{
    #region Field

    [Header("UI Elements")]
    [SerializeField] private Image imageCategoty;
    [SerializeField] private Image imageTrait;
    [SerializeField] private Image imageEvolution;
    [SerializeField] private TMP_Text textName;
    [SerializeField] private TMP_Text textCost;
    [SerializeField] private bool isNameRecolor;

    private int _setVersion;
    private readonly AddressableBinding<Sprite> _bindingEvolution = new();
    private IconManager iconManager;

    #endregion

    #region Lifecycle

    private void OnDestroy()
    {
        Clear();
    }

    private void Awake()
    {
        iconManager = IconManager.Instance;
    }

    #endregion

    #region Helpers

    public void SetMoveAsync(Move move)
    {
        if (move == null) return;

        if (textName != null) 
        {
            textName.text = move.MoveName;
            if (isNameRecolor) textName.color = ColorManager.GetElementColor(move.Element);
        }

        if(textCost != null) textCost.text = $"{move.Cost}";

        if (imageCategoty != null) imageCategoty.sprite = iconManager.Category.GetIcon(move.Category);

        if (move.Trait != Trait.None) 
        {
            if (imageTrait != null) 
            {
                imageTrait.sprite = iconManager.Trait.GetIcon(move.Trait);
                imageTrait.enabled = true;
            }
        } else 
        {
            if (imageTrait != null) 
            {
                imageTrait.enabled = false;
            }
        }

        if (move.CurrentEvolution != MoveEvolution.None) 
        {
            if (imageEvolution != null) 
            {
                _ = SetEvolutionAsync(move.EvolutionAddress);
                imageEvolution.enabled = true;
            }
        } else 
        {
            if (imageEvolution != null) 
            {
                imageEvolution.enabled = false;
            }
        }
    }

    public void Clear()
    {
        if (imageCategoty != null) imageCategoty.sprite = null;
        if (imageTrait != null) imageTrait.sprite = null;
        if (imageEvolution != null) imageEvolution.sprite = null;
        if (textName != null) textName.text = "";
        if (textCost != null) textCost.text = "";

        if (imageTrait != null) imageTrait.enabled = false;
        if (imageEvolution != null) imageEvolution.enabled = false;
 

        _bindingEvolution.Release();
        _bindingEvolution.Cancel();
        _setVersion++;
    }

    #endregion

    #region Addressable

    private async System.Threading.Tasks.Task SetEvolutionAsync(string address)
    {
        int version = ++_setVersion;
        var task = _bindingEvolution.LoadAsync(address);

        var asset = await task;

        if (version != _setVersion) return;

        imageEvolution.sprite = asset;
    }

    #endregion
}
