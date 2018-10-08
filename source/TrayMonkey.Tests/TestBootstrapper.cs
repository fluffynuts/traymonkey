using DryIoc;
using NExpect;
using NUnit.Framework;
using TrayMonkey.InbuiltActions;
using TrayMonkey.Infrastructure;

namespace TrayMonkey.Tests
{
    [TestFixture]
    public class TestBootstrapper
    {
        [Test]
        public void MultipleRegistrationsWithDryIoc_MonkeyActions()
        {
            // Arrange
            var container = Bootstrapper.Bootstrap();
            container.Register<IConsumesActions, ConsumesActions>();
            // Pre-assert
            // Act
            var result = container.Resolve<IConsumesActions>();
            // Assert
            Expectations.Expect(result.Actions).Not.To.Be.Empty();
        }

        public interface IConsumesActions
        {
            IMonkeyAction[] Actions { get; }
        }

        public class ConsumesActions: IConsumesActions
        {
            public IMonkeyAction[] Actions { get; }

            public ConsumesActions(IMonkeyAction[] actions)
            {
                Actions = actions;
            }
        }
    }
}