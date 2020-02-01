# FlexDMD
FlexDMD is a DMD renderer extension for [Visual Pinball](https://sourceforge.net/projects/vpinball) and Pinball front-ends like [PinballY](https://github.com/mjrgh/PinballY). It can also be used as a drop-in replacement for [UltraDMD](https://ultradmd.wordpress.com/), solving some of its rough corners.

<b>Disclaimer: this software is in a pre-alpha state. Use at your own risk !
  The API and docs are not stabilized yet, and are likely to change.</b>

Documentation :
* [Features](#features)
* [Installation](#installation)
* [Configuration](#configuration)
* [Build Instructions](#build-instructions)
* [Architecture](#architecture)
* [Embedding the DMD inside VPX](./VPXDMD.md)
* [Adding a DMD to latest tables from JPSalas](./JPSalas.md)
* [Porting an UltraDMD to FlexDMD](./UltraDMD.md)

## Features
The main features are summarized below:
* Flexible rendering library, allowing to create any style of DMD animation, including so called "Video Mode" (like saucer attack in Attack From Mars)
* Rely on dmddevice.dll for rendering (see [Freezy's DMD extensions](https://github.com/freezy/dmd-extensions)), therefore supporting output to real DMD devices, virtual DMD, network,...
* Allows to render inside VPX embedded DMD renderer, therefore allowing desktop DMD integration, with support for clean exclusive fullscreen mode (see [docs](./VPXDMD.md)).
* In process server (as opposed to out of process), to avoid spilling orphan processes everywhere, with the same object lifecycle management as B2SServer to guarantee that closing the table will close the DMD.
* Drop-in replacement for UltraDMD (strictly compatible API with nearly identical rendering).

## Installation
[Download the latest release](https://github.com/vbousquet/flexdmd/releases). It comes with the following files ;
- FlexDMDUI.exe, an application to install and test it,
- FlexDMD.dll, the FlexDMD library itself,
- FlexUDMD.dll, the UltraDMD replacement.

Beside these files, you will need ```DMDDevice.dll``` for 32 bits rendering, and ```DMDDevice64.dll``` for 64 bits rendering. There can be different flavors of these. The ones used for developping FlexDMD are [Freezy's DMD Extensions](https://github.com/freezy/dmd-extensions), where you can find the needed files in the "Release" section. Just place them alongside the FlexDMD files.

You can place these 5 files where you want. It happens that [VPinMame](https://sourceforge.net/projects/pinmame/) also uses DMDDevice.dll and requires it to be placed alongside itself. Therefore a good place to place FlexDMD and DMDDevice files would be in your VPinMame folder (usually C:\Visual Pinball\VPinMAME).

The FlexDMDUI.exe application allows you to install the different parts and test them. So just launch it, check it finds all the needed files (first panel), then simply press ```register``` (second panel) for the FlexDMD extension, and, if you want it, just press ```register``` (third panel) for the UltraDMD replacement.

To check that everything is fine, switch from the 'Installer' tab to the 'Designer' tab, select the output you want to test between FlexDMD, UltraDMD or both, and press 'Run'.

Finally, you should head to the [Scripts folder](./Scripts/) where you will find a few prepared scripts for some popular tables. Just download them and place them in your 'table' directory alongside the .vpx table file and VPX 10.6+ will detect and use them.

## Configuration
There is no configuration file:
* The DMD is entirely defined in the table script,
* The output is configured using the DmdDevice configuration file.

You may want to change the render mode of a table from monochrome to full color, or set the color of the DMD of one table. To do so, you will need to edit the table's script, and, after creating the FlexDMD object, change its RenderMode or Color property.

RenderMode can have 3 values : GRAY_2 (0), GRAY_4 (1), RGB (2). The default is GRAY_4, that is to say it renders to a monochrome DMD with 16 shades of colors. To use full color, you will add `FlexDMD.RenderMode = 2`

Color is used to set the color of monochroms DMD. By default, FlexDMD uses a classic red/orange color. To use something else, just add `FlexDMD.Color = &hFFFF00` where `&hFFFF00` stands for a BGR color expressed as an hexadecimal value. That is to say, that each of the Blue, Green, Red colors are expressed as 2 letters, from 00 to FF where 0 is the minimum and FF correspond to 255 and is the maximum for this component. Here are a few examples to better understand;
```vbscript
FlexDMD.Color = &hFF0000 ' Blue
FlexDMD.Color = &h00FF00 ' Green
FlexDMD.Color = &h0000FF ' Red
FlexDMD.Color = &hFFFFFF ' White
FlexDMD.Color = &h2058FF ' Default reddish color
```

You can find example scripts in the Scripts folder (America's Most Haunted shows how to set a custom DMD color, Diablo shows how to render in full color).

## Build Instructions
1. Download and install [Visual Studio 2019](https://visualstudio.microsoft.com/fr/downloads/)
2. Clone the repo: `git clone https://github.com/vbousquet/flexdmd.git`
3. Open the .sln file in Visual Studio and build the solution.

## Architecture
The following diagram shows the overall architecture of Visual Pinball and where FlexDMD sits.
<br></br>![Overall Architecture](./media/architecture.svg)
