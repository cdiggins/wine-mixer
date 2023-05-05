using System.Diagnostics;
using System.Text;

namespace WineMixer;

/// <summary>
/// Represents the state of the tanks, which one have wine, and what the blend of wines
/// currently is, in that tank. If we think of wine blending as a graph problem
/// the state is a node in the graph.  
/// </summary>
public class State
{
    public State(Configuration configuration)
        : this(configuration, Enumerable
            .Range(0, configuration.Count)
            .Select(_ => (Mix?)null)
            .ToArray(), 0)
    {
    }

    public State(Configuration configuration, IReadOnlyList<Mix?> contents, int depth)
    {
        Debug.Assert(configuration.Sizes.Count == contents.Count);
        Configuration = configuration;
        Contents = contents;
        Depth = depth;
        StateId = _nextStateId++;
        var i = 0;
        foreach (var mix in Contents)
        {
            if (mix != null)
            {
                UsedTanks++;
                Volume += GetTankSize(i);
            }
            i++;
        }

        AverageMix = Contents.Average();
        AverageMixDistance = TargetDistance(AverageMix);
        BestMix = Contents.MinBy(TargetDistance);
        BestMixDistance = TargetDistance(BestMix);
    }

    private static int _nextStateId;

    public int NumTanks => Configuration.Count;
    public int NumWines => Configuration.NumWines;
    public Configuration Configuration { get; }
    public IReadOnlyList<Mix?> Contents { get; }
    public Mix Target => Configuration.Target;
    public int Depth { get; }
    public int StateId { get; }
    public double Volume { get; }
    public int UsedTanks { get; }

    public bool IsTankOccupied(int i) 
        => Contents[i] != null;

    public Mix? this[int i]
        => Contents[i];

    public double GetTankSize(int i)
        => Configuration[i];

    public IEnumerable<Operation> GetValidOperations() =>
        Enumerable.Empty<Operation>()
            .Concat(GetValidAddWineOperations())
            .Concat(GetValidTankCombineOperations());

    //.Concat(GetValidTankSplitOperations())
    public double TargetDistance(Mix? mix) 
        => Configuration.TargetDistance(mix);

    public bool IsValidTankSplit(TankSplit split) =>
        IsTankOccupied(split.Input) 
        && !IsTankOccupied(split.OutputA) 
        && !IsTankOccupied(split.OutputB);

    public bool IsValidTankCombine(TankCombine combine) =>
        !IsTankOccupied(combine.Output)
        && IsTankOccupied(combine.InputA)
        && IsTankOccupied(combine.InputB);

    public IEnumerable<TankSplit> GetValidTankSplitOperations()
        => Configuration.ValidTankSplits.Where(IsValidTankSplit);

    public IEnumerable<TankCombine> GetValidTankCombineOperations()
        => Configuration.ValidTankCombines.Where(IsValidTankCombine);

    public IEnumerable<AddWine> GetValidAddWineOperations()
        => Configuration.ValidAddWines.Where(x => !IsTankOccupied(x.Tank));

    public Mix CombineResult(int a, int b)
        => Contents[a]! + Contents[b]!;

    public State Apply(Operation? operation)
    {
        if (operation == null) return this;
        var newContents = Contents.ToArray();
        switch (operation)
        {
            case AddWine addWine:
                newContents[addWine.Tank] = Mix.CreateFromIndex(addWine.Wine, NumWines) * GetTankSize(addWine.Tank);
                break;

            case TankCombine tankCombine:
                newContents[tankCombine.InputA] = null;
                newContents[tankCombine.InputB] = null;
                newContents[tankCombine.Output] = CombineResult(tankCombine.InputA, tankCombine.InputB);
                break;

            case TankSplit tankSplit:
                newContents[tankSplit.OutputA] = Contents[tankSplit.Input];
                newContents[tankSplit.OutputB] = Contents[tankSplit.Input];
                newContents[tankSplit.Input] = null;
                break;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(operation));
        }

        return new State(Configuration, newContents, Depth+1);
    }

    public StringBuilder BuildString(StringBuilder sb, bool details = true)
    {
        sb.AppendLine($"State {StateId} depth={Depth} volume={Volume} tanks={UsedTanks}/{NumTanks}");
        if (details)
        {
            sb.AppendLine($"Best mix is {BestMix} of distance {BestMixDistance:0.######}");
            sb.AppendLine($"Average mix is {AverageMix} of distance {AverageMixDistance:0.######}");
            for (var i = 0; i < NumTanks; ++i)
            {
                var t = Contents[i];
                if (t != null)
                    sb.AppendLine($"Tank {i} has size {Configuration[i]} and contains {t.Normal} of distance {TargetDistance(t):0.###}"); else 
                    sb.AppendLine($"Tank {i} has size {Configuration[i]} and is empty");
            }
        }
        return sb;
    }

    public override string ToString() 
        => BuildString(new StringBuilder()).ToString();

    public Mix? AverageMix { get; }
    public double AverageMixDistance { get; }
    public Mix? BestMix { get; }
    public double BestMixDistance { get; }
}