using System;
using System.Collections.Generic;
using System.Text;

namespace TemplatePattern.SortingAlgorithms
{
    public class BubbleSorting : AbstractSortingClass
    {
        protected override string AlgorithmName => "Bubble sort";

        public override void Sort()
        {
            int i, j;

            for (j = N - 1; j > 0; j--)
            {
                for (i = 0; i < j; i++)
                {
                    if (Array[i] > Array[i + 1])
                    {
                        int temporary = Array[i];
                        Array[i] = Array[i + 1];
                        Array[i + 1] = temporary;
                    }
                }
            }
        }

    }
}
