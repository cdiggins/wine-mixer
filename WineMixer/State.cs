using System.Diagnostics;
using System.Text;
using WineMixer.Serialization;

namespace WineMixer;

/// <summary>
/// Represents the state of the tanks: which ones have wine, and what the blend of wines
/// currently is in each tank. If we think of wine blending as a graph problem
/// the state is a node in the graph.  
/// </summary>
public class State
{
    public static State Create(Configuration configuration)
    {
        var contents = new Mix?[configuration.NumTanks];

        // TODO: for now the first N tanks contain the first N wines. 
        var numWines = configuration.NumWines;
        for (var i = 0; i < numWines; ++i)
        {
            var mix = Mix.CreateFromIndex(i, numWines);
            contents[i] = mix * configuration.Sizes[i];
        }

        return new State(configuration, contents);
    }

    public State(Configuration configuration, IReadOnlyList<Mix?> contents, int depth = 0)
    {
        Debug.Assert(configuration.Sizes.Count == contents.Count);
        Configuration = configuration;
        Contents = contents;
        Depth = depth;
        StateId = _nextStateId++;
        var i = 0;

        var sum = Configuration.EmptyMix;
        foreach (var mix in Contents)
        {
            if (mix != null)
            {
                UsedTanks++;
                Volume += GetTankSize(i);

                var dist = mix.DistanceOfNormals(NormalizedTarget);
                sum += mix;

                if (dist < BestMixDistance)
                {
                    BestMixDistance = Math.Min(dist, BestMixDistance);
                    BestMix = mix;
                }
            }
            i++;

        }

        AverageMix = sum / UsedTanks;
        AverageMixDistance = TargetDistance(AverageMix);
        TotalWine = Contents.Sum(m => m?.Sum ?? 0);
    }

    private static int _nextStateId;

    public int NumTanks => Configuration.NumTanks;
    public int NumWines => Configuration.NumWines;
    public Configuration Configuration { get; }
    public IReadOnlyList<Mix?> Contents { get; }
    public IEnumerable<Mix> Mixes => Contents.Where(m => m != null);
    public Mix UnnormalizedTarget => Configuration.Target;
    public Mix NormalizedTarget => Configuration.Target.Normal;
    public int Depth { get; }
    public int StateId { get; }
    public double Volume { get; }
    public int UsedTanks { get; }
    public double TotalWine { get; }
    public IReadOnlyList<Transfer> Transfers => ComputeTransfers();
    private List<Transfer> _transfers;

    public Mix AverageMix { get; }
    public double AverageMixDistance { get; } = double.MaxValue;
    public Mix BestMix { get; }
    public Mix ScaledBestMix => BestMix * Configuration.Target.Sum; 
    public double BestMixDistance { get; } = double.MaxValue;

    public bool IsOccupied(int i) 
        => Contents[i] != null;

    public Mix? this[int i]
        => Contents[i];

    public double GetTankSize(int i)
        => Configuration[i];

    public double TargetDistance(Mix? mix) 
        => Configuration.TargetDistance(mix);

    public IEnumerable<TankList> OccupiedTankLists()
        => Configuration.TankLists.Where(IsOccupied);

    public IEnumerable<TankList> UnoccupiedTankLists(int volume)
        => Configuration.TankLists.Where(tl => tl.Volume == volume && !tl.Tanks.Any(IsOccupied));

    private IReadOnlyList<Transfer> ComputeTransfers()
    {
        if (_transfers == null) 
        {
            _transfers = new List<Transfer>();
            foreach (var occ in OccupiedTankLists())
            {
                var volume = occ.Volume;
                foreach (var unocc in UnoccupiedTankLists(volume))
                {
                    if (occ.Count > 1 || unocc.Count > 1)
                        _transfers.Add(new Transfer(occ, unocc));
                }
            }
        }
        return _transfers;
    }

    public Mix GetMix(TankList tanks)
        => tanks.Tanks.Aggregate(Configuration.EmptyMix, (mix, tank) => mix + Contents[tank]);

