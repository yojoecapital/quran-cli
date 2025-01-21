namespace QuranCli.Commands
{
    public static class VersionHandler
    {
        public static void Handle()
        {
            Logger.Message(Program.version);
        }
    }
}