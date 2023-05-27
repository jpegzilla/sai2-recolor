# sai2 recolor tool

recolors sai2.exe based on a text file containing the desired colors.

text file should look like this (work in progress):

```
PANELCOLOR=efefef
CANVASBG=efefef
SCROLLBARBG=efefef
SCROLLBARFG=efefef
TOOLSBG=efefef
INACTIVEBTN=efefef
PANELBORDER=efefef
ACTIVECANVASBG=efefef
TOOLSPANELBG=efefef

...
```

I'll include a default configuration file `theme.txt` which will provide a simple dark mode. however, you can edit the file to contain literally whatever color (6-character hex code, no alpha channel) you want for each category. then, follow the steps below from [the original creator](https://github.com/NotBoogie/SaiThemeColorChanger).

disclaimer: I don't know a damn thing about c#.

## from original description:

drag the sai2.exe file into the executable to change the ui colors.  be sure to run it from somewhere with the appropriate permissions. if you want to make your own custom colors, just edit the hex list in the source code.

also make sure you back up your sai folder just to be safe lol. I haven't noticed any issues using a modified version for a couple months now, but still.

## note from [busteanHAN's fork](https://github.com/BusteanHAN/SaiThemeColorChanger):

fork: abused pixel hex value inspection to find even more areas to be changed into a proper darkmode, not just pale gray

text blacks and button whites are #000000 and #ffffff respectively, which cannot be changed in the binary without completely breaking the program. address filtering could be applied to filter which #0s and #fs are changed, but the locations themselves are discovered manually through brute force. that takes time; I do not have that much of it.