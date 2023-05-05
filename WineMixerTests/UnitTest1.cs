using System.Diagnostics;
using WineMixer;

namespace WineMixerTests;

public static class Tests
{
    public static Mix Target = new Mix(0.1, 0.15, 0.25, 0.5);

    public static int[][] PossibleTankSizes()
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

    public static IReadOnlyList<Configuration> GetInputConfigurations()
        => PossibleTankSizes().Select(tc => ToConfiguration(tc, Target)).ToList();

    public static Configuration ToConfiguration(int[] sizes, Mix target)
    {
        return new Configuration(sizes, target);
    }

    [Test]
    [TestCaseSource(nameof(GetInputConfigurations))]
    public static void OutputCombineSteps(Configuration sizes)
    {
        Console.WriteLine($"Valid combine steps");
        var i = 0;

        foreach (var step in sizes.ValidTankCombines)
        {
            Console.WriteLine($"Tank combine {i++} = {step}");
        }
    }
        
    [Test]
    [TestCaseSource(nameof(GetInputConfigurations))]
    public static void OutputBreakdowns(Configuration sizes)
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

    public static void OutputStateAnalysis(State state)
    {
        Console.WriteLine(state);

        var ops = state.GetValidOperations().ToList();
        Console.WriteLine($"Found {ops.Count} immediate operations");

        var combineTree = state.CombineTree().ToList();
        Console.WriteLine($"Found {combineTree.Count} states reachable via combines only");

        //var opTree = state.OperationTree().ToList();
        //Console.WriteLine($"Found {opTree.Count} reachable states");

        /*
        var deltas = state.GetCombineDeltas(target).ToList();
        var smallestDelta = deltas.MinBy(d => d.Length);
        var numDeltas = deltas.Count;
        if (smallestDelta != null)
        {
            Console.WriteLine($"Found {numDeltas} deltas, best is {smallestDelta.DeltaMix} with distance {smallestDelta.Length}");
        }
        else
        {
            Console.WriteLine($"No deltas found");
        }

        // BUG: the number of transitions reported seems bogus. 
        var transitions = state.GetTransitions().ToList();
        Console.WriteLine($"{transitions.Count} transitions found");

        Console.WriteLine($"{totalTanks - usedTanks} add operations found");

        var combinations = state.GetValidTankCombineOperations().ToList();
        Console.WriteLine($"{combinations.Count} combine operations found");

        var bestCombineTuple = state.BestCombine(target);
        var bestCombineOp = bestCombineTuple.Item1;
        var bestCombineMix = bestCombineTuple.Item2;
        var bestCombineDist = target.BlendDistance(bestCombineMix);
        Console.WriteLine($"The best combine operation is {bestCombineOp} and would yield {bestCombineMix} with distance {bestCombineDist}");

        var sw = Stopwatch.StartNew();
        var bestCombineTreeMix = bestCombine?.BestMix;
        var dist2 = target.BlendDistance(bestCombineTreeMix);
        Console.WriteLine($"Found {combines.Count} combine operations in {sw.Elapsed} time");
        Console.WriteLine($"The best mix was {bestCombineTreeMix} with distance {dist2}");
        */
    }

    public static double Score(State state)
    {
        return state.BestMixDistance + state.AverageMixDistance / 1000;
    }

    public static Operation? ChooseOperationGreedily(State state)
    {
        var ops = state.GetValidOperations().ToList();
        var r = ops.MinBy(op => Score(state.Apply(op)));
        Console.WriteLine($"From {ops.Count} operations greedily chose {r}");
        return r;
    }

    public static State ApplyGreedyOperation(State state)
    {
        return state.Apply(ChooseOperationGreedily(state));
    }

    [Test]
    public static void Experiment()
    {
        var config = GetInputConfigurations()[1];
        var state = new State(config);

        for (var i = 0; i < 10; ++i)
        {
            OutputStateAnalysis(state);
            //state = state.ApplyRandomOperation();
            state = ApplyGreedyOperation(state);
        }
    }

    [Test]
    public static void CountReachableStates()
    {
        var config = GetInputConfigurations()[1];
        var state = new State(config);

        var cnt = 0;
        foreach (var tmp in state.OperationTree())
        {
            cnt++;
            if (cnt % 1000 == 0)
            {
                Debug.WriteLine($"State number {cnt}");
                Debug.WriteLine(tmp);
            }
        }
    }

    [Test]
    public static void SomethingWrongWithDistance()
    {
        var mix = new Mix(0.6666666666666666, 0, 0, 0.3333333333333333) * 2;
        var target = new Mix(0.1, 0.15, 0.25, 0.5);

        Console.WriteLine($"target={target} length={target.Length} normal={target.Normal} normal length={target.Normal.Length}");
        Console.WriteLine($"mix={mix} length={mix.Length} normal={mix.Normal} length={target.Normal.Length}");
        Console.WriteLine($"target to mix blend distance = {target.DistanceOfNormals(mix)} regular distance {target.Distance(mix)}");

        Console.WriteLine($"target - mix = {target - mix}");
        Console.WriteLine($"mix - target = {mix - target}");
    }
}