using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TemplatePattern.Tests
{
    [TestClass]
    public class QuickSortingTests
    {
        [TestMethod]
        public void QuickSorting_FullTest()
        {
            QuickSorting sorting = new QuickSorting();
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
        public void QuickSorting_SortTest()
        {
            QuickSorting sorting = new QuickSorting();
            sorting.Array = new[] { 3, 2, 7, 3, 1 };
            sorting.N = sorting.Array.Length;

            sorting.Sort();

            CollectionAssert.AreEqual(sorting.Array, new int[] { 1, 2, 3, 3, 7 });

        }

        [TestMethod]
        public void QuickSorting_QuicksortSequentialTest()
        {
            QuickSorting sorting = new QuickSorting();
            int[] testArray = new[] {3, 2, 7, 3, 1, 4, 5, 4};

            sorting.QuicksortSequential(testArray,2,5);

            CollectionAssert.AreEqual(testArray, new int[] { 3, 2, 1,3,4,7, 5, 4 });

        }
        [TestMethod]
        public void QuickSorting_QuicksortParallelTest()
        {
            QuickSorting sorting = new QuickSorting();
            int[] testArray = new[] {3, 2, 7, 3, 1, 4, 5, 4};

            sorting.QuicksortParallel(testArray,2,5);

            CollectionAssert.AreEqual(testArray, new int[] { 3, 2, 1,3,4,7, 5, 4 });

        }
        [TestMethod]
        public void QuickSorting_SwapTest()
        {
            QuickSorting sorting = new QuickSorting();
            int[] testArray = new[] {3, 2, 7, 3, 1, 4, 5, 4};
            int i = 2;
            int j = 5;
            int old_i = testArray[i];
            int old_j = testArray[j];

            sorting.Swap(testArray,i,j);

            Assert.AreEqual(testArray[i],old_j);
            Assert.AreEqual(testArray[j],old_i);

        }

        [TestMethod]
        public void QuickSorting_PartitionTest()
        {
            QuickSorting sorting = new QuickSorting();
            int[] testArray = new[] { 3, 2, 7, 3, 1, 4, 5, 4 };

            sorting.Partition(testArray, 2, 5);

            CollectionAssert.AreEqual(testArray, new int[] { 3, 2, 1, 3, 7,4, 5, 4 });

        }
    }
}
