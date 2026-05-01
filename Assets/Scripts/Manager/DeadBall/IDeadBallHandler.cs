using Aremoreno.Enums.Character;

public interface IDeadBallHandler
{
    bool IsReady { get; }
    void Setup(TeamSide teamSide);
    void ResetPositions();
    void Execute();
}
