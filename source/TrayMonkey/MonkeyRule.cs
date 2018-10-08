namespace TrayMonkey
{
    public class MonkeyRule
    {
        public string Name { get; }
        public string Process { get; }
        public string OnActivated { get; }
        public string OnDeactivated { get; }

        public MonkeyRule(
            string name,
            string process,
            string onActivated,
            string onDeactivated)
        {
            Name = name;
            Process = process;
            OnActivated = onActivated;
            OnDeactivated = onDeactivated;
        }
    }
}