using SaiThemeUtils;
using System.Collections.Generic;

// Hex color code -> replacement (won't work with pure white and pure black, but everything else seems fine!)
// Basically this replaces left hex with the right hex.
// You can swap out the values to get other colors, I haven't noticed any issues using a version with these values modified
namespace SaiThemeColorReplacement {
    public class InterfaceColors {
        public static List<ReplacerHelper> ReplaceColors(Dictionary<string, string> config) {
            return new List<ReplacerHelper> {
                // main interface
                new ReplacerHelper("f8f8f8", config.GetValueOrDefault("MainPanelColor", "212121")),
                new ReplacerHelper("c0c0c0", config.GetValueOrDefault("InactiveCanvasBackground", "111111")), // also affects slider bars
                new ReplacerHelper("b0b0b0", config.GetValueOrDefault("ActiveCanvasBackground", "111111")),

                // scrollbars
                new ReplacerHelper("e8e8e8", config.GetValueOrDefault("ScrollbarBackground", "3a3a3a")),
                new ReplacerHelper("969696", config.GetValueOrDefault("ScrollbarThumb", "2a2a2a")),

                // buttons
                new ReplacerHelper("d4d4d4", config.GetValueOrDefault("InactiveButton", "212121")),
                new ReplacerHelper("dca280", "ff0000")
            };
        }
    }
}