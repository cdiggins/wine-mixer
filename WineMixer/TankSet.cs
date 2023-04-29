namespace Champagne;

public class TankSet
{
    public TankSizes TankSizes { get; }
    public IReadOnlyList<int> Indices { get; }
    public int TotalSize { get; }
    public int NumTanks => Indices.Count;

    public TankSet(TankSizes tankSizes, params int[] indices)
    {
        TankSizes = tankSizes;
        Indices = indices;

        if (indices.Distinct().Count() != indices.Length)
            throw new ArgumentException("Can't have repeated indices");

        foreach (var i in indices)
        {
            if (i < 0 || i >= TankSizes.Sizes.Count)
                throw new ArgumentOutOfRangeException(nameof(indices));
            TotalSize += TankSizes.Sizes[i];
        }
    }
}