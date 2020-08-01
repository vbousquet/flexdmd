# FlexDMD API

The following section contains preliminary documentation.

## Introduction

FlexDMD API is inspired from [LibGDX Scene2D API](https://github.com/libgdx/libgdx/wiki/Scene2d) since it is very simple and easy to use.
You have a Stage objet (the DMD), on which you add Actor objects (Image, Video, Label,...) that you can animate with Action objects (fade, move,...).

FlexDMD API is divided into the following components:
* a few properties:
  - ```bool Run { get; set; }```
  - ```bool Show { get; set; }```
  - ```string GameName { get; set; }```
  - ```ushort Width { get; set; }```
  - ```ushort Height { get; set; }```
  - ```Color Color { get; set; }```
  - ```RenderMode RenderMode { get; set; }```
  - ```string ProjectFolder { get; set; }```
  - ```string TableFile { get; set; }```
  - ```bool Clear { get; set; }```
  - ```object DmdColoredPixels { get; }```
  - ```object DmdPixels { get; }```
  - ```IGroupActor Stage { get; }```
* methods to manage threading:
  - ```void LockRenderThread();```
  - ```void UnlockRenderThread();```
* a simple factory pattern to create all the rendering objects:
  - ```IGroupActor NewGroup(string name);```
  - ```IFrameActor NewFrame(string name);```
  - ```ILabelActor NewLabel(string name, Font font, string text);```
  - ```IVideoActor NewVideo(string name, string video);```
  - ```IImageActor NewImage(string name, string image);```
* a few specific objects for fonts and the UltraDMD compatibility layer:
  - ```Font NewFont(string font, Color tint, Color borderTint, int borderSize);```
  - ```IUltraDMD NewUltraDMD();```

The API is directly available in the source [here](../FlexDMD/IFlexDMD.cs)

## Configuring the rendering

Rendering is configured using FlexDMD's properties:
- ```GameName``` is shared with DmdDevice (always define it, since it will allow to save position of DMD per table)
- ```Width``` and ```Height``` define the size (usually 128x32 for DMD rendered on the physical DMD, but can be anything else for DMD included directly on the table's playfield)
- ```RenderMode``` selects between monochrome with 4 shades (value = 0) / monochrome with 16 shades (value = 1) / full color (value = 2)
- ```Color``` is the color used for text rendering, and in monochrome DMD render mode

When defining color, you should use ```FlexDMD.Color = RGB(255,0,0)``` to dfine the color (first component is red component, then green and blue).

## Threading

All rendering happens in a separate thread to avoid stutterings. Therefore, when you modify the stage, you have to lock it. To do so, enclose your modifications between a pair of ```LockRenderThread``` /  ```UnlockRenderThread```.

## Using actors

All actors offer a base set of properties and methods:
- ```string Name { get; set; }```
- ```float X { get; set; }```
- ```float Y { get; set; }```
- ```float Width { get; set; }```
- ```float Height { get; set; }```
- ```bool Visible { get; set; }```
- ```void SetBounds(float x, float y, float width, float height);```
- ```void SetPosition(float x, float y);```
- ```void SetAlignedPosition(float x, float y, Alignment alignment);```
- ```void SetSize(float width, float height);```
- ```void Remove();```
- ```IActionFactory ActionFactory { get; }```
- ```void AddAction(Action action);```
- ```void ClearActions();```

The action factory allows to create action objects for the different animation features. Then, these objects need to be added to their target using the ```addAction```. Action can be combined into sequences using the composite actions.


## UltraDMD API

FlexDMD allows to use the [UltraDMD API](https://ultradmd.wordpress.com/) which is a simple and easy to use scene oriented API, thanks to the work from Stephen Rakonza. To do so, you just need to create it through ```NewUltraDMD```. Render thread is handled silently by this object. An additional method is provided to load UltraDMD setup from the windows' registry: ```LoadSetup```.
