using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Aremoreno.Enums.Move;

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
    private MoveEvolution _cachedEvolution = MoveEvolution.None;

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
    }

    private void OnDisable()
    {
        MoveEvents.OnMoveCutsceneStart -= HandleMoveCutsceneStart;
        MoveEvents.OnMoveCutsceneEnd -= HandleMoveCutsceneEnd;
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
    // Move Data
    // -------------------------

    private void SetMove(Move move)
    {
        textMoveName.text = move.MoveName;
        textMoveName.color = ColorManager.GetElementColor(move.Element);

        if(move.CurrentEvolution == _cachedEvolution) return;

        _cachedEvolution = move.CurrentEvolution;

        if(move.CurrentEvolution == MoveEvolution.None) 
        {
            imageEvolutionGoBefore.SetActive(false);
            imageEvolutionGoAfter.SetActive(false);
        } else 
        {
            SetEvolution(move);
        }
    }

    private void SetEvolution(Move move) 
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
    // Address
    // -------------------------
    private async System.Threading.Tasks.Task SetEvolutionAsync(string address, Image imageEvolution)
    {
        var task = _bindingEvolution.LoadAsync(address);
        imageEvolution.sprite = await task;
    }
}
