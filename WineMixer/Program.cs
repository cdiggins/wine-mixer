using System.Diagnostics;

namespace WineMixer
{
    public class Program
    {
        public static Random Random = new Random();
        public static Mix Target;
        public static TankSizes TankSizes;
        public static int NumWines => Target.NumWines;
        public static Transition Root;
        public static Dictionary<Transition, double> Scores = new ();
        public static List<Transition> Transitions = new ();

        public static double GetScore(Transition transition)
        {
            if (Scores.TryGetValue(transition, out var score))
                return score;
            var bestTank = transition.CurrentState.BestTank(Target);
            var dist = bestTank.Item2;
            var mix = bestTank.Item3;
            var usedWines = mix?.CountUsedWines() ?? 0;
            var newScore = dist * 1000.0 + transition.Length - usedWines * 100;
            Scores.Add(transition, newScore);
            return newScore;
        }

        public static State ApplyRandomOperation(State state)
        {
            var operations = state.GetValidOperations(Target.NumWines).ToList();
            var operation = operations[Random.Next(operations.Count)];
            return state.Apply(operation);
        }

        public static void Output(State state)
        {
            var bestTank = state.BestTank(Target);
            var tank = bestTank.Item1;
            var dist = bestTank.Item2;
            var mix = bestTank.Item3;
            var usedWines = state.UsedWines();
            Console.WriteLine($"Distance from target = {dist}, wines = {usedWines}");
            Console.WriteLine($"Target = {Target}, ");
            Console.WriteLine($"Tank [{tank}] contains [{mix}]");
        }

        public static void OutputValidCombineOperations()
        {
            Console.WriteLine("Valid combine operations:");
            var i = 0;
            foreach (var tc in TankSizes.ValidTankCombines)
            {
                Console.WriteLine($"Operation {i++}: {tc}");
            }
        }

        public static void Shuffle<T>(List<T> list)
        {
            for (var i = 0; i < list.Count; ++i)
            {
                var j = Random.Next(list.Count);
                var tmp = list[i];
                list[i] = list[j];
                list[j] = tmp;
            }
        }

        public static List<Transition> GetRandomTransitions(int n)
        {
            var tmp = Scores.Keys.ToList();
            if (tmp.Count < n) return tmp;
            Shuffle(tmp);
            return tmp.Take(n).ToList();
        }

        public static void OldComputeTransitions()
        {
            const int randomBatchSize = 1000;
            const int bestBatchSize = 100;
            Console.WriteLine($"Computing new batch of transitions {bestBatchSize} of {randomBatchSize}");
            var best = GetRandomTransitions(randomBatchSize).OrderBy(GetScore).Take(bestBatchSize);

            var max = double.MaxValue;
            var cnt = 0;
            foreach (var t in best)
            {
                cnt += t.ComputeTransitions();

                foreach (var t2 in t.Transitions)
                {
                    var tmp = GetScore(t2);
                    if (tmp < max)
                        max = tmp;
                }
            }
            Console.WriteLine($"Best score was {max}, created {cnt} new transitions");
        }

        public static Transition GetRandomChild(Transition t)
        {
            if (t.Transitions.Count == 0)
                return t;
            var n = Random.Next(t.Transitions.Count);
            return t.Transitions[n];
        }

        public static Transition GetBestChild(Transition t)
        {
            if (t.Transitions.Count == 0)
                return t;
            return t.Transitions.OrderBy(GetScore).First();
        }

        public static Transition GetRandomTransition()
        {
            var r = Root;
            while (true)
            {
                if (r.Transitions == null)
                {
                    r.ComputeTransitions();
                    return GetBestChild(r);
                }
                else if (r.Transitions.Count == 0)
                {
                    return r;
                }
                else
                {
                    r = GetRandomChild(r);
                }
            }
        }

        public static void ComputeTransitions()
        {
            const int BatchSize = 100;
            Console.WriteLine($"Computing new batch of transitions");
            
            var bestScore = double.MaxValue;

            for (var i = 0; i < BatchSize; ++i)
            {
                var t = GetRandomTransition();
                var tmp = GetScore(t);
                if (tmp < bestScore)
                    bestScore = tmp;
            }

            Console.WriteLine($"Best score was {bestScore}, created {BatchSize} new transitions");
        }

        public static Transition Best()
        {
            Transition r = null;
            var minScore = double.MaxValue;
            foreach (var kv in Scores)
            {
                if (kv.Value < minScore)
                {
                    r = kv.Key;
                    minScore = kv.Value;
                }
            }

            return r;
        }

        public static void Output(Transition t)
        {
            Console.WriteLine($"Transition has {t.Length} steps and score {GetScore(t)}");
            Output(t.CurrentState);
            var ops = t.GetOperations();
            foreach (var op in ops)
            {
                Console.WriteLine(op);
            }
        }

        public static void Main(string[] args)
        {
            Target = Mix.LoadFromFile(args[1]);
            TankSizes = TankSizes.LoadFromFile(args[0], NumWines);
            var state = new State(TankSizes, NumWines);

            Root = new Transition(null, state, null, null, null);
            Transitions.Add(Root);
            
            // NOTE: used for debugging 
            //OutputValidCombineOperations();

            for (var i = 0; i < 10; ++i)
            {
                var sw = Stopwatch.StartNew();
                Console.WriteLine($"Total transitions considered {Scores.Count}");
                ComputeTransitions();

                Console.WriteLine("Best transition:  ");
                Output(Best());

                Console.WriteLine($"Time elapsed = {sw.Elapsed}");
            }

        }
    }
}