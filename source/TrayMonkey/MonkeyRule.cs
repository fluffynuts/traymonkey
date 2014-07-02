namespace TrayMonkey
{
    public class MonkeyRule
    {
        public string Name { get; protected set; }
        public string Process { get; protected set; }
        public string OnActivated { get; protected set; }
        public string OnDeactivated { get; protected set; }

        public MonkeyRule(string name, string process, string onActivated, string onDeactivated)
        {
            Name = name;
            Process = process;
            OnActivated = onActivated;
            OnDeactivated = onDeactivated;
        }
    }
}