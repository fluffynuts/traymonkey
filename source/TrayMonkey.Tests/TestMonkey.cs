using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;

namespace TrayMonkey.Tests
{
    [TestFixture]
    public class TestMonkey
    {
        [Test]
        public void Construct_WhenGivenNullConfig_ShouldThrow()
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var ex = Assert.Throws<ArgumentNullException>(() => new Monkey(null, null, null));

            //---------------Test Result -----------------------
            Assert.AreEqual("config", ex.ParamName);
        }


        [Test]
        public void Construct_WhenGivenNullActiveProcessFinder_ShouldThrow()
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var ex = Assert.Throws<ArgumentNullException>(() => new Monkey(Substitute.For<IMonkeyConfig>(), null, null));

            //---------------Test Result -----------------------
            Assert.AreEqual("activeProcessFinder", ex.ParamName);
        }
    }
}
