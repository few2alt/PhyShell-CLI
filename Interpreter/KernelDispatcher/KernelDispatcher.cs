using ShellCore.Attributes;
using ShellCore.Kernel;
using ShellCore.KernelLogger;
using System.Reflection;

namespace ShellCore.KernelDispatcher
{
    public class KernelDispatcher
    {
        private CommandKernel kernel;
        private Action<string> action;
        public void RegisterCommandClass(CommandKernel kernel)
        {
            this.kernel = kernel;
            Assembly assembly = Assembly.GetExecutingAssembly();
            foreach (Type type in assembly.GetTypes())
            {
                foreach (MethodInfo method in type.GetMethods())
                {
                    KernelCommandAttribute attributeInfo = (KernelCommandAttribute)method.GetCustomAttribute(typeof(KernelCommandAttribute));
                    if(attributeInfo == null)
                    {
                        continue;
                    }
                    try
                    {
                      action = (Action<string>)Delegate.CreateDelegate(typeof(Action<string>), method);
                    } catch (Exception ex)
                    {
                        Logger.Log($"{ex} \n\nMaybe you should change your method to static?", Logger.LOG_STATE.Critical);
                        Environment.Exit(-1);
                    }
                    if (this.kernel.Commands.ContainsKey(attributeInfo.CommandName))
                    {
                        Logger.Log($"{attributeInfo.CommandName} with description: {attributeInfo.CommandDescription} already exists in commands.", Logger.LOG_STATE.Warning);
                        return;
                    }
                    this.kernel.Commands.Add(attributeInfo.CommandName, action);
                    this.kernel.CommandNames.Add(attributeInfo.CommandName);
                }
            }


        }


    }
}
