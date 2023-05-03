using System.Diagnostics;
using WineMixer;
using WineMixer.Experimental;

namespace WineMixerTests
{
    public class TankSet 
    {
        public string Text { get; }
        public TankSizes Sizes { get; }
        public IReadOnlyList<int> Tanks { get; }

        public TankSet(TankSizes sizes, params int[] tanks)
            : this(sizes, (IEnumerable<int>)tanks)
        { }

        public TankSet(TankSizes sizes, IEnumerable<int> tanks)
        {
            Sizes = sizes;
            Tanks = tanks.ToList();
            Text = string.Join(", ", Tanks.Select(t => Sizes[t]));
        }

        public TankSet(TankSizes sizes, params TankSet[] tankSets)
            : this(sizes, tankSets.SelectMany(ts => ts.Tanks))
        { }

        public override string ToString()
            => Text;

        public override int GetHashCode()
            => Text.GetHashCode();

        public override bool Equals(object? obj)
            => obj is TankSet ts && ts.Text.Equals(Text);
    }

    public class Node
    {
        public int Size { get; }
        public int Tank { get; }
        public List<Edge> IncomingEdges { get; } = new();
        public List<Edge> OutgoingEdges { get; } = new();
        public Node(int tank, int size) { Tank = tank; Size = size; }
        public HashSet<TankSet> TanksSets { get; } = new();
    }

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

    public class Graph
    {
        public Node[] Nodes { get; }
        public List<Edge> Edges { get; }
        public TankSizes TankSizes { get; }

        public Graph(TankSizes tankSizes)
        {
            TankSizes = tankSizes;
            Nodes = Enumerable.Range(0, tankSizes.NumTanks).Select(i => new Node(i, tankSizes[i])).ToArray();
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

    public static class Tests
    {
        public static int[][] GetTanksConfigurations()
        {
            return new[]
            {
                new [] { 1, 2, 3, 4, 5, 6, 7, 8 },
                new [] { 1, 2, 3, 5, 8, 10, 13, 15, 20, 25 },
                new [] { 1, 2, 2, 3, 4, 4, 5 },
                new [] { 1, 1, 2, 3, 3, 4, 5, 5, 6, 6, 6, 6, 7, 7, 7 },
                new [] { 1, 1, 2, 3, 3, 4, 5, 5, 6, 6, 6, 6, 7, 7, 8, 8, 12, 12, 13, 15, 16, 20, 24, 24, 28 },
                new [] { 1, 1, 2, 3, 5, 5, 6, 8, 10, 11, 12, 13, 15, 18, 20, 25, 23, 25, 25, 28, 30, 35, 40, 50 },
            };
        }

        public static IEnumerable<TankSizes> GetInputTankSizes()
            => GetTanksConfigurations().Select(ToTankSize);

        public static TankSizes ToTankSize(int[] sizes)
        {
            return new TankSizes(sizes, 1);
        }

        [Test]
        [TestCaseSource(nameof(GetInputTankSizes))]
        public static void OutputCombineSteps(TankSizes sizes)
        {
            Console.WriteLine($"Valid combine steps");
            var i = 0;

            foreach (var step in sizes.ValidTankCombines)
            {
                Console.WriteLine($"Tank combine {i++} = {step}");
            }
        }
        
        [Test]
        [TestCaseSource(nameof(GetInputTankSizes))]
        public static void OutputBreakdowns(TankSizes sizes)
        {
            var g = new Graph(sizes);
            foreach (var n in g.Nodes)
            {
                Console.WriteLine($"Node tank={n.Tank} size={n.Size} in={n.IncomingEdges.Count} out={n.OutgoingEdges.Count} sets={n.TanksSets.Count}");
            }

            foreach (var n in g.Nodes)
            {
                Console.WriteLine($"Node has tank={n.Tank} size={n.Size} in={n.IncomingEdges.Count} out={n.OutgoingEdges.Count} sets={n.TanksSets.Count}");
                foreach (var ts in n.TanksSets)
                {
                    Console.WriteLine($"  {ts}");
                }
            }
        }
    }
}