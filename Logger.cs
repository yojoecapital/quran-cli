using System;
using System.Linq;

namespace QuranCli
{
    internal static class Logger
    {
        public static void Info(object message)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Error.WriteLine($"[INFO] {message}".PadRight(Console.WindowWidth));
            Console.ResetColor();
        }

        public static void Error(object message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine($"[ERROR] {message}".PadRight(Console.WindowWidth));
            Console.ResetColor();
        }

        public static void Message(object message) => Console.WriteLine(message.ToString().PadRight(Console.WindowWidth));

        public static void Percent(long current, long total)
        {
            var percent = Math.Max(0, Math.Min(0.9999f, (float)current / total)) * 100;
            if (float.IsNaN(percent)) percent = 0;
            Console.Write($"{percent:F2} %".PadRight(Console.WindowWidth) + '\r');
        }
    }
}
