using System;
using System.Collections.Generic;
using QuranCli.Data.Models;
using QuranCli.Data.Yaml;

namespace QuranCli.Commands
{
    public static class GetNoteHandler
    {
        public static void Handle(int[] ids)
        {
            var list = new List<Note>(ids.Length);
            foreach (var id in ids)
            {
                try
                {
                    list.Add(Note.SelectById(id));
                }
                catch { }
            }
            if (list.Count == 0) throw new Exception("No notes found");
            YamlProcessor.Write(list);
        }
    }
}