using System.Windows.Forms;
using PeanutButter.TrayIcon;

namespace TrayMonkey
{
    public interface IMonkeyTrayIcon
    {
        void Show();
    }
    
    public class MonkeyTrayIcon: IMonkeyTrayIcon
    {
        private readonly IMonkey _monkey;
        private readonly TrayIcon _trayIcon;

        public MonkeyTrayIcon(
            IMonkey monkey
        )
        {
            _monkey = monkey;
            _trayIcon = new TrayIcon(Resources.face_monkey);
            SetupExitMenuItem();
        }

        private void SetupExitMenuItem()
        {
            _trayIcon.AddMenuItem(
                "&Exit",
                () =>
                {
                    _monkey.Stop();
                    _trayIcon.Hide();
                    Application.Exit();
                });
        }

        public void Show()
        {
            _trayIcon.Show();
        }
    }
}