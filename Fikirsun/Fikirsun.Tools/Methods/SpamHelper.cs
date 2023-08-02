using Fikirsun.Entities;

namespace Fikirsun.Tools
{
    public static class SpamHelper
    {
        static char[] ayirac = { ' ', ',', '.', '!', '?' }; // bölme işaretleri            

        public static string Invoke(SpamWord[] spams, string text)
        {

            var commentWords = text.Trim().Split(ayirac, StringSplitOptions.RemoveEmptyEntries);

            foreach (var word in commentWords)
            {
                if (spams.FirstOrDefault(x => x.Name == word) != null)
                {
                    string spamAlert = word.Substring
                        (0, word.Length == 2
                        ? word.Length - 1
                        : word.Length - 2);
                    return $"Yorumunuz bir spam kelime içeriyor : {spamAlert}**";
                }
            }
            return "";
        }
    }
}
