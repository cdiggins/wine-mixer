
using System.Diagnostics;

namespace Champagne
{
    public class Program
    {
        public static Random Random = new Random();
        public static Mix Target;
        public static TankSizes TankSizes;
        public static int NumWines => Target.NumWines;

        public static State ApplyRandomStep(State state)
        {
            var steps = state.GetValidSteps(Target.NumWines).ToList();
            var step = steps[Random.Next(steps.Count)];
            return state.Apply(step);
        }

        public static TankCombine? FindBestTankCombine(State state)
        {
            return state.GetValidTankCombineSteps().OrderBy(x => state.Apply(x).Distance(Target)).FirstOrDefault();
        }

        public static IEnumerable<Mix> GetInitialMixes()
            => Enumerable.Range(0, NumWines).Select(i => new Mix(i, NumWines));

        public static void OutputTree(ProcessTree pt, string indent = "")
        {
            Console.WriteLine($"{indent} children={pt.Children?.Count ?? -1} distance={pt.Process.Distance} step={pt.Process.Step}");
            if (pt.Children != null)
                foreach (var child in pt.Children)
                    OutputTree(child, indent + "  ");
        }

        public static void ExpandTree(ProcessTree pt, int level)
        {
            if (level > 0)
            {
                pt.ComputeNextLevel();
                foreach (var c in pt.Children)
                    ExpandTree(c, level - 1);
            }
        }

        public static void OutputState(State state)
        {
            var d = state.Distance(Target);
            Console.WriteLine($"Distance from target = {d}");
            Console.WriteLine(state);
        }

        public static void OutputValidCombineSteps()
        {
            Console.WriteLine("Valid combine steps:");
            var i = 0;
            foreach (var tc in TankSizes.ValidTankCombines)
            {
                Console.WriteLine($"Step {i++}: {tc}");
            }
        }

        public static void ApplyRandomSteps(ProcessTree pt)
        {
            for (var i = 0; i < 100; ++i)
            {
                pt.ComputeNextLevel();
                if (pt.Count == 0)
                    break;
                var n = Random.Next(pt.Children.Count);
                pt = pt.Children[n];
            }
        }

        public static void Main(string[] args)
        {
            Target = Mix.LoadFromFile(args[1]);
            TankSizes = TankSizes.LoadFromFile(args[0], NumWines);
            var state = new State(TankSizes, NumWines);

            var pt = new ProcessTree(state, Target);
            OutputValidCombineSteps();

            for (var i = 3; i < 15; ++i)
            {
                var sw = Stopwatch.StartNew();
                ExpandTree(pt, i);
                Console.WriteLine($"Found {pt.Count} steps at {i} depths");
                Console.WriteLine($"Time elapsed = {sw.Elapsed}");
            }

        }
    }
}