public interface IDuelHandler 
{
    void StartDuel();
    void AddParticipant(DuelParticipant p);
    void Resolve();
    void Cancel();
}
