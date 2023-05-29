using SaiThemeUtils;
using SaiThemeColorReplacement;
using System;
using System.Collections.Generic;
using System.IO;

namespace SaiThemeColorChanger {
    // Hex strat derived from https://social.msdn.microsoft.com/Forums/vstudio/en-US/a0b2133f-ae23-4c0b-b136-dd531952f3c7/find-amp-replace-hex-values-in-a-dll-file-using-c?forum=csharpgeneral

    class Program {
        static void Main(string[] args) {
            string inputPath = "";

            Dictionary<string, string> config = FileUtils.ReadConfig("config.txt");

            if (args.Length > 0)
                inputPath = args[0];

            if (inputPath.Length == 0) {
                Logger.LogColor("please drag the [sai2.exe] file into this window and press enter.\r\n", ConsoleColor.Green);
                inputPath = Console.ReadLine();
                // inputPath = @StringUtils.AddQuotesIfRequired(Console.ReadLine());

                Console.WriteLine("\r\n");

                while (!File.Exists(inputPath)) {
                    Logger.LogColor($"[{inputPath}] is not a valid path.", ConsoleColor.Red);
                    inputPath = Console.ReadLine();
                }
            }

            if (!File.Exists(inputPath)) {
                Logger.LogColor($"[{inputPath}] is not a valid path.", ConsoleColor.Red);
                Console.ReadKey();
                return;
            }

            if (FileUtils.IsFileLocked(new FileInfo(inputPath))) {
                Logger.LogColor("[your copy of sai2.exe is in use.] please try again after closing this program as well as sai2.", ConsoleColor.Red);
                Environment.Exit(1);
            }

            // the output path has to be the same as the original file. ths is because the program will look for a .ini file
            // with the same filename as the executable. for example, if you call sai "sai21.exe" instead of "sai2.exe", sai
            // will complain that it's missing a "sai21.ini" file. you could just rename the .ini if you really wanted to.
            string outputPath = inputPath;

            List<ReplacerHelper> colorsToReplace = InterfaceColors.ReplaceColors(config);

            //colorsToReplace.Add(new ReplacerHelper("f0f0f0", "212121")); // Tools background
            //colorsToReplace.Add(new ReplacerHelper("e0e0e0", "313131")); // Tools panel background

            //colorsToReplace.Add(new ReplacerHelper("b1b1b1", "313131")); // Panel borders 1
            //colorsToReplace.Add(new ReplacerHelper("d0d0d0", "313131")); // Panel borders 2
            //colorsToReplace.Add(new ReplacerHelper("d8d8d8", "313131")); // Panel borders 3
            //colorsToReplace.Add(new ReplacerHelper("dadada", "313131")); // Panel borders 4
            //colorsToReplace.Add(new ReplacerHelper("e4e4e4", "313131")); // Panel borders 5
            //colorsToReplace.Add(new ReplacerHelper("f4f4f4", "313131")); // Panel borders 6

            //colorsToReplace.Add(new ReplacerHelper("c6c6c6", "707070")); // Panel separator button 1

            //colorsToReplace.Add(new ReplacerHelper("cecece", "111111")); // Corners 1
            //colorsToReplace.Add(new ReplacerHelper("c9c9c9", "111111")); // Corners 2
            //colorsToReplace.Add(new ReplacerHelper("eeeeee", "2d2d2d")); // Corners 3
            //colorsToReplace.Add(new ReplacerHelper("dedede", "313131")); // Corners 4
            //colorsToReplace.Add(new ReplacerHelper("b4b4b4", "313131")); // Corners 5
            //colorsToReplace.Add(new ReplacerHelper("8c8c8c", "212121")); // Corners 6
            //colorsToReplace.Add(new ReplacerHelper("7d7d7d", "212121")); // Corners 7

            //colorsToReplace.Add(new ReplacerHelper("204080", "a1a1a1")); // Brown 1
            //colorsToReplace.Add(new ReplacerHelper("204172", "a1a1a1")); // Brown 2

            //colorsToReplace.Add(new ReplacerHelper("1c4da8", "ffab7f")); // Brown 3
            //colorsToReplace.Add(new ReplacerHelper("234a93", "ffab7f")); // Brown 4
            //colorsToReplace.Add(new ReplacerHelper("254177", "ffab7f")); // Brown 5
            //colorsToReplace.Add(new ReplacerHelper("1f2e49", "ffab7f")); // Brown 6
            //colorsToReplace.Add(new ReplacerHelper("436616", "ffab7f")); // Brown 7
            //colorsToReplace.Add(new ReplacerHelper("22437f", "ffab7f")); // Brown 8
            //colorsToReplace.Add(new ReplacerHelper("214d9e", "ffab7f")); // Brown 9
            //colorsToReplace.Add(new ReplacerHelper("90b0e8", "ffab7f")); // Brown 10

            //colorsToReplace.Add(new ReplacerHelper("ff3050", "ffab7f")); // Blue 1
            //colorsToReplace.Add(new ReplacerHelper("c02040", "ffab7f")); // Blue 2
            //colorsToReplace.Add(new ReplacerHelper("90203b", "ffab7f")); // Blue 3

            // colorsToReplace.Add(new ReplacerHelper("ffe3e3", "a25e5e")); // Blue 4
            // colorsToReplace.Add(new ReplacerHelper("ffdbdb", "855050")); // Blue 5
            // colorsToReplace.Add(new ReplacerHelper("ffd3d3", "855050")); // Blue 6
            // colorsToReplace.Add(new ReplacerHelper("ffeddb", "b36b6b")); // Blue 7
            // colorsToReplace.Add(new ReplacerHelper("fff1e3", "a16060")); // Blue 8

            //colorsToReplace.Add(new ReplacerHelper("ffcbcb", "ec5e5e")); // Blue corner 1
            //colorsToReplace.Add(new ReplacerHelper("ffc4c4", "ff8a8a")); // Blue corner 2
            //colorsToReplace.Add(new ReplacerHelper("ffbfbf", "ff8080")); // Blue corner 3

            //colorsToReplace.Add(new ReplacerHelper("548cd7", "a1a1a1")); // Canvas select border 1
            //colorsToReplace.Add(new ReplacerHelper("6e9ee0", "a1a1a1")); // Canvas select border 2
            //colorsToReplace.Add(new ReplacerHelper("b6cced", "a1a1a1")); // Canvas select border 3
            //colorsToReplace.Add(new ReplacerHelper("d9e4f8", "a1a1a1")); // Canvas select border 4

            // all colors above have values in config.txt now


            // start color wheel code ---------------------------------------------------------

            // ring: f1f1f1, a4a4a4, f6f6f6, bebebe, c1c1c1, eaeaea
            // intended ring color (?): acacac, a6a6a6, b2b2b2, b6b6b6, cecece, d3d3d3

            // TODO: this code appears to try to fix the aliasing that occurs when drawing circles. the colors are close but probably
            // should be computed using the background color.

            // for (int i = 162; i <= 254; i++) {
            //    colorsToReplace.Add(new ReplacerHelper("" + i.ToString("X2") + i.ToString("X2") + i.ToString("X2"), "212121")); // Color wheel fix
            // }

            // colorsToReplace.Add(new ReplacerHelper("a6a6a6", "212121")); // Color wheel fix 1
            // colorsToReplace.Add(new ReplacerHelper("707070", "212121")); // Color wheel fix 2
            // colorsToReplace.Add(new ReplacerHelper("9f9f9f", "212121")); // Color wheel fix 3
            // colorsToReplace.Add(new ReplacerHelper("a6a6a6", "212121")); // Color wheel fix 4

            /*            for (int i = 1; i <= 8; i++) {
                            for (int j = 1; j <= 8; j++) {
                                for (int k = 1; k <= 8; k++) {
                                    if (i != j || i != k) {
                                        colorsToReplace.Add(new ReplacerHelper("f" + i + "f" + j + "f" + k, "212121")); // Color wheel
                                    }
                                }
                            }
                        }*/

            // end color wheel code ---------------------------------------------------------
            string backupFilePath = @$"{inputPath}.bak";

            if (File.Exists(backupFilePath)) {
                if (FileUtils.IsFileLocked(new FileInfo(backupFilePath))) {
                    Logger.LogColor("your copy of sai2.exe is in use. please try again after closing this program as well as sai2.", ConsoleColor.Red);
                    Environment.Exit(1);
                }

                File.Delete(inputPath);
                File.Copy(backupFilePath, inputPath);
                File.Delete(backupFilePath);
            }

            Logger.LogColor($"making a backup copy of [{inputPath}]...\r\n", ConsoleColor.Green);

            FileUtils.MakeBackup(inputPath);

            Logger.LogColor($"replacing binary... [{inputPath}] -> [{outputPath}]\r\n", ConsoleColor.Green);

            ReplaceHex(inputPath, outputPath, colorsToReplace);

            Logger.LogColor($"freshly modified file saved to [{outputPath}].\r\n", ConsoleColor.Green);
            Logger.LogColor("finished! press [enter] to close this window.", ConsoleColor.Green);
            Console.ReadKey();
        }

