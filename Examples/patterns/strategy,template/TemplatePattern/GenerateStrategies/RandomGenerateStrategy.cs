using System;
using System.Collections.Generic;
using System.Text;

namespace TemplatePattern.GenerateStrategies
{

    public class RandomGenerateStrategy : IGenerateStrategy
    {
        public void Generate(ref int[] Array, int N)
        {
            Array = new int[N];

            for (int i = 0; i < N; i++)
            {
                Array[i] = Program.r.Next(-100, 100);
            }
        }
        public string StrategyName => "Random generating strategy";
    }
}
