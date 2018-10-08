using System;
using System.Linq;
using System.Windows.Forms;
using PeanutButter.Utils;
using TrayMonkey.Infrastructure;

namespace TrayMonkey.InbuiltActions
{
    public class SetVolume : IMonkeyAction
    {
        private readonly IRestoreVolumeAction _restoreVolumeAction;
        private readonly ISystemVolume _systemVolume;
        private readonly INotifier _notifier;
        public string Identifier => "SetVolume";

        public SetVolume(
            ISystemVolume systemVolume,
            INotifier notifier,
            IMonkeyAction[] allActions)
        {
            _restoreVolumeAction = allActions
                .OfType<IRestoreVolumeAction>()
                .Single();
            _systemVolume = systemVolume;
            _notifier = notifier;
        }

        public void Run(string[] args)
        {
            if (args.Length < 1) return;
            var dd = new DecimalDecorator(args[0]);
            try
            {
                var floatVal = (float) dd.ToDecimal();
                var originalVolume = _systemVolume.SetVolume(floatVal);
                _restoreVolumeAction.StoreVolume(originalVolume);
                _notifier.ShowDebugInfo($"Set system volume to {floatVal}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to set system volume: {ex.Message}");
            }
        }
    }
}