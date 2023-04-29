namespace Champagne;

// Possible transitions:
// 1. add two wines and combine them 
// 2. add one wine and combine it with an existing one.
// 3. combine two existing wines 

public class Transition
{
    public Transition Prev { get; }
    public State PrevState { get; }
    public State CurrentState { get; }
    public AddWine? AddWineA { get; }
    public AddWine? AddWineB { get; }
    public TankCombine Combine { get; }
    public TankSizes TankSizes => PrevState.TankSizes;
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

    public IReadOnlyList<Transition> Transitions { get; set; }
    public bool IsValid { get;}

    public Transition(Transition prev, State state, TankCombine? combine, AddWine? addWineA, AddWine? addWineB)
    {
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
        return PrevState.IsTankOccupied(tank) || AddWineA?.Tank == tank || AddWineB?.Tank == tank;
    }

    public int ComputeTransitions()
    {
        Transitions ??= GetPossibleTransitions();
        return Transitions.Count;
    }

    private IReadOnlyList<Transition> GetPossibleTransitions()
    {
        if (!IsValid) return Array.Empty<Transition>();
        if (CurrentState == null) return Array.Empty<Transition>();

        var r = new List<Transition>();
        foreach (var tc in TankSizes.ValidTankCombines)
        {
            r.Add(new Transition(this, CurrentState, tc, null, null));
            
            for (var w = 0; w < NumWines; ++w)
            {
                var addWineA = new AddWine(tc.InputA, w);
                r.Add(new Transition(this, CurrentState, tc, addWineA, null));

                for (var w2 = 0; w2 < NumWines; ++w)
                {
                    var addWineB = new AddWine(tc.InputB, w);
                    r.Add(new Transition(this, CurrentState, tc, addWineA, addWineB));
                }
            }
        }

        r = r.Where(t => t.IsValid).ToList();
        return r;
    }
}