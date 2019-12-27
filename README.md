# FlexDMD
FlexDMD is a DMD renderer extension for Visual Pinball. 
FlexDMD was created as a drop-in replacement for UltraDMD, since it was not updated anymore. 

For install instructions and detailed informations, look in the [documentation](./docs/FlexDMD.md).

<b>Disclaimer: this software is in a pre-alpha state. Use at your own risk !</b>

## Overall design
The main features are summarized below:
* Drop-in replacement for UltraDMD (strictly compatible API).
* Rely on dmddevice.dll for rendering (see [Freezy's DMD extensions](https://github.com/freezy/dmd-extensions)), therefore supporting output to real DMD devices, virtual DMD, network,...
* Allows to render inside VPX embedded DMD renderer, therefore allowing desktop DMD integration, with support for clean exclusive fullscreen mode (see [docs](./docs/VPXDMD.md)).
* In process server (as opposed to out of process), to avoid spilling orphan processes everywhere, with the same object lifecycle management as B2SServer to guarantee that closing the table will close the DMD.
* Offers a few improvements over UltraDMD (variable font width, comma separated scores, animated score background,...).

## Overall Architecture
The following diagram shows the overall architecture of Visual Pinball and where FlexDMD sits.

<br></br>![Overall Architecture](./docs/media/architecture.svg)
