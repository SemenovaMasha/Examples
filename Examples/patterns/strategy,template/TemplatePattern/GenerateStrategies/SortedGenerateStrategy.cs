using System;
using System.Collections.Generic;
using System.Text;

namespace TemplatePattern.GenerateStrategies
{
    public class SortedGenerateStrategy : IGenerateStrategy
    {
        public void Generate(ref int[] Array, int N)
        {
            Array = new int[N];

            for (int i = 0; i < N; i++)
            {
                Array[i] = i;
            }
        }
        public string StrategyName => "Sorted generating strategy";
    }
}
