namespace WineMixer;

public class TankFinder
{
    public TankFinder(IReadOnlyList<int> tankSizes, int maxSize)
        => TankSizes = tankSizes;

    public IReadOnlyList<int> TankSizes { get; }

    public Dictionary<int, List<List<int>>> Lookup { get; } = new();

    public int NumTanks
        => TankSizes.Count;

    public int GetTankSize(int n)
        => TankSizes[n];

    public int GetTankSizeSum(IEnumerable<int> tanks)
        => tanks.Sum(GetTankSize);

    public IEnumerable<List<int>> GetPermutationsOfVolume(int target, int maxCount)
        => GetPermutationsOfVolume(target, maxCount, 0, new List<int>(), 0);

    public IEnumerable<List<int>> GetPermutationsOfVolume(int target, int maxCount, int current, List<int> prevList, int curIndex)
    {
        if (current == target)
            yield return prevList;
        if (current >= target)
            yield break;
        if (curIndex >= NumTanks)
            yield break;
        foreach (var tmp in GetPermutationsOfVolume(target, maxCount, current, prevList, curIndex + 1))
            yield return tmp;
        current += GetTankSize(curIndex);
        if (current <= target && prevList.Count < maxCount)
        {
            var nextList = prevList.Append(curIndex).ToList();
            if (current == target)
                yield return nextList;
            else
                foreach (var tmp in GetPermutationsOfVolume(target, maxCount, current, nextList, curIndex + 1))
                    yield return tmp;
        }
    }
}
