using System;
using System.Collections.Generic;
using System.IO;

namespace SaiThemeUtils {
    // https://stackoverflow.com/questions/4133377/splitting-a-string-number-every-nth-character-number
    public static class StringExtensions {
        public static List<string> SplitInParts(this String s, Int32 partLength) {
            if (s == null)
                throw new ArgumentNullException(nameof(s));
            if (partLength <= 0)
                throw new ArgumentException("part length has to be positive.", nameof(partLength));

            var output = new List<string>();

            for (var i = 0; i < s.Length; i += partLength)
                output.Add(s.Substring(i, Math.Min(partLength, s.Length - i)));

            return output;
        }

    }

    public class StringUtils {
        // https://stackoverflow.com/a/34033925
        public static string AddQuotesIfRequired(string path) {
            return !string.IsNullOrWhiteSpace(path) ?
              path.Contains(" ") && (!path.StartsWith("\"") && !path.EndsWith("\"")) ?
              "\"" + path + "\"" : path :
              string.Empty;
        }
    }

    public class ReplacerHelper {
        public ReplacerHelper(string search, string replace) {
            Search = search;
            Replace = replace;
        }

        public string Search {
            get;
        }
        public string Replace {
            get;
        }
    }

    public class FileUtils {
        // https://stackoverflow.com/a/937558
        public static bool IsFileLocked(FileInfo file) {
            try {
                using (FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None)) {
                    stream.Close();
                }
            } catch (IOException) { // file is in use or doesn't exist
                return true;
            }

            return false;
        }

        public static Dictionary<string, string> ReadConfig(string path) {
            if (!File.Exists(path)) {
                path = "defaultConfig.txt";
            }

            string[] lines = File.ReadAllLines(path);
            var output = new Dictionary<string, string>();

            foreach (string line in lines) {
                if (line.StartsWith("#") || String.IsNullOrWhiteSpace(line)) {
                    continue;
                }

                string[] keyValue = line.Split("=");
                string property = @keyValue[0];
                string color = ColorUtils.CorrectHexColor(@keyValue[1]);

                output.Add(property, color);
            }

            return output;
        }

        public static void MakeBackup(string path) {
            string targetPath = $"{Path.GetDirectoryName(path)}\\{@Path.GetFileName(path)}.bak";

            if (!File.Exists(targetPath)) {
                File.Copy(path, targetPath);
                Logger.LogColor($"backup copy generated in [{targetPath}]\r\n", ConsoleColor.Green);
            } else {
                Logger.LogColor($"backup copy already exists in [{targetPath}]\r\n", ConsoleColor.Yellow);
            }
        }
    }

    public class Logger {
        public static void Log(string msg, ConsoleColor? fg = null, ConsoleColor? bg = null) {
            if (fg == null && bg == null) {
                Console.ResetColor();
            } else {
                Console.ForegroundColor = (ConsoleColor)fg;
                Console.BackgroundColor = (ConsoleColor)bg;
            }

            Console.WriteLine(msg);
            Console.ResetColor();
        }

        // https://stackoverflow.com/a/60492990
        public static void LogColor(string message, ConsoleColor color) {
            var pieces = System.Text.RegularExpressions.Regex.Split(message, @"(\[[^\]]*\])");

            for (int i = 0; i < pieces.Length; i++) {
                string piece = pieces[i];

                if (piece.StartsWith("[") && piece.EndsWith("]")) {
                    Console.ForegroundColor = color;
                    piece = piece.Substring(1, piece.Length - 2);
                }

                Console.Write(piece);
                Console.ResetColor();
            }

            Console.WriteLine();
        }
    }

    public class ColorUtils {
        /// <summary>
        /// converts hex colors between big- and little-endianness.
        /// </summary>
        public static string CorrectHexColor(string hex) {
            var output = hex.SplitInParts(2);
            output.Reverse();

            return String.Join("", output.ToArray()).ToLower();
        }
    }
}