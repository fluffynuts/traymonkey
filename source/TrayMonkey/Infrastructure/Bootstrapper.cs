using System.Linq;
using System.Reflection;
using DryIoc;
using PeanutButter.INIFile;
using PeanutButter.TinyEventAggregator;
using PeanutButter.TrayIcon;
using PeanutButter.Utils;
using TrayMonkey.InbuiltActions;

namespace TrayMonkey.Infrastructure
{
    public static class Bootstrapper
    {
        public static IContainer Bootstrap()
        {
            var container = new Container();
            var thisAssembly = typeof(Bootstrapper).Assembly;
            RegisterMonkeyActionsOn(container, thisAssembly);
            RegisterUserConfigOn(container);
            RegisterSingletonsOn(container);
            RegisterAllOneToOneOn(container, thisAssembly);
            return container;
        }

        private static void RegisterSingletonsOn(IContainer container)
        {
            container.Register<IConfig, Config>(Reuse.Singleton);
            container.RegisterDelegate<IEventAggregator>(_ => EventAggregator.Instance);
            container.Register<ISystemVolume, SystemVolume>(Reuse.Singleton);
            container.Register<IMonkey, Monkey>(Reuse.Singleton);
            container.RegisterDelegate<ITrayIcon>(
                _ => new TrayIcon(Resources.face_monkey),
                Reuse.Singleton);
            container.Register<IProcessHelper, ProcessHelper>(Reuse.Singleton);
        }

        private static void RegisterMonkeyActionsOn(
            Container container,
            params Assembly[] assemblies)
        {
            container.RegisterMany(
                assemblies,
                t => t == typeof(IMonkeyAction),
                Reuse.Singleton);
        }

        private static void RegisterAllOneToOneOn(
            IContainer container,
            params Assembly[] assemblies)
        {
            assemblies.ForEach(
                asm =>
                {
                    var allTypes = asm.GetTypes();
                    allTypes.Where(t => t.IsInterface)
                        .ForEach(
                            serviceType =>
                            {
                                if (serviceType?.Namespace?.StartsWith("DryIoc") ?? true)
                                    return;
                                var implementations = allTypes.Where(
                                    t => t.ImplementsServiceType(serviceType)
                                ).ToArray();

                                if (implementations.Length != 1)
                                {
                                    return;
                                }

                                if (container.GetServiceRegistrations()
                                    .Any(r => r.ServiceType == serviceType))
                                {
                                    return;
                                }

                                try
                                {
                                    container.Register(
                                        serviceType,
                                        implementations.Single(),
                                        Reuse.Singleton);
                                }
                                catch (ContainerException)
                                {
                                    /*
                                     * ignore: could be from an interface we don't expect to inject
                                     * -> tests should cover all interesting constructables
                                     */
                                }
                            });
                });
        }

        private static void RegisterUserConfigOn(
            IContainer container)
        {
            container.RegisterDelegate<IINIFile>(
                resolverContext =>
                {
                    var config = new AutoReloadingConfig(
                        resolverContext.Resolve<IEventAggregator>(),
                        resolverContext.Resolve<IConfig>());
                    config.Watch();
                    return config;
                },
                Reuse.Singleton);
        }
    }
}