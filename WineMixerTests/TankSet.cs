using WineMixer;

namespace WineMixerTests;

public class TankSet 
{
    public string Text { get; }
    public Configuration Sizes { get; }
    public IReadOnlyList<int> Tanks { get; }

    public TankSet(Configuration sizes, params int[] tanks)
        : this(sizes, (IEnumerable<int>)tanks)
    { }

    public TankSet(Configuration sizes, IEnumerable<int> tanks)
    {
        Sizes = sizes;
        Tanks = tanks.ToList();
        Text = string.Join(", ", Tanks.Select(t => Sizes[t]));
    }

    public TankSet(Configuration sizes, params TankSet[] tankSets)
        : this(sizes, tankSets.SelectMany(ts => ts.Tanks))
    { }

    public override string ToString()
        => Text;

    public override int GetHashCode()
        => Text.GetHashCode();

    public override bool Equals(object? obj)
        => obj is TankSet ts && ts.Text.Equals(Text);
}