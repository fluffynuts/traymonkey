namespace TrayMonkey.InbuiltActions
{
    public interface IMonkeyAction
    {
        string Identifier { get; }
        void Run(string[] args);
    }

    public class SetVolume : IMonkeyAction
    {
        public string Identifier => "SetVolume";
        public void Run(string[] args)
        {
        }
    }
}