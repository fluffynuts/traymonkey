using System;
using System.Collections.Generic;
using ImTools;
using PeanutButter.INIFile;
using PeanutButter.TinyEventAggregator;
using TrayMonkey.Infrastructure;

namespace TrayMonkey
{
    public interface IMonkeyConfig
    {
        IEnumerable<MonkeyRule> Rules { get; }
        int Heartbeat { get; }
    }

    public class MonkeyConfig : IMonkeyConfig
    {
        public IEnumerable<MonkeyRule> Rules => _rules.ToArray();

        public int Heartbeat { get; private set; }

        private readonly IINIFile _iniFile;
        private List<MonkeyRule> _rules;

        public MonkeyConfig(
            IINIFile iniFile,
            IEventAggregator eventAggregator,
            IConfig config,
            INotifier notifier)
        {
            _iniFile = iniFile ?? throw new ArgumentNullException(nameof(iniFile));
            Refresh();
            eventAggregator.GetEvent<ConfigurationReloadedEvent>()
                .Subscribe(_ =>
                {
                    Refresh();
                    notifier.ShowInfo("Configuration reloaded", $"Configuration reloaded from {config.ConfigFile}");
                });
            
        }

        public void Refresh()
        {
            lock (this)
            {
                SetHeartbeat();
                LoadRules();
            }
        }

        private void SetHeartbeat()
        {
            Heartbeat = 500;
            if (!_iniFile.HasSection(""))
                return;

            var configuredHeartbeat = GetSettingValueFrom(_iniFile[""], "heartbeat");
            if (int.TryParse(configuredHeartbeat, out var configValue))
                Heartbeat = configValue;
        }

        private void LoadRules()
        {
            _rules = new List<MonkeyRule>();
            foreach (var section in _iniFile.Sections)
            {
                if (string.IsNullOrEmpty(section)) continue;
                var sectionData = _iniFile[section];
                _rules.Add(
                    new MonkeyRule(
                        section,
                        GetProcessPathFrom(sectionData),
                        GetOnActivatedFrom(sectionData),
                        GetOnDeactivatedFrom(sectionData)));
            }
        }

        private string GetProcessPathFrom(IDictionary<string, string> value)
        {
            return GetSettingValueFrom(value, "Process");
        }

        private string GetOnDeactivatedFrom(IDictionary<string, string> values)
        {
            return GetSettingValueFrom(values, "OnDeactivated");
        }

        private string GetOnActivatedFrom(IDictionary<string, string> values)
        {
            return GetSettingValueFrom(values, "OnActivated");
        }

        private static string GetSettingValueFrom(
            IDictionary<string, string> values,
            string settingName)
        {
            return (values.ContainsKey(settingName))
                ? values[settingName]
                : null;
        }
    }
}