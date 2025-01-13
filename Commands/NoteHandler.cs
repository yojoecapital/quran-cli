using QuranCli.Arguments;

namespace QuranCli.Commands
{
    internal static class NoteHandler
    {
        public static void Handle(AyatSelection selection, string text, AyatSelectionOrGroup selectionOrGroup)
        {
            if (!string.IsNullOrEmpty(text))
            {
                if (selectionOrGroup == null)
                {

                }
                else
                {

                }
            }
        }

        // public static bool TryGetNoteForSelection(AyatSelection ayatSelection)
        // {

        // }
    }
}