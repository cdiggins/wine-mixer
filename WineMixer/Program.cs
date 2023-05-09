using System.Diagnostics;

namespace WineMixer
{
    public class Program
    {
        public static Random Random = new Random();
        public static Mix Target;
        public static Configuration Configuration;
        public static int NumWines => Target.Count;
        public static Transition Root;
        public static Dictionary<Transition, double> Scores = new ();
        public static List<Transition> Transitions = new ();

        public static double GetScore(Transition transition)
        {
            if (Scores.TryGetValue(transition, out var score))
                return score;

            var mix = transition.CurrentState.BestMix;
            var dist = transition.CurrentState.TargetDistance(mix);
            var usedWines = mix?.CountUsedWines() ?? 0;
            var newScore = dist * 1000.0 + transition.Length - usedWines * 100;
            Scores.Add(transition, newScore);
            return newScore;
        }

        public static State ApplyRandomOperation(State state)
        {
            var operations = state.GetValidOperations().ToList();
            var operation = operations[Random.Next(operations.Count)];
            return state.Apply(operation);
        }

        public static void Output(State state)
        {
            Console.WriteLine(state);
        }

        public static void OutputValidCombineOperations()
        {
            Console.WriteLine("Valid combine operations:");
            var i = 0;
            foreach (var tc in Configuration.ValidTankCombines)
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

        public static Transition GetBestChild(Transition t)
        {
            if (t.GetOrComputeTransitions().Count == 0)
                return t;
            return t.GetOrComputeTransitions().OrderBy(GetScore).First();
        }

        public static Transition GetRandomTransition()
        {
            var r = Root;
            while (true)
            {
                var ts = r.GetOrComputeTransitions();
                if (ts.Count == 0)
                {
                    return r;
                }
                else
                {
                    var i = Random.Next(ts.Count);
                    r = ts[i];
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
            var tankSizesFileName = args[0];
            var wineMixFileName = args[1];
            var optionsFileName = args.Length > 2 ? args[2] : "";
            Configuration = Configuration.LoadFromFiles(tankSizesFileName, wineMixFileName, optionsFileName);

            var state = new State(Configuration);

            Root = new Transition(null, state, null, null, null);
            Transitions.Add(Root);
            
            // NOTE: used for debugging 
            OutputValidCombineOperations();

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