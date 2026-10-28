namespace ShellCore.Kernel
{
    public class CommandKernel
    {
        public Dictionary<string,Action<string>> Commands { get; set; } = new Dictionary<string,Action<string>>();
        public List<string> CommandNames { get; set; } = new List<string>();
    }
}
