using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class SpecialOption : MonoBehaviour
{
    [SerializeField] private string specialOptionId;
    [SerializeField] private Button button;
    [SerializeField] private float cooldownDuration;
    [SerializeField] private float cooldownRemaining = 0f;
    private bool wasTimeFrozenLastFrame = false;

    public string SpecialOptionId => specialOptionId;

    void OnEnable()
    {
        BattleEvents.OnBattleStart += HandleBattleStart;
    }

    void OnDisable()
    {
        BattleEvents.OnBattleStart -= HandleBattleStart;
    }

    private void HandleBattleStart()
    {
        cooldownRemaining = 0f;
        button.interactable = true;
    }

    public bool IsOnCooldown() => cooldownRemaining > 0f;

    public void StartCooldown()
    {
        cooldownRemaining = cooldownDuration;
        button.interactable = false;
    }

    public void UpdateCooldown()
    {
        bool isTimeFrozen = BattleManager.Instance.IsTimeFrozen;

        // If time is frozen, disable the button and exit early
        if (isTimeFrozen)
        {
            button.interactable = false;
            wasTimeFrozenLastFrame = true;
            return;
        }

        // When time unfreezes, restore the correct interactable state
        if (wasTimeFrozenLastFrame && !isTimeFrozen)
        {
            wasTimeFrozenLastFrame = false;

            if (!IsOnCooldown())
                button.interactable = true; // only become clickable again if cooldown done
        }

        // Normal cooldown countdown
        if (cooldownRemaining > 0f)
        {
            cooldownRemaining -= Time.deltaTime;
            if (cooldownRemaining <= 0f)
            {
                cooldownRemaining = 0f;
                button.interactable = true;
            }
        }
    }

    public void SetInteractable(bool isInteractable)
    {
        button.interactable = isInteractable;
    }

}