    public Mix GetMix(Transfer transfer)
        => GetMix(transfer.Inputs);

    public bool IsOccupied(TankList tanks)
        => tanks.Tanks.All(IsOccupied);

    public bool IsUnoccupied(TankList tanks)
        => !tanks.Tanks.Any(IsOccupied);

    public bool IsTransferValid(Transfer transfer)
        => transfer.Inputs.Volume == transfer.Outputs.Volume
        && transfer.Inputs.IsValid()
        && transfer.Outputs.IsValid()
        && IsOccupied(transfer.Inputs)
        && IsUnoccupied(transfer.Outputs);

    public State Apply(Transfer transfer)
    {
        if (transfer.Inputs.Count == 0) 
            throw new Exception("Transfer cannot have 0 inputs");

        Debug.Assert(IsTransferValid(transfer));

        var mix = GetMix(transfer);
        var newContents = Contents.ToArray();

        foreach (var inputTank in transfer.Inputs.Tanks)
            newContents[inputTank] = null;

        var totalVolume = transfer.Outputs.Volume;
        Debug.Assert(mix.Sum.AlmostEquals(totalVolume));

        var testSum = 0.0;
        foreach (var outputTank in transfer.Outputs.Tanks)
        {
            if (newContents[outputTank] != null)
                throw new Exception("Attempting to transfer to a non-empty container");

            var relativeVolume = GetTankSize(outputTank) / totalVolume;
            var relativeMix = mix * relativeVolume;
            testSum += relativeMix.Sum;
            newContents[outputTank] = relativeMix;
        }

        // Just make sure things are adding up. 
        Debug.Assert(testSum.AlmostEquals(totalVolume));

        // Get the new state
        var r = new State(Configuration, newContents, Depth+1);

        // Assure that the amount of wine in the system is the same
        Debug.Assert(r.TotalWine.AlmostEquals(TotalWine));

        return r;
    }

    public StringBuilder BuildString(StringBuilder sb = null, bool details = true, bool contents = true)
    {
        sb ??= new StringBuilder();
        sb.AppendLine($"State {StateId} depth={Depth} volume={Volume} tanks={UsedTanks}/{NumTanks}");

        if (details)
        {
            sb.AppendLine($"# of possible transfers {Transfers.Count}");
            sb.AppendLine($"Target is {UnnormalizedTarget}");
            sb.AppendLine($"Normalized target is {NormalizedTarget}");
            sb.AppendLine($"Best mix is {BestMix} of distance {BestMixDistance:0.######}");
            sb.AppendLine($"Normalized best mix is {BestMix.Normal}");
            sb.AppendLine($"Scaled best mix is {ScaledBestMix}");
            
            // NOTE: the average mix is not interesting at this time. 
            // sb.AppendLine($"Average mix is {AverageMix?.Normal} of distance {AverageMixDistance:0.######}");
        }

        if (contents)
        {
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
        => BuildString().ToString();

    public IEnumerable<State> GetNextStates()
        => Transfers.Select(Apply);

    public IEnumerable<Mix> Offsets()
    {
        foreach (var m in Mixes)
        {
            var target = NormalizedTarget.Normal * m.Sum;
            var offset = target - m;
            // Just check that the offset + the 
            var tmp = m + offset;
            Debug.Assert(TargetDistance(tmp).AlmostEquals(0));
            yield return m + offset * 2;
        }
    }

    public void CheckTotalWineIsValid()
    {
        if (!TotalWine.AlmostEquals(Configuration.InitialWineAmount))
            throw new Exception($"The amount of wine in the system is not correct, expected {Configuration.InitialWineAmount} but was {TotalWine}");
    }

    public void CheckThatTankAmountsAreValid()
    {
        for (var i = 0; i < Contents.Count; ++i)
        {
            if (Contents[i] == null) continue;
            var amount = Contents[i]!.Sum;
            if (!amount.AlmostEquals(GetTankSize(i)))
                throw new Exception($"Invalid amount of wine: {amount} expected {GetTankSize(i)}");
        }
    }
}