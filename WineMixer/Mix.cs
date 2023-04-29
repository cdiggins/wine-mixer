using Microsoft.VisualBasic.CompilerServices;

namespace Champagne;

public class Mix
{
    public IReadOnlyList<double> Values { get; }
    public int NumWines => Values.Count;

    public double DistanceFrom(Mix? other)
    {
        if (other == null)
            return double.MaxValue;
        var sumSqrDeltas = 0.0;
        for (var i = 0; i < Values.Count; i++)
        {
            sumSqrDeltas += Math.Pow(Values[i] - other.Values[i], 2);
        }
        return Math.Sqrt(sumSqrDeltas);
    }

    public static Mix Add(Mix mixA, Mix mixB)
    {
        var tmp = new double[mixA.NumWines];
        for (var i = 0; i < mixA.NumWines; ++i)
        {
            tmp[i] = mixA.Values[i] + mixB.Values[i];
        }

        return new(tmp);
    }

    public static Mix operator +(Mix a, Mix b)
        => Add(a, b);

    public static Mix operator *(Mix mix, double x)
        => Multiply(mix, x);

    public static Mix Multiply(Mix mix, double x)
    {
        var tmp = new double[mix.NumWines];
        for (var i = 0; i < mix.NumWines; ++i)
        {
            tmp[i] = mix.Values[i] * x;
        }

        return new(tmp);
    }

    public Mix(IReadOnlyList<double> values)
        => Values = values;

    public static Mix Normalize(Mix mix)
        => mix * (1.0 / mix.GetLength());

    public Mix Normal()
        => Normalize(this);

    public static Mix Combine(Mix m1, double w1, Mix m2, double w2)
    {
        var tmp = new double[m1.NumWines];
        var sum = w1 + w2;
        for (var i = 0; i < m1.NumWines; ++i)
        {
            tmp[i] = ((m1.Values[i] * w1) + (m2.Values[i] * w2)) / sum;
        }

        return new Mix(tmp);
    }

    public double GetLength()
    {
        var sumSqrDeltas = 0.0;
        for (var i = 0; i< Values.Count; i++)
        {
            sumSqrDeltas += Math.Pow(Values[i], 2);
        }
        return Math.Sqrt(sumSqrDeltas);
    }

    public Mix(int i, int numWines)
    {
        var tmp = new double[numWines]; 
        tmp[i] = 1.0;
        Values = tmp;
    }

    public override string ToString()
    {
        return "(" + string.Join(", ", Values) + ")";
    }

    public static Mix LoadFromFile(string fileName)
    {
        var lines = File.ReadAllLines(fileName);
        var r = new List<double>();
        foreach (var line in lines)
        {
            var amt = double.Parse(line);
            r.Add(amt);
        }
        return new Mix(r);
    }
}