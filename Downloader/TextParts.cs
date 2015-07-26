using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Downloader
{
    static class TextParts
    {
        public static List<List<string>> getParts(string content, string publishedBy, int MAX_CHAR, int MAX_LINE)
        {
            string publishedByFirstName = publishedBy.Split(' ')[0];

            List<string> lines = createLines(content, publishedByFirstName, MAX_CHAR, MAX_LINE);
            var parts = createParts(lines, MAX_LINE);
            return addPublishedByAndPartNumber(parts, publishedByFirstName);
        }

        private static List<List<string>> addPublishedByAndPartNumber(List<List<string>> parts, string publishedBy)
        {
            int partCount = 0;
            foreach (var part in parts)
            {
                partCount++;
                for (int i = 0; i < part.Count; i++)
                {
                    if (i + 1 == part.Count)
                    {
                        if (parts.Count > 1)
                        {
                            part[i] += "[" + partCount + "/" + (parts.Count) + "] - " + publishedBy;
                        }
                        else
                        {
                            part[i] += " - " + publishedBy;
                        }
                    }
                }
            }

            return parts;
        }

        public static List<List<string>> createParts(List<string> lines, int MAX_LINE)
        {
            int totalLines = (int)Math.Ceiling((double)lines.Count / MAX_LINE);

            List<List<string>> parts = new List<List<string>>(totalLines);
            List<string> part = new List<string>();

            int lineCount = 0;

            foreach (var line in lines)
            {
                part.Add(line);

                if ((lineCount + 1) % MAX_LINE == 0)
                {
                    parts.Add(part);
                    part = new List<string>();
                }

                lineCount++;
            }

            if(part.Count > 0) parts.Add(part);

            return parts;
        }

        private static List<string> createLines(string textContent, string publishedBy, int MAX_CHAR, int MAX_LINE)
        {
            if (string.IsNullOrWhiteSpace(textContent)) return new List<string>() { publishedBy };

            var lines = new List<string>();
            var currentLine = String.Empty;
            var split = textContent.Split(' ');

            foreach (var w in split)
            {
                var word = w.Trim();

                if (word.Length >= MAX_CHAR)
                {
                    if (!String.IsNullOrWhiteSpace(currentLine))
                    {
                        lines.Add(currentLine);
                        currentLine = "";
                    }

                    lines.Add(word.Substring(0, MAX_CHAR - 1));

                    if (word.Length <= 2 * MAX_CHAR)
                    {
                        currentLine = word.Substring(MAX_CHAR - 1, word.Length - MAX_CHAR - 1) + ' ';
                    }

                    continue;
                }

                if ((String.Format("{0} {1}", currentLine, w).Length > (((lines.Count + 1) % MAX_LINE == 0) ? (MAX_CHAR - publishedBy.Length - 5) : MAX_CHAR)))
                {
                    lines.Add(currentLine);
                    currentLine = "";
                }

                if (!string.IsNullOrWhiteSpace(word))
                {
                    currentLine += w + " ";
                }
            }

            lines.Add(currentLine);

            return lines;
        }
    }
}
