namespace Model;

public class IParticipantCompetitionComparer : IComparer<IParticipant>
{
    // Sort on reverse DistanceFromStart
    public int Compare(IParticipant? x, IParticipant? y)
    {
        if (x == null && y == null)
            return 0;
        if (x == null && y != null)
            return 1;
        if (x != null && y == null)
            return -1;

        return x!.CompetitionPoints - y!.CompetitionPoints;
    }
}