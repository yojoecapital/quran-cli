using System;

namespace QuranCli
{
    internal static class Logger
    {
        public static bool verbose;

        public static void Info(object message)
        {
            if (!verbose) return;
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Error.WriteLine($"[INFO] {message}".PadRight(Console.WindowWidth));
            Console.ResetColor();
        }

        public static void Error(object message)
        {
            var s = message.ToString().Trim();
            s = s.EndsWith('.') ? s : s + '.';
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine($"[ERROR] {s}".PadRight(Console.WindowWidth));
            Console.ResetColor();
        }

        public static void Message(object message)
        {
            if (string.IsNullOrWhiteSpace(message.ToString())) return;
            Console.WriteLine(message.ToString().PadRight(Console.WindowWidth));
        }

        public static void Percent(long current, long total)
        {
            var percent = Math.Max(0, Math.Min(0.9999f, (float)current / total)) * 100;
            if (float.IsNaN(percent)) percent = 0;
            Console.Write($"{percent:F2} %".PadRight(Console.WindowWidth) + '\r');
        }
    }
}
