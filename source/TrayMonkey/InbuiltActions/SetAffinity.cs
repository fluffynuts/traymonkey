using System;
using System.Diagnostics;
using System.Linq;
using PeanutButter.Utils;
using TrayMonkey.Infrastructure;

namespace TrayMonkey.InbuiltActions
{
    public class SetAffinity: IMonkeyAction
    {
        private readonly IProcessHelper _processHelper;
        private readonly IAffinityHelper _affinityHelper;
        private readonly INotifier _notifier;
        public string Identifier => "SetAffinity";


        public SetAffinity(
            IProcessHelper processHelper,
            IAffinityHelper affinityHelper,
            INotifier notifier)
        {
            _processHelper = processHelper;
            _affinityHelper = affinityHelper;
            _notifier = notifier;
        }

        public void Run(string[] args)
        {
            if (args.Length == 0)
            {
                return;
            }
            var processors = args[0].Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => int.TryParse(s.Trim(), out var result) ? result : 0)
                .ToArray();
            if (args.Length > 1)
            {
                var processes = _processHelper.FindProcessesByName(args[1]);
                _affinityHelper.SetAffinity(processes, processors);
                if (processes.Any())
                {
                    _notifier.ShowDebugInfo($"Set affinity {args[0]} for {args[1]} ({processes.Length} processes)");
                }
                else
                {
                    _notifier.ShowDebugInfo($"No processes found to match {args[0]} for affinity set");
                }
            }
            else
            {
                _affinityHelper.SetForegroundAffinity(processors);
            }
        }
    }

    public interface IAffinityHelper
    {
        void SetForegroundAffinity(int[] cores);
        void SetAffinity(Process[] processes, int[] cores);
    }

    public class AffinityHelper: IAffinityHelper
    {
        private readonly IProcessHelper _processHelper;

        public AffinityHelper(IProcessHelper processHelper)
        {
            _processHelper = processHelper;
        }

        public void SetForegroundAffinity(int[] cores)
        {
            SetAffinity(new[] { _processHelper.ForegroundProcess }, cores);
        }
        
        public void SetAffinity(Process[] processes, int[] cores)
        {
            var affinity = cores.Aggregate(0, (acc, cur) => acc |= cur);
            // expect cores 1-indexed, but IdealProcessor is zero-based
            var rr = new RoundRobin<int>(cores.Select(c => c - 1).ToArray());
            foreach (var process in processes)
            foreach (ProcessThread thread in process.Threads)
            {
                thread.IdealProcessor = rr.Next();
                thread.ProcessorAffinity = (IntPtr)affinity;
            }
        }
    }

    public class RoundRobin<T>
    {
        private readonly T[] _values;
        private int _current;

        public RoundRobin(T[] values)
        {
            if (values.IsEmpty())
            {
                throw new InvalidOperationException("RoundRobin with no values");
            }

            _current = 0;
            _values = values;
        }

        public T Next()
        {
            var result = _values[_current];
            _current++;
            if (_current >= _values.Length)
                _current = 0;
            return result;
        }
    }
}