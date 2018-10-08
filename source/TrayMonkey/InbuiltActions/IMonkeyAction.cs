namespace TrayMonkey.InbuiltActions
{
    public interface IMonkeyAction
    {
        string Identifier { get; }
        void Run(string[] args);
    }
}