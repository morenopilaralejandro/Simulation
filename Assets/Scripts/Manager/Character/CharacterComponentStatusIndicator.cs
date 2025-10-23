using UnityEngine;
using Simulation.Enums.Character;

public class CharacterComponentStatusIndicator : MonoBehaviour
{
    private Character character;

    [Header("Static Sprites")]
    [SerializeField] private SpriteRenderer indicatorRenderer;
    [SerializeField] private Sprite trippingSprite;
    [SerializeField] private Sprite tiredSprite;
    [SerializeField] private Sprite exhaustedSprite;

    [Header("Animated Indicators")]
    [SerializeField] private Animator indicatorAnimator;
    private string stunAnimationTrigger = "Stun";

    private bool showingStatus;
    private StatusEffect? currentStatus;
    private FatigueState currentFatigue => this.character.FatigueState;

    public void Initialize(CharacterData characterData, Character character) 
    {
        this.character = character;
        UpdateStatusIndicator(null);
    }

    public void UpdateStatusIndicator(StatusEffect? newStatus)
    {
        currentStatus = newStatus;

        // stop animation first
        if (indicatorAnimator != null)
            indicatorAnimator.ResetTrigger(stunAnimationTrigger);

        indicatorRenderer.enabled = true;

        if (showingStatus && currentStatus.HasValue)
        {
            switch (currentStatus.Value)
            {
                case StatusEffect.Stunned:
                    // play animation instead of showing static sprite
                    indicatorRenderer.sprite = null; // hide static indicator
                    if (indicatorAnimator != null)
                    {
                        indicatorRenderer.enabled = false; // hide sprite renderer if using animator sprite
                        indicatorAnimator.gameObject.SetActive(true);
                        indicatorAnimator.SetTrigger(stunAnimationTrigger);
                    }
                    break;

                case StatusEffect.Tripping:
                    if (indicatorAnimator != null)
                        indicatorAnimator.gameObject.SetActive(false);
                    indicatorRenderer.enabled = true;
                    indicatorRenderer.sprite = trippingSprite;
                    break;
            }
        }
        else
        {
            // no active status, use fatigue
            if (indicatorAnimator != null)
                indicatorAnimator.gameObject.SetActive(false);
            indicatorRenderer.enabled = true;

            switch (currentFatigue)
            {
                case FatigueState.Tired:
                    indicatorRenderer.sprite = tiredSprite;
                    break;
                case FatigueState.Exhausted:
                    indicatorRenderer.sprite = exhaustedSprite;
                    break;
                default:
                    indicatorRenderer.sprite = null;
                    break;
            }
        }
    }
}
