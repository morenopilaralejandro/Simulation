using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Aremoreno.Enums.Move;

public class MoveCutscenePanel : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI textMoveName;
    [SerializeField] private GameObject imageEvolutionGo;
    [SerializeField] private Image imageEvolution;
    [SerializeField] private CanvasGroup canvasGroup;

    private void Awake() 
    {
        SetVisible(false);
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

        if(move.CurrentEvolution == MoveEvolution.None) 
        {
            imageEvolutionGo.SetActive(false);
        } else 
        {
            imageEvolutionGo.SetActive(true);
            imageEvolution.sprite = move.EvolutionSprite;
        }
    }
}
