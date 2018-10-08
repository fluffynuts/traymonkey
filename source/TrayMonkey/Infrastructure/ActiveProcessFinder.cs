using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace TrayMonkey.Infrastructure
{
    public interface IProcessHelper
    {
        string ForegroundProcessName { get; }
        Process ForegroundProcess { get; }
        void UpdateActiveProcess();
        Process[] FindProcessesByName(string s);
    }

    public class ProcessHelper : IProcessHelper
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out uint ProcessId);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        public string ForegroundProcessName => ForegroundProcess?.MainModule?.FileName;
        public Process ForegroundProcess { get; private set; }

        public void UpdateActiveProcess()
        {
            try
            {
                IntPtr hwnd = GetForegroundWindow();
                GetWindowThreadProcessId(hwnd, out var pid);
                ForegroundProcess = Process.GetProcessById((int) pid);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to get foreground process: {ex.Message}");
                ForegroundProcess = null;
            }
        }

        public Process[] FindProcessesByName(string name)
        {
            name = name.ToLowerInvariant();
            return Process.GetProcesses().Where(p =>
            {
                try
                {
                    return p?.MainModule?.FileName?.ToLowerInvariant()
                        .EndsWith(name) ?? false;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Unable to find processes by name '{name}': {ex.Message}");
                    return false;
                }
            }).ToArray();
        }
    }
}