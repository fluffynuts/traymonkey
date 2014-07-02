using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using PeanutButter.RandomGenerators;

namespace TrayMonkey.Tests
{
    [TestFixture]
    public class TestMonkeyRule
    {
        [Test]
        public void Construct_WhenGivenParameters_ShouldSetPropertiesFromParameters()
        {
            //---------------Set up test pack-------------------
            var name = RandomValueGen.GetRandomString();
            var process = RandomValueGen.GetRandomString();
            var onActivated = RandomValueGen.GetRandomString();
            var onDeactivated = RandomValueGen.GetRandomString();

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var rule = new MonkeyRule(name, process, onActivated, onDeactivated);

            //---------------Test Result -----------------------
            Assert.AreEqual(name, rule.Name);
            Assert.AreEqual(process, rule.Process);
            Assert.AreEqual(onActivated, rule.OnActivated);
            Assert.AreEqual(onDeactivated, rule.OnDeactivated);
        }

    }
}
