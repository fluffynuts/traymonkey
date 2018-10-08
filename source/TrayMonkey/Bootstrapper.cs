using System;
using System.IO;
using System.Linq;
using System.Reflection;
using DryIoc;
using PeanutButter.INIFile;
using PeanutButter.Utils;
using TrayMonkey.InbuiltActions;

namespace TrayMonkey
{
    public static class Bootstrapper
    {
        public static IContainer Bootstrap()
        {
            var container = new Container();
            var thisAssembly = typeof(Bootstrapper).Assembly;
            RegisterUserConfigOn(container);
            RegisterSingletonsOn(container);
            RegisterAllOneToOneOn(container, thisAssembly);
            RegisterMonkeyActionsOn(container);
            return container;
        }

        private static void RegisterSingletonsOn(IContainer container)
        {
            container.Register<IMonkey, Monkey>(Reuse.Singleton);
        }

        private static void RegisterMonkeyActionsOn(
            Container container,
            params Assembly[] assemblies)
        {
            container.RegisterMany(
                assemblies,
                t => t == typeof(IMonkeyAction));
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
            var path = Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.ApplicationData),
                "TrayMonkey",
                "config.ini");
            var config = new INIFile(path);
            container.RegisterDelegate<IINIFile>(
                resolverContext => config,
                Reuse.Singleton);
        }
    }
}