﻿using System.Collections.Generic;
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
        OriginalTarget = target;
        Target = OriginalTarget.SumOfOne;
        Sizes = sizes;
        if (sizes.Count < NumWines)
            throw new Exception("The number of starting tanks has to be equal to or greater than the number of wines");
        Options = options;
        TankLists = ComputeTankLists(new TankList(0), 0, Options.MaxInputOrOutputTanks).ToList();
        Volume = Sizes.Sum();
        EmptyMix = new Mix(Enumerable.Repeat(0.0, NumWines).ToArray());
        InitialWineAmount = sizes.Take(NumWines).Sum();
    }

    public IReadOnlyList<int> Sizes { get; }
    public int NumTanks => Sizes.Count;
    public int NumWines => Target.Count;
    public int Volume { get; }
    public Mix Target { get; }
    public Mix OriginalTarget { get; }
    public Mix EmptyMix { get; }
    public Options Options { get; }
    public const string DefaultOptionsFileName = "options.json";
    public double InitialWineAmount { get; }

    public IReadOnlyList<TankList> TankLists { get; }

    public int this[int i]
        => Sizes[i];

    public double TargetDistance(Mix mix)
        => Target.Distance(mix.SumOfOne);

    public Mix ScaleMixToOriginalTarget(Mix mix)
        => mix.SumOfOne * OriginalTarget.Sum;

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
        else
        {
            if (string.IsNullOrEmpty(optionsFileName))
                optionsFileName = DefaultOptionsFileName;
            var optionsText = JsonSerializer.Serialize(options, new JsonSerializerOptions() { WriteIndented = true });
            File.WriteAllText(optionsFileName, optionsText);
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

        for (var i = parent.Last + 1; i < NumTanks; i++)
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