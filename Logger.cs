using System;

namespace QuranCli
{
    public static class Logger
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

        public static void Percent(long current, long total, bool newLine = false)
        {
            Percent((double)current / total, newLine);
        }

        public static void Percent(double fraction, bool newLine = false)
        {
            var percent = Math.Max(0, Math.Min(1, fraction)) * 100;
            if (double.IsNaN(percent)) percent = 0;
            Console.Write($"{percent:F2} %".PadRight(Console.WindowWidth) + '\r');
            if (newLine) Console.WriteLine();
        }
    }
}
