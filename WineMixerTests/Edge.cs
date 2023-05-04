using WineMixer;

namespace WineMixerTests;

public class Edge
{
    public TankCombine Combine { get; }
    public Node Output { get; }
    public IReadOnlyList<Node> Inputs { get; }

    public Edge(TankCombine tc, Node output, params Node[] inputs)
    {
        Combine = tc;
        Output = output;
        Inputs = inputs;
    }
}