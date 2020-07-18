using System;
using System.Collections.Generic;
using System.Text;

namespace TemplatePattern.GenerateStrategies
{
    public interface IGenerateStrategy
    {
        void Generate(ref int[] Array, int N);
        string StrategyName { get; }
    }
}
