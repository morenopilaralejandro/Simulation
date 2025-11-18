using UnityEngine;
using Simulation.Enums.Battle;

[RequireComponent(typeof(LineRenderer))]
public class CharacterTargetIndicator : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    private Character controlledCharacter => BattleManager.Instance.ControlledCharacter[BattleManager.Instance.GetUserSide()];
    private Character targetedCharacter => BattleManager.Instance.TargetedCharacter[BattleManager.Instance.GetUserSide()];
    private float lineY = 0.01f;

    private void Awake()
    {
        CharacterTargetManager.Instance.RegisterIndicator(this);
    }

    private void OnDestroy()
    {
        CharacterTargetManager.Instance.UnregisterIndicator();
    }

    public void Update()
    {
        // Hide line if we have missing references
        if (controlledCharacter == null || 
            targetedCharacter == null || 
            (BattleManager.Instance.CurrentPhase != BattlePhase.Battle && BattleManager.Instance.CurrentPhase != BattlePhase.Deadball))
        {
            lineRenderer.positionCount = 0;
            return;
        }

        lineRenderer.positionCount = 2;

        Vector3 startPos = controlledCharacter.transform.position;
        Vector3 endPos = targetedCharacter.transform.position;

        startPos.y = lineY;
        endPos.y = lineY;

        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);
    }

    public void ShowFreeAim(Vector3 startPos, Vector3 endPos)
    {
        lineRenderer.positionCount = 2;
        startPos.y = lineY;
        endPos.y = lineY;
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);
    }

    public void Hide()
    {
        lineRenderer.positionCount = 0;
    }
}
