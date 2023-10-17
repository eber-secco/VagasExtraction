using System.Globalization;
using System.Text;

namespace VagasExtraction.Tools
{
    public class Tool
    {
        public static string RemoveSpecialChars(string texto)
        {
            if (string.IsNullOrEmpty(texto))
                return string.Empty;

            var s = texto.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var t in s)
            {
                var uc = CharUnicodeInfo.GetUnicodeCategory(t);
                if (uc != UnicodeCategory.NonSpacingMark)
                    sb.Append(t);
            }

            return sb.ToString();
        }
    }
}
