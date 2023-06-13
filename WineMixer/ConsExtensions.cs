namespace WineMixer;

public static class ConsExtensions
{
    public static ConsList<T> ToConsList<T>(this T any)
        => new(any);

    public static ConsList<T> Prepend<T>(this ConsList<T> list, T value)
        => new(value, list);

    // TODO: benchmark this function. 
    public static bool Contains<T>(this ConsList<T> list, T value)
    {
        for (; list != ConsList<T>.Empty; list = list.Rest)
            if (list.First.Equals(value))
                return true;
        return false;
    }
}