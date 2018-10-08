using System;
using System.Windows.Forms;
using DryIoc;

namespace TrayMonkey
{
    static class Program
    {
        private static IMonkeyTrayIcon _trayIcon;
        private static IMonkey _monkey;

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            SetupMonkey();
            Application.Run();
        }

        private static void SetupMonkey()
        {
            var container = Bootstrapper.Bootstrap();
            _trayIcon = container.Resolve<IMonkeyTrayIcon>();
            _trayIcon.Show();
            _monkey = container.Resolve<IMonkey>();
            _monkey.Start();
        }
    }

}