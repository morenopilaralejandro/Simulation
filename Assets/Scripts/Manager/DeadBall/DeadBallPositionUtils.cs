using UnityEngine;
using Simulation.Enums.Battle;
using Simulation.Enums.Character;
using Simulation.Enums.DeadBall;

public class DeadBallPositionUtils
{
    private readonly DeadBallManager manager;

    public DeadBallPositionUtils(DeadBallManager manager)
    {
        this.manager = manager;
    }

    /* -------------------------------------------------------------
     *  Corner placement
     * ------------------------------------------------------------- */

    public CornerPlacement GetBallCornerPlacement(Vector3 ballPos)
    {
        bool isRight = ballPos.x > 0f;
        bool isBottom = ballPos.z < 0f;

        if (!isRight && !isBottom) return CornerPlacement.TopLeft;
        if (isRight && !isBottom)  return CornerPlacement.TopRight;
        if (!isRight && isBottom)  return CornerPlacement.BottomLeft;
        return CornerPlacement.BottomRight;
    }

    public Vector3 FlipPositionOnCorner(
        Vector3 basePos,
        CornerPlacement cornerPlacement)
    {
        Vector3 pos = basePos;

        switch (cornerPlacement)
        {
            case CornerPlacement.TopRight:
                pos.x *= -1f;
                break;

            case CornerPlacement.BottomLeft:
                pos.z *= -1f;
                break;

            case CornerPlacement.BottomRight:
                pos.x *= -1f;
                pos.z *= -1f;
                break;
        }

        return pos;
    }

    /* -------------------------------------------------------------
     *  Endline / sideline placement
     * ------------------------------------------------------------- */

    public BoundPlacement GetBallEndPlacement(Vector3 ballPos)
    {
        return ballPos.z > 0f
            ? BoundPlacement.Top
            : BoundPlacement.Bottom;
    }

    public BoundPlacement GetBallSidePlacement(Vector3 ballPos)
    {
        return ballPos.x > 0f
            ? BoundPlacement.Right
            : BoundPlacement.Left;
    }

    /* -------------------------------------------------------------
     *  Formation helpers
     * ------------------------------------------------------------- */

    /// <summary>
    /// Adjusts index order so Away team formations mirror correctly.
    /// </summary>
    public int GetTeamAdjustedIndex(
        int index,
        int length,
        TeamSide teamSide)
    {
        return teamSide == TeamSide.Away
            ? length - 1 - index
            : index;
    }

    public void SetCornerPositions(
        Character[] characters,
        Vector3[] basePositions,
        TeamSide teamSide,
        CornerPlacement cornerPlacement)
    {
        int count = Mathf.Min(characters.Length, basePositions.Length);

        for (int i = 0; i < count; i++)
        {
            int adjustedIndex = GetTeamAdjustedIndex(
                i,
                basePositions.Length,
                teamSide);

            Vector3 basePos = basePositions[adjustedIndex];
            Vector3 finalPos = FlipPositionOnCorner(basePos, cornerPlacement);

            characters[i].Teleport(finalPos);
        }
    }

    /* -------------------------------------------------------------
     *  Throw-in helpers
     * ------------------------------------------------------------- */

    public Vector3 FlipPositionOnThrowIn(
        Vector3 basePos,
        Vector3 ballPosition,
        BoundPlacement boundPlacement)
    {
        // Align formation with current ball Z position
        Vector3 pos = basePos;
        pos.z += ballPosition.z;

        switch (boundPlacement)
        {
            case BoundPlacement.Right:
                pos.x *= -1f;
                break;

            case BoundPlacement.Left:
            default:
                break;
        }

        return pos;
    }
}
