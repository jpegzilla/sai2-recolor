using System;
using System.Collections.Generic;
using System.IO;

namespace SaiThemeColorChanger {
    // Hex strat derived from https://social.msdn.microsoft.com/Forums/vstudio/en-US/a0b2133f-ae23-4c0b-b136-dd531952f3c7/find-amp-replace-hex-values-in-a-dll-file-using-c?forum=csharpgeneral

    // https://stackoverflow.com/questions/4133377/splitting-a-string-number-every-nth-character-number
    static class StringExtensions {
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

    class Program {
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

        // https://stackoverflow.com/a/937558
        static bool IsFileLocked(FileInfo file) {
            try {
                using (FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None)) {
                    stream.Close();
                }
            } catch (IOException) { // file is in use or doesn't exist
                return true;
            }

            return false;
        }

        static void Log(string msg, ConsoleColor? fg = null, ConsoleColor? bg = null) {
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
        static void LogColor(string message, ConsoleColor color) {
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

        /// <summary>
        /// converts hex colors between big <-> little endianness.
        /// </summary>
        static string CorrectHexColor(string hex) {
            var output = hex.SplitInParts(2);
            output.Reverse();

            return String.Join("", output.ToArray()).ToLower();
        }

        static Dictionary<string, string> ReadConfig(string path) {
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
                string color = CorrectHexColor(@keyValue[1]);

                LogColor($"key: [{property}], value: [{color}]", ConsoleColor.Red);

                output.Add(property, color);
            }

            return output;
        }

        static void Main(string[] args) {
            string inputPath = "";

            CorrectHexColor("CD5F2A");

            Dictionary<string, string> config = ReadConfig("config.txt");

            if (args.Length > 0)
                inputPath = args[0];

            string AddQuotesIfRequired(string path) {
                return !string.IsNullOrWhiteSpace(path) ?
                  path.Contains(" ") && (!path.StartsWith("\"") && !path.EndsWith("\"")) ?
                  "\"" + path + "\"" : path :
                  string.Empty;
            }

            if (inputPath.Length == 0) {
                LogColor("please drag the [sai2.exe] file into this window and press enter.\r\n", ConsoleColor.Green);
                inputPath = @AddQuotesIfRequired(Console.ReadLine());
                while (!Directory.Exists(Path.GetDirectoryName(inputPath))) {
                    LogColor($"[{inputPath}] is not a valid path.", ConsoleColor.Red);
                    inputPath = Console.ReadLine();
                }
            }

            if (!Directory.Exists(Path.GetDirectoryName(inputPath))) {
                LogColor($"[{inputPath}] is not a valid path.", ConsoleColor.Red);
                Console.ReadKey();
                return;
            }

            if (IsFileLocked(new FileInfo(inputPath))) {
                LogColor("your copy of sai2.exe is in use. please try again after closing this program as well as sai2.", ConsoleColor.Red);
                Environment.Exit(1);
            }

            // the output path has to be the same as the original file. ths is because the program will look for a .ini file
            // with the same filename as the executable. for example, if you call sai "sai21.exe" instead of "sai2.exe", sai
            // will complain that it's missing a "sai21.ini" file. you could just rename the .ini if you really wanted to.
            string outputPath = inputPath;

            List<ReplacerHelper> toReplace = new List<ReplacerHelper>();
            // Hex color code -> replacement (won't work with pure white and pure black, but everything else seems fine!)
            // Basically this replaces left hex with the right hex.
            // You can swap out the values to get other colors, I haven't noticed any issues using a version with these values modified
            toReplace.Add(new ReplacerHelper("f8f8f8", config.GetValueOrDefault("MainPanelColor", "212121")));
            toReplace.Add(new ReplacerHelper("c0c0c0", config.GetValueOrDefault("CanvasBackgroundColor", "111111")));
            toReplace.Add(new ReplacerHelper("e8e8e8", "3a3a3a")); // Scrollbar insides
            toReplace.Add(new ReplacerHelper("969696", "2a2a2a")); // Scrollbars
            toReplace.Add(new ReplacerHelper("f0f0f0", "212121")); // Tools background
            toReplace.Add(new ReplacerHelper("d4d4d4", "212121")); // Inactive scrollbar
            toReplace.Add(new ReplacerHelper("b0b0b0", "111111")); // Active canvas background
            toReplace.Add(new ReplacerHelper("e0e0e0", "313131")); // Tools panel background

            toReplace.Add(new ReplacerHelper("b1b1b1", "313131")); // Panel borders 1
            toReplace.Add(new ReplacerHelper("d0d0d0", "313131")); // Panel borders 2
            toReplace.Add(new ReplacerHelper("d8d8d8", "313131")); // Panel borders 3
            toReplace.Add(new ReplacerHelper("dadada", "313131")); // Panel borders 4
            toReplace.Add(new ReplacerHelper("e4e4e4", "313131")); // Panel borders 5
            toReplace.Add(new ReplacerHelper("f4f4f4", "313131")); // Panel borders 6

            toReplace.Add(new ReplacerHelper("c6c6c6", "707070")); // Panel separator button 1

            toReplace.Add(new ReplacerHelper("cecece", "111111")); // Corners 1
            toReplace.Add(new ReplacerHelper("c9c9c9", "111111")); // Corners 2
            toReplace.Add(new ReplacerHelper("eeeeee", "2d2d2d")); // Corners 3
            toReplace.Add(new ReplacerHelper("dedede", "313131")); // Corners 4
            toReplace.Add(new ReplacerHelper("b4b4b4", "313131")); // Corners 5
            toReplace.Add(new ReplacerHelper("8c8c8c", "212121")); // Corners 6
            toReplace.Add(new ReplacerHelper("7d7d7d", "212121")); // Corners 7

            toReplace.Add(new ReplacerHelper("204080", "a1a1a1")); // Brown 1
            toReplace.Add(new ReplacerHelper("204172", "a1a1a1")); // Brown 2

            toReplace.Add(new ReplacerHelper("1c4da8", "ffab7f")); // Brown 3
            toReplace.Add(new ReplacerHelper("234a93", "ffab7f")); // Brown 4
            toReplace.Add(new ReplacerHelper("254177", "ffab7f")); // Brown 5
            toReplace.Add(new ReplacerHelper("1f2e49", "ffab7f")); // Brown 6
            toReplace.Add(new ReplacerHelper("436616", "ffab7f")); // Brown 7
            toReplace.Add(new ReplacerHelper("22437f", "ffab7f")); // Brown 8
            toReplace.Add(new ReplacerHelper("214d9e", "ffab7f")); // Brown 9
            toReplace.Add(new ReplacerHelper("90b0e8", "ffab7f")); // Brown 10

            toReplace.Add(new ReplacerHelper("ff3050", "ffab7f")); // Blue 1
            toReplace.Add(new ReplacerHelper("c02040", "ffab7f")); // Blue 2
            toReplace.Add(new ReplacerHelper("90203b", "ffab7f")); // Blue 3

            // toReplace.Add(new ReplacerHelper("ffe3e3", "a25e5e")); // Blue 4
            // toReplace.Add(new ReplacerHelper("ffdbdb", "855050")); // Blue 5
            // toReplace.Add(new ReplacerHelper("ffd3d3", "855050")); // Blue 6
            // toReplace.Add(new ReplacerHelper("ffeddb", "b36b6b")); // Blue 7
            // toReplace.Add(new ReplacerHelper("fff1e3", "a16060")); // Blue 8

            toReplace.Add(new ReplacerHelper("ffcbcb", "ec5e5e")); // Blue corner 1
            toReplace.Add(new ReplacerHelper("ffc4c4", "ff8a8a")); // Blue corner 2
            toReplace.Add(new ReplacerHelper("ffbfbf", "ff8080")); // Blue corner 3

            toReplace.Add(new ReplacerHelper("548cd7", "a1a1a1")); // Canvas select border 1
            toReplace.Add(new ReplacerHelper("6e9ee0", "a1a1a1")); // Canvas select border 2
            toReplace.Add(new ReplacerHelper("b6cced", "a1a1a1")); // Canvas select border 3
            toReplace.Add(new ReplacerHelper("d9e4f8", "a1a1a1")); // Canvas select border 4

            // all colors above have values in config.txt now


            // start color wheel code ---------------------------------------------------------

            // TODO: this code appears to try to fix the aliasing that occurs when drawing circles. the colors are close but probably
            // should be computed using the background color.

            // for (int i = 162; i <= 254; i++) {
            //    toReplace.Add(new ReplacerHelper("" + i.ToString("X2") + i.ToString("X2") + i.ToString("X2"), "212121")); // Color wheel fix
            // }

            // toReplace.Add(new ReplacerHelper("a6a6a6", "212121")); // Color wheel fix 1
            // toReplace.Add(new ReplacerHelper("707070", "212121")); // Color wheel fix 2
            // toReplace.Add(new ReplacerHelper("9f9f9f", "212121")); // Color wheel fix 3
            // toReplace.Add(new ReplacerHelper("a6a6a6", "212121")); // Color wheel fix 4

            /*            for (int i = 1; i <= 8; i++) {
                            for (int j = 1; j <= 8; j++) {
                                for (int k = 1; k <= 8; k++) {
                                    if (i != j || i != k) {
                                        toReplace.Add(new ReplacerHelper("f" + i + "f" + j + "f" + k, "212121")); // Color wheel
                                    }
                                }
                            }
                        }*/

            // end color wheel code ---------------------------------------------------------
            string backupFilePath = @$"{inputPath}.bak";

            if (File.Exists(backupFilePath)) {
                if (IsFileLocked(new FileInfo(backupFilePath))) {
                    LogColor("your copy of sai2.exe is in use. please try again after closing this program as well as sai2.", ConsoleColor.Red);
                    Environment.Exit(1);
                }

                File.Delete(inputPath);
                File.Copy(backupFilePath, inputPath);
                File.Delete(backupFilePath);
            }

            LogColor($"making a backup copy of [{inputPath}]...\r\n", ConsoleColor.Green);

            makeCopy(inputPath);

            LogColor($"replacing binary... [{inputPath}] -> [{outputPath}]\r\n", ConsoleColor.Green);

            replaceHex(inputPath, outputPath, toReplace);

            LogColor($"freshly modified file saved to [{outputPath}].\r\n", ConsoleColor.Green);
            LogColor("finished! press [enter] to close this window.", ConsoleColor.Green);
            Console.ReadKey();
        }

        // Fuggin fug fug
        // Cut the hex string -> byte array
        /*        public static byte[] GetByteArray(string str)
                {
                    // https://stackoverflow.com/questions/321370/how-can-i-convert-a-hex-string-to-a-byte-array
                    return Enumerable.Range(0, str.Length)
                                        .Where(x => x % 2 == 0)
                                        .Select(x => Convert.ToByte(str.Substring(x, 2), 16))
                                        .ToArray();
                } */

        public static byte[] GetByteArray(string str) {
            // https://stackoverflow.com/questions/311165/how-do-you-convert-a-byte-array-to-a-hexadecimal-string-and-vice-versa
            int NumberChars = str.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(str.Substring(i, 2), 16);
            return bytes;
        }

        public static void makeCopy(string path) {
            // string targetPath = Path.GetDirectoryName(path) + @"\" + Path.GetFileNameWithoutExtension(path) + Path.GetExtension(path) + ".bak";
            string targetPath = $"{Path.GetDirectoryName(path)}\\{@Path.GetFileName(path)}.bak";

            if (!File.Exists(targetPath)) {
                File.Copy(path, targetPath);
                LogColor($"backup copy generated in [{targetPath}]\r\n", ConsoleColor.Green);
            } else {
                LogColor($"backup copy already exists in [{targetPath}]\r\n", ConsoleColor.Yellow);
            }
        }

        // TODO: this needs to be modified to exactly match certain cases. replacing #000000 and #ffffff might be one
        public static bool findHex(byte[] sequence, int position, byte[] seeker) {
            if (position + seeker.Length > sequence.Length) return false;

            for (int i = 0; i < seeker.Length; i++) {
                if (seeker[i] != sequence[position + i]) return false;
            }

            return true;
        }

        public static void replaceHex(string targetFile, string resultFile, string searchString, string replacementString) {

            var targetDirectory = Path.GetDirectoryName(resultFile);
            if (targetDirectory == null) return;
            Directory.CreateDirectory(targetDirectory);

            byte[] fileContent = File.ReadAllBytes(targetFile);

            byte[] seeker = GetByteArray(searchString);
            byte[] hider = GetByteArray(replacementString);

            for (int i = 0; i < fileContent.Length; i++) {
                if (!findHex(fileContent, i, seeker)) continue;
                for (int j = 0; j < seeker.Length; j++) {
                    fileContent[i + j] = hider[j];
                }
            }

            File.WriteAllBytes(resultFile, fileContent);
        }

        public static void replaceHex(string targetFile, string resultFile, List<ReplacerHelper> toReplace) {

            var targetDirectory = Path.GetDirectoryName(resultFile);
            if (targetDirectory == null) return;
            Directory.CreateDirectory(targetDirectory);

            byte[] fileContent = File.ReadAllBytes(targetFile);

            foreach (ReplacerHelper replacerHelper in toReplace) {
                byte[] seeker = GetByteArray(replacerHelper.Search);
                byte[] hider = GetByteArray(replacerHelper.Replace);

                bool ringFlag = false;

                for (int r = 162; r <= 254; r++) {
                    if (replacerHelper.Search == ("" + r.ToString("X2") + r.ToString("X2") + r.ToString("X2"))) {
                        ringFlag = true;
                    }
                }

                for (int i = 1; i <= 8; i++) {
                    for (int j = 1; j <= 8; j++) {
                        for (int k = 1; k <= 8; k++) {
                            if (i != j || i != k) {
                                if (replacerHelper.Search == ("f" + i + "f" + j + "f" + k)) {
                                    ringFlag = true;
                                }
                            }
                        }
                    }
                }

                if (ringFlag == false) {
                    for (int i = 0; i < fileContent.Length; i++) {
                        if (!findHex(fileContent, i, seeker)) continue;

                        for (int j = 0; j < seeker.Length; j++) {
                            fileContent[i + j] = hider[j];
                        }
                    }
                } else {
                    for (int i = 3160329; i < 3243232; i++) {
                        if (!findHex(fileContent, i, seeker)) continue;

                        for (int j = 0; j < seeker.Length; j++) {
                            fileContent[i + j] = hider[j];
                        }
                    }
                }
            }

            try {
                File.WriteAllBytes(resultFile, fileContent);
            } catch (Exception e) {
                Log($"writing to file failed. {e}", ConsoleColor.White, ConsoleColor.Red);
            }
        }

    }
}