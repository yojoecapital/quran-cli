using System;
using System.Diagnostics;
using System.IO;

namespace QuranCli.Utilities
{
    public static class EditorHelper
    {
        public static string OpenEditorAndReadInput(string initialText = "", string extension = "md")
        {
            string tempFile = $"{Path.GetTempFileName()}.{extension}";
            string editor = Environment.GetEnvironmentVariable("EDITOR")
                ?? Environment.GetEnvironmentVariable("VISUAL")
                ?? GetDefaultEditor();
            try
            {
                if (!string.IsNullOrEmpty(initialText)) File.WriteAllText(tempFile, initialText);
                if (string.IsNullOrEmpty(editor)) throw new Exception("No text editor found. Set the EDITOR environment variable");
                Logger.Info($"Trying to start '{editor}' with file '{tempFile}'.");
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = editor,
                        Arguments = tempFile,
                        UseShellExecute = false
                    }
                };
                process.Start();
                process.WaitForExit();
                return File.ReadAllText(tempFile);
            }
            catch
            {
                throw new Exception($"Could not start '{editor}'");
            }
            finally
            {
                if (File.Exists(tempFile))
                {
                    Logger.Info($"Cleaning up '{tempFile}'.");
                    File.Delete(tempFile);
                }
            }
        }

        private static string GetDefaultEditor()
        {
            if (OperatingSystem.IsWindows()) return "notepad";
            if (OperatingSystem.IsLinux()) return "nano";
            if (OperatingSystem.IsMacOS()) return "vim";
            return null;
        }
    }
}