using System;

namespace QuranCli
{
    internal static partial class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Repository.TestData();
        }
    }
}