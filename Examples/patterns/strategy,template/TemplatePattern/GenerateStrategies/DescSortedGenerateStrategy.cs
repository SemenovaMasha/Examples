using System;
using System.Collections.Generic;
using System.Text;

namespace TemplatePattern.GenerateStrategies
{
    public class DescSortedGenerateStrategy : IGenerateStrategy
    {
        public void Generate(ref int[] Array, int N)
        {
            Array = new int[N];

            for (int i = 0; i < N; i++)
            {
                Array[i] = N - i;
            }
        }
        public string StrategyName => "Desc generating strategy";
    }
}
