using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DryIoc;
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
            var ex = Assert.Throws<ArgumentNullException>(() => new Monkey(null, null));

            //---------------Test Result -----------------------
            Assert.AreEqual("config", ex.ParamName);
        }


        [Test]
        public void Construct_WhenGivenNullActiveProcessFinder_ShouldThrow()
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var ex = Assert.Throws<ArgumentNullException>(() => new Monkey(Substitute.For<IMonkeyConfig>(), null));

            //---------------Test Result -----------------------
            Assert.AreEqual("activeProcessFinder", ex.ParamName);
        }
    }

    [TestFixture]
    public class DiscoveryTests
    {
        public interface IFoo
        {
            string Name { get; }
        }

        public class Foo1: IFoo
        {
            public string Name => this.GetType().Name;
        }        
        
        public class Foo2: IFoo
        {
            public string Name => this.GetType().Name;
        }

        public interface IConsumesFoo
        {
            IFoo[] Services { get; }
        }

        public class ConsumesFoo : IConsumesFoo
        {
            public IFoo[] Services { get; }

            public ConsumesFoo(IFoo[] services)
            {
                Services = services;
            }
        }

        [Test]
        public void MultipleRegistrationsWithDryIoc()
        {
            // Arrange
            var container = new Container();
            container.RegisterMany(
                new[] { typeof(IConsumesFoo).Assembly }, 
                type => type == typeof(IFoo));
            container.Register<IConsumesFoo, ConsumesFoo>();
            // Pre-assert
            // Act
            var result = container.Resolve<IConsumesFoo>();
            // Assert
            Assert.That(result.Services, Has.Exactly(2).Items);
        }
    }
}
