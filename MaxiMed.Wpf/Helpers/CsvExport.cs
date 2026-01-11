using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Wpf.Helpers
{
    public static class CsvExport
    {
        public static void Save(string filePath, string[] headers, IEnumerable<string[]> rows)
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Join(";", headers.Select(Escape)));

            foreach (var r in rows)
                sb.AppendLine(string.Join(";", r.Select(Escape)));

            File.WriteAllText(filePath, sb.ToString(), new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
        }

        private static string Escape(string? s)
        {
            s ??= "";
            if (s.Contains(';') || s.Contains('"') || s.Contains('\n') || s.Contains('\r'))
                return "\"" + s.Replace("\"", "\"\"") + "\"";
            return s;
        }

        public static string Money(decimal v) => v.ToString("0.00", CultureInfo.InvariantCulture);
        public static string Date(DateTime d) => d.ToString("dd.MM.yyyy");
        public static string DateTime(DateTime d) => d.ToString("dd.MM.yyyy HH:mm");
    }
}
