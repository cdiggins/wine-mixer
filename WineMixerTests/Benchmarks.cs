using System.Diagnostics;
using WineMixer;

namespace WineMixerTests
{
    public class Benchmarks
    {
        public static List<int> EmptyList = new List<int>();
        public static SmallIntSet EmptyIntSet = new SmallIntSet(MaxSize);

        private const int MaxSize = 32;
        public IEnumerable<List<int>> GetCombinations(int x)
        {
            if (x == 0)
            {
                yield return EmptyList;
            }
            else
            {
                foreach (var tmp in GetCombinations(x - 1))
                {
                    yield return tmp;
                    var tmp2 = tmp.ToList();
                    tmp2.Add(x);
                    yield return tmp2;
                }
            }
        }

        public IEnumerable<SmallIntSet> GetCombinations2(int x)
        {
            if (x == 0)
            {
                yield return EmptyIntSet;
            }       
            else
            {
                foreach (var tmp in GetCombinations2(x - 1))
                {
                    yield return tmp;
                    var tmp2 = tmp.Clone();
                    tmp2.Add(x);
                    yield return tmp2;
                }
            }
        }

        public IEnumerable<ConsList<int>> GetCombinations3(int x)
        {
            if (x == 0)
            {
                yield return ConsList<int>.Empty;
            }
            else
            {
                foreach (var tmp in GetCombinations3(x - 1))
                {
                    yield return tmp;
                    yield return tmp.Prepend(x);
                }
            }
        }

        public int CountCombinations(int upto)
        {
            var r = GetCombinations(upto).Count();
            Console.WriteLine($"Combinations of {upto} = {r}");
            return r;
        }

        public int CountCombinations2(int upto)
        {
            var r = GetCombinations2(upto).Count();
            Console.WriteLine($"Combinations of {upto} = {r}");
            return r;
        }

        public int CountCombinations3(int upto)
        {
            var r = GetCombinations3(upto).Count();
            Console.WriteLine($"Combinations of {upto} = {r}");
            return r;
        }

        public IEnumerable<(TParameter, TimeSpan?)> ProfileAndTest<TParameter, TInput, TOutput>(
            IEnumerable<TParameter> parameters,
            Func<TParameter, TInput> inputGenerator,
            Func<TInput, TOutput> f, 
            Func<TInput, TOutput, bool> validator)
        {
            var i = 0;
            foreach (var p in parameters)
            {
                Console.WriteLine($"Running test {i++} with parameter {p}");
                var input = inputGenerator(p);
                var sw = Stopwatch.StartNew();
                var output = f(input);
                var elapsed = sw.Elapsed;
                var passed = validator(input, output);
                yield return (p, passed ? elapsed : null);
            }
        }

        [Test]
        public void TestCountingCombinations()
        {
            Console.WriteLine("Approach 1");
            RunTest(CountCombinations);
            Console.WriteLine("Approach 2");
            RunTest(CountCombinations2);
            Console.WriteLine("Approach 3");
            RunTest(CountCombinations3);
        }

        public void RunTest(Func<int, int> f)
        {
            var results = ProfileAndTest(
                Enumerable.Range(20, 7),
                x => x,
                f,
                (i, o) =>
                {
                    Console.WriteLine($"Input = {i}, Output = {o}");
                    return true;
                }
            ).ToList();

            var i = 0;
            foreach (var r in results)
            {
                var passedStr = r.Item2 == null ? "failed" : "passed";
                Console.WriteLine($"Test {i++} with parameter {r.Item1} {passedStr} in {r.Item2?.TotalSeconds:0.###} seconds");
            }
        }
    }
}
