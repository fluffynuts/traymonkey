using System;
using System.IO;
using System.Windows.Forms;
using PeanutButter.INI;
using PeanutButter.TrayIcon;

namespace TrayMonkey
{
    static class Program
    {
        private static TrayIcon _trayIcon;
        private static Monkey _monkey;

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            SetupMonkey();
            SetupTrayIcon();
            Application.Run();
        }

        private static void SetupMonkey()
        {
            var iniFile = GetConfigFilePathForUser();
            var config = new MonkeyConfig(iniFile);
            var processFinder = new ActiveProcessFinder();
            _monkey = new Monkey(config, processFinder);
            _monkey.Start();
        }

        private static IINIFile GetConfigFilePathForUser()
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TrayMonkey", "config.ini");
            return new INIFile(path);
        }

        private static void SetupTrayIcon()
        {
            _trayIcon = new TrayIcon(Resources.face_monkey);
            _trayIcon.Show();
            SetupTrayIconMenus();
        }

        private static void SetupTrayIconMenus()
        {
            SetupExitItem();
        }

        private static void SetupExitItem()
        {
            _trayIcon.AddMenuItem("&Exit", () =>
            {
                _monkey.Stop();
                _trayIcon.Hide();
                Application.Exit();
            });
        }
    }
}
