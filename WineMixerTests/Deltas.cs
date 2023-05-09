using System.Net.NetworkInformation;
using WineMixer;

/// <summary>
/// Given an occupied tank, and an empty, this represents
/// the mix that needs to be added to the empty tank to
/// bring the occupied tank to the target. 
/// </summary>
public class CombineDelta
{
    public int OccupiedTank { get; }
    public int EmptyTank { get; }
    public int TargetTank { get; }
    public Mix DeltaMix { get; }
    public double Length => DeltaMix.Length;

    public CombineDelta(Mix deltaMix, int occupiedTank, int emptyTank, int targetTank)
    {
        DeltaMix = deltaMix;
        OccupiedTank = occupiedTank;
        EmptyTank = emptyTank;
        TargetTank = targetTank;
    }
}

public static class DeltaExtensions
{
    public static IEnumerable<CombineDelta> GetCombineDeltas(this State state, Mix target)
    {
        foreach (var tc in state.Configuration.ValidTankCombines)
        {
            if (!state.IsOccupied(tc.Output))
            {
                var mixA = state[tc.InputA];
                var mixB = state[tc.InputB];

                if (mixA != null && mixB == null)
                {
                    var d = target - mixA;
                    yield return new CombineDelta(d, tc.InputA, tc.InputB, tc.Output);
                }

                if (mixB != null && mixA == null)
                {
                    var d = target - mixB;
                    yield return new CombineDelta(d, tc.InputB, tc.InputA, tc.Output);
                }
            }
        }
    }

    public static Mix Difference(this Mix? self, Mix target)
    {
        return self == null ? target : target - self;
    }

    /// <summary>
    /// Returns the list of mixes that represents the difference between
    /// a tank and the target. 
    /// </summary>
    public static IEnumerable<Mix> GetTankDifferences(this State state, Mix target)
    {
        return state.Contents.Select(x => x.Difference(target));
    }
}