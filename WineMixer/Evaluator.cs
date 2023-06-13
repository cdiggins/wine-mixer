namespace WineMixer;

public class Evaluator
{
    public Options Options { get; }

    public Evaluator(Options options)
        => Options = options;

    public double EvaluateByTransfers(State state) 
        => state.Transfers.Min(t => state.TargetDistance(state.GetMix(t)));

    public double EvaluateByMix(State state)
        => state.Mixes.Min(state.TargetDistance);

    public double Evaluate(State state, Transfer transfer) 
        => EvaluateByTransfers(state.Apply(transfer));

    public Transfer ChooseBestTransfer(List<(Transfer, double)> choices)
        => choices.Count > 0 ? choices.MinBy(tuple => tuple.Item2).Item1 : null;

    public Transfer GetBestTransfer(State state)
        => ChooseBestTransfer(Options.RunInParallel 
            ? state.Transfers.AsParallel().Select(t => (t, Evaluate(state, t))).ToList() 
            : state.Transfers.Select(t => (t, Evaluate(state, t))).ToList());
}