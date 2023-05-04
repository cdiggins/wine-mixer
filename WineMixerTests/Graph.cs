using System.Diagnostics;
using WineMixer;

namespace WineMixerTests;

public class Graph
{
    public Node[] Nodes { get; }
    public List<Edge> Edges { get; }
    public TankSizes TankSizes { get; }

    public Graph(TankSizes tankSizes)
    {
        TankSizes = tankSizes;
        Nodes = Enumerable.Range(0, tankSizes.Count).Select(i => new Node(i, tankSizes[i])).ToArray();
        Edges = TankSizes.ValidTankCombines.Select(CreateEdge).ToList();

        foreach (var e in Edges)
        {
            foreach (var n in e.Inputs)
            {
                n.OutgoingEdges.Add(e);
            }
            e.Output.IncomingEdges.Add(e);
        }

        foreach (var node in Nodes.OrderBy(n => n.Size))
        {
            if (node.IncomingEdges.Count == 0)
            {
                node.TanksSets.Add(new TankSet(TankSizes, node.Tank));
            }

            foreach (var edge in node.IncomingEdges)
            {
                Debug.Assert(edge.Inputs.Count == 2);
                var nodeA = edge.Inputs[0];
                var nodeB = edge.Inputs[1];

                foreach (var tsA in nodeA.TanksSets)
                {
                    foreach (var tsB in nodeB.TanksSets)
                    {
                        // NOTE: if we have seen a tank set before, then we don't really want to keep it. 
                        var ts = new TankSet(TankSizes, tsA, tsB);
                        node.TanksSets.Add(ts);
                    }
                }
            }
        }
    }

    public Edge CreateEdge(TankCombine tc)
    {
        return new Edge(tc, 
            Nodes[tc.Output], 
            Nodes[tc.InputA],
            Nodes[tc.InputB]);
    }
}