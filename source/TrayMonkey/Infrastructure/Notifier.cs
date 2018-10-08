using System.Windows.Forms;
using PeanutButter.INIFile;
using PeanutButter.TrayIcon;
using PeanutButter.Utils;

namespace TrayMonkey.Infrastructure
{
    public interface INotifier
    {
        void ShowInfo(string message);
        
        void ShowInfo(
            string title,
            string message);

        void Show(
            string title,
            string message,
            ToolTipIcon icon,
            int milliseconds
        );

        void ShowDebugInfo(string message);
    }
    
    public class Notifier : INotifier
    {
        public const int DEFAULT_SHOW_TIME = 5000;
        private readonly ITrayIcon _trayIcon;
        private readonly IINIFile _config;

        public Notifier(
            ITrayIcon trayIcon,
            IINIFile config)
        {
            _trayIcon = trayIcon;
            _config = config;
        }

        public void ShowDebugInfo(string message)
        {
            var isDebug = _config.GetValue("", "debug", "false").AsBoolean();
            if (!isDebug)
            {
                return;
            }
            ShowInfo(message);
        }

        public void ShowInfo(string message)
        {
            ShowInfo("Information", message);
        }

        public void ShowInfo(
            string title,
            string message)
        {
            Show(message,
                 title,
                 ToolTipIcon.Info,
                 DEFAULT_SHOW_TIME);
        }

        public void Show(
            string title,
            string message,
            ToolTipIcon icon,
            int milliseconds
        )
        {
            _trayIcon.ShowBalloonTipFor(milliseconds,
                                       title,
                                       message,
                                       icon);
        }
    }
}