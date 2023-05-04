using WineMixer;

namespace WineMixerTests;

public static class Tests
{
    public const int NumWines = 4;

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

    public static IReadOnlyList<TankSizes> GetInputTankSizes()
        => GetTanksConfigurations().Select(ToTankSize).ToList();

    public static TankSizes ToTankSize(int[] sizes)
    {
        return new TankSizes(sizes, NumWines);
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

    public static void OutputStateAnalysis(State state, Mix target)
    {
        Console.WriteLine($"State analysis");
        OutputStateContents(state, target);

        var totalTanks = state.NumTanks;
        var usedTanks = state.Contents.Count(x => x != null);
        Console.WriteLine($"tanks used = {usedTanks} / {totalTanks} ");

        var totalVolume = state.TankSizes.Volume;
        var usedVolume = state.Contents.Select((x, i) => x == null ? 0 : state.TankSizes[i]).Sum();
        Console.WriteLine($"volume = {usedVolume} / {totalVolume}");

        var totalWines = state.NumWines;
        var numUsedWines = state.UsedWines();
        Console.WriteLine($"used wines = {numUsedWines} / {totalWines}");

        var diffs = state.GetTankDifferences(target).ToList();
        var bestDiff = diffs.MinBy(diff => diff.GetLength());
        var dist = bestDiff?.GetLength() ?? double.MaxValue;
        Console.WriteLine($"best difference is {bestDiff} has distance {dist}");

        var bestMix = state.Contents.MinBy(target.Distance);
        Console.WriteLine($"best tank mix is {bestMix} which has distance {target.Distance(bestMix)}");

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
        var bestCombineDist = target.Distance(bestCombineMix);
        Console.WriteLine($"The best combine operation is {bestCombineOp} and would yield {bestCombineMix} with distance {bestCombineDist}");
    }

    public static void OutputStateContents(State state, Mix target)
    {
        var i = 0;
        foreach (var mix in state.Contents)
        {
            var dist = target.Distance(mix);
            Console.WriteLine($"Tank {i}:{state.TankSizes[i]} has {mix} which has distance {dist}");
            i++;
        }
    }

    public static double EvaluateOperation(State state, Operation op, IReadOnlyList<Mix> targets)
    {
        var tmp = state.Apply(op);
        var r = double.MaxValue;
        foreach (var mix in tmp.Contents)
        {
            foreach (var target in targets)
            {
                r = Math.Min(target.Distance(mix), r);
            }
        }

        return r;
    }

    public static Operation? ChooseOperationGreedily(State state, Mix target)
    {
        // Chose the op that either
        // 1 produces the best wine or
        // 2 produces the best delta. 

        var deltas = state.GetCombineDeltas(target).ToList();
        var mixes = deltas.Select(d => d.DeltaMix).Append(target).ToList();
        var ops = state.GetValidOperations().ToList();
        Console.WriteLine($"Considering {ops.Count} operations against {mixes.Count} targets");

        Operation? r = null;
        var min = double.MaxValue;
        foreach (var op in ops)
        {
            var tmp = EvaluateOperation(state, op, mixes);
            if (tmp < min)
            {
                r = op;
                min = tmp;
            }
        }
        
        Console.WriteLine($"Chose an operation {r} with value {min}");
        return r;
    }

    [Test]
    public static void Experiment()
    {
        var sizes = GetInputTankSizes()[1];
        var target = new Mix(0.1, 0.15, 0.25, 0.5);
        var state = new State(sizes, target.Count);

        for (var i = 0; i < 15; ++i)
        {
            OutputStateAnalysis(state, target);
            //state = state.ApplyRandomOperation();
            var op = ChooseOperationGreedily(state, target);
            state = state.Apply(op);
        }
    }

    [Test]
    public static void SomethingWrongWithDistance()
    {
        var mix = new Mix(0.6666666666666666, 0, 0, 0.3333333333333333);
        var target = new Mix(0.1, 0.15, 0.25, 0.5);
        
        Console.WriteLine($"target={target} length={target.GetLength()} mix={mix} length={mix.GetLength()}");
        Console.WriteLine($"target to mix distance = {target.Distance(mix)}");
        Console.WriteLine($"mix to target distance = {mix.Distance(target)}");

        Console.WriteLine($"target - mix = {target - mix}");
        Console.WriteLine($"mix - target = {mix - target}");
    }
}