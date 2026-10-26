namespace Interpreter.Kernel
{
    public interface ICommandHandler
    {
        public void Handle();
        public void RegisterCommand(string command, Action<string> action);
    }
}
