namespace WineMixer;

/// <summary>
/// Good example of dynamic programming.
/// This creates a data structure roughly n^2 which
/// is used to quickly look-up permutations.
/// This was motivated gby the question: is there a fast way to
/// compute the tanks that add up to a specific volume.
/// The answer is yes. 
/// </summary>
public class NewTankFinder2 
{
    public NewTankFinder2(IReadOnlyList<int> tankSizes, int maxSize)
        => TankSizes = tankSizes;

    public IReadOnlyList<int> TankSizes { get; }

    public int NumTanks 
        => TankSizes.Count;

    public int GetTankSize(int n)
        => TankSizes[n];

    public int GetTankSizeSum(IEnumerable<int> tanks)
        => tanks.Sum(GetTankSize);

    public IEnumerable<List<int>> GetPermutationsOfVolume(int target)
    {
        return GetPermutationsOfVolume(target, 0, new List<int>(), 0);
    }

    public IEnumerable<List<int>> GetPermutationsOfVolume(int target, int current, List<int> prevList, int curIndex)
    {
        if (current == target)
            yield return prevList;
        if (current >= target)
            yield break;
        if (curIndex >= NumTanks) 
            yield break;
        foreach (var tmp in GetPermutationsOfVolume(target, current, prevList, curIndex + 1))
            yield return tmp;
        current += GetTankSize(curIndex);
        if (current <= target)
        {
            var nextList = prevList.Append(curIndex).ToList();
            if (current == target)
                yield return nextList;
            else 
                foreach (var tmp in GetPermutationsOfVolume(target, current, nextList, curIndex + 1))
                    yield return tmp;
        }
    }
}


/// <summary>
/// Good example of dynamic programming.
/// This creates a data structure roughly n^2 which
/// is used to quickly look-up permutations.
/// This was motivated gby the question: is there a fast way to
/// compute the tanks that add up to a specific volume.
/// The answer is yes. 
/// </summary>
public class NewTankFinder : IEqualityComparer<List<int>>
{
    public NewTankFinder(IReadOnlyList<int> tankSizes, int maxSize)
    {
        TankSizes = tankSizes;
        Lookup = new List<List<int>>[maxSize + 1];
    }

    public IReadOnlyList<int> TankSizes { get; }

    public int NumTanks => TankSizes.Count;

    /// <summary>
    /// Key is a tank volume V
    /// Value is a list of tank sizes (Xi) for which
    /// a non-zero set of permutations adding up to V-Xi exists
    /// Worst case space complexity is N^2 where N is number of tanks. 
    /// </summary>
    public List<List<int>>[] Lookup;

    public int GetTankSize(int n)
        => TankSizes[n];

    public int GetTankSizeSum(IEnumerable<int> tanks)
        => tanks.Sum(GetTankSize);

    /// <summary>
    /// Returns a sequence of lists of tank indices 
    /// </summary>
    public IEnumerable<List<int>> GetPermutationsOfVolume(int volume)
    {
        if (volume == 0)
            return new[] { new List<int>() };
        if (volume < 0)
            return Enumerable.Empty<List<int>>();

        // Check if we have to compute the permutations for this volume 
        if (Lookup[volume] == null)
        {
            Lookup[volume] = new List<List<int>>();

            // This is the list of tanks, for which there exist permutations V - X
            // In other words, I can take that tank, and add it to a one or more 
            // permutations of the remainder size. 
            for (var i = 0; i < NumTanks; ++i)
            {
                var size = GetTankSize(i);

                if (size <= volume)
                {
                    // Are there any permutations for the remaining volume? 
                    foreach (var group in GetPermutationsOfVolume(volume - size))
                    {
                        // Make sure that the group doesn't contain this tank. 
                        // This could be accelerated 
                        if (!group.Contains(i))
                        {
                            var group2 = group.ToList();
                            group2.Add(i);
                            group2.Sort();
                            Lookup[volume].Add(group2);
                        }
                    }
                }
            }

            // Remove duplicates. 
            Lookup[volume] = Lookup[volume].Distinct(this).ToList();
        }

        return Lookup[volume];
    }

    public IEnumerable<List<int>> NaiveRecursive(int volume)
    {
        if (volume == 0)
            yield return new List<int>();
        if (volume <= 0)
            yield break;
        for (var i = 0; i < NumTanks; i++)
        {
            var size = TankSizes[i];
            foreach (var tmp in NaiveRecursive(volume - size))
            {
                if (tmp.Contains(i))
                    continue;
                var tmp2 = tmp.ToList();
                tmp2.Add(i);
                yield return tmp2;
            }
        }
    }

