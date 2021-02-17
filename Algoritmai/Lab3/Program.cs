using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Lab3
{
    class Program
    {
        static void Main(string[] args)
        {
            TestSearch();
            // TestOtherThing();
        }

        static void TestSearch() {
            bool res;
            var q = 1000000;
            var kiekiai = new int[] {1000000, 2500000, 5000000, 7500000, 10000000};
            var timer = new Stopwatch();
            var seed = 123456;

            var bst = new MyBst();
            bst.GenerateData(q, seed);

            Console.WriteLine("BST search");
            Console.WriteLine("{0, -10}| {1, -20}", "Quantity", "Time");

            var rand = new Random(seed + 1);
            foreach (var k in kiekiai)
            {
                timer.Reset();
                timer.Start();
                for (int i = 0; i < k; i++)
                {
                    var val = (short)rand.Next(short.MinValue, short.MaxValue);
                    res = bst.Contains(val);
                }
                timer.Stop();
                Console.WriteLine("{0, -10}| {1, -20}", k, timer.Elapsed);
            }

            Console.WriteLine("BST parallel search");
            Console.WriteLine("{0, -10}| {1, -20}", "Quantity", "Time");

            rand = new Random(seed + 1);
            foreach (var k in kiekiai)
            {
                timer.Reset();
                timer.Start();
                Parallel.For(0, k, i => {
                    var val = (short)rand.Next(short.MinValue, short.MaxValue);
                    res = bst.Contains(val);
                });
                timer.Stop();
                Console.WriteLine("{0, -10}| {1, -20}", k, timer.Elapsed);
            }

        }

        static void TestOtherThing() {
            var kiekiai = new int[] {1000, 2500, 5000, 7500, 10000};
            var timer = new Stopwatch();
            var seed = 521324;

            Console.WriteLine("Test F");
            Console.WriteLine("{0, -10}| {1, -20}", "Quantity", "Time");

            var i = 0;
            foreach (var k in kiekiai)
            {
                var lygtis = new Lygtis(k);
                lygtis.TestWithRecursion(seed + i);
                
                i += 5;
            }

            i = 0;
            foreach (var k in kiekiai)
            {
                var lygtis = new Lygtis(k);
                timer.Reset();
                timer.Start();
                lygtis.TestWithRecursion(seed + i);                
                timer.Stop();
                Console.WriteLine("{0, -10}| {1, -20}", k, timer.Elapsed);
                
                i += 5;
            }

            Console.WriteLine("Test parallel F");
            Console.WriteLine("{0, -10}| {1, -20}", "Quantity", "Time");

            i = 0;
            foreach (var k in kiekiai)
            {
                var lygtis = new Lygtis(k);
                lygtis.TestParallel(seed + i);
                
                i += 5;
            }

            i = 0;
            foreach (var k in kiekiai)
            {
                var lygtis = new Lygtis(k);
                timer.Reset();
                timer.Start();
                lygtis.TestParallel(seed + i);                
                timer.Stop();
                Console.WriteLine("{0, -10}| {1, -20}", k, timer.Elapsed);
                
                i += 5;
            }
        }
    }
}
