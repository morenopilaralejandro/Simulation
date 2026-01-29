using Simulation.Enums.Character;

public interface IDeadBallHandler
{
    bool IsReady { get; }
    void Setup(TeamSide teamSide);
    void HandleInput();
    void Execute();
}
