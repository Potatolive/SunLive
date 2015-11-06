using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Downloader
{
    static class TextParts
    {
        public static List<List<string>> getParts(string content, string publishedBy, int MAX_CHAR, int MAX_LINE, bool multiParts)
        {
            string publishedByFirstName = publishedBy.Split(' ')[0];

            try
            {
                List<string> lines = createLines(content, publishedByFirstName, MAX_CHAR, MAX_LINE);
                var parts = createParts(lines, MAX_LINE);

                if(parts.Count > MAX_LINE)
                {
                    parts.RemoveRange(MAX_LINE, parts.Count - MAX_LINE);
                }

                if (!multiParts)
                {
                    parts = new List<List<string>>() { parts[0] };
                }

                return addPublishedByAndPartNumber(parts, publishedByFirstName, MAX_CHAR, MAX_LINE);
            }
            catch (Exception ex)
            {
                List<List<string>> parts = new List<List<string>>() { new List<string>() { " " } };
                return addPublishedByAndPartNumber(parts, publishedByFirstName, MAX_CHAR, MAX_LINE);
            }
        }

        private static List<List<string>> addPublishedByAndPartNumber(List<List<string>> parts, string publishedBy, int MAX_CHAR, int MAX_LINE)
        {
            int partCount = 0;
            foreach (var part in parts)
            {
                partCount++;
                for (int i = 0; i < part.Count; i++)
                {
                    if (i + 1 == part.Count)
                    {

                        publishedBy = Regex.Replace(publishedBy, @"[^\u0000-\u007F]", string.Empty);

                        string publishedByText = string.Empty;

                        if (parts.Count > 1)
                        {
                            publishedByText = "[" + partCount + "/" + (parts.Count) + "] - " + publishedBy;
                        }
                        else
                        {
                            publishedByText = " - " + publishedBy;
                        }

                        if (part.Count < MAX_LINE && !string.IsNullOrWhiteSpace(part[i]))
                        {
                            part.Add(" - " + publishedBy);
                        }
                        else
                        {
                            if (part[i] != null && part[i].Length + publishedByText.Length > MAX_CHAR)
                            {
                                part[i] = part[i].Substring(0, MAX_CHAR - publishedByText.Length);
                            }

                            part[i] += publishedByText;
                        }
                        break;
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
            if (string.IsNullOrWhiteSpace(textContent)) return new List<string>() { string.Empty };

            textContent = textContent.Replace('\n', ' ').Replace('\t', ' ');
            textContent = Regex.Replace(textContent, @"[^\u0000-\u007F]", string.Empty);

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
                    }

                    currentLine = "";

                    lines.Add(word.Substring(0, MAX_CHAR - 1));

                    if (word.Length >= MAX_CHAR)
                    {
                        currentLine = word.Substring(MAX_CHAR, word.Length - MAX_CHAR) + ' ';
                    }

                    continue;
                }

                if ((String.Format("{0} {1}", currentLine, w).Length > (((lines.Count + 1) % MAX_LINE == 0) ? (MAX_CHAR - publishedBy.Length - 5) : MAX_CHAR)))
                {
                    if (!String.IsNullOrWhiteSpace(currentLine))
                    {
                        lines.Add(currentLine);
                    }
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
