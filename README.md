# FlexDMD
FlexDMD is a DMD renderer extension for [Visual Pinball](https://sourceforge.net/projects/vpinball) and Pinball front-ends like [PinballY](https://github.com/mjrgh/PinballY).

It can also be used as a drop-in replacement for UltraDMD, solving some of its rough corners.

For install instructions and detailed informations, look in the [documentation](./docs/FlexDMD.md).

<b>Disclaimer: this software is in a pre-alpha state. Use at your own risk !
  The API and docs are not stabilized yet, and are likely to change.</b>

## Overall design
The main features are summarized below:
* Flexible rendering library, allowing to create any style of DMD animation, including so called "Video Mode"
* Rely on dmddevice.dll for rendering (see [Freezy's DMD extensions](https://github.com/freezy/dmd-extensions)), therefore supporting output to real DMD devices, virtual DMD, network,...
* Allows to render inside VPX embedded DMD renderer, therefore allowing desktop DMD integration, with support for clean exclusive fullscreen mode (see [docs](./docs/VPXDMD.md)).
* In process server (as opposed to out of process), to avoid spilling orphan processes everywhere, with the same object lifecycle management as B2SServer to guarantee that closing the table will close the DMD.
* Drop-in replacement for UltraDMD (strictly compatible API with nearly identical rendering).

## Overall Architecture
The following diagram shows the overall architecture of Visual Pinball and where FlexDMD sits.

<br></br>![Overall Architecture](./docs/media/architecture.svg)
