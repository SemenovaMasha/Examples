using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TemplatePattern.Tests
{
    [TestClass]
    public class MergeSortingTests
    {
        [TestMethod]
        public void MergeSorting_FullTest()
        {
            MergeSorting sorting = new MergeSorting();
            sorting.SetGenerateStrategy(new DescSortedGenerateStrategy());

            sorting.TestSortingAlgorithm();

            int[] expected = new int [sorting.N];
            for (int i = 0; i < sorting.N; i++)
            {
                expected[i] = i+1;
            }

            CollectionAssert.AreEqual(sorting.Array, expected);

        }

        [TestMethod]
        public void MergeSorting_SortTest()
        {
            MergeSorting sorting = new MergeSorting();
            sorting.Array = new[] { 3, 2, 7, 3, 1 };
            sorting.N = sorting.Array.Length;

            sorting.Sort();

            CollectionAssert.AreEqual(sorting.Array, new int[]{1,2,3,3,7});

        }

        [TestMethod]
        public void MergeSorting_DoMergeTest()
        {
            MergeSorting sorting = new MergeSorting();
            int[] tmp_array = new[] {3, 2, 7, 3, 1, 6};
            sorting.N = tmp_array.Length;

            sorting.DoMerge(tmp_array, 1,3,5);

            CollectionAssert.AreEqual(tmp_array, new int[]{ 3, 2, 3, 1, 6 ,7});

        }

        [TestMethod]
        public void MergeSorting_RecursiveTest()
        {
            MergeSorting sorting = new MergeSorting();
            int[] tmp_array = new[] {3, 2, 7, 3, 1, 6};
            sorting.N = tmp_array.Length;

            sorting.MergeSort_Recursive(tmp_array, 1,4);

            CollectionAssert.AreEqual(tmp_array, new int[]{ 3, 1,2,3,7 ,6});

        }
    }
}
