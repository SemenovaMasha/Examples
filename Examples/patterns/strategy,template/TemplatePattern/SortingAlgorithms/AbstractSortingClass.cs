using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using TemplatePattern.GenerateStrategies;

namespace TemplatePattern.SortingAlgorithms
{
    public abstract class AbstractSortingClass
    {
        public int[] Array;
        protected Stopwatch sw;
        protected abstract string AlgorithmName { get; }
        public int N;
        private IGenerateStrategy GenerateStrategy;

        public void SetGenerateStrategy(IGenerateStrategy generateStrategy)
        {
            this.GenerateStrategy = generateStrategy;
        }

        public void TestSortingAlgorithm()
        {
            sw = new Stopwatch();
            sw.Start();

            N = 10_000;
            GenerateStrategy.Generate(ref Array, N);

            if (N <= 20) Print();
            Sort();
            if (N <= 20) Print();

            sw.Stop();
            Console.WriteLine($"Algorithm: '{AlgorithmName}' Total time: {sw.ElapsedTicks} ");
        }

        public abstract void Sort();

        protected void Print()
        {
            for (int i = 0; i < N; i++)
            {
                Console.Write($"{Array[i]} ");
            }

            Console.WriteLine();
        }
    }
}
