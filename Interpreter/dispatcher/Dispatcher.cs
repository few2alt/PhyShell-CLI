namespace Interpreter.Logger
{
    public static class Dispatcher
    {
        public enum LOG_STATE : int
        {
            Safe = 0,
            Warning = 1,
            Critical = 2
        }
        public static void Log(string log, LOG_STATE log_state)
        {
            if (log_state == LOG_STATE.Safe)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[SAFE]─> {log}");
                Console.ForegroundColor = ConsoleColor.White;
            } else if (log_state == LOG_STATE.Warning)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine($"[Warning]─> {log}");
                Console.ForegroundColor = ConsoleColor.White;
            } else if (log_state == LOG_STATE.Critical)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"[Critical]─> {log}");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
    }
}
