using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PeanutButter.INI;

namespace TrayMonkey
{
    public interface IMonkeyConfig
    {
        IEnumerable<MonkeyRule> Rules { get; }
        int Heartbeat { get; }
    }

    public class MonkeyConfig : IMonkeyConfig
    {
        public IEnumerable<MonkeyRule> Rules
        {
            get { return _rules; }
        }

        public int Heartbeat { get; private set; }

        private IINIFile _iniFile;
        private List<MonkeyRule> _rules;

        public MonkeyConfig(IINIFile iniFile)
        {
            if (iniFile == null) throw new ArgumentNullException("iniFile");
            _iniFile = iniFile;
            LoadIniFile();
        }

        private void LoadIniFile()
        {
            SetHeartbeat();
            LoadRules();
        }

        private void SetHeartbeat()
        {
            Heartbeat = 500;
            if (!_iniFile.HasSection("")) return;
            var configuredHeartbeat = GetSettingValueFrom(_iniFile[""], "heartbeat");
            int configvalue;
            if (Int32.TryParse(configuredHeartbeat, out configvalue))
                Heartbeat = configvalue;
        }

        private void LoadRules()
        {
            _rules = new List<MonkeyRule>();
            foreach (var section in _iniFile.Sections)
            {
                if (String.IsNullOrEmpty(section)) continue;
                var sectionData = _iniFile[section];
                _rules.Add(new MonkeyRule(section, GetProcessPathFrom(sectionData), GetOnActivatedFrom(sectionData), GetOnDeactivatedFrom(sectionData)));
            }
        }

        private string GetProcessPathFrom(Dictionary<string, string> value)
        {
            return GetSettingValueFrom(value, "Process");
        }

        private string GetOnDeactivatedFrom(Dictionary<string, string> values)
        {
            return GetSettingValueFrom(values, "OnDeactivated");
        }

        private string GetOnActivatedFrom(Dictionary<string, string> values)
        {
            return GetSettingValueFrom(values, "OnActivated");
        }

        private static string GetSettingValueFrom(Dictionary<string, string> values, string settingName)
        {
            return (values.ContainsKey(settingName)) ? values[settingName] : null;
        }
    }
}
