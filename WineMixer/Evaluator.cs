namespace WineMixer;

public class Evaluator
{
    public Options Options { get; }

    public Evaluator(Options options)
        => Options = options;

    public double Evaluate(State state) 
        => state.Transfers.Min(t => state.TargetDistance(state.GetMix(t)));

    public double Evaluate(State state, Transfer transfer) 
        => Evaluate(state.Apply(transfer));

    public Transfer ChooseBestTransfer(IEnumerable<(Transfer, double)> choices) 
        => choices.MinBy(tuple => tuple.Item2).Item1;

    public Transfer GetBestTransfer(State state)
        => ChooseBestTransfer(Options.RunInParallel 
            ? state.Transfers.AsParallel().Select(t => (t, Evaluate(state, t))).ToList() 
            : state.Transfers.Select(t => (t, Evaluate(state, t))).ToList());
}