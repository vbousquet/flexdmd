# Installation

Releases of FlexDMD are available [here](https://github.com/vbousquet/flexdmd/releases/latest/), and come with the following files ;
- FlexDMDUI.exe, an application to install and test it,
- FlexDMD.dll, the FlexDMD library itself,
- FlexUDMD.dll, the UltraDMD replacement.

Beside these files, you will need ```DMDDevice.dll``` for 32 bits rendering, and ```DMDDevice64.dll``` for 64 bits rendering. There can be different flavors of these. The ones used for developping FlexDMD are [Freezy's DMD Extensions](https://github.com/freezy/dmd-extensions), where you can find the needed files in the "Release" section. Just place them alongside the FlexDMD files.

You can place these 5 files where you want. It happens that [VPinMame](https://sourceforge.net/projects/pinmame/) also uses DMDDevice.dll and requires it to be placed alongside itself. Therefore a good place to place FlexDMD and DMDDevice files would be in your VPinMame folder (usually C:\Visual Pinball\VPinMAME).

The FlexDMDUI.exe application allows you to install the different parts and test them. So just launch it, check it finds all the needed files (first panel), then simply press ```register``` (second panel) for the FlexDMD extension, and, if you want it, just press ```register``` (third panel) for the UltraDMD replacement.

Finally, switch from the 'Installer' tab to the 'Designer' tab, select the output you want to test between FlexDMD, UltraDMD or both, and press 'Run'.
