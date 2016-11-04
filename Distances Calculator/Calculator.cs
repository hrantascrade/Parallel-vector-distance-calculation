using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distances_Calculator
{
    class Calculator
    {
        DistaceCalculator distanceCalculator;
        double[][] matrix;
        static void Swap<T>(ref T x, ref T y)
        {
            T t = y;
            y = x;
            x = t;
        }

        public Calculator(Distance distance)
        {
            switch (distance)
            {
                case Distance.L1Distance:
                    distanceCalculator = new L1Distance();
                    break;
                case Distance.L2Distance:
                    distanceCalculator = new L2Distance();
                    break;
                case Distance.HammingDistance:
                    distanceCalculator = new HammingDistance();
                    break;
            }
        }

        public double[][] calculateSimple(List<float>[] vs1, List<float>[] vs2)
        {
            matrix = new double[vs1.Length][];
            for (int i = 0; i < matrix.Length; ++i)
                matrix[i] = new double[vs2.Length];

            for (int i = 0; i < vs1.Length; ++i)
            {
                for (int j = 0; j < vs2.Length; ++j)
                {
                    matrix[i][j] = distanceCalculator.Distance(vs1[i], vs2[j]);
                }
            }

            return matrix;
        }

        public void calculatePart(int from, int to, List<float>[] vs1, List<float>[] vs2)
        {
            for (int i = from; i < to; ++i)
            {
                for (int j = 0; j < vs2.Length; ++j)
                {
                    matrix[i][j] = distanceCalculator.Distance(vs1[i], vs2[j]);
                }
            }
        }
        public double[][] calculateAsync(List<float>[] vs1, List<float>[] vs2)
        {
            if (vs1.Length < vs2.Length)
                Swap(ref vs1, ref vs2);

            matrix = new double[vs1.Length][];
            for (int i = 0; i < matrix.Length; ++i)
                matrix[i] = new double[vs2.Length];

            int numOfThreads = Environment.ProcessorCount;
            int size = (vs1.Length + numOfThreads - 1) / numOfThreads;

            Task[] tasks = new Task[numOfThreads];
            for (int i = 0; i < numOfThreads; ++i)
            {
                Task t;

                int from = i * size;
                int to = (i + 1) * size;

                if (i == numOfThreads - 1)
                    t = new Task(() => calculatePart(from, vs1.Length, vs1, vs2));
                else
                    t = new Task(() => calculatePart(from, to, vs1, vs2));

                t.Start();
                tasks[i] = t;
            }

            Task.WaitAll(tasks);

            return matrix;
        }

        void calcWMT(List<float> a, List<float> b, int i, int j)
        {
            matrix[i][j] = distanceCalculator.Distance(a, b);
        }
        public double[][] calculateWithManyThreads(List<float>[] vs1, List<float>[] vs2)
        {
            matrix = new double[vs1.Length][];
            for (int i = 0; i < matrix.Length; ++i)
                matrix[i] = new double[vs2.Length];

            Task[] tasks = new Task[vs1.Length * vs2.Length];
            for (int i = 0; i < vs1.Length; ++i)
            {
                for (int j = 0; j < vs2.Length; ++j)
                {
                    int from = i, to = j;
                    Task t = new Task(() => calcWMT(vs1[from], vs2[to], from, to));
                    t.Start();
                    tasks[i * vs1.Length + j] = t;
                }
            }

            Task.WaitAll(tasks);

            return matrix;
        }
    }
}
