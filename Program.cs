using System;
using QuranCli.Data;

namespace QuranCli
{
    internal static partial class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            using var _ = new Repository();
        }
    }
}