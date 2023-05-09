using System.Collections.Generic;

namespace WineMixer;

/// <summary>
/// A transition is a combination of one or more steps to achieve a new state of blended wines.
/// If wine blending is considered a graph problem a transition is an edge in the graph. 
/// A transition always consists of combining two wines, and optionally adding zero, one or two wines.
/// If wines are added a transition involves using one of those.
/// The purpose of a transition is to reduce the complexity of the search space, and to facilitate the algorithms.
/// A transition has a pointer to the previous transition, the previous state, and the new state created
/// by applying the transition. It is possible to construct an invalid transition, in which case the "IsValid"
/// flag will be false. 
/// </summary>
public class Transition
{
    public Transition Prev { get; }
    public State PrevState { get; }
    public State CurrentState { get; }
    public AddWine? AddWineA { get; }
    public AddWine? AddWineB { get; }
    public TankCombine Combine { get; }
    public Configuration Configuration => PrevState.Configuration;
    public int NumWines => PrevState.NumWines;

    public int Length
    {
        get
        {
            var cnt = 1;
            for (var x = Prev; x != null; x = x.Prev)
                cnt++;
            return cnt;
        }
    }

    private IReadOnlyList<Transition> _transitions;
    
    public bool IsValid { get;}

    public Transition(Transition? prev, State state, TankCombine? combine, AddWine? addWineA, AddWine? addWineB)
    {
        if (state == null) throw new ArgumentNullException(nameof(state));
        Prev = prev;
        PrevState = state;
        Combine = combine;
        AddWineA = addWineA;
        AddWineB = addWineB;
        IsValid = ComputeValidity();
        if (IsValid)
        {
            if (AddWineA != null)
                state = state.Apply(AddWineA);

            if (AddWineB != null)
                state = state.Apply(AddWineB);

            if (Combine != null)
                CurrentState = state.Apply(Combine);
            else
            {
                CurrentState = state;
            }
        }
    }

    public bool ComputeValidity()
    {
        if (Combine == null)
            return true;
        
        if (AddWineA == null)
        {
            if (AddWineB == null)
                return false;
        }

        if (AddWineA != null)
        {
            // If adding wine, it must be used in the combine step
            if (Combine.InputA != AddWineA.Tank && Combine.InputB != AddWineA.Tank)
                return false;
        }

        if (AddWineB != null)
        {
            // If adding wine, it must be used in the combine step
            if (Combine.InputA != AddWineB.Tank && Combine.InputB != AddWineB.Tank)
                return false;
        }

        // Combined tank must be occupied 
        if (!TankOccupiedOrAddedTo(Combine.InputA))
            return false;
        if (!TankOccupiedOrAddedTo(Combine.InputB))
            return false;

        return true;
    }

    public bool TankOccupiedOrAddedTo(int tank)
    {
        return PrevState.IsOccupied(tank) || AddWineA?.Tank == tank || AddWineB?.Tank == tank;
    }

    public IReadOnlyList<Transition> GetOrComputeTransitions()
    {
        return _transitions ??= GetPossibleTransitions();
    }

    private IReadOnlyList<Transition> GetPossibleTransitions()
    {
        if (!IsValid) return Array.Empty<Transition>();
        if (PrevState == null) return Array.Empty<Transition>();

        var r = new List<Transition>();
        foreach (var tc in Configuration.ValidTankCombines)
        {
            r.Add(new Transition(this, CurrentState, tc, null, null));
            
            for (var w = 0; w < NumWines; ++w)
            {
                var addWineA = new AddWine(tc.InputA, w);
                r.Add(new Transition(this, CurrentState, tc, addWineA, null));

                for (var w2 = 0; w2 < NumWines; ++w2)
                {
                    var addWineB = new AddWine(tc.InputB, w2);
                    r.Add(new Transition(this, CurrentState, tc, addWineA, addWineB));
                }
            }
        }

        r = r.Where(t => t.IsValid).ToList();
        return r;
    }

    public List<Operation> GetOperations()
    {
        var stk = new Stack<Operation>();
        var t = this;
        while (t != null)
        {
            if (t.Combine != null)
                stk.Push(t.Combine);

            if (t.AddWineB != null)
                stk.Push(t.AddWineB);

            if (t.AddWineA != null)
                stk.Push(t.AddWineA);
            
            t = t.Prev;
        }

        var r = new List<Operation>();
        while (stk.Count > 0)
        {
            r.Add(stk.Pop());
        }

        return r;
    }
}