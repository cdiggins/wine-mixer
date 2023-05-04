using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WineMixer;

public static class Extensions
{
    public static IEnumerable<Transition> GetTransitions(this State state)
    {
        var t = new Transition(null, state, null, null, null);
        return t.GetOrComputeTransitions();
    }

    private static readonly Random Rng = new();

    public static Operation? RandomOperation(State state)
    {
        return state.GetValidOperations().ToList().GetRandomElement();
    }

    public static T? GetRandomElement<T>(this IReadOnlyList<T> self)
    {
        return self.Count == 0 
            ? default : self[Rng.Next(self.Count)];
    }

    public static State ApplyRandomOperation(this State state)
    {
        return state.Apply(RandomOperation(state));
    }

    public static (TankCombine?, Mix?) BestCombine(this State state, Mix target)
    {
        var tc = state.GetValidTankCombineOperations().MinBy(tc => target.Distance(state.CombineResult(tc.InputA, tc.InputB)));
        return tc != null 
            ? (tc, state.CombineResult(tc.InputA, tc.InputB)) 
            : (null, null);
    }
}