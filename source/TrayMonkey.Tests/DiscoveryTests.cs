using DryIoc;
using NUnit.Framework;

namespace TrayMonkey.Tests
{
    [TestFixture]
    [Explicit("Test tests are for discovery purposes only")]
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