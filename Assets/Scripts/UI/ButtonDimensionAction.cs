using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Aremoreno.Enums.Battle;

public class ButtonDimensionAction : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private string dimensionActionId;
    [SerializeField] private Button button;
    [SerializeField] private float cooldownDuration = 10f;

    private float cooldownRemaining;
    private bool isTimeFrozen;

    private Coroutine cooldownRoutine;

    public string DimensionActionId => dimensionActionId;

    public bool IsOnCooldown => cooldownRemaining > 0f;

    private void OnEnable()
    {
        BattleEvents.OnBattleStart += HandleBattleStart;

        button.onClick.AddListener(OnButtonPressed);
    }

    private void OnDisable()
    {
        BattleEvents.OnBattleStart -= HandleBattleStart;

        button.onClick.RemoveListener(OnButtonPressed);

        if (cooldownRoutine != null)
        {
            StopCoroutine(cooldownRoutine);
            cooldownRoutine = null;
        }
    }

    private void HandleBattleStart(BattleType battleType)
    {
        cooldownRemaining = 0f;

        if (cooldownRoutine != null)
        {
            StopCoroutine(cooldownRoutine);
            cooldownRoutine = null;
        }

        button.interactable = true;
    }

    public void StartCooldown()
    {
        if (cooldownRoutine != null)
        {
            StopCoroutine(cooldownRoutine);
            cooldownRoutine = null;
        }

        cooldownRoutine = StartCoroutine(CooldownRoutine());
    }

    private IEnumerator CooldownRoutine()
    {
        cooldownRemaining = cooldownDuration;

        RefreshInteractable();

        while (cooldownRemaining > 0f)
        {
            // Only count down while battle is NOT frozen
            if (!BattleManager.Instance.IsTimeFrozen)
            {
                cooldownRemaining -= Time.deltaTime;
            }

            yield return null;
        }

        cooldownRemaining = 0f;

        RefreshInteractable();

        cooldownRoutine = null;
    }

    public void SetTimeFrozen(bool frozen)
    {
        isTimeFrozen = frozen;

        RefreshInteractable();
    }

    private void RefreshInteractable()
    {
        bool interactable =
            !isTimeFrozen &&
            !BattleManager.Instance.IsTimeFrozen &&
            !IsOnCooldown;

        LogManager.Trace(
            $"[ButtonDimensionAction] RefreshInteractable | " +
            $"isTimeFrozen={isTimeFrozen} | " +
            $"battleFrozen={BattleManager.Instance.IsTimeFrozen} | " +
            $"cooldownRemaining={cooldownRemaining} | " +
            $"interactable={interactable}"
        );

        button.interactable = interactable;
    }

    public void OnButtonPressed()
    {
        if (
            isTimeFrozen ||
            BattleManager.Instance.IsTimeFrozen ||
            IsOnCooldown
        )
            return;

        switch (dimensionActionId)
        {
            case "dimension-action-pause":
                HandlePause();
                break;

            default:
                HandleDefault();
                break;
        }

        StartCooldown();

        UIEvents.RaiseButtonDimensionActionPressed(dimensionActionId);
    }

    private void HandlePause()
    {
        if (
            PauseManager.Instance.IsPaused ||
            !PauseManager.Instance.CanPause()
        )
            return;

        PauseManager.Instance.StartPause(
            BattleManager.Instance.GetUserSide()
        );
    }

    private void HandleDefault()
    {
        LogManager.Trace("[ButtonDimensionAction] Default action");
    }
}
