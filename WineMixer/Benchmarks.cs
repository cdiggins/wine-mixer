using BenchmarkDotNet.Attributes;

namespace WineMixer
{
    public class Benchmarks
    {
        public IEnumerable<List<int>> AllCombinationsUpTo(int x)
        {
            if (x == 0)
                yield return new List<int>();

            foreach (var tmp in AllCombinationsUpTo(x - 1))
            {
                yield return tmp;
                var tmp2 = tmp.ToList();
                tmp2.Add(x);
                yield return tmp2;
            }
        }

        [Params(1, 2, 3, 4, 5, 6, 7, 8, 9, 10)]
        public int Count { get; set; }

        [Benchmark()]
        public void CountCombinations()
        {
            var r = AllCombinationsUpTo(Count).Count();
            Console.WriteLine($"Combinations of {Count} = {r}");
        }
    }
}
