using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CoreAudioApi;
using PeanutButter.Utils;

namespace TrayMonkey
{
    public interface IMonkey
    {
        void Start();
        void Stop();
    }

    public class Monkey
        : IMonkey
    {
        private CancellationTokenSource _cancellationTokenSource;
        private Task _taskWaitable;
        private IMonkeyConfig _config;
        private readonly IActiveProcessFinder _activeProcessFinder;
        private Action _pendingDeactivationActions;
        private Dictionary<string, Action<string[]>> _runActions;
        private float? _restoreVolumeTo;
        private string _lastActiveProcess;

        public Monkey(IMonkeyConfig config, IActiveProcessFinder activeProcessFinder)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _activeProcessFinder = activeProcessFinder ?? throw new ArgumentNullException(nameof(activeProcessFinder));
            _runActions = new Dictionary<string, Action<string[]>>(StringComparer.OrdinalIgnoreCase);
            SetupRunActions();
        }

        private void SetupRunActions()
        {
            SetupVolumeSetAction();
            SetupVolumeRestoreAction();
        }

        private void SetupVolumeRestoreAction()
        {
            _runActions["restorevolume"] = (args) =>
            {
                float? restoreVolume;
                lock (this)
                {
                    restoreVolume = _restoreVolumeTo;
                    _restoreVolumeTo = null;
                }
                if (restoreVolume.HasValue)
                    SetSystemVolumeTo(restoreVolume.Value);
            };
        }

        private void SetupVolumeSetAction()
        {
            _runActions["setvolume"] = (args) =>
            {
                if (args.Length < 1) return;
                var dd = new DecimalDecorator(args[0]);
                try
                {
                    var floatVal = (float) dd.ToDecimal();
                    var originalVolume = SetSystemVolumeTo(floatVal);
                    lock (this)
                    {
                        _restoreVolumeTo = originalVolume;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Unable to set system volume: {ex.Message}");
                }
            };
        }

        private float SetSystemVolumeTo(float volume)
        {
            lock (this)
            {
                var enumerator = new MMDeviceEnumerator();
                var defaultDeviceEndpoint = enumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia).AudioEndpointVolume;
                var channels = new List<AudioEndpointVolumeChannel>();
                for (var i = 0; i < defaultDeviceEndpoint.Channels.Length; i++)
                    channels.Add(defaultDeviceEndpoint.Channels[i]);
                var originalVolume = channels.Select(c => c.VolumeLevelScalar).Max();
                Debug.WriteLine($"Setting system volume to: {volume}");
                Debug.WriteLine($"Saving restore volume of: {originalVolume}");
                foreach (var channel in channels)
                    channel.VolumeLevelScalar = volume;
                return originalVolume;
            }
        }

        public void Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _taskWaitable = Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    if (!SleepForHeartbeatTime()) return;
                    try
                    {
                        RunRuleRound();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Rule run fails: " + ex.Message);
                    }
                }
            }, _cancellationTokenSource.Token);
        }

        private void RunRuleRound()
        {
            var activeProcess = _activeProcessFinder.GetActiveProcessFileName();
            if (_lastActiveProcess == activeProcess) return;
            _lastActiveProcess = activeProcess;
            RunPendingDeactivationActions();
            if (activeProcess == null)
            {
                return;
            }
            var matchingRules = _config.Rules.Where(r => activeProcess.ToLower().EndsWith(r.Process.ToLower()));
            if (!matchingRules.Any())
            {
                RunPendingDeactivationActions();
                return;
            }
            var toRunNow = matchingRules.Select(r => r.OnActivated).Where(s => !String.IsNullOrEmpty(s));
            var toRunOnDeactivation = matchingRules.Select(r => r.OnDeactivated).Where(s => !String.IsNullOrEmpty(s));
            foreach (var r in toRunNow)
                Run(r);
            _pendingDeactivationActions = () =>
            {
                foreach (var r in toRunOnDeactivation)
                    Run(r);
            };
        }

        private void RunPendingDeactivationActions()
        {
            if (_pendingDeactivationActions == null) return;
            _pendingDeactivationActions();
            _pendingDeactivationActions = null;
        }

        private void Run(string s)
        {
            var parts = s.Split(' ');
            var fn = parts[0];
            var args = parts.Length > 1 ? parts.Skip(1).ToArray() : new string[] {};
            if (_runActions.ContainsKey(fn))
            {
                _runActions[fn](args);
            }
        }

        private bool SleepForHeartbeatTime()
        {
            var slept = 0;
            while (slept < _config.Heartbeat && !_cancellationTokenSource.IsCancellationRequested)
            {
                var slice = 10;
                slept += slice;
                Thread.Sleep(slice);
            }
            return !_cancellationTokenSource.IsCancellationRequested;
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
            _taskWaitable.Wait();
        }
    }
}
