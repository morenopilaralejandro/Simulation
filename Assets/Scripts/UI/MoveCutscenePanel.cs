using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Aremoreno.Enums.Move;
using Aremoreno.Enums.Wing;

public class MoveCutscenePanel : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI textMoveName;
    [SerializeField] private GameObject imageEvolutionGoBefore;
    [SerializeField] private GameObject imageEvolutionGoAfter;
    [SerializeField] private Image imageEvolutionBefore;
    [SerializeField] private Image imageEvolutionAfter;
    [SerializeField] private CanvasGroup canvasGroup;

    private readonly AddressableBinding<Sprite> _bindingEvolution = new();
    private string _cachedEvolutionAddress = "";

    private void Awake() 
    {
        SetVisible(false);
        imageEvolutionGoBefore.SetActive(false);
        imageEvolutionGoAfter.SetActive(false);
    }

    private void OnDestroy() 
    {
        _bindingEvolution.Release();
        _bindingEvolution.Cancel();
    }

    private void OnEnable()
    {
        MoveEvents.OnMoveCutsceneStart += HandleMoveCutsceneStart;
        MoveEvents.OnMoveCutsceneEnd += HandleMoveCutsceneEnd;

        WingEvents.OnWingCutsceneStart += HandleWingCutsceneStart;
        WingEvents.OnWingCutsceneEnd += HandleWingCutsceneEnd;
    }

    private void OnDisable()
    {
        MoveEvents.OnMoveCutsceneStart -= HandleMoveCutsceneStart;
        MoveEvents.OnMoveCutsceneEnd -= HandleMoveCutsceneEnd;

        WingEvents.OnWingCutsceneStart -= HandleWingCutsceneStart;
        WingEvents.OnWingCutsceneEnd -= HandleWingCutsceneEnd;
    }

    // -------------------------
    // Event Handlers
    // -------------------------

    private void HandleMoveCutsceneStart(Move move)
    {
        SetMove(move);
        SetVisible(true);
    }

    private void HandleMoveCutsceneEnd()
    {
        SetVisible(false);
    }

    private void HandleWingCutsceneStart(Wing wing)
    {
        SetWing(wing);
        SetVisible(true);
    }

    private void HandleWingCutsceneEnd()
    {
        SetVisible(false);
    }

    // -------------------------
    // Visibility
    // -------------------------

    private void SetVisible(bool visible)
    {
        canvasGroup.alpha = visible ? 1f : 0f;
        canvasGroup.interactable = visible;
        canvasGroup.blocksRaycasts = visible;
    }

    // -------------------------
    // Move
    // -------------------------

    private void SetMove(Move move)
    {
        textMoveName.text = move.MoveName;
        textMoveName.color = ColorManager.GetElementColor(move.Element);

        if(move.EvolutionAddress == _cachedEvolutionAddress) return;

        _cachedEvolutionAddress = move.EvolutionAddress;

        if(move.CurrentEvolution == MoveEvolution.None) 
        {
            imageEvolutionGoBefore.SetActive(false);
            imageEvolutionGoAfter.SetActive(false);
        } else 
        {
            SetMoveEvolution(move);
        }
    }

    private void SetMoveEvolution(Move move) 
    {
        if (move.IsBefore) 
        {
            _ = SetEvolutionAsync(move.EvolutionAddress, imageEvolutionBefore);
            imageEvolutionGoBefore.SetActive(true);
            imageEvolutionGoAfter.SetActive(false);
        } else 
        {
            _ = SetEvolutionAsync(move.EvolutionAddress, imageEvolutionAfter);
            imageEvolutionGoAfter.SetActive(true);
            imageEvolutionGoBefore.SetActive(false);
        }
    }

    // -------------------------
    // Wing
    // -------------------------
    private void SetWing(Wing wing)
    {
        textMoveName.text = wing.WingName;
        textMoveName.color = ColorManager.GetWingColor(wing.WingColorType);

        if(wing.WingEvolutionAddress == _cachedEvolutionAddress) return;

        _cachedEvolutionAddress = wing.WingEvolutionAddress;

        if(wing.CurrentEvolution == WingEvolution.None) 
        {
            imageEvolutionGoBefore.SetActive(false);
            imageEvolutionGoAfter.SetActive(false);
        } else 
        {
            SetWingEvolution(wing);
        }
    }

    private void SetWingEvolution(Wing wing) 
    {
        /*
        if (move.IsBefore) 
        {
            _ = SetEvolutionAsync(move.EvolutionAddress, imageEvolutionBefore);
            imageEvolutionGoBefore.SetActive(true);
            imageEvolutionGoAfter.SetActive(false);
        } else 
        {
            _ = SetEvolutionAsync(move.EvolutionAddress, imageEvolutionAfter);
            imageEvolutionGoAfter.SetActive(true);
            imageEvolutionGoBefore.SetActive(false);
        }
        */

            _ = SetEvolutionAsync(wing.WingEvolutionAddress, imageEvolutionAfter);
            imageEvolutionGoAfter.SetActive(true);
            imageEvolutionGoBefore.SetActive(false);

    }

    // -------------------------
    // Address
    // -------------------------
    private async System.Threading.Tasks.Task SetEvolutionAsync(string address, Image imageEvolution)
    {
        imageEvolution.enabled = false;
        var task = _bindingEvolution.LoadAsync(address);
        imageEvolution.sprite = await task;
        imageEvolution.enabled = true;
    }
}
