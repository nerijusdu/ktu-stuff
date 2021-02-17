using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Lab3
{
    class Lygtis
    {
        int[,] A, B, C;
        int[,] Farray = new int[100, 100];
        private int count;

        public Lygtis(int count)
        {
            A = FillArray(count);
            B = FillArray(count);
            C = FillArray(count);
            this.count = count;
        }

        private int[,] FillArray(int count)
        {
            var rand = new Random();
            var arr = new int[count, count];

            for (int i = 0; i < count; i++)
            {
                for (int j = 0; j < count; j++)
                {
                    arr[i, j] = rand.Next(0, 5);
                }
            }

            return arr;
        }

        public int TestWithRecursion(int seed)
        {
            var rand = new Random(seed);
            int i = rand.Next(0, count);
            int j = rand.Next(0, count);
            return F(i, j, int.MaxValue);
        }

        public int TestParallel(int seed)
        {
            var rand = new Random(seed);
            int i = rand.Next(0, count);
            int j = rand.Next(0, count);
            return FParallel(i, j, int.MaxValue);
        }

        private int F(int i, int j, int min)
        {
            if (j <= i + 2)
            {
                return 0;
            }
            int k = j - 1;

            int sum = F(i, k, min) + F(k, j, min) + D(i, k, j);

            if (sum < min)
            {
                min = sum;
            }

            return min;
        }

        private int D(int i, int j, int k)
        {
            return A[i, j] + B[j, k] + C[k, j];
        }

        private int FParallel(int i, int j, int min)
        {
            if (j <= i + 2)
            {
                return 0;
            }
            int k = j - 1;
            Task[] tasks = new Task[2];

            tasks[0] = Task.Factory.StartNew((Object p) => { 
                var data = p as CustomData; 
                if (data == null) 
                    return; 
                data.Res = FParallel(data.I, data.K, data.Min); 
            }, new CustomData() { I = i, J = j, K = k, Min = min });

            tasks[1] = Task.Factory.StartNew((Object p) => { 
                var data = p as CustomData; 
                if (data == null) 
                    return; 
                data.Res = FParallel(data.K, data.J, data.Min); 
            }, new CustomData() { I = i, J = j, K = k, Min = min });
            
            Task.WaitAll(tasks);

            int sum =   (tasks[0].AsyncState as CustomData).Res + 
                        (tasks[1].AsyncState as CustomData).Res + 
                        D(i, k, j);

            if (sum < min)
            {
                min = sum;
            }

            return min;
        }

        public void Greitaveika()
        {
            var kiekiai = new List<int> { 100, 500, 1000, 2500, 5000 };
            var timer = new Stopwatch();
            Console.WriteLine("{0,-15}|{1,-15}", "Kiekis", "Laikas");
            kiekiai.ForEach(k =>
            {
                timer.Reset();
                timer.Start();
                var uzd = new Lygtis(k);
                uzd.TestWithRecursion(123);
                timer.Stop();
                Console.WriteLine("{0,-15}|{1,20}", k, timer.Elapsed);
            });
        }
    }

    internal class CustomData
    {
        public int I { get; set; }
        public int J { get; set; }
        public int K { get; set; }
        public int Min { get; set; }
        public int Res { get; set; }
    }
}
