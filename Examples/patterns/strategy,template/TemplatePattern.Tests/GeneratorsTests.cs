using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TemplatePattern.Tests
{
    [TestClass]
    public class GeneratorsTests
    {
        [TestMethod]
        public void RandomGenerateStrategy_Test()
        {
            RandomGenerateStrategy generateStrategy = new RandomGenerateStrategy();
            int N = 1000;
            int[] array = new int[N];

            generateStrategy.Generate(ref array,N);

            Assert.AreEqual(array.Length,N);
            CollectionAssert.DoesNotContain(array, -500);
            CollectionAssert.DoesNotContain(array, 500);
        }
        [TestMethod]
        public void SortedGenerateStrategy_Test()
        {
            SortedGenerateStrategy generateStrategy = new SortedGenerateStrategy();
            int N = 5;
            int[] array = new int[N];

            generateStrategy.Generate(ref array,N);

            Assert.AreEqual(array.Length,N);
            CollectionAssert.AreEqual(array, new int[]{0,1,2,3,4});
        }
        [TestMethod]
        public void DescSortedGenerateStrategy_Test()
        {
            DescSortedGenerateStrategy generateStrategy = new DescSortedGenerateStrategy();
            int N = 5;
            int[] array = new int[N];

            generateStrategy.Generate(ref array,N);

            Assert.AreEqual(array.Length,N);
            CollectionAssert.AreEqual(array, new int[]{5,4,3,2,1});
        }
    }
}
