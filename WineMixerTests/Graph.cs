using System.Diagnostics;
using WineMixer;

namespace WineMixerTests;

public class Graph
{
    public Node[] Nodes { get; }
    public List<Edge> Edges { get; }
    public Configuration Configuration { get; }

    public Graph(Configuration configuration)
    {
        Configuration = configuration;
        Nodes = Enumerable.Range(0, configuration.Count).Select(i => new Node(i, configuration[i])).ToArray();
        Edges = Configuration.ValidTankCombines.Select(CreateEdge).ToList();

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
                node.TanksSets.Add(new TankSet(Configuration, node.Tank));
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
                        var ts = new TankSet(Configuration, tsA, tsB);
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