    public bool Equals(List<int> x, List<int> y)
    {
        return x.SequenceEqual(y);
    }

    public int GetHashCode(List<int> obj)
    {
        return obj.Aggregate(0, HashCode.Combine);
    }
}


/// <summary>
/// Good example of dynamic programming.
/// This creates a data structure roughly n^2 which
/// is used to quickly look-up permutations.
/// This was motivated gby the question: is there a fast way to
/// compute the tanks that add up to a specific volume.
/// The answer is yes. 
/// </summary>
public class TankListFinder
{
    public TankListFinder(IReadOnlyList<int> tankSizes, int maxSize)
    {
        TankSizes = tankSizes;
        Lookup = new List<ConsList<int>>[maxSize + 1];
    }

    public IReadOnlyList<int> TankSizes { get; }

    public int NumTanks => TankSizes.Count;

    /// <summary>
    /// Key is a tank volume V
    /// Value is a list of tank sizes (Xi) for which
    /// a non-zero set of permutations adding up to V-Xi exists
    /// Worst case space complexity is N^2 where N is number of tanks. 
    /// </summary>
    public List<ConsList<int>>[] Lookup;

    public int GetTankSize(int n)
        => TankSizes[n];

    public int GetTankSizeSum(IEnumerable<int> tanks)
        => tanks.Sum(GetTankSize);

    /// <summary>
    /// Returns a sequence of lists of tank indices 
    /// </summary>
    public IEnumerable<ConsList<int>> GetPermutationsOfVolume(int volume)
    {
        if (volume == 0)
            return new[] { ConsList<int>.Empty };
        if (volume < 0)
            return Enumerable.Empty<ConsList<int>>();

        // Check if we have to compute the permutations for this volume 
        if (Lookup[volume] == null)
        {
            Lookup[volume] = new List<ConsList<int>>();

            // This is the list of tanks, for which there exist permutations V - X
            // In other words, I can take that tank, and add it to a one or more 
            // permutations of the remainder size. 
            for (var i = 0; i < NumTanks; ++i)
            {
                var size = GetTankSize(i);

                if (size <= volume)
                {
                    // Are there any permutations that don't contain this tank? 
                    foreach (var group in GetPermutationsOfVolume(volume - size))
                    {
                        if (!group.Contains(i))
                        {
                            Lookup[volume].Add(group.Prepend(i));
                        }
                    }
                }
            }
        }

        return Lookup[volume];
    }

    public IEnumerable<ConsList<int>> NaiveRecursive(int volume)
    {
        if (volume == 0)
            yield return ConsList<int>.Empty;
        if (volume <= 0)
            yield break;
        for (var i = 0; i < NumTanks; i++)
        {
            var size = TankSizes[i];
            foreach (var tmp in NaiveRecursive(volume - size))
            {
                if (tmp.Contains(i))
                    continue;
                yield return tmp.Prepend(i);
            }
        }
    }
}

/// <summary>
/// Good example of dynamic programming.
/// This creates a data structure roughly n^2 which
/// is used to quickly look-up permutations.
/// This was motivated gby the question: is there a fast way to
/// compute the tanks that add up to a specific volume.
/// The answer is yes. 
/// </summary>
public class ListBasedTankListFinder
{
    public ListBasedTankListFinder(IReadOnlyList<int> tankSizes, int maxSize)
    {
        TankSizes = tankSizes;
        Lookup = new List<List<int>>[maxSize + 1];
    }

    public IReadOnlyList<int> TankSizes { get; }

    public int NumTanks => TankSizes.Count;

    /// <summary>
    /// Key is a tank volume V
    /// Value is a list of tank sizes (Xi) for which
    /// a non-zero set of permutations adding up to V-Xi exists
    /// Worst case space complexity is N^2 where N is number of tanks. 
    /// </summary>
    public List<List<int>>[] Lookup;

    public int GetTankSize(int n)
        => TankSizes[n];

    public int GetTankSizeSum(IEnumerable<int> tanks)
        => tanks.Sum(GetTankSize);

