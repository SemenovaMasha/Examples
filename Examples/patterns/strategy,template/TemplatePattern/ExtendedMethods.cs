using System;
using System.Collections.Generic;
using System.Text;
using TemplatePattern.GenerateStrategies;
using TemplatePattern.SortingAlgorithms;

namespace TemplatePattern
{
    public static class ExtendedMethods
    {
        public static void Test(this AbstractSortingClass sortingClass, IGenerateStrategy generateStrategy)
        {
            sortingClass.SetGenerateStrategy(generateStrategy);
            sortingClass.TestSortingAlgorithm();
        }
    }
}
