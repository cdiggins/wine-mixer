namespace Champagne;

// Possible transitions:
// 1. add two wines and combine them 
// 2. add one wine and combine it with an existing one.
// 3. combine two existing wines 

public class Transition
{
    public Mix Target { get; }
    public State State { get; } 
    public Transition Previous { get; }

    // AddWine A
    // AddWine B 
    // CombineWines (A, B) or existing. 

    public Step Step { get; }
    public List<Step> NextSteps { get; set; } = new();
    public double Distance { get; }
    public Transition(Transition previous, Step step)
    {
        Target = previous.Target;
        Previous = previous;
        Step = step;
        State = previous.State.Apply(step);
        ComputeNextSteps();
        Distance = State.Distance(Target);
    }

    public Transition(State source, Mix target)
    {
        State = source;
        Target = target;
        ComputeNextSteps();
        Distance = State.Distance(Target);
    }

    public void ComputeNextSteps()
    {
        var steps = State.GetValidSteps(Target.NumWines);

        // If you added wine twice in a row, don't add more you either combine or split. 
        if (LastTwoStepsAreAdds())
        {
            steps = steps.Where(x => x is not AddWine);
        }

        // TODO: validate whether this makes a difference 
        // BUG : This does not do what we want.
        //steps = State.RemoveBadCombines(steps, Target);

        NextSteps = steps.ToList();
    }

    public bool LastTwoStepsAreAdds()
    {
        return Step is AddWine && Previous?.Step is AddWine;
    }

    public bool IsStuck 
        => NextSteps.Count == 0;

    public IReadOnlyList<Transition> GetPossibleTransitions()
    {
        var r = NextSteps.Select(step => new Transition(this, step));
        
        // TODO: Validate: Do not include transitions where the distance is less
        r = r.Where(p => p.Distance <= Distance);

        // TODO: log the difference between NextSteps.Count and r.Count 
        var xs = r.ToList();
        return xs;
    }
}