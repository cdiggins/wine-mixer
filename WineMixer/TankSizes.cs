namespace WineMixer;

/// <summary>
/// Used to store the sizes of each tanks, and the number of wines.
/// This class also pre-computes possible steps to reduce over-computation.
/// </summary>
public class TankSizes
{
    public TankSizes(IReadOnlyList<int> sizes, int numWines)
    {
        NumWines = numWines;
        Sizes = sizes;
        ValidTankSplits = ComputeValidTankSplits().ToList();
        ValidTankCombines = ComputeValidTankCombines().ToList();
        ValidAddWines = ComputeAddWines().ToList();
        Volume = Sizes.Sum();
    }

    public IReadOnlyList<int> Sizes { get; }
    public int Count => Sizes.Count;
    public int NumWines { get; }
    public int Volume { get; }

    public int this[int i]
        => Sizes[i];

    public bool IsValidTankCombine(TankCombine tankCombine)
        => Sizes[tankCombine.Output] == Sizes[tankCombine.InputA] + Sizes[tankCombine.InputB];

    public bool IsValidTankSplit(TankSplit tankSplit)
        => Sizes[tankSplit.Input] == Sizes[tankSplit.OutputA] + Sizes[tankSplit.OutputB];

    // Complexity O(N^3) where N = number of tanks, but called only once 
    public IEnumerable<TankSplit> ComputeValidTankSplits()
    {
        for (var i = 0; i < Count; ++i)
        for (var j = i + 1; j < Count; ++j)
        for (var k = 0; k < Count; ++k)
        {
            if (i != j && i != k && j != k)
            {
                var ts = new TankSplit(i, j, k);
                if (IsValidTankSplit(ts))
                    yield return ts;
            }
        }
    }

    // Complexity O(N^3) where N = number of tanks, but called only once 
    public IEnumerable<TankCombine> ComputeValidTankCombines()
    {
        for (var i = 0; i < Count; ++i)
        for (var j = i + 1; j < Count; ++j)
        for (var k = 0; k < Count; ++k)
        {
            if (i != j && i != k && j != k)
            {
                var tc = new TankCombine(i, j, k);
                if (IsValidTankCombine(tc))
                    yield return tc;
            }
        }
    }

    public IEnumerable<AddWine> ComputeAddWines()
    {
        for (var i = 0; i < Count; ++i)
            for (var j = 0; j < NumWines; ++j)
                yield return new AddWine(i, j);
    }

    public IReadOnlyList<TankSplit> ValidTankSplits { get; }
    public IReadOnlyList<TankCombine> ValidTankCombines { get; }
    public IReadOnlyList<AddWine> ValidAddWines { get; }

    public static TankSizes LoadFromFile(string fileName, int numWines)
    {
        var lines = File.ReadAllLines(fileName);
        var xs = lines.Select(int.Parse).ToList();
        return new TankSizes(xs, numWines);
    }
}