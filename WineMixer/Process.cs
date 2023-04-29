namespace Champagne;

public class Process
{
    public Mix Target { get; }
    public State State { get; } 
    public Process Previous { get; }

    // AddWine A
    // AddWine B 
    // CombineWines (A, B) or existing. 

    public Step Step { get; }
    public List<Step> NextSteps { get; set; } = new();
    public double Distance { get; }
    public Process(Process previous, Step step)
    {
        Target = previous.Target;
        Previous = previous;
        Step = step;
        State = previous.State.Apply(step);
        ComputeNextSteps();
        Distance = State.Distance(Target);
    }

    public Process(State source, Mix target)
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

    public IReadOnlyList<Process> GetPossibleTransitions()
    {
        // Do not include transitions where the distance is less
        var r = NextSteps.Select(step => new Process(this, step)).Where(p => p.Distance <= Distance).ToList();
        // TODO: log the difference between NextSteps.Count and r.Count 
        return r;
    }
}

public class ProcessTree
{
    public ProcessTree Parent { get; }
    public Process Process { get; }
    public IReadOnlyList<ProcessTree> Children { get; set; }

    public ProcessTree(State state, Mix target)
     : this(null, new Process(state, target))
    {
    }

    public ProcessTree(ProcessTree parent, Process process) 
        => (Parent, Process) = (parent, process);

    public void ComputeNextLevel()
    {
        if (Children == null)
        {
            var transitions = Process.GetPossibleTransitions();
            Children = transitions.Select(p => new ProcessTree(this, p)).ToList();
        }
    }

    public int Count 
        => 1 + Children?.Sum(c => c.Count) ?? 0;
}