using System;
using System.Linq;
using QuranCli.Arguments;
using QuranCli.Data.Models;
using QuranCli.Utilities;

namespace QuranCli.Commands
{

    internal static class TopHandler
    {
        public static void Handle(string selectionString, ushort topN)
        {
            if (!AyatSelection.TryParse(selectionString, out var selection)) throw new Exception("Could not parse selection");
            var mostSimilar = StringExtensions.FindTopNSimilar(
                selection.GetAyat().Select(ayah => ayah.Verse),
                topN
            );
            var versesScores = mostSimilar.Select(tuple => new VersesScore()
            {
                Verse1 = tuple.BaseString,
                Verse2 = tuple.SimilarString,
                Score = tuple.Score
            });
            Logger.Message(YamlSerializable.ToYaml(versesScores));
        }
    }
}