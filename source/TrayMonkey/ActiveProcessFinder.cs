using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TrayMonkey
{
    public interface IActiveProcessFinder
    {
        string GetActiveProcessFileName();
    }

    public class ActiveProcessFinder : IActiveProcessFinder
    {
        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out uint ProcessId);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        public string GetActiveProcessFileName()
        {
            try
            {
                IntPtr hwnd = GetForegroundWindow();
                uint pid;
                GetWindowThreadProcessId(hwnd, out pid);
                Process p = Process.GetProcessById((int)pid);
                var activeProcessFileName = p.MainModule.FileName;
                return activeProcessFileName;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("Unable to get foreground process: {0}", ex.Message));
                return null;
            }
        }
    }
}
