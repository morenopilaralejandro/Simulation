using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class CharacterTargetIndicator : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    private Character controlledCharacter => BattleManager.Instance.ControlledCharacter[BattleManager.Instance.GetUserSide()];
    private Character targetedCharacter => BattleManager.Instance.TargetedCharacter[BattleManager.Instance.GetUserSide()];
    private float lineY = 0f;

    public void Update()
    {
        // Hide line if we have missing references
        if (controlledCharacter == null || targetedCharacter == null)
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
}
