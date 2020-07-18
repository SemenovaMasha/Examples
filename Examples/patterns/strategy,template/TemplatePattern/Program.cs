using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Threading.Tasks;
using TemplatePattern.GenerateStrategies;
using TemplatePattern.SortingAlgorithms;

namespace TemplatePattern
{
    public class Program
    {
        public static void Main(string[] args)
        {
            AbstractSortingClass[] sorters = { new BubbleSorting(), new MergeSorting(), new QuickSorting() };
            IGenerateStrategy[] generators = { new RandomGenerateStrategy(), new SortedGenerateStrategy(), new DescSortedGenerateStrategy(), };

            foreach (var generator in generators)
            {
                Console.WriteLine(Environment.NewLine + generator.StrategyName);
                foreach (var sorter in sorters)
                {
                    sorter.Test(generator);
                }
            }

            Console.ReadKey();
        }

        public static Random r = new Random();
    }
      
  
}
