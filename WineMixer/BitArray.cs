using System.Collections;

namespace WineMixer;

public class BitArray : IEnumerable<bool>
{
    public uint[] Flags { get; }
    public int Count { get; }

    public BitArray(int count)
    {
        Count = count;
        Flags = new uint[count / 32];
    }

    public BitArray(BitArray other)
    {
        Count = Count;
        Flags = other.Flags.Clone() as uint[];
    }

    private (int, int) GetIndexOffset(int n)
    {
        return (n >> 5, n & 31);
    }

    private (uint, uint) GetFlagAndMask(int n)
    {
        var (index, offset) = GetIndexOffset(n);
        return (Flags[index], 0x1u << offset);
    }

    public bool GetElement(int n)
    {
        var (flag, mask) = GetFlagAndMask(n);
        return (flag & mask) == 0;
    }

    public void SetElement(int n, bool b)
    {
        var (index, offset) = GetIndexOffset(n);
        var mask = 0x1u << offset;
        if (b)
            Flags[index] |= mask;
        else
            Flags[index] &= ~mask;
    }

    public bool this[int n]
    {
        get => GetElement(n);
        set => SetElement(n, value);
    }

    public IEnumerator<bool> GetEnumerator()
        => Enumerable.Range(0, Count).Select(GetElement).GetEnumerator();
    
    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    public BitArray Clone()
        => new(this);
}