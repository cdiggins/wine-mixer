using System.Diagnostics;

namespace WineMixer;

public static class Extensions
{
    const double Epsilon = 0.000001;

    public static IEnumerable<Transition> GetTransitions(this State state)
        => new Transition(null, state, null, null, null).GetOrComputeTransitions();

    private static readonly Random Rng = new();

    public static Operation? RandomOperation(State state) 
        => state.GetValidOperations().ToList().GetRandomElement();

    public static T? GetRandomElement<T>(this IReadOnlyList<T> self) 
        => self.Count == 0 ? default : self[Rng.Next(self.Count)];

    public static State ApplyRandomOperation(this State state) 
        => state.Apply(RandomOperation(state));

    public static bool AlmostEquals(this double self, double x) 
        => Math.Abs(self - x) < Epsilon;

    public static IEnumerable<State> CombineTree(this State state)
    {
        foreach (var op in state.GetValidTankCombineOperations())
        {
            var state1 = state.Apply(op);
            yield return state1;
            foreach (var state2 in state1.CombineTree())
            {
                yield return state2;
            }
        }
    }

    public static IEnumerable<State> OperationTree(this State state, int maxDepth)
    {
        if (maxDepth <= 0)
            yield break;
        ;
        foreach (var op in state.GetValidOperations())
        {
            var state1 = state.Apply(op);
            yield return state1;
            foreach (var state2 in state1.OperationTree(maxDepth - 1))
            {
                yield return state2;
            }
        }
    }

    public static Mix? Average(this IEnumerable<Mix?> mixes)
    {
        Mix? result = null;
        var i = 0;
        foreach (var m in mixes)
        {
            if (m == null)
                continue;
            if (result == null) 
                result = m;
            else
                result += m;
            i++;
        }
        return result == null 
            ? result : result / i;
    }
}