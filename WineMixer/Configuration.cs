using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;

namespace WineMixer;

/// <summary>
/// Used to store the sizes of each tanks, and the number of wines.
/// This class also pre-computes possible steps to reduce over-computation.
/// </summary>
public class Configuration
{
    public Configuration(IReadOnlyList<int> sizes, Mix target, Options options)
    {
        Target = target;
        Sizes = sizes;
        Options = options;
        ValidTankSplits = ComputeValidTankSplits().ToList();
        ValidTankCombines = ComputeValidTankCombines().ToList();
        ValidAddWines = ComputeAddWines().ToList();
        TankLists = ComputeTankLists(new TankList(0), 0, Options.MaxInputOrOutputTanks).ToList();
        Volume = Sizes.Sum();
    }

    public IReadOnlyList<int> Sizes { get; }
    public int Count => Sizes.Count;
    public int NumWines => Target.Count;
    public int Volume { get; }
    public Mix Target { get; }
    public Options Options { get; }

    public IReadOnlyList<TankSplit> ValidTankSplits { get; }
    public IReadOnlyList<TankCombine> ValidTankCombines { get; }
    public IReadOnlyList<AddWine> ValidAddWines { get; }
    public IReadOnlyList<TankList> TankLists { get; }

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

    public double TargetDistance(Mix? mix)
    {
        return Target.DistanceOfNormals(mix);
    }

    public static Configuration LoadTankSizesFromFile(string fileName, Mix target, Options options)
    {
        var lines = File.ReadAllLines(fileName);
        var xs = lines.Select(int.Parse).ToList();
        return new Configuration(xs, target, options);
    }

    public static Configuration LoadFromFiles(string tankSizeFileName, string wineMixFileName, string optionsFileName)
    {
        var target = Mix.LoadFromFile(wineMixFileName);
        var options = new Options();
        if (!string.IsNullOrEmpty(optionsFileName) && File.Exists(optionsFileName))
        {
            var optionsText = File.ReadAllText(optionsFileName);
            options = JsonSerializer.Deserialize<Options>(optionsText);
        }

        return LoadTankSizesFromFile(tankSizeFileName, target, options);
    }

    /// <summary>
    /// This algorithm is O(N^MaxDepth)
    /// </summary>
    public IEnumerable<TankList> ComputeTankLists(TankList parent, int depth, int maxDepth)
    {
        if (depth == maxDepth)
            yield break;

        for (var i = parent.Last + 1; i < Count; i++)
        {
            var vol = parent.Volume + Sizes[i];
            var tmp = parent.Tanks.ToList();
            tmp.Add(i);
            var child = new TankList(vol, tmp);
            Debug.Assert(child.IsValid());
            yield return child;
            foreach (var desc in ComputeTankLists(child, depth + 1, maxDepth))
            {
                Debug.Assert(desc.IsValid());
                yield return desc;
            }
        }
    }
}