        public static byte[] GetByteArray(string str) {
            // https://stackoverflow.com/questions/311165/how-do-you-convert-a-byte-array-to-a-hexadecimal-string-and-vice-versa
            int NumberChars = str.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(str.Substring(i, 2), 16);
            return bytes;
        }

        // TODO: this needs to be modified to exactly match certain cases. replacing #000000 and #ffffff might be one
        public static bool FindHex(byte[] sequence, int position, byte[] seeker) {
            if (position + seeker.Length > sequence.Length) return false;

            for (int i = 0; i < seeker.Length; i++) {
                if (seeker[i] != sequence[position + i]) return false;
            }

            return true;
        }

        // public static void ReplaceHex(string targetFile, string resultFile, string searchString, string replacementString) {

        //     var targetDirectory = Path.GetDirectoryName(resultFile);
        //     if (targetDirectory == null) return;
        //     Directory.CreateDirectory(targetDirectory);

        //     byte[] fileContent = File.ReadAllBytes(targetFile);

        //     byte[] seeker = GetByteArray(searchString);
        //     byte[] hider = GetByteArray(replacementString);

        //     for (int i = 0; i < fileContent.Length; i++) {
        //         if (!FindHex(fileContent, i, seeker)) continue;
        //         for (int j = 0; j < seeker.Length; j++) {
        //             fileContent[i + j] = hider[j];
        //         }
        //     }

