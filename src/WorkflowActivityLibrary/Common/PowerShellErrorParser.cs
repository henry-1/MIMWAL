using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace MicrosoftServices.IdentityManagement.WorkflowActivityLibrary.Common
{
    [SuppressMessage("SonarAnalyzer.CSharp", "S6444", Justification = "Regex is constrained to a simple, bounded pattern.")]
    public static class PowerShellErrorParser
    {
        private static readonly Regex AnsiRegex = new Regex(@"_x001B_\[[0-9;]*[a-zA-Z]", RegexOptions.Compiled);
        private static readonly Regex NewlineRegex = new Regex(@"_x000D_|_x000A_", RegexOptions.Compiled);
        private static readonly Regex Ps7LineNumberRegex = new Regex(@"^\d+\s*\|", RegexOptions.Compiled);

        public static string ExtractCleanErrors(string rawCliXml , string psExePath)
        {
            if (string.IsNullOrEmpty(rawCliXml) || rawCliXml.Trim().Length == 0)
                return string.Empty;

            // 1. Strip CLIXML header prefix to ensure standard XML parsing
            string cleanXml = rawCliXml;
            if (cleanXml.StartsWith("< CLIXML"))
            {
                int firstOpenBracket = cleanXml.IndexOf('<', 2);
                if (firstOpenBracket >= 0)
                    cleanXml = cleanXml.Substring(firstOpenBracket);
            }

            // 2. Extract raw text inside error nodes safely
            StringBuilder rawTextBuilder = new StringBuilder();
            try
            {
                XDocument doc = XDocument.Parse(cleanXml);
                XNamespace ns = doc.Root.GetDefaultNamespace();

                foreach (XElement node in doc.Descendants(ns + "S"))
                {
                    XAttribute sAttr = node.Attribute("S");
                    if (sAttr != null && sAttr.Value == "Error")
                    {
                        rawTextBuilder.Append(node.Value);
                    }
                }
            }
            catch
            {
                Regex fallbackRegex = new Regex("<S S=\"Error\">(.*?)</S>");
                foreach (Match match in fallbackRegex.Matches(rawCliXml))
                {
                    rawTextBuilder.Append(match.Groups[1].Value);
                }
            }

            string rawText = rawTextBuilder.ToString();
            rawText = AnsiRegex.Replace(rawText, string.Empty);
            rawText = NewlineRegex.Replace(rawText, "\n");

            string[] lines = rawText.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            // 3. Route based on layout signature markers
            if (psExePath.Contains("powershell.exe"))
            {
                return ParsePowerShell5(lines);
            }

            return ParsePowerShell7(lines);
        }

        /// <summary>
        /// Strict PowerShell 5 Layout Engine Parser
        /// </summary>
        [SuppressMessage("SonarAnalyzer.CSharp", "S127", Justification = "Approved.")]
        [SuppressMessage("SonarAnalyzer.CSharp", "S3776", Justification = "Approved.")]
        private static string ParsePowerShell5(string[] lines)
        {
            List<string> errorLines = new List<string>();
            for(var i = 0; i < lines.Length; i++)
            {
                var currentLine = lines[i].Trim();
                if (string.IsNullOrEmpty(currentLine)) continue;

                if (currentLine.StartsWith("At line:"))
                {
                    if (i > 0)
                        errorLines.Add(lines[i - 1]);

                    errorLines.Add(lines[i]);
                    i++;

                    while (i < lines.Length && lines[i].Trim().StartsWith("+"))
                    {
                        var errorLine = lines[i].Trim();

                        if (errorLine.Contains("~~~"))
                        {
                            i++; // CRITICAL: Safely advance the index BEFORE skipping
                            continue; // This jumps strictly to the next iteration of the WHILE loop
                        }

                        // Normal processing for valid "+" error rows
                        if (errorLine.StartsWith("+ ") || errorLine.StartsWith(": "))
                            errorLine = errorLine.Substring(2).Trim();

                        errorLines.Add(errorLine);
                        i++;
                    }
                }
            }
            return string.Join(Environment.NewLine, errorLines.ToArray());
        }

        [SuppressMessage("SonarAnalyzer.CSharp", "S1144", Justification = "Approved.")]
        [SuppressMessage("SonarAnalyzer.CSharp", "S3776", Justification = "Approved.")]
        private static string ParsePowerShell6(string[] lines)
        {
            List<string> filteredLines = new List<string>();
            bool isSkippingScriptEcho = false;

            foreach (string rawLine in lines)
            {
                string line = rawLine.Trim();
                if (string.IsNullOrEmpty(line)) continue;

                // When "At line:" hits, turn on the flag to drop code mirrors
                if (line.StartsWith("At line:"))
                {
                    isSkippingScriptEcho = true;
                    continue;
                }

                // Engine metadata indicators mark the absolute end of a script echo block
                if (line.Contains("CategoryInfo") || line.Contains("FullyQualifiedErrorId"))
                {
                    isSkippingScriptEcho = false;
                    continue;
                }

                // Drop any line in the code mirror zone
                if (isSkippingScriptEcho) continue;

                // Drop structural artifacts 
                if (line.Contains("~~~")) continue;
                if (line.StartsWith("+")) continue;

                // Strip leading terminal markers if present
                if (line.StartsWith(":")) line = line.Substring(1).Trim();

                if (string.IsNullOrEmpty(line)) continue;

                if (!filteredLines.Contains(line))
                {
                    filteredLines.Add(line);
                }
            }

            return string.Join(Environment.NewLine, filteredLines.ToArray());
        }

        /// <summary>
        /// Strict PowerShell 7 / Core Layout Engine Parser
        /// </summary>
        [SuppressMessage("SonarAnalyzer.CSharp", "S3776", Justification = "Approved.")]
        private static string ParsePowerShell7(string[] lines)
        {
            List<string> filteredLines = new List<string>();

            foreach (string rawLine in lines)
            {
                string line = rawLine.Trim();
                if (string.IsNullOrEmpty(line)) continue;

                // Drop PS7 tracking frames completely
                if (line == "Exception:" || line == "Write-Error:") continue;
                if (line.StartsWith("Line |")) continue;
                if (Ps7LineNumberRegex.IsMatch(line)) continue;
                if (line.Contains("~~~")) continue;

                // Strip margin pipes
                if (line.StartsWith("|")) line = line.Substring(1).Trim();

                if (string.IsNullOrEmpty(line)) continue;

                if (!filteredLines.Contains(line))
                {
                    filteredLines.Add(line);
                }
            }

            return string.Join(Environment.NewLine, filteredLines.ToArray());
        }
    }
}
