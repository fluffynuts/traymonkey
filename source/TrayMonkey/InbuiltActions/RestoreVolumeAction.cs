using System;
using TrayMonkey.Infrastructure;

namespace TrayMonkey.InbuiltActions
{
    public interface IRestoreVolumeAction
    {
        void StoreVolume(float volume);
    }

    public class RestoreVolumeAction : IMonkeyAction, IRestoreVolumeAction
    {
        private readonly ISystemVolume _systemVolume;
        private readonly INotifier _notifier;
        public string Identifier => "RestoreVolume";
        private float? _storedVolume;
        
        public RestoreVolumeAction(
            ISystemVolume systemVolume,
            INotifier notifier)
        {
            _systemVolume = systemVolume;
            _notifier = notifier;
        }

        public void Run(string[] args)
        {
            float? restoreVolume;
            lock (this)
            {
                restoreVolume = _storedVolume;
                _storedVolume = null;
            }

            if (restoreVolume.HasValue)
            {
                _systemVolume.SetVolume(restoreVolume.Value);
                _notifier.ShowDebugInfo($"Restored system volume to {restoreVolume}");
            }
        }

        public void StoreVolume(float volume)
        {
            _storedVolume = volume;
        }
    }
}