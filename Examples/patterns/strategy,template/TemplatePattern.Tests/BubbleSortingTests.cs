using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TemplatePattern.Tests
{
    [TestClass]
    public class BubbleSortingTests
    {
        [TestMethod]
        public void BubbleSortingT_FullTest()
        {
            BubbleSorting sorting = new BubbleSorting();
            sorting.SetGenerateStrategy(new DescSortedGenerateStrategy());

            sorting.TestSortingAlgorithm();

            int[] expected = new int[sorting.N];
            for (int i = 0; i < sorting.N; i++)
            {
                expected[i] = i + 1;
            }

            CollectionAssert.AreEqual(sorting.Array, expected);
        }

        [TestMethod]
        public void BubbleSortingT_SortTest()
        {
            BubbleSorting sorting = new BubbleSorting();
            sorting.Array = new[] { 3, 2, 7, 3, 1 };
            sorting.N = sorting.Array.Length;

            sorting.Sort();

            CollectionAssert.AreEqual(sorting.Array, new int[] { 1, 2, 3, 3, 7 });
        }
    }
}
