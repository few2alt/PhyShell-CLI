namespace ShellCore.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class KernelCommandAttribute : Attribute
    {
        public string CommandName { get; set; } = string.Empty;
        public string CommandDescription { get; set; } = string.Empty;
        public KernelCommandAttribute(string commandName, string commandDescription = "")
        {
            CommandName = commandName;
            CommandDescription = commandDescription;
        }

    }
}
