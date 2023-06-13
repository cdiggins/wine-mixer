using System.Collections;

namespace WineMixer;

/// <summary>
/// A cons list is based on the Lisp Cons data type.
/// It is a simple and efficient way to create 
/// linked lists. 
/// </summary>
public class ConsList<T> : IEnumerable<T>
{
    public T First;
    public ConsList<T> Rest;

    public ConsList(T first, ConsList<T> rest = null) 
        => (First, Rest) = (first, rest ?? Empty);

    public static ConsList<T> Empty 
        = new(default);

    public IEnumerator<T> GetEnumerator()
    {
        for (var node = this; node != Empty; node = node.Rest)
            yield return node.First;
    }

    IEnumerator IEnumerable.GetEnumerator() 
        => ((IEnumerable<T>)this).GetEnumerator();
}