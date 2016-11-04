using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Distances_Calculator
{
    enum Distance
    {
        L1Distance,
        L2Distance,
        HammingDistance
    }
    class Program
    {
        int length = 512;
        int numOfQueryVectors = 1024;
        int numOfDatasetVectors = 1024;

        List<float>[] queryVectors;
        List<float>[] datasetVectors;

        public List<float>[] QueryVectors
        {
            get
            {
                return queryVectors;
            }
        }

        public List<float>[] DatasetVectors
        {
            get
            {
                return datasetVectors;
            }
        }

        Program()
        {
            Generate();
        }

        Program(int length, int numOfQueryVectors, int numOfDatasetVectors) : this()
        {
            this.length = length;
            this.numOfQueryVectors = numOfQueryVectors;
            this.numOfDatasetVectors = numOfDatasetVectors;
            Generate();
        }

        Program(string queryVectorsPath, string datasetVectorsPath)
        {
            //Initialize vectors from corresponding paths
            Initialize(queryVectorsPath, queryVectors);
            Initialize(queryVectorsPath, datasetVectors);
        }

        Program(string queryVectorsPath, string datasetVectorsPath, int length, int numOfQueryVectors, int numOfDatasetVectors)
            : this(queryVectorsPath, datasetVectorsPath)
        {
            this.length = length;
            this.numOfQueryVectors = numOfQueryVectors;
            this.numOfDatasetVectors = numOfDatasetVectors;

            ValidateQueryVectors();
            ValidateDatasetVectors();
        }

        void ValidateQueryVectors()
        {
            Validate(queryVectors, numOfQueryVectors, length);
        }

        void ValidateDatasetVectors()
        {
            Validate(datasetVectors, numOfDatasetVectors, length);
        }

        void Validate(List<float>[] vectors, int vectorsCount, int vectorLenght)
        {
            if (vectors.Length != vectorsCount)
                throw new InvalidDataException("Number of vectors doesn't match with provided value");

            for (int i = 0; i < vectorsCount; ++i)
                if (vectors[i].Count != vectorLenght)
                    throw new InvalidDataException("Size of vectors doesn't match with provided value");
        }

        void Initialize(string filePath, List<float>[] vectors)
        {
            List<List<float>> data = new List<List<float>>();
            try
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    int Row = 0;
                    while (!sr.EndOfStream)
                    {
                        string[] line = sr.ReadLine().Split(',');
                        data.Add(new List<float>());
                        for (int i = 0; i < line.Length; ++i)
                            data[Row].Add(float.Parse(line[i]));
                        Row++;
                    }
                }
            } catch (IOException ex)
            {
                Console.WriteLine("Couldn't read the file. Error: {0}", ex.Message);
            }

            vectors = data.ToArray();
        }

        List<float>[] generateRandomVectors(int numOfVectors)
        {
            Random random = new Random();
            List<float>[] vectors = new List<float>[numOfVectors];

            for (int i = 0; i < vectors.Length; ++i)
            {
                vectors[i] = new List<float>();
                for (int j = 0; j < length; ++j)
                {
                    vectors[i].Add(NextFloat(random));
                }
            }

            return vectors;
        }

        float NextFloat(Random random)
        {
            double mantissa = (random.NextDouble() * 2.0) - 1.0;
            double exponent = Math.Pow(2.0, random.Next(-126, 128));
            return (float)(mantissa * exponent);
        }
        void Generate()
        {
            //Generate random vectors
            queryVectors = generateRandomVectors(numOfQueryVectors);
            datasetVectors = generateRandomVectors(numOfDatasetVectors);
        }

        static void Main(string[] args)
        {
            Program p = new Program();
            Calculator calc = new Calculator(Distance.L2Distance);

            double[][] result;

            var watch = System.Diagnostics.Stopwatch.StartNew();
            result = calc.calculateSimple(p.QueryVectors, p.DatasetVectors);
            watch.Stop();
            Console.WriteLine("Simple Engine: {0} milliseconds", watch.ElapsedMilliseconds);

            watch.Reset();

            watch.Start();
            result = calc.calculateAsync(p.QueryVectors, p.DatasetVectors);
            watch.Stop();
            Console.WriteLine("Threading Engine {0} milliseconds", watch.ElapsedMilliseconds);

            watch.Reset();

            watch.Start();
            result = calc.calculateWithManyThreads(p.QueryVectors, p.DatasetVectors);
            watch.Stop();
            Console.WriteLine("Many Threads Engine {0} milliseconds", watch.ElapsedMilliseconds);
        }
    }
}
