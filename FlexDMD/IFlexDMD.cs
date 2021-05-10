/* Copyright 2019 Vincent Bousquet

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
   */
using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace FlexDMD
{

    #region Interfaces
    [Guid("3ABF2DA1-819B-462E-AC1C-6BF8BF625D36"), ComVisible(true)]
    public enum Interpolation
    {
        Linear,
        ElasticIn, ElasticOut, ElasticInOut,
        QuadIn, QuadOut, QuadInOut,
        CubeIn, CubeOut, CubeInOut,
        QuartIn, QuartOut, QuartInOut,
        QuintIn, QuintOut, QuintInOut,
        SineIn, SineOut, SineInOut,
        BounceIn, BounceOut, BounceInOut,
        CircIn, CircOut, CircInOut,
        ExpoIn, ExpoOut, ExpoInOut,
        BackIn, BackOut, BackInOut
    }

    [Guid("DCA215A5-EF1B-4924-B5EA-BF108398A318"), ComVisible(true)]
    public interface ICompositeAction
    {
        ICompositeAction Add([MarshalAs(UnmanagedType.Struct)]Action action);
    }

    [Guid("7A165BD9-9825-488D-B292-87CAAC46CB3C"), ComVisible(true)]
    public interface ITweenAction
    {
        Interpolation Ease { get; set; }
    }

    [Guid("DE32F29E-F8C8-4E79-AEE1-4725A320B0B6"), ComVisible(true)]
    public interface IActionFactory
    {
        [return: MarshalAs(UnmanagedType.Struct)] Action Wait(float secondsToWait);
        [return: MarshalAs(UnmanagedType.Struct)] Action Delayed(float secondsToWait, [MarshalAs(UnmanagedType.Struct)] Action action);
        ICompositeAction Parallel();
        ICompositeAction Sequence();
        [return: MarshalAs(UnmanagedType.Struct)] Action Repeat([MarshalAs(UnmanagedType.Struct)]Action action, int count);
        [return: MarshalAs(UnmanagedType.Struct)] Action Blink(float secondsShow, float secondsHide, int repeat);
        [return: MarshalAs(UnmanagedType.Struct)] Action Show(bool visible);
        [return: MarshalAs(UnmanagedType.Struct)] Action AddTo(IGroupActor parent);
        [return: MarshalAs(UnmanagedType.Struct)] Action RemoveFromParent();
        [return: MarshalAs(UnmanagedType.Struct)] Action AddChild([MarshalAs(UnmanagedType.Struct)] Actor child);
        [return: MarshalAs(UnmanagedType.Struct)] Action RemoveChild([MarshalAs(UnmanagedType.Struct)] Actor child);
        [return: MarshalAs(UnmanagedType.Struct)] Action Seek(float pos);
        ITweenAction MoveTo(float x, float y, float duration);
    }


    [Guid("6F205A9B-B007-4DCD-A635-51B2C939A796"), ComVisible(true)]
    public interface IActor
    {
        string Name { get; set; }
        float X { get; set; }
        float Y { get; set; }
        float Width { get; set; }
        float Height { get; set; }
        bool Visible { get; set; }
        bool FillParent { get; set; }
        bool ClearBackground { get; set; }
        void SetBounds(float x, float y, float width, float height);
        void SetPosition(float x, float y);
        void SetAlignedPosition(float x, float y, Alignment alignment);
        void SetSize(float width, float height);
        float PrefWidth { get; }
        float PrefHeight { get; }
        void Pack();
        void Remove();
        IActionFactory ActionFactory { get; }
        void AddAction([MarshalAs(UnmanagedType.Struct)]Action action);
        void ClearActions();
    }

    [Guid("1BF9F8AE-1BA0-4FA2-AD03-48E9FD0F4C92"), ComVisible(true)]
    public interface IGroupActor
    {
        // Actor interface
        string Name { get; set; }
        float X { get; set; }
        float Y { get; set; }
        float Width { get; set; }
        float Height { get; set; }
        bool Visible { get; set; }
        bool FillParent { get; set; }
        bool ClearBackground { get; set; }
        void SetBounds(float x, float y, float width, float height);
        void SetPosition(float x, float y);
        void SetAlignedPosition(float x, float y, Alignment alignment);
        void SetSize(float width, float height);
        float PrefWidth { get; }
        float PrefHeight { get; }
        void Pack();
        void Remove();
        IActionFactory ActionFactory { get; }
        void AddAction([MarshalAs(UnmanagedType.Struct)]Action action);
        void ClearActions();

        // Group interface
        bool Clip { get; set; }
        int ChildCount { get; }
        bool HasChild(string name);
        IGroupActor GetGroup(string name);
        IFrameActor GetFrame(string name);
        ILabelActor GetLabel(string name);
        IVideoActor GetVideo(string name);
        IImageSequenceActor GetImageSeq(string name);
        IImageActor GetImage(string name);
        void RemoveAll();
        void AddActor([MarshalAs(UnmanagedType.Struct)]Actor child);
        void RemoveActor([MarshalAs(UnmanagedType.Struct)]Actor child);
    }

    [Guid("05E06A6B-94DB-4F7F-B7A8-F8E09716A041"), ComVisible(true)]
    public interface IFrameActor
    {
        // Actor interface
        string Name { get; set; }
        float X { get; set; }
        float Y { get; set; }
        float Width { get; set; }
        float Height { get; set; }
        bool Visible { get; set; }
        bool FillParent { get; set; }
        bool ClearBackground { get; set; }
        void SetBounds(float x, float y, float width, float height);
        void SetPosition(float x, float y);
        void SetAlignedPosition(float x, float y, Alignment alignment);
        void SetSize(float width, float height);
        float PrefWidth { get; }
        float PrefHeight { get; }
        void Pack();
        void Remove();
        IActionFactory ActionFactory { get; }
        void AddAction([MarshalAs(UnmanagedType.Struct)]Action action);
        void ClearActions();

        // Frame interface
        int Thickness { get; set; }
        Color BorderColor { get; set; }
        bool Fill { get; set; }
        Color FillColor { get; set; }
    }

    [Guid("42CAEB83-C045-443D-9528-4304E9F27A20"), ComVisible(true)]
    public interface IImageActor
    {
        // Actor interface
        string Name { get; set; }
        float X { get; set; }
        float Y { get; set; }
        float Width { get; set; }
        float Height { get; set; }
        bool Visible { get; set; }
        bool FillParent { get; set; }
        bool ClearBackground { get; set; }
        void SetBounds(float x, float y, float width, float height);
        void SetPosition(float x, float y);
        void SetAlignedPosition(float x, float y, Alignment alignment);
        void SetSize(float width, float height);
        float PrefWidth { get; }
        float PrefHeight { get; }
        void Pack();
        void Remove();
        IActionFactory ActionFactory { get; }
        void AddAction([MarshalAs(UnmanagedType.Struct)]Action action);
        void ClearActions();

        // Image interface
        Scaling Scaling { get; set; }
        Alignment Alignment { get; set; }
        Bitmap Bitmap { get; set; }
    }


    [Guid("225D3F5A-E69F-4290-81CC-79F21ADD35AF"), ComVisible(true)]
    public interface IImageSequenceActor
    {
        // Actor interface
        string Name { get; set; }
        float X { get; set; }
        float Y { get; set; }
        float Width { get; set; }
        float Height { get; set; }
        bool Visible { get; set; }
        bool FillParent { get; set; }
        bool ClearBackground { get; set; }
        void SetBounds(float x, float y, float width, float height);
        void SetPosition(float x, float y);
        void SetAlignedPosition(float x, float y, Alignment alignment);
        void SetSize(float width, float height);
        float PrefWidth { get; }
        float PrefHeight { get; }
        void Pack();
        void Remove();
        IActionFactory ActionFactory { get; }
        void AddAction([MarshalAs(UnmanagedType.Struct)] Action action);
        void ClearActions();

        // Image Sequence interface
        int FPS { get; set; }
        Scaling Scaling { get; set; }
        Alignment Alignment { get; set; }
        float Length { get; }
        bool Loop { get; set; }
        bool Paused { get; set; }
        void Seek(float posInSeconds);
    }

    [Guid("CF9AFD55-03A3-458D-8EAB-119C55090BAB"), ComVisible(true)]
    public interface IVideoActor
    {
        // Actor interface
        string Name { get; set; }
        float X { get; set; }
        float Y { get; set; }
        float Width { get; set; }
        float Height { get; set; }
        bool Visible { get; set; }
        bool FillParent { get; set; }
        bool ClearBackground { get; set; }
        void SetBounds(float x, float y, float width, float height);
        void SetPosition(float x, float y);
        void SetAlignedPosition(float x, float y, Alignment alignment);
        void SetSize(float width, float height);
        float PrefWidth { get; }
        float PrefHeight { get; }
        void Pack();
        void Remove();
        IActionFactory ActionFactory { get; }
        void AddAction([MarshalAs(UnmanagedType.Struct)]Action action);
        void ClearActions();

        // Video interface
        Scaling Scaling { get; set; }
        Alignment Alignment { get; set; }
        float Length { get; }
        bool Loop { get; set; }
        bool Paused { get; set; }
        void Seek(float posInSeconds);
    }

    [Guid("A8AAD77F-4F01-433B-B653-B6F14234F4F2"), ComVisible(true)]
    public interface ILabelActor
    {
        // Actor interface
        string Name { get; set; }
        float X { get; set; }
        float Y { get; set; }
        float Width { get; set; }
        float Height { get; set; }
        bool Visible { get; set; }
        bool FillParent { get; set; }
        bool ClearBackground { get; set; }
        void SetBounds(float x, float y, float width, float height);
        void SetPosition(float x, float y);
        void SetAlignedPosition(float x, float y, Alignment alignment);
        void SetSize(float width, float height);
        float PrefWidth { get; }
        float PrefHeight { get; }
        void Pack();
        void Remove();
        IActionFactory ActionFactory { get; }
        void AddAction([MarshalAs(UnmanagedType.Struct)]Action action);
        void ClearActions();

        // Label interface
        bool AutoPack { get; set; }
        Alignment Alignment { get; set; }
        Font Font { [return: MarshalAs(UnmanagedType.Struct)] get; [param: MarshalAs(UnmanagedType.Struct)] set; }
        string Text { get; set; }
    }

    [Guid("77FB8996-E143-42A4-B695-14E0314D92FC"), ComVisible(true)]
    public enum RenderMode
    {
        DMD_GRAY_2,
        DMD_GRAY_4,
        DMD_RGB,
        SEG_2x16Alpha,
        SEG_2x20Alpha,
        SEG_2x7Alpha_2x7Num,
        SEG_2x7Alpha_2x7Num_4x1Num,
        SEG_2x7Num_2x7Num_4x1Num,
        SEG_2x7Num_2x7Num_10x1Num,
        SEG_2x7Num_2x7Num_4x1Num_gen7,
        SEG_2x7Num10_2x7Num10_4x1Num,
        SEG_2x6Num_2x6Num_4x1Num,
        SEG_2x6Num10_2x6Num10_4x1Num,
        SEG_4x7Num10,
        SEG_6x4Num_4x1Num,
        SEG_2x7Num_4x1Num_1x16Alpha,
        SEG_1x16Alpha_1x16Num_1x7Num
    }

    [Guid("B592E61D-9553-4D91-A0F5-FDF111E28F5E"), ComVisible(true)]
    public interface IFlexDMD
    {
        #region Properties

        /// <summary>
        /// Component version.
        /// Major part is multiplied by 1000, added to the minor part. For example 1.9 version will return 1009.
        /// </summary>
        int Version { get; }

        /// <summary>
        /// You need to set to true to start the rendering daemon thread, and to false to stop it.
        /// </summary>
        bool Run { get; set; }

        /// <summary>
        /// By default, FlexDMD outputs through DmdDevice in order to render to a virtual or real DMD. You may set this to false if you don't want this output, for example, when you want to render only using Visual Pinball embedded DMD.
        /// </summary>
        bool Show { get; set; }

        /// <summary>
        /// Define the game name which will be shared with the rendering backend to allow contextual skinning.
        /// Note that changing the game name after Init result in a DMD reinitialization (with flickering).
        /// </summary>
        string GameName { get; set; }

        /// <summary>
        /// Defines or read the DMD width. Note that the width may only be changed on an unitiliazed DMD (i.e. before calling Init)
        /// Note that changing the width after Init result in a DMD reinitialization (with flickering).
        /// </summary>
        ushort Width { get; set; }

        /// <summary>
        /// Defines or read the DMD height. Note that the height may only be changed on an unitiliazed DMD (i.e. before calling Init)
        /// Note that changing the height after Init result in a DMD reinitialization (with flickering).
        /// </summary>
        ushort Height { get; set; }

        /// <summary>
        /// Defines the RGB color (only used for 2 & 4 bit planes render mode).
        /// Note that changing the color after Init result in a DMD reinitialization (with flickering).
        /// </summary>
        Color Color { get; set; }

        /// <summary>
        /// The DMD is rendered with a 4 bit plane shade of gray. You can choose a 2 bit plane or a full RGB rendering here.
        /// 0 is 2 bit planes
        /// 1 is 4 bit planes, the default
        /// 2 is full RGB
        /// Note that changing the render mode after Init result in a DMD reinitialization (with flickering).
        /// </summary>
        RenderMode RenderMode { get; set; }

        /// <summary>
        /// Defines the project folder where assets are looked for.
        /// </summary>
        string ProjectFolder { get; set; }

        /// <summary>
        /// Defines the table file name (relative to the project folder, see ProjectFolder).
        /// You need to define the table file, if you intend to use images stored inside the table file (for example 'VPX.image' with 'image' stored in the table file)
        /// </summary>
        string TableFile { get; set; }

        /// <summary>
        /// If sets to True, the DMD is cleared before rendering for each frame. Default is false.
        /// </summary>
		bool Clear { get; set; }
		
        /// <summary>
        /// Returns the DMD content as an array of uint RGB pixels for rendering inside VPX's embedded DMD
        /// </summary>
        object DmdColoredPixels { [return: MarshalAs(UnmanagedType.Struct, SafeArraySubType = VarEnum.VT_ARRAY)] get; }

        /// <summary>
        /// Returns the DMD content as an array of byte pixels (0 => black, 255 => full light) for rendering inside VPX's embedded DMD
        /// </summary>
        object DmdPixels { [return: MarshalAs(UnmanagedType.Struct, SafeArraySubType = VarEnum.VT_ARRAY)] get; }

        /// <summary>
        /// Set the segments for Alpha/Num display
        /// </summary>
        object Segments { [param: In, MarshalAs(UnmanagedType.Struct, SafeArraySubType = VarEnum.VT_ARRAY)] set; }

        /// <summary>
        /// Returns the main rendering surface. Note that you need to synchronize the modification using Lock/Unlock to avoid concurrent modification with the render thread.
        /// </summary>
        IGroupActor Stage { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Lock the render thread to allow modifying the stage avoiding concurrent modifications.
        /// </summary>
        void LockRenderThread();

        /// <summary>
        /// Unlock the render thread to allow the render thread to render the stage after modifying it.
        /// </summary>
        void UnlockRenderThread();

        /// <summary>
        /// </summary>
        IGroupActor NewGroup(string name);

        /// <summary>
        /// </summary>
        IFrameActor NewFrame(string name);

        /// <summary>
        /// </summary>
        ILabelActor NewLabel(string name, [param: MarshalAs(UnmanagedType.Struct)] Font font, string text);

        /// <summary>
        /// </summary>
        IVideoActor NewVideo(string name, string video);

        /// <summary>
        /// </summary>
        IImageSequenceActor NewImageSequence(string name, int fps, string images);

        /// <summary>
        /// </summary>
        IImageActor NewImage(string name, string image);

        /// <summary>
		/// Create a new font to be used for labels.
		/// Note that for the time being, border size is limited to either 0 (no border) or 1 (1px border)
        /// </summary>
        [return: MarshalAs(UnmanagedType.Struct)] Font NewFont(string font, Color tint, Color borderTint, int borderSize);

        /// <summary>
        /// </summary>
        IUltraDMD NewUltraDMD();

        #endregion
    }

    [Guid("83fbf3e4-b4f4-415a-9a5b-7c2f635ff83b"), ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IDMDObjectEvents
    {
        #region Events

        [DispId(1)]
        void onDMDChanged();

        #endregion
    }

    /// <summary>
    /// This interface is the original UltraDMD API implemented by FlexDMD to ease the transition.
    /// 
    /// DMDObject represents the DMD display.  It consists of a set of predefined, but very flexible scenes.  Some of these scenes are
    /// displayed immediately and replace whatever is currently displayed on the DMD.  Other scenes are placed into a queue, and displayed
    /// in the order in which they were queued.  This allows for more complex looking animations.
    /// 
    /// This early DMD interface attempts to expose a simplified object model for creating scenes with transitions, animations, video and text.
    /// The starting point is a set of 'scenes' required by many pinball machines.  Most scenes/animations that are not directly supported here,
    /// can be achieved by creating a video or animated GIF.
    /// 
    /// There is no direct access to the scene queue; but there is limited control through the ability to clear the queue, clear the
    /// currently rendering scene, or clear the currently rendering scene only if it matches a specific scene identifier.
    /// </summary>
    [Guid("F7E68187-251F-4DFB-AF79-F1D4D69EE188"), ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IUltraDMD
    {
        #region Methods

        /// <summary>
        /// Load the setup from the windows' registry.
        /// </summary>
        void LoadSetup();

        /// <summary>
        /// Init must be called before any other method.  It initializes the scene queue and internal state.
        /// </summary>
        void Init();

        /// <summary>
        /// Uninit currently does nothing; but in the future it is possible that some resources may need to be released.  This could be the
        /// best place to do that.
        /// </summary>
        void Uninit();

        /// <summary>
        /// The DMDObject interface will change over time.  Any time a breaking change is introduced, the major version will be incremented.
        /// Your table should be hard coded to an exact match to this major version.
        /// </summary>
        /// <returns>Integer major version</returns>
        Int32 GetMajorVersion();

        /// <summary>
        /// The DMDObject interface minor version will be rev'd on every non-breaking change.  The minor version returned should be greater
        /// than or equal to the version for which your table was authored.
        /// </summary>
        /// <returns>Integer minor version</returns>
        Int32 GetMinorVersion();

        /// <summary>
        /// The Build Number is an always increasing number.  It is a six digit base 10 number made up of Year Month Day YYMMDD.  It is
        /// provided for reference only.
        /// </summary>
        /// <returns>Integer build number</returns>
        Int32 GetBuildNumber();

        /// <summary>
        /// Some tables may want to allow the user to select whether or not to show the VirtualDMD.
        /// </summary>
        /// <returns>boolean previous visible state</returns>
        bool SetVisibleVirtualDMD(bool bHide);

        /// <summary>
        /// Some DMDs are displayed as a mirror image (Pin2K)
        /// </summary>
        /// <returns>boolean previous flipY state</returns>
        bool SetFlipY(bool flipY);

        /// <summary>
        /// Check to see if the DMD is currently rendering a video or animation.  If multiple scenes are queued, IsRendering will return
        /// true, even if the DMD is not currently animating.
        /// </summary>
        /// <returns></returns>
        bool IsRendering();

        /// <summary>
        /// Cancels all rendering and empties the scene queue.
        /// </summary>
        void CancelRendering();

        /// <summary>
        /// Cancels the specified scene if it is currently rendering.  The scene identifier is user defined.
        /// </summary>
        /// <param name="sceneId">User defined sceneId</param>
        void CancelRenderingWithId(string sceneId);

        /// <summary>
        /// This Clears the DMD display.  It does NOT clear the scene queue or stop any rendering.  It simply
        /// does a single wipe of the DMD and darkens all LEDs / pixels.
        /// </summary>
        void Clear();

        /// <summary>
        /// Your table's DMD assets should all go into a single folder, but this is no requirement.  The basePath is prepended
        /// to any filename supplied to any method.  It allows for specifying only filenames.  Set the project folder base
        /// path immediately after calling Init.  There is nothing wrong with setting a project folder, queueing a few scenes,
        /// then setting a different project folder.
        /// </summary>
        /// <param name="basePath">Asset base path</param>
        void SetProjectFolder(string basePath);

        /// <summary>
        /// SetVideoStretchMode sets the default display mode for how videos are displayed.  Videos do not need to be
        /// edited to have their aspect ratio matched to the DMD aspect ratio.  Videos can be 640x460, or 640x360, or
        /// any other resolution or aspect ratio.  The video stretch mode determines what horizontal section of the
        /// video will be displayed: top, middle, or bottom.  Alternately, it is possible to display the video distorted
        /// and stretch to fit the DMD aspect ratio.
        /// </summary>
        /// <param name="mode">mode - stretch: 0, crop to top: 1, crop to center: 2, crop to bottom: 3</param>
        void SetVideoStretchMode(Int32 mode);

        /// <summary>
        /// The scoreboard "scene" can be customized with a background image, and custom brightness for the current
        /// player, and other text.  The background is limited to a static image: video and animated gif are not
        /// yet supported.
        /// </summary>
        /// <param name="filename">background image must provide appropriate contrast for the scores and other text</param>
        /// <param name="selectedBrightness">current player brightness 0-15</param>
        /// <param name="unselectedBrightness">not selected players and other text 0-15</param>
        void SetScoreboardBackgroundImage(string filename, Int32 selectedBrightness, Int32 unselectedBrightness);

        /// <summary>
        /// The simplest form of animation is a series of still images.  This creates an animation which can loop, or not,
        /// over a comma separated list of image files.  The identifier returned, cast to a string, can be passed in as a background
        /// for supported scenes.
        /// </summary>
        /// <param name="fps">The speed at which the animation occurs, roughly in frames per second</param>
        /// <param name="loop">An animation can loop indefinitely</param>
        /// <param name="imagelist">Comma separated list of image file names</param>
        /// <returns>The returned animation identifier may be converted to a string, then passed in as a background</returns>
        Int32 CreateAnimationFromImages(Int32 fps, bool loop, string imagelist);

        /// <summary>
        /// Register a video into the DMD cache.
        /// </summary>
        /// <param name="videoStretchMode">See 'SetVideoStretchMode' for information on the videoStretchMode.</param>
        /// <param name="loop">A video can loop the video indefinitely</param>
        /// <param name="videoFilename">The video filename.</param>
        /// <returns>The returned identifier may be converted to a string, then passed in as a background</returns>
        Int32 RegisterVideo(Int32 videoStretchMode, bool loop, string videoFilename);

        /// <summary>
        /// Displays the UltraDMD animation sequence
        /// </summary>
        void DisplayVersionInfo();

        /// <summary>
        /// The basic scoreboard supports up to 4 players, with a field at the lower left and another at the lower right.  These lower
        /// text fields can be used to display the number of "credits", and "ball" number or "game over".  The scoreboard is one of the
        /// only scenes which displays on the DMD immediately before returning to the caller.  Most other scenes are queued.  If any
        /// other animation is queued, DisplayScoreboard will return immediately, without changing the DMD display.
        /// </summary>
        /// <param name="cPlayers">Count of players 0-4</param>
        /// <param name="highlightedPlayer">Highlighted player 0-4</param>
        /// <param name="score1">Score player 1 displayed in the upper left</param>
        /// <param name="score2">Score player 2 displayed in the upper right</param>
        /// <param name="score3">Score player 3 displayed beneath player 1</param>
        /// <param name="score4">Score player 4 displayed beneath player 2</param>
        /// <param name="lowerLeft">Text will appear left aligned at the bottom left</param>
        /// <param name="lowerRight">Text will appear right aligned at the bottom right</param>
        void DisplayScoreboard(Int32 cPlayers, Int32 highlightedPlayer, Int64 score1, Int64 score2, Int64 score3, Int64 score4, string lowerLeft, string lowerRight);

        void DisplayScoreboard00(int cPlayers, int highlightedPlayer, Int64 score1, Int64 score2, Int64 score3, Int64 score4, string lowerLeft, string lowerRight);

        /// <summary>
        /// Other than the scoreboard scene, Scene00 may be the only scene required for a pinball table.  It can
        /// display a background image, animated GIF, video, or animation image list returned from CreateAnimationFromImages.
        /// Over the background can be either a single line of text, or two lines of text (top line smaller than the 
        /// bottom line).  Each line of text can be a different brightness.  The whole scene can be transitioned
        /// onto the DMD display using any of the predefined animation types.  The scene will pause then optionally
        /// transition off the DMD display.
        /// </summary>
        /// <param name="background">Background image, animation image list, animated GIF or video</param>
        /// <param name="toptext">Top line of text, may be empty string</param>
        /// <param name="topBrightness">Top line brightness 0-15</param>
        /// <param name="bottomtext">Bottom line of text, may be empty string</param>
        /// <param name="bottomBrightness">Bottom line brightness 0-15</param>
        /// <param name="animateIn">Any of the predefined animation types</param>
        /// <param name="pauseTime">Pause before out animation, roughly in milliseconds</param>
        /// <param name="animateOut">Any of the predefined animation types, will execute after the specified pause time</param>
        void DisplayScene00(string background, string toptext, Int32 topBrightness, string bottomtext, Int32 bottomBrightness, Int32 animateIn, Int32 pauseTime, Int32 animateOut);

        /// <summary>
        /// Scene 00 with more options.  Specifically, the ability to display outline and/or fill font for either
        /// the top and/or bottom text.
        /// </summary>
        /// <param name="background">Background image, animation image list, animated GIF or video</param>
        /// <param name="toptext">Top line of text, may be empty string</param>
        /// <param name="topBrightness">Font fill brightness 0-15</param>
        /// <param name="topOutlineBrightness">Outline font brightness 0-15</param>
        /// <param name="bottomtext">Bottom line of text, may be empty string</param>
        /// <param name="bottomBrightness">Font fill brightness 0-15</param>
        /// <param name="bottomOutlineBrightness">Outline font brightness 0-15</param>
        /// <param name="animateIn">Any of the predefined animation types</param>
        /// <param name="pauseTime">Pause before out animation, roughly in milliseconds</param>
        /// <param name="animateOut">Any of the predefined animation types, will execute after the specified pause time</param>
        void DisplayScene00Ex(string background, string toptext, Int32 topBrightness, Int32 topOutlineBrightness, string bottomtext, Int32 bottomBrightness, Int32 bottomOutlineBrightness, Int32 animateIn, Int32 pauseTime, Int32 animateOut);

        /// <summary>
        /// Scene 00 with still more options.  By specifying a sceneId, it is possible to make updates to the text without
        /// queueing a different scene.  This allows for a continuously running animation with a changing text overlay.  This
        /// prevents flicker when making changes in quick succession.
        /// </summary>
        /// <param name="sceneId">User defined scene identifier</param>
        /// <param name="cancelPrevious">Allows replacing an existing scene</param>
        /// <param name="background">Background image, animation image list, animated GIF or video</param>
        /// <param name="toptext">Top line of text, may be empty string</param>
        /// <param name="topBrightness">Font fill brightness 0-15</param>
        /// <param name="topOutlineBrightness">Outline font brightness 0-15</param>
        /// <param name="bottomtext">Bottom line of text, may be empty string</param>
        /// <param name="bottomBrightness">Font fill brightness 0-15</param>
        /// <param name="bottomOutlineBrightness">Outline font brightness 0-15</param>
        /// <param name="animateIn">Any of the predefined animation types</param>
        /// <param name="pauseTime">Pause before out animation, roughly in milliseconds</param>
        /// <param name="animateOut">Any of the predefined animation types, will execute after the specified pause time</param>
        void DisplayScene00ExWithId(string sceneId, bool cancelPrevious, string background, string toptext, Int32 topBrightness, Int32 topOutlineBrightness, string bottomtext, Int32 bottomBrightness, Int32 bottomOutlineBrightness, Int32 animateIn, Int32 pauseTime, Int32 animateOut);

        /// <summary>
        /// Modifies the scene identified by sceneId, if the scene is currently being rendered.  If the
        /// currently rendering scene doesn't match sceneId, modify returns immediately without doing
        /// anything.
        /// </summary>
        /// <param name="id">Identifier for the scene to modify</param>
        /// <param name="toptext">Top text to modify</param>
        /// <param name="bottomtext">Bottom text to modify</param>
        void ModifyScene00(string id, string toptext, string bottomtext);

        /// <summary>
        /// Modifies the scene identified by sceneId, if the scene is currently being rendered.  If the
        /// currently rendering scene doesn't match sceneId, modify returns immediately without doing
        /// anything.  The scene will be extended to the new pauseTime.
        /// </summary>
        /// <param name="id">Identifier for the scene to modify</param>
        /// <param name="toptext">Top text to modify</param>
        /// <param name="bottomtext">Bottom text to modify</param>
        /// <param name="pauseTime">Set the PauseTime from now</param>
        void ModifyScene00Ex(string id, string toptext, string bottomtext, int pauseTime);

        /// <summary>
        /// Scene 01 is similar to Scene 00, except that only a single line of text is supported; and the
        /// text is displayed in a horizontally scrolling animated overlay.
        /// </summary>
        /// <param name="sceneId">User defined scene identifier</param>
        /// <param name="background">Background image, animation image list, animated GIF or video</param>
        /// <param name="text">Text to display</param>
        /// <param name="textBrightness">Text fill brightness 0-15</param>
        /// <param name="textOutlineBrightness">Text outline brightness 0-15</param>
        /// <param name="animateIn">Any of the predefined animation types</param>
        /// <param name="pauseTime">Pause before out animation, roughly in milliseconds</param>
        /// <param name="animateOut">Any of the predefined animation types, will execute after the specified pause time</param>
        void DisplayScene01(string sceneId, string background, string text, Int32 textBrightness, Int32 textOutlineBrightness, Int32 animateIn, Int32 pauseTime, Int32 animateOut);

        /// <summary>
        /// Simple text display.  This text scene is not queued.  It is displayed immediately.
        /// </summary>
        /// <param name="text">Text to display</param>
        /// <param name="textBrightness">Font fill brightness</param>
        /// <param name="textOutlineBrightness">Font outline brightness</param>
        void DisplayText(string text, Int32 textBrightness, Int32 textOutlineBrightness);

        /// <summary>
        /// Scrolling credits is a useful way of displaying multiple lines of text which scroll vertically.  This can be used to
        /// display high scores, credits, game instructions, etc.
        /// </summary>
        /// <param name="background">Background image, animation image list, animated GIF or video</param>
        /// <param name="text">Multiple lines of text.  Each line of text is separated by a '|' character.</param>
        /// <param name="textBrightness">Text brightness 0-15</param>
        /// <param name="animateIn">Any of the predefined animation types</param>
        /// <param name="pauseTime">Pause before out animation, roughly in milliseconds</param>
        /// <param name="animateOut">Any of the predefined animation types, will execute after the specified pause time</param>
        void ScrollingCredits(string background, string text, Int32 textBrightness, Int32 animateIn, Int32 pauseTime, Int32 animateOut);

        #endregion
    }

    #endregion

}
