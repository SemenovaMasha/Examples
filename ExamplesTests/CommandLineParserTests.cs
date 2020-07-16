using ArgsParser;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExamplesTests
{
    [TestClass]
    public class CommandLineParserTests
    {
        [TestMethod]
        public void GetString_IfSetStringArgument_CanGetStringArgument()
        {
            CommandLineParser parser = new CommandLineParser("x*", new string[] { "-x", "param" });

            Assert.AreEqual(parser.GetString('x'), "param");
        }

        [TestMethod]
        public void GetString_IfSetTwoStringArguments_CanGetTwoStringArguments()
        {
            CommandLineParser parser = new CommandLineParser("x*,y*", new string[] { "-x", "paramX", "-y", "paramY" });

            Assert.AreEqual(parser.GetString('x'), "paramX");
            Assert.AreEqual(parser.GetString('y'), "paramY");
        }

        [TestMethod]
        public void GetBool_IfSetBoolArgument_CanGetBoolArgument()
        {
            CommandLineParser parser = new CommandLineParser("b", new string[] { "-b" });

            Assert.AreEqual(parser.GetBool('b'), true);
        }

        [TestMethod]
        public void GetInt_IfSetIntArgument_CanGetIntArgument()
        {
            CommandLineParser parser = new CommandLineParser("i#", new string[] { "-i", "45" });

            Assert.AreEqual(parser.GetInt('i'), 45);
        }

        [TestMethod]
        public void GetDouble_IfSetDoubleArgumentWithDot_CanGetDoubleArgument()
        {
            CommandLineParser parser = new CommandLineParser("d##", new string[] { "-d", "34.6" });

            Assert.AreEqual(parser.GetDouble('d'), 34.6);
        }

        [TestMethod]
        public void GetDouble_IfSetDoubleArgumentWithComma_CanGetDoubleArgument()
        {
            CommandLineParser parser = new CommandLineParser("d##", new string[] { "-d", "34,6" });

            Assert.AreEqual(parser.GetDouble('d'), 34.6);
        }
    }
}
