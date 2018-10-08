using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PeanutButter.TinyEventAggregator;
using TrayMonkey.InbuiltActions;
using TrayMonkey.Infrastructure;
using static System.String;

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
        private readonly IProcessHelper _processHelper;
        private Action _pendingDeactivationActions;
        private string _lastActiveProcess;
        private readonly Dictionary<string, IMonkeyAction> _actions;

        public Monkey(
            IMonkeyConfig config, 
            IProcessHelper processHelper,
            IMonkeyAction[] monkeyActions)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _processHelper = processHelper ?? throw new ArgumentNullException(nameof(processHelper));
            _actions = monkeyActions.ToDictionary(a => a.Identifier, a => a, StringComparer.OrdinalIgnoreCase);
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
            lock (_config)
            {
                _processHelper.UpdateActiveProcess();
                var activeProcess = _processHelper.ForegroundProcessName;
                if (_lastActiveProcess == activeProcess)
                    return;

                _lastActiveProcess = activeProcess;
                RunPendingDeactivationActions();
                if (activeProcess == null)
                {
                    return;
                }

                var matchingRules = _config.Rules
                    .Where(r => activeProcess.ToLower().EndsWith(r.Process.ToLower()))
                    .ToArray();
                if (!matchingRules.Any())
                {
                    RunPendingDeactivationActions();
                    return;
                }

                var toRunNow = matchingRules
                    .Select(r => r.OnActivated)
                    .Where(s => !IsNullOrEmpty(s))
                    .ToArray();
                var toRunOnDeactivation = matchingRules
                    .Select(r => r.OnDeactivated)
                    .Where(s => !IsNullOrEmpty(s))
                    .ToArray();
                foreach (var r in toRunNow)
                    Run(r);
                _pendingDeactivationActions = () =>
                {
                    foreach (var r in toRunOnDeactivation)
                        Run(r);
                };
            }
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
            if (_actions.ContainsKey(fn))
            {
                _actions[fn].Run(args);
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
