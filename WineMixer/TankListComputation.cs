using System.Diagnostics;

namespace WineMixer;

/// <summary>
/// Good example of dynamic programming? 
/// </summary>
public class TankListComputation
{
    public Configuration Config { get; set; }
    public Dictionary<int, TankList> TankListsByVolume { get; set; }

    /// <summary>
    /// This algorithm is O(N^MaxDepth)
    /// </summary>
    public IEnumerable<TankList> ComputeTankLists(TankList parent, int volume)
    {
        // The idea is to compute all tank lists up to a given number ... 
        yield break;
    }
}