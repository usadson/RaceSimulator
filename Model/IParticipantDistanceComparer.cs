namespace Model;

public class IParticipantDistanceComparer : IComparer<IParticipant>
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

        return y!.DistanceFromStart - x!.DistanceFromStart;
    }
}