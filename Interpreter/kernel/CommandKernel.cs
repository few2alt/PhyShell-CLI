using Interpreter.Logger;
using System.Diagnostics;
using System.Text;
using System.Xml;
namespace Interpreter.Kernel
{
    public class CommandKernel : ICommandHandler 
    {
     
        public Dictionary<string, Action<string>> commands = new Dictionary<string, Action<string>>();
        public delegate void BeforeCommandInvoked();
        public event BeforeCommandInvoked beforeCommandInvoked = () => { };
        public delegate void AfterCommandInvoked();
        public event AfterCommandInvoked afterCommandInvoked = () => { };
        public delegate void BeforePointerUpdated();
        public event BeforePointerUpdated beforePointerUpdated = () => { };
        public delegate void AfterPointerUpdated();
        public event AfterPointerUpdated afterPointerUpdated = () => { };
        public delegate void BeforeCommandLoaded();
        public event BeforeCommandLoaded beforeCommandsLoaded = () => { };
        public delegate void AfterCommandLoaded();
        public event AfterCommandLoaded afterCommandsLoaded = () => { };
        public delegate void BeforeCommandRegistered();
        public event BeforeCommandRegistered beforeCommandRegistered = () => { };
        public delegate void AfterCommandRegistered();
        public event AfterCommandRegistered afterCommandRegistered = () => { };
        public List<string> registeredCommands = new List<string>();
        public CommandKernel()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;
            LoadCommands();
            ShowWelcomingInfo();
            LoadEvents();
            UpdatePointer();
        }
        public void LoadEvents()
        {
            afterCommandInvoked += () =>
            {
                UpdatePointer();
            };
        }
        public void ShowWelcomingInfo()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine($"""
                                 @@@@                               Welcome to PhyShell! 
                         @@@@   @@@@@@   @@@@                This is open-source software aimed at optimizing operating systems
                        @@@@@@@@@@@@@@@@@@@@@@               and improving the user experience.
                        @@@@@@@@@@@@@@@@@@@@@@               
                   @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@          Machine Name:         {Environment.MachineName}
                  @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@         Is 64 bit sys:        {Environment.Is64BitOperatingSystem}
                   @@@@@@@@@@@@@@@   @@@@@@@@@@@@@@          OS:                   {Environment.OSVersion}
                    @@@@@@@@@@          @@@@@@@@@@           Username:             {Environment.UserName}
                  @@@@@@@@@@@            @@@@@@@@@@@         Output Encoding:      {Console.OutputEncoding.EncodingName}
                @@@@@@@@@@@@              @@@@@@@@@@@@       Input Encoding:       {Console.InputEncoding.EncodingName}
                @@@@@@@@@@@@              @@@@@@@@@@@@
                  @@@@@@@@@@@            @@@@@@@@@@@  
                    @@@@@@@@@@          @@@@@@@@@@    
                   @@@@@@@@@@@@@@    @@@@@@@@@@@@@@   
                  @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@          Recommended to run this app with admin rights.
                   @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@           Write ,help' to get list of commands.
                        @@@@@@@@@@@@@@@@@@@@@@        
                        @@@@@@@@@@@@@@@@@@@@@@        
                         @@@@   @@@@@@   @@@@         
                                 @@@@                 
                [Made by PhySoftware Devs]
                """);
            Console.ForegroundColor = ConsoleColor.White;
        }
        public void ReloadKernel()
        {
            LoadCommands();
            ShowWelcomingInfo();
            LoadEvents();
            UpdatePointer();
        }
        private void UpdatePointer()
        {
            beforePointerUpdated.Invoke();
            Console.Write($"┌ {Directory.GetCurrentDirectory()}" + "~\n");
            Console.Write($"└►");
            afterPointerUpdated.Invoke();
        }
        private void UpdatePointer(string text)
        {
            beforePointerUpdated.Invoke();
            Console.Write($"┌ {text}" + "~\n");
            Console.Write($"└►");
            afterPointerUpdated.Invoke();
        }
        public void LoadCommands()
        {
            beforeCommandsLoaded.Invoke();
            RegisterCommand("dir", (string command) =>
            {
               ChangeFolder(command); 
            });
            RegisterCommand("rd", (string command) =>
            {
                RemoveFolder(command);
            });
            RegisterCommand("rf", (string command) =>
            {
                RemoveFile(command);
            });
            RegisterCommand("clear", (string command) =>
            {
                Console.Clear();
            });
            RegisterCommand("kill", (string command) =>
            {
                KillProcessesByName(command);
            });
            RegisterCommand("help", (string command) =>
            {
                GetAllCommandsList();
            });
            RegisterCommand("cd", (string command) =>
            {
                Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(),command));
                Dispatcher.Log($"Created directory [{command}] at current folder.", Dispatcher.LOG_STATE.Safe);
            });
            RegisterCommand("cf", (string command) =>
            {
                CreateFile(command);
            });
            RegisterCommand("tree", (string command) =>
            {
                ShowDirectoryTree(command);
            });
            RegisterCommand("read", (string command) =>
            {
                ReadFile(command);
            });
            RegisterCommand("write", (string command) =>
            {
                WriteInFile(command);
            });
            RegisterCommand("autorun", (string command) =>
            {
                CheckAutoRun();
            });
            RegisterCommand("info", (string command) =>
            {
                ShowWelcomingInfo();
                
            });
            afterCommandsLoaded.Invoke();
        }
        
       
        public void CheckAutoRun()
        {
            string[] files = Directory.GetFiles(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),"Microsoft","Windows", "Start Menu","Programs","Startup"));
            foreach (var file in files)
            {
                Console.WriteLine(file);
            }

        }
        public void ReadFile(string path)
        {
            if (path.StartsWith("~"))
            {
                byte[] bytes = File.ReadAllBytes(Path.Combine(Directory.GetCurrentDirectory(), path.Substring(1)));
                Console.WriteLine(Encoding.UTF8.GetString(bytes));
            } else
            {
                byte[] bytes = File.ReadAllBytes(path);
                Console.WriteLine(Encoding.UTF8.GetString(bytes));
            }
        }
   
     public async Task WriteInFile(string path)
        {
            if (path.StartsWith("~"))
            {
                using (FileStream fstream = File.Open(Path.Combine(Directory.GetCurrentDirectory(), path.Substring(1)), FileMode.Append))
                {
                    Dispatcher.Log($"File: {path.Substring(1)} opened.", Dispatcher.LOG_STATE.Safe);
                    
                        Dispatcher.Log($"Enter something to write in choosen file.", Dispatcher.LOG_STATE.Safe);
                        string data = Console.ReadLine();
                        byte[] bytes = Encoding.UTF8.GetBytes(data);
                        await fstream.WriteAsync(bytes);
                   

                }
            } else
            {
                using (FileStream fstream = File.Open(path, FileMode.Append))
                {
                    Dispatcher.Log($"File opened.", Dispatcher.LOG_STATE.Safe);
                        Dispatcher.Log($"Enter something to write in choosen file.", Dispatcher.LOG_STATE.Safe);
                        string data = Console.ReadLine();
                        byte[] bytes = Encoding.UTF8.GetBytes(data);
                        await fstream.WriteAsync(bytes);
                }
            }
        }
        public void CreateFile(string path)
        {
           if (path.StartsWith("~"))
            {
                File.Create(Path.Combine(Directory.GetCurrentDirectory(), path.Substring(1)));
                Dispatcher.Log($"Created {path.Substring(1)} file at current folder.", Dispatcher.LOG_STATE.Safe);
            } else
            {
                File.Create(path);
                Dispatcher.Log($"Created {path} file.", Dispatcher.LOG_STATE.Safe);
            }
        }
        public void ShowDirectoryTree(string directory)
        {
            if (directory.StartsWith("~"))
            {
                string[] dirs = Directory.GetDirectories(Path.Combine(Directory.GetCurrentDirectory(), directory.Substring(1)));
                string[] files =  Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(),directory.Substring(1)));
                foreach (var dir in dirs)
                {
                    Console.WriteLine(dir);
                }
                foreach (var file in files)
                {
                    Console.WriteLine(file);
                }
             } else {
                string[] dirs = Directory.GetDirectories(directory);
                string[] files = Directory.GetFiles(directory);
                foreach (var dir in dirs)
                {
                    Console.WriteLine(dir);
                }
                foreach (var file in files)
                {
                    Console.WriteLine(file);
                }
            }
        }
        public void RemoveFolder(string dir)
        {
            if (dir.StartsWith("~"))
            {
                Directory.Delete(Path.Combine(Directory.GetCurrentDirectory(), dir.Substring(1)));
                Dispatcher.Log($"Deleted {dir.Substring(1)} folder at current directory.", Dispatcher.LOG_STATE.Safe);
            }
            else
            {
                Directory.Delete(dir);
                Dispatcher.Log($"Deleted {dir} folder.", Dispatcher.LOG_STATE.Safe);
            }
        }
        public void RemoveFile(string dir)
        {
            if (dir.StartsWith("~"))
            {
                File.Delete(Path.Combine(Directory.GetCurrentDirectory(), dir.Substring(1)));
                Dispatcher.Log($"Deleted {dir.Substring(1)} file at current directory.", Dispatcher.LOG_STATE.Safe);
            } else
            {
                File.Delete(dir);
                Dispatcher.Log($"Deleted {dir} file at current directory.", Dispatcher.LOG_STATE.Safe);
            }
        }
        public void ChangeFolder(string dir)
        {
            try
            {
                if (dir.StartsWith('~'))
                {
                    Directory.SetCurrentDirectory(Path.Combine(Directory.GetCurrentDirectory(), dir.Substring(1)));
                }
                else
                {
                    Directory.SetCurrentDirectory(dir);
                }
                Dispatcher.Log($"Set current directory to {Directory.GetCurrentDirectory()}", Dispatcher.LOG_STATE.Safe);
            }
            catch
            {
                if (dir != "dir")
                    Dispatcher.Log($"Cant find {dir} directory.", Dispatcher.LOG_STATE.Critical);
                else if (dir == "dir")
                {
                    Directory.SetCurrentDirectory("C:\\");
                }
            }
        }
        public void GetAllCommandsList()
        {
            Console.WriteLine("Available commands:");
            Console.ForegroundColor = ConsoleColor.Green;
            foreach(string command in registeredCommands)
            {
                Console.WriteLine(command);
            }
            Console.ForegroundColor = ConsoleColor.White;
        }
        public void KillProcessesByName(string name)
        {
            Process[] processes = Process.GetProcessesByName(name);
            foreach (Process process in processes)
            {
                process.Kill(true);
                Dispatcher.Log($"Killed process: {process.ProcessName}", Dispatcher.LOG_STATE.Safe);
            }
        }
       
        public void RegisterCommand(string command, Action<string> action)
        {
            beforeCommandRegistered?.Invoke();
            registeredCommands?.Add(command);
            commands.Add(command, (string command) =>
            {
                beforeCommandInvoked?.Invoke();
                action?.Invoke(command);
                afterCommandInvoked?.Invoke();
            });
            afterCommandRegistered?.Invoke();
        }
        public void Handle()
        {
           string? input = Console.ReadLine();
            string[]? result = input?.Split(' ', 2);
            try
            {
                if (result?.Length > 1)
                    commands[result[0]]?.Invoke(result[1]);
                else
                    commands[result[0]]?.Invoke(result[0]);
            } catch
            {
                if (!commands.ContainsKey(result[0]))
                {
                    Dispatcher.Log($"Unknown Command: {result[0]}", Dispatcher.LOG_STATE.Critical);
                } else
                {
                    Dispatcher.Log($"Invalid syntax of {result[0]} command.", Dispatcher.LOG_STATE.Critical);
                }
               UpdatePointer();
            }
            
        }
       
    }
}