    /// <summary>
    /// Returns a sequence of lists of tank indices 
    /// </summary>
    public IEnumerable<List<int>> GetPermutationsOfVolume(int volume)
    {
        if (volume == 0)
            return new[] { new List<int>() };
        if (volume < 0)
            return Enumerable.Empty<List<int>>();

        // Check if we have to compute the permutations for this volume 
        if (Lookup[volume] == null)
        {
            Lookup[volume] = new List<List<int>>();

            // This is the list of tanks, for which there exist permutations V - X
            // In other words, I can take that tank, and add it to a one or more 
            // permutations of the remainder size. 
            for (var i = 0; i < NumTanks; ++i)
            {
                var size = GetTankSize(i);

                if (size <= volume)
                {
                    // Are there any permutations for the remaining volume? 
                    foreach (var group in GetPermutationsOfVolume(volume - size))
                    {
                        // Make sure that the group doesn't contain this tank. 
                        // This could be accelerated 
                        if (!group.Contains(i))
                        {
                            var group2 = group.ToList();
                            group2.Add(i);
                            group2.Sort();

                            // TODO: make sure that the new group has not already been created. 
                            Lookup[volume].Add(group2);
                        }
                    }
                }
            }
        }

        return Lookup[volume];
    }

    public IEnumerable<List<int>> NaiveRecursive(int volume)
    {
        if (volume == 0)
            yield return new List<int>();
        if (volume <= 0)
            yield break;
        for (var i = 0; i < NumTanks; i++)
        {
            var size = TankSizes[i];
            foreach (var tmp in NaiveRecursive(volume - size))
            {
                if (tmp.Contains(i))
                    continue;
                var tmp2 = tmp.ToList();
                tmp2.Add(i);
                yield return tmp2;
            }
        }
    }
}

/// <summary>
/// Good example of dynamic programming.
/// This creates a data structure roughly n^2 which
/// is used to quickly look-up permutations.
/// This was motivated gby the question: is there a fast way to
/// compute the tanks that add up to a specific volume.
/// The answer is yes. 
/// </summary>
public class OldTankListFinder
{
    public OldTankListFinder(IReadOnlyList<int> tankSizes, int maxSize)
    {
        TankSizes = tankSizes;
        Lookup = new List<int>[maxSize + 1];
    }

    public IReadOnlyList<int> TankSizes { get; }

    public int NumTanks => TankSizes.Count;

    /// <summary>
    /// Key is a tank volume V
    /// Value is a list of tank sizes (Xi) for which
    /// a non-zero set of permutations adding up to V-Xi exists
    /// Worst case space complexity is N^2 where N is number of tanks. 
    /// </summary>
    public List<int>[] Lookup;

    public int GetTankSize(int n)
        => TankSizes[n];

    public int GetTankSizeSum(IEnumerable<int> tanks)
        => tanks.Sum(GetTankSize);

    /// <summary>
    /// Returns a sequence of lists of tank indices 
    /// </summary>
    public IEnumerable<List<int>> GetPermutationsOfVolume(int volume)
    {
        if (volume == 0)
            yield return new List<int>();
        if (volume <= 0)
            yield break;

        // Check if we have to compute the permutations for this volume 
        if (Lookup[volume] == null)
        {
            // This is the list of tanks, for which there exist permutations V - X
            // In other words, I can take that tank, and add it to a one or more 
            // permutations of the remainder size. 
            var tanks = new List<int>();
            for (var i = 0; i < NumTanks; ++i)
            {
                var size = GetTankSize(i);

                // Are there any permutations that don't contain this tank? 
                if (GetPermutationsOfVolume(volume - size)
                    .Any(xs => !xs.Contains(i)))
                {
                    tanks.Add(i);
                }
            }

            Lookup[volume] = tanks;
        }

        // Get the list of tank indices that have non-zero 
        // number of permutations adding up to 
        foreach (var tank in Lookup[volume])
        {
            // Get all permutations by adding the stored volume to the list of all
            // sub-permutations  
            var size = GetTankSize(tank);
            foreach (var list in GetPermutationsOfVolume(volume - size))
            {
                if (list.Contains(tank))
                    continue;
                list.Add(tank);
                //Debug.Assert(GetTankSizeSum(list) == volume);
                yield return list;
            }
        }
    }

    public IEnumerable<List<int>> NaiveRecursive(int volume)
    {
        if (volume == 0)
            yield return new List<int>();
        if (volume <= 0)
            yield break;
        for (var i = 0; i < NumTanks; i++)
        {
            var size = TankSizes[i];
            foreach (var tmp in NaiveRecursive(volume - size))
            {
                if (tmp.Contains(i))
                    continue;
                tmp.Add(i);
                yield return tmp;
            }
        }
    }
}