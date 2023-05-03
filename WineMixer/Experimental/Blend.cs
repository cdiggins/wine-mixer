/*
 * This is a work in progress to consider an alternative approach.
 * Rather than computing actual wines, this looks at the possibility
 * of looking at the possible percentages that can be made regardless of inputs
 * and from there we can reverse the process to figure out which wines we put where.
 * This may turn out to significantly reduce the search space that needs to be considered.
 *
 * The thing is that once wine is blended into a container, that container cannot be reused, unless
 * it is blended. We fill everything up eventually.
 *
 * So the other thing to consider is the space of combinations ... nothing else.
 * How big does it really get?
 *
 * Alternatively I can consider ... more like this: all of the combines possible on the first step.
 * All of the combines possible on the second step.
 *
 * Maybe it shrinks really quickly.
 *
 * Tanks might be dead-ends: can't be combined into other thanks.
 * A tank is only interesting if it can potentially contain the wine we want.
 *
 * There are only so many combine steps we can take.
 * Eventually we run out of space (and possible transitions). 
 *
 */
namespace WineMixer.Experimental
{
    public class Tank
    {
        public int Size { get; }
        public List<Tank> InputTanks { get; } = new List<Tank>();
        public List<Blend> PossibleBlends { get; } = new List<Blend>();
    }

    public class TankSet
    {
        public TankSizes Sizes { get; }
        public List<Tank> Tanks { get; } = new List<Tank>();
    }

    public class Blend
    {
        public List<double> Amounts { get; } = new List<double>();

        public static Blend Combine(Blend b1, double amount1, Blend b2, double amount2)
        {
            var r = new Blend();
            foreach (var x in b1.Amounts)
            {
                r.Amounts.Add(x * amount1 / amount1 + amount2);
            }
            foreach (var x in b2.Amounts)
            {
                r.Amounts.Add(x * amount2 / amount1 + amount2);
            }

            return r;
        }

        /// <summary>
        /// Represents all of the possibilities of mixing amounts of wine. 
        /// </summary>
        public static IEnumerable<Blend> ComputePossibilities(List<Blend> blends1, double amount1, List<Blend> blends2, double amount2)
        {
            foreach (var b1 in blends1)
            {
                foreach (var b2 in blends2)
                {
                    yield return Combine(b1, amount1, b2, amount2);
                }
            }
        }
    }
}
