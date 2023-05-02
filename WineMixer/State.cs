using System.Diagnostics;
using System.Text;

namespace Champagne;

public class State
{
    public State(TankSizes tankSizes, int numWines)
        : this(tankSizes, Enumerable
            .Range(0, tankSizes.NumTanks)
            .Select(_ => (Mix?)null)
            .ToArray(), numWines)
    {
    }

    public State(TankSizes tankSizes, IReadOnlyList<Mix?> contents, int numWines)
    {
        Debug.Assert(tankSizes.Sizes.Count == contents.Count);
        TankSizes = tankSizes;
        Contents = contents;
        NumWines = numWines;
    }

    public int NumTanks => TankSizes.NumTanks;
    public int NumWines { get; }
    public TankSizes TankSizes { get; }
    public IReadOnlyList<Mix?> Contents { get; }

    public bool IsTankOccupied(int i) 
        => Contents[i] != null;

    public Mix? this[int i]
        => Contents[i];

    public IEnumerable<Step> GetValidSteps(int numWines)
    {
        return Enumerable.Empty<Step>()
            .Concat(GetValidAddWineSteps(numWines))
            .Concat(GetValidTankCombineSteps())
            //.Concat(GetValidRemoveWineSteps())
            //.Concat(GetValidTankSplitSteps())
            ;
    }

    public bool IsBetterOrSame(TankCombine tankCombine, Mix target)
    {
        var mixA = this[tankCombine.InputA];
        var mixB = this[tankCombine.InputB];
        if (mixA == null || mixB == null) throw new Exception("Illegal tank combine step");

        var distA = target.DistanceFrom(mixA);
        var distB = target.DistanceFrom(mixB);
        
        var tmp = Apply(tankCombine);
        var newMix = tmp[tankCombine.Output];
        if (newMix == null) throw new Exception("Internal error, combined mix didn't do anything");

        var newDist = target.DistanceFrom(newMix);

        return newDist <= distA || newDist <= distB;
    }

    // Combining two tanks and producing a worse wine is a bad idea. 
    public IEnumerable<Step> RemoveBadCombines(IEnumerable<Step> steps, Mix target)
    {
        return steps.Where(step => step is TankCombine tc && IsBetterOrSame(tc, target));
    }

    public bool IsValidTankSplit(TankSplit split)
    {
        return IsTankOccupied(split.Input) 
               && !IsTankOccupied(split.OutputA) 
               && !IsTankOccupied(split.OutputB);
    }

    public bool IsValidTankCombine(TankCombine combine)
    {
        return !IsTankOccupied(combine.Output)
               && IsTankOccupied(combine.InputA)
               && IsTankOccupied(combine.InputB);
    }

    public IEnumerable<TankSplit> GetValidTankSplitSteps()
        => TankSizes.ValidTankSplits.Where(IsValidTankSplit);

    public IEnumerable<TankCombine> GetValidTankCombineSteps()
        => TankSizes.ValidTankCombines.Where(IsValidTankCombine);

    public IEnumerable<AddWine> GetValidAddWineSteps(int numWines)
        => TankSizes.ValidAddWines.Where(x => !IsTankOccupied(x.Tank));

    public IEnumerable<RemoveWine> GetValidRemoveWineSteps()
    {
        for (var i = 0; i < NumTanks; ++i)
            if (!IsTankOccupied(i))
                yield return new RemoveWine(i);
    }

    public State Apply(Step step)
    {
        var newContents = Contents.ToArray();
        switch (step)
        {
            case AddWine addWine:
                newContents[addWine.Tank] = new Mix(addWine.Wine, NumWines);
                break;

            case TankCombine tankCombine:
                newContents[tankCombine.InputA] = null;
                newContents[tankCombine.InputB] = null;
                newContents[tankCombine.Output] = Mix.Combine(
                    Contents[tankCombine.InputA],
                    TankSizes[tankCombine.InputA],
                    Contents[tankCombine.InputB],
                    TankSizes[tankCombine.InputB]);
                break;

            case TankSplit tankSplit:
                newContents[tankSplit.OutputA] = Contents[tankSplit.Input];
                newContents[tankSplit.OutputB] = Contents[tankSplit.Input];
                newContents[tankSplit.Input] = null;
                break;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(step));
        }

        return new State(TankSizes, newContents, NumWines);
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        for (var i = 0; i < NumTanks; ++i)
        {
            sb.AppendLine($"Tank {i} has size {TankSizes[i]} and contains {Contents[i]}");
        }
        return sb.ToString();
    }

    public double BestDistance(Mix target)
    {
        return BestTank(target).Item2;
    }

    public (int, double, Mix) BestTank(Mix target)
    {
        var d = double.MaxValue;
        var index = 0;
        for (var i = 1; i < NumTanks; ++i)
        {
            var d2 = target.DistanceFrom(this[i]);
            if (d2 < d)
            {
                d = d2;
                index = i;
            }
        }

        return (index, d, this[index]);  
    }

    public int UsedWines()
    {
        var used = new bool[NumWines];
        foreach (var mix in Contents)
        {
            if (mix != null)
            {
                for (var i = 0; i < mix.Values.Count; ++i)
                {
                    if (mix.Values[i] > 0)
                        used[i] = true;
                }
            }
        }

        return used.Count(x => x);
    }

    // TEMP: disaster
    /*
    public double TotalScore(Mix target)
    {
        var mix = (Mix?)null;
        for (var i = 0; i < Contents.Count; ++i)
        {
            if (mix == null)
                mix = Contents[i];
            else
            {
                if (Contents[i] != null)
                {
                    mix += Contents[i] * TankSizes[i];
                }
            }
        }

        if (mix != null)
            return double.MaxValue;

        return mix.Normal().DistanceFrom(target);
    }
    */
}