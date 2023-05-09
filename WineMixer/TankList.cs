namespace WineMixer;

/// <summary>
/// A tank list is a sequence of tanks, always ordered and with no repetitions.
/// </summary>
public class TankList
{
    public int Volume { get; }
    public int Count => Tanks.Count;
    public int this[int i] => Tanks[i];
    public IReadOnlyList<int> Tanks { get; }
    public TankList(int volume, params int[] tanks) => (Volume, Tanks) = (volume, tanks);
    public TankList(int volume, List<int> tanks) => (Volume, Tanks) = (volume, tanks);
    public bool HasTank(int n) => Tanks.Contains(n);
    public int Last => Count == 0 ? -1 : Tanks[Count - 1];
    public bool IsValid()
    {
        for (var i = 1; i < Count; ++i)
        {
            if (this[i] <= this[i - 1])
                return false;
        }

        return true;
    }
}