using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace TrayMonkey.Infrastructure
{
    public interface IConfig
    {
        string ConfigFile { get; }
    }

    public class Config : IConfig
    {
        public string ConfigFile =>
            Debugger.IsAttached
                ? DebugDataConfigPath()
                : AppDataConfigPath();

        private static string DebugDataConfigPath()
        {
            return Path.Combine(
                Path.GetDirectoryName(
                    new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath
                ),
                "monkey.ini");
        }

        private static string AppDataConfigPath()
        {
            return Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.ApplicationData),
                "TrayMonkey",
                "config.ini");
        }
    }
}