# FlexDMD
FlexDMD is a DMD renderer extension for [Visual Pinball](https://sourceforge.net/projects/vpinball) and Pinball front-ends like [PinballY](https://github.com/mjrgh/PinballY). It can also be used as a drop-in replacement for [UltraDMD](https://ultradmd.wordpress.com/), solving some of its rough corners.

Documentation :
* [Features](#features)
* [Installation](#installation)
* [Configuration](#configuration)
* [Build Instructions](#build-instructions)
* [Architecture](#architecture)
* [Embedding the DMD inside VPX](./VPXDMD.md)
* [Adding a DMD to latest tables from JPSalas](./JPSalas.md)
* [Using FlexDMD with UltraDMD tables](./UltraDMD.md)

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

Finally, you should head to the [Scripts folder](./Scripts/) where you will find a few prepared scripts;
* to add DMD for some popular tables: download and place them in your 'table' directory alongside the .vpx table file and VPX 10.6+ will detect and use them,
* for PinballY front-end: download and place in the PinballY's "Scripts" folder,
* to show some of the features offered by FlexDMD for more advanced users.

## Configuration
FlexDMD does not have any configuration file:
* The DMD is entirely defined in the table script,
* The output is configured using the DmdDevice configuration file.

For UltraDMD tables:
* to position the DMD, if you are using Freezy's DMD, simply run the table, right click on the DMD and select the option to save its position,
* to select wether to run in full color or in monochrome, simply run FlexDMD's companion application (FlexDMDUI.exe) and use the UltraDMD configuration tab,
* to select the text and monochrome color, simply run FlexDMD's companion application (FlexDMDUI.exe) and use the UltraDMD configuration tab.

## Build Instructions
1. Download and install [Visual Studio 2019](https://visualstudio.microsoft.com/fr/downloads/)
2. Clone the repo: `git clone https://github.com/vbousquet/flexdmd.git`
3. Open the .sln file in Visual Studio and build the solution.

## Architecture
The following diagram shows the overall architecture of Visual Pinball and where FlexDMD sits.
<br></br>![Overall Architecture](./media/architecture.svg)
