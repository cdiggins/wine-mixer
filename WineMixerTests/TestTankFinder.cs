using System.Diagnostics;
using WineMixer;

namespace WineMixerTests
{
    public class TestTankFinder
    {
        //public static int[] TankSizes = new[] { 1, 1, 2, 3, 5, 6, 8, 13 };
        
        // public static int[] BigTankSizes = new[] { 1, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 13 };
        
        public static int[] TankSizes = 
        {
            20, 20, 20, 20,
            20, 20, 20, 20,
            1, 1, 2, 2, 3, 3, 4,
            4, 5, 5, 6, 10, 10,
            12, 12, 13, 15, 16,
            20, 24, 25, 25, 30,
            30, 32, 40, 40, 60, 80
        };

        public static int Volume = 80;//

        [Test]
        public void ProfileTankFinder()
        {
            /*
            {
                var t = new TankListFinder(TankSizes, Volume);
                Console.WriteLine("First run");
                var sw = Stopwatch.StartNew();
                var init = t.GetPermutationsOfVolume(Volume).ToList();
                for (var i = 0; i < 10; ++i)
                {
                    Console.WriteLine(string.Join(", ", init[i]));
                }
                Console.WriteLine($"Found {init.Count} permutations adding up to {Volume} in {sw.Elapsed.TotalSeconds:#.00} seconds");
                Console.WriteLine($"Number of tank indices in lookup {t.Lookup.Count(xs => xs != null)}");
                Console.WriteLine($"Total entries in lookup {t.Lookup.Sum(x => x?.Count() ?? 0)}");
            }
            */

            {
                var t = new NewTankFinder2(TankSizes, Volume);
                Console.WriteLine("First run");
                var sw = Stopwatch.StartNew();
                var init = t.GetPermutationsOfVolume(Volume).ToList();
                for (var i = 0; i < 10; ++i)
                {
                    Console.WriteLine(string.Join(", ", init[i]));
                }
                Console.WriteLine($"Found {init.Count} permutations adding up to {Volume} in {sw.Elapsed.TotalSeconds:#.00} seconds");
                //Console.WriteLine($"Number of tank indices in lookup {t.Lookup.Count(xs => xs != null)}");
                //Console.WriteLine($"Total entries in lookup {t.Lookup.Sum(x => x?.Count() ?? 0)}");
            }

            /*
            {
                var t = new ListBasedTankListFinder(TankSizes, Volume);
                Console.WriteLine("First run");
                var sw = Stopwatch.StartNew();
                var init = t.GetPermutationsOfVolume(Volume).ToList();
                Console.WriteLine($"Found {init.Count} permutations adding up to {Volume} in {sw.Elapsed.TotalSeconds:#.00} seconds");
                Console.WriteLine($"Number of tank indices in lookup {t.Lookup.Count(xs => xs != null)}");
                Console.WriteLine($"Total entries in lookup {t.Lookup.Sum(x => x?.Count() ?? 0)}");
            }*/

            /*

            {
                var t = new TankListFinder(TankSizes, Volume);
                Console.WriteLine("Naive run");
                var sw = Stopwatch.StartNew();
                var init = t.NaiveRecursive(Volume).ToList();
                Console.WriteLine($"Found {init.Count} permutations adding up to {Volume} in {sw.Elapsed.TotalSeconds:#.00} seconds");
            }
            */

            /*
            {
                Console.WriteLine("Second run");
                var sw = Stopwatch.StartNew();
                var init = t.GetPermutationsOfVolume(Volume).ToList();
                Console.WriteLine($"Found {init.Count} permutations adding up to {Volume} in {sw.Elapsed.TotalSeconds:#.00} seconds");
            }
            */
        }
    }
}
