namespace WineMixerTests;

public class Node
{
    public int Size { get; }
    public int Tank { get; }
    public List<Edge> IncomingEdges { get; } = new();
    public List<Edge> OutgoingEdges { get; } = new();
    public Node(int tank, int size) { Tank = tank; Size = size; }
    public HashSet<TankSet> TanksSets { get; } = new();
}