        //     File.WriteAllBytes(resultFile, fileContent);
        // }

        public static void ReplaceHex(string targetFile, string resultFile, List<ReplacerHelper> colorsToReplace) {
            var targetDirectory = Path.GetDirectoryName(resultFile);
            if (targetDirectory == null) return;
            Directory.CreateDirectory(targetDirectory);

            byte[] fileContent = File.ReadAllBytes(targetFile);

            foreach (ReplacerHelper replacerHelper in colorsToReplace) {
                byte[] seeker = GetByteArray(replacerHelper.Search);
                byte[] hider = GetByteArray(replacerHelper.Replace);

                Logger.LogColor($"\r\n[seeker] {replacerHelper.Search}", ConsoleColor.Yellow);
                Logger.PrintByteArray(seeker);
                Logger.LogColor($"[hider] {replacerHelper.Replace}", ConsoleColor.Yellow);
                Logger.PrintByteArray(hider);
                Console.WriteLine(" ");

                bool ringFlag = false;

                //for (int r = 162; r <= 254; r++) {
                //    if (replacerHelper.Search == ("" + r.ToString("X2") + r.ToString("X2") + r.ToString("X2"))) {
                //        ringFlag = true;
                //    }
                //}

                //for (int i = 1; i <= 8; i++) {
                //    for (int j = 1; j <= 8; j++) {
                //        for (int k = 1; k <= 8; k++) {
                //            if (i != j || i != k) {
                //                if (replacerHelper.Search == ("f" + i + "f" + j + "f" + k)) {
                //                    ringFlag = true;
                //                }
                //            }
                //        }
                //    }
                //}

                if (ringFlag == false) {
                    for (int i = 0; i < fileContent.Length; i++) {
                        if (!FindHex(fileContent, i, seeker)) continue;

                        for (int j = 0; j < seeker.Length; j++) {
                            fileContent[i + j] = hider[j];
                        }
                    }
                } else {
                    for (int i = 3160329; i < 3243232; i++) {
                        if (!FindHex(fileContent, i, seeker)) continue;

                        for (int j = 0; j < seeker.Length; j++) {
                            fileContent[i + j] = hider[j];
                        }
                    }
                }
            }

            try {
                File.WriteAllBytes(resultFile, fileContent);
            } catch (Exception e) {
                Logger.Log($"writing to file failed. {e}", ConsoleColor.White, ConsoleColor.Red);
            }
        }
    }
}