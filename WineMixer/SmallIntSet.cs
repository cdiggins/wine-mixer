using System.Collections;

namespace WineMixer;

/// <summary>
/// This is an efficient representations of a small set of integers
/// that don't have a big value.
/// </summary>
public class SmallIntSet : BitArray
{
    public SmallIntSet(int count)
        : base(count)
    { }

    public SmallIntSet(SmallIntSet other)
        : base(other) 
    { }

    public void Add(int n)
        => this[n] = true;

    public void Remove(int n)
        => this[n] = false;

    public bool Contains(int n)
        => this[n];

    public new SmallIntSet Clone()
        => new(this);
}