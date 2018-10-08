using System;
using System.IO;
using PeanutButter.INIFile;
using PeanutButter.TinyEventAggregator;

namespace TrayMonkey.Infrastructure
{
    public class AutoReloadingConfig : INIFile
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IConfig _config;
        private readonly FileSystemWatcher _watcher;

        public AutoReloadingConfig(
            IEventAggregator eventAggregator,
            IConfig config) : base(config.ConfigFile)
        {
            _eventAggregator = eventAggregator;
            _config = config;
            _watcher = new FileSystemWatcher(System.IO.Path.GetDirectoryName(config.ConfigFile));
        }

        public void Watch()
        {
            _watcher.Changed += Reload;
        }

        public void Stop()
        {
            _watcher.Changed -= Reload;
        }

        private void Reload(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Changed &&
                string.Equals(e.FullPath, _config.ConfigFile, StringComparison.OrdinalIgnoreCase))
            {
                base.Reload();
                _eventAggregator
                    .GetEvent<ConfigurationReloadedEvent>()
                    .Publish(true);
            }
        }
    }
}