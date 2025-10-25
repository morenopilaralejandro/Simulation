public interface IDuelHandler 
{
    void AddParticipant(DuelParticipant participant);
    void Resolve();
    void EndDuel(DuelParticipant winner, DuelParticipant loser);
    void CancelDuel();
}
