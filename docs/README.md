# FlexDMD
FlexDMD is a DMD renderer extension for [Visual Pinball](https://github.com/vpinball/vpinball) and Pinball front-ends like [PinballY](https://github.com/mjrgh/PinballY). It can also be used as a drop-in replacement for the great [UltraDMD](https://ultradmd.wordpress.com/) from Stephen Rakonza, solving some of its rough corners since its development seems to have stopped.

![Example Video](https://github.com/vbousquet/flexdmd/blob/master/docs/Demo.gif)

Documentation :
* [Features](#features)
* [Installation](#installation)
* [Configuration](#configuration)
* [Build Instructions](#build-instructions)
* [Architecture](#architecture)
* [FlexDMD API](./FlexDMD_API.md)
* [Using FlexDMD with UltraDMD tables](./UltraDMD.md)
* [Embedding the DMD inside VPX](./VPXDMD.md)
* [Adding a DMD to latest tables from JPSalas](./JPSalas.md)

## Features
The main features are summarized below:
* Flexible rendering library, allowing to create any style of DMD animation, including so called "Video Mode" (like saucer attack in Attack From Mars)
* Rely on dmddevice.dll for rendering (see [Freezy's DMD extensions](https://github.com/freezy/dmd-extensions)), therefore supporting output to real DMD devices, virtual DMD, network,...
* Allows to render inside VPX embedded DMD renderer, therefore allowing desktop DMD integration, with support for clean exclusive fullscreen mode (see [docs](./VPXDMD.md)).
* In process server (as opposed to out of process), to avoid spilling orphan processes everywhere, with the same object lifecycle management as B2SServer to guarantee that closing the table will close the DMD.
* Drop-in replacement for UltraDMD (strictly compatible API with nearly identical rendering).

## Installation

Download the latest release or snapshot build: 
| Release | Snapshot |
| ------- | -------- |
| [GitHub Releases](https://github.com/vbousquet/flexdmd/releases) | [GitHub Actions](https://github.com/vbousquet/flexdmd/actions?query=workflow%3A%22CI%22) |

It comes with the following files;
- FlexDMDUI.exe, an application to install and test it,
- FlexDMD.dll, the FlexDMD library itself,
- FlexUDMD.dll, the UltraDMD replacement,
- FlexDMD.log.config, a file that instruct FlexDMD to produce a log file.

Beside these files, you will need ```DmdDevice.dll``` for 32 bits rendering, and ```DmdDevice64.dll``` for 64 bits rendering. There can be different flavors of these. The ones used for developping FlexDMD are [Freezy's DMD Extensions](https://github.com/freezy/dmd-extensions), where you can find the needed files in the [Release](https://github.com/freezy/dmd-extensions/releases) section. Just place them alongside the FlexDMD files (the 64bits version need to be renamed to DmdDevice64.dll).

You can place these 6 files where you want. It happens that [VPinMame](https://sourceforge.net/projects/pinmame/) also uses DMDDevice.dll and requires it to be placed alongside itself. Therefore a good place to place FlexDMD and DMDDevice files would be in your VPinMame folder (usually C:\Visual Pinball\VPinMAME).

The FlexDMDUI.exe application allows you to install the different parts and test them. So just launch it, check it finds all the needed files (first panel), then simply press ```register``` (second panel) for the FlexDMD extension, and, if you want it, just press ```register``` (third panel) for the UltraDMD replacement.

To check that everything is fine, switch from the 'Installer' tab to one of the 'Designer' tab and press 'Run'.

A [VPX table](https://github.com/vbousquet/flexdmd/tree/master/FlexDemo) which demonstrates some of the capabilities of FlexDMD is provided with each release; just open & run it with [Visual Pinball X](https://github.com/vpinball/vpinball) version 10.6+ to see examples of what can be done.

Finally, you will also find a [Scripts folder](https://github.com/vbousquet/flexdmd/tree/master/Scripts/) with a few prepared scripts;
* to add DMD for some popular tables: download and place them in your 'Table' directory alongside the .vpx table file and VPX 10.6+ will detect and use them,
* for PinballY front-end: download and place in the PinballY's "Scripts" folder (there are a few options at the beginning of the script),
* to show some of the features offered by FlexDMD for more advanced users.

## Configuration
FlexDMD does not have any configuration file:
* The DMD is entirely defined in the table script (monochrome/colors, number of dots,...),
* The output is configured using the DmdDevice configuration file (target output, position on screen, size of dots,...).

Therefore to position the DMD, if you are using Freezy's DMD, simply run the table, right click on the DMD and select the option to save its position.

When using FlexDMD as an UltraDMD replacement, you may want to edit UltraDMD options regarding DMD's coloring. This is easily done using the FlexDMD companion application (FlexDMDUI.exe) in the configuration tab.

## Build Instructions
1. Download and install [Visual Studio 2019](https://visualstudio.microsoft.com/fr/downloads/)
2. Clone the repo: `git clone https://github.com/vbousquet/flexdmd.git`
3. Open the .sln file in Visual Studio and build the solution.

## Architecture
The following diagram shows the overall architecture of Visual Pinball and where FlexDMD sits.
<br></br>![Overall Architecture](./media/architecture.svg)

# Installing from the command line
Whilst it is not the preferred way of using it, FlexDMDUI.exe can also be used from the command line with the given commands:
* ```FlexDMDUI.exe /register path``` will unblock the FlexDMD's DLL (FlexDMD.dll) at the given path and register it. If needed, it will also unblock Freezy's DMD at the given path (dmddevice.dll and dmddevice64.dll).
* ```FlexDMDUI.exe /register-udmd path``` will unblock the FlexDMD's UltraDMD bridge DLL (FlexUDMD.dll) at the given path and register it.
* ```FlexDMDUI.exe /unregister``` will unregister FlexDMD.
* ```FlexDMDUI.exe /unregister-udmd``` will unregister FlexDMD's UltraDMD bridge.
