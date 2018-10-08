using System;
using System.Diagnostics;
using System.Windows.Forms;
using PeanutButter.TrayIcon;
using TrayMonkey.Infrastructure;

namespace TrayMonkey
{
    public interface IMonkeyTrayIcon
    {
        void Show();

        void ShowNotification(
            int timeout,
            string title,
            string text,
            ToolTipIcon icon,
            Action clickAction = null,
            Action closeAction = null);
    }

    public class MonkeyTrayIcon : IMonkeyTrayIcon
    {
        private readonly IMonkey _monkey;
        private readonly IConfig _config;
        private readonly ITrayIcon _trayIcon;
        private readonly IAutoReloadingConfig _autoReloadingConfig;

        public MonkeyTrayIcon(
            IMonkey monkey,
            IConfig config,
            ITrayIcon trayIcon,
            IAutoReloadingConfig autoReloadingConfig
        )
        {
            _monkey = monkey;
            _config = config;
            _trayIcon = trayIcon;
            _autoReloadingConfig = autoReloadingConfig;
            SetupEditMenuItem();
            AddDivider();
            SetupExitMenuItem();
        }

        private void SetupEditMenuItem()
        {
            _trayIcon.AddMenuItem(
                "&Edit config...",
                () =>
                {
                    var startInfo = new ProcessStartInfo()
                    {
                        FileName = _config.ConfigFile,
                        UseShellExecute = true
                    };
                    Process.Start(startInfo);
                });
        }

        private void AddDivider()
        {
            _trayIcon.AddMenuSeparator();
        }

        private void SetupExitMenuItem()
        {
            _trayIcon.AddMenuItem(
                "&Exit",
                () =>
                {
                    _monkey.Stop();
                    _trayIcon.Hide();
                    _autoReloadingConfig.Stop();
                    Application.Exit();
                });
        }

        public void Show()
        {
            _trayIcon.Show();
            _autoReloadingConfig.Watch();
        }

        public void ShowNotification(
            int timeout,
            string title,
            string text,
            ToolTipIcon icon,
            Action clickAction = null,
            Action closeAction = null)
        {
            _trayIcon.ShowBalloonTipFor(
                timeout,
                title,
                text,
                icon,
                clickAction,
                closeAction);
        }
    }
}