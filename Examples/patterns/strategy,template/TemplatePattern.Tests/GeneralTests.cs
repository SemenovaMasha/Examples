using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TemplatePattern.Tests
{
    [TestClass]
    public class GeneralTests
    {
        [TestMethod]
        public void ExtendedMethod_Test()
        {
            AbstractSortingClass sorting = new BubbleSorting();
            IGenerateStrategy strategy = new DescSortedGenerateStrategy();
            
            sorting.Test(strategy);
            
            int[] expected = new int[sorting.N];
            for (int i = 0; i < sorting.N; i++)
            {
                expected[i] = i + 1;
            }
            CollectionAssert.AreEqual(sorting.Array, expected);
        }

    }
}
