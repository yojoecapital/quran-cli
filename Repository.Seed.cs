using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace QuranCli
{
    internal partial class Repository
    {
        public static void TestData()
        {
            string url = "https://cdn.jsdelivr.net/npm/quran-json@3.1.2/dist/quran_en.json";

            // Fetch and parse the JSON
            var data = FetchAndParseJson(url);

            // Display some data
            foreach (var surah in data)
            {
                Console.WriteLine($"Surah Name: {surah["name"]}");
                Console.WriteLine($"Translation: {surah["translation"]}");
                Console.WriteLine($"Total Verses: {surah["total_verses"]}");

                Console.WriteLine("First Verse:");
                var verses = (JsonElement)surah["verses"];
                var firstVerse = verses[0];
                Console.WriteLine($"  {firstVerse.GetProperty("id")}: {firstVerse.GetProperty("text")}");
                Console.WriteLine();
            }
        }

        private static List<Dictionary<string, object>> FetchAndParseJson(string url)
        {
            var data = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(json) ?? throw new Exception("Failed to parse JSON.");
            return data;
        }
    }
}