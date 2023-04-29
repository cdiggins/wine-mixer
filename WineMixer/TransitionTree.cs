namespace Champagne;

public class TransitionTree
{
    public TransitionTree Parent { get; }
    public Transition Transition { get; }
    public IReadOnlyList<TransitionTree> Children { get; set; }

    public TransitionTree(State state, Mix target)
        : this(null, new Transition(state, target))
    {
    }

    public TransitionTree(TransitionTree parent, Transition transition) 
        => (Parent, Transition) = (parent, transition);

    public void ComputeNextLevel()
    {
        if (Children == null)
        {
            var transitions = Transition.GetPossibleTransitions();
            Children = transitions.Select(p => new TransitionTree(this, p)).ToList();
        }
    }

    public int Count 
        => 1 + Children?.Sum(c => c.Count) ?? 0;
}