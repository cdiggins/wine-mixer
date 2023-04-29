﻿
using System.Diagnostics;

namespace Champagne
{
    public class Program
    {
        public static Random Random = new Random();
        public static Mix Target;
        public static TankSizes TankSizes;
        public static int NumWines => Target.NumWines;
        public static List<Transition> Transitions = new List<Transition>();
        public static List<Transition> Considered = new List<Transition>();

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

        public static void OutputState(State state)
        {
            var d = state.Distance(Target);
            var score = state.TotalScore(Target);
            var usedWines = state.UsedWines();
            Console.WriteLine($"Distance from target = {d}, score = {score}, wines = {usedWines}");
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

        public static void ComputeTransitions()
        {
            var newTransitions = new List<Transition>();
            foreach (var t in Transitions)
            {
                var n = t.ComputeTransitions();
                Considered.Add(t);
                foreach (var t2 in t.Transitions)
                {
                    newTransitions.Add(t2);
                }
            }

            Transitions = newTransitions;
        }

        public static void Main(string[] args)
        {
            Target = Mix.LoadFromFile(args[1]);
            TankSizes = TankSizes.LoadFromFile(args[0], NumWines);
            var state = new State(TankSizes, NumWines);

            var t = new Transition(null, state, null, null, null);
            Transitions.Add(t);

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