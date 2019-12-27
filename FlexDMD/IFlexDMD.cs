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

    /// <summary>
    /// DMDOjbect represents the DMD display.  It consists of a set of predefined, but very flexible scenes.  Some of these scenes are
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
    [Guid("F7E68187-251F-4DFB-AF79-F1D4D69EE188"), ComVisible(true)]
    /* [Guid("6de5b6e5-717b-48d3-a890-d1d2320ddc43"), ComVisible(true)] */
    public interface IDMDObject
    {
        #region Properties

        /// <summary>
        /// Define the game name which will be shared with the rendering backend to allow contextual skinning.
        /// Note that changing the game name after Init result in a DMD reinitialization.
        /// </summary>
        string GameName { get; set; }

        /// <summary>
        /// Defines or read the DMD width. Note that the width may only be changed on an unitiliazed DMD (i.e. before calling Init)
        /// Note that changing the width after Init result in a DMD reinitialization.
        /// </summary>
        ushort DmdWidth { get; set; }

        /// <summary>
        /// Defines or read the DMD height. Note that the height may only be changed on an unitiliazed DMD (i.e. before calling Init)
        /// Note that changing the height after Init result in a DMD reinitialization.
        /// </summary>
        ushort DmdHeight { get; set; }

        /// <summary>
        /// The DMD is rendered with a 4 bit plane shade of gray. You can choose a 2 bit plane or a full RGB rendering here.
        /// 0 is 2 bit planes
        /// 1 is 4 bit planes, the default
        /// 2 is full RGB
        /// Note that changing the render mode after Init result in a DMD reinitialization.
        /// </summary>
        int RenderMode { get; set; }

        /// <summary>
        /// Defines the RGB color (only used for 2 & 4 bit planes render mode).
        /// Note that changing the color after Init result in a DMD reinitialization.
        /// </summary>
        Color DmdColor { get; set; }

        /// <summary>
        /// Returns the DMD content as an array of uint RGB pixels
        /// </summary>
        object DmdColoredPixels
        {
            [return: MarshalAs(UnmanagedType.Struct, SafeArraySubType = VarEnum.VT_ARRAY)]
            get;
        }

        /// <summary>
        /// Returns the DMD content as an array of byte pixels (0 => black, 255 => full light)
        /// </summary>
        object DmdPixels
        {
            [return: MarshalAs(UnmanagedType.Struct, SafeArraySubType = VarEnum.VT_ARRAY)]
            get;
        }

        #endregion

        #region Methods
		
        /// <summary>
        /// Defines the table file name (relative to the project folder, see SetProjectFolder).
		/// You need to define the table file, if you intend to use images stored inside the table file (for example 'VPX.image.png' with image.png stored in the table file)
        /// </summary>
        void SetTableFile(string tableFile);
		
        /// <summary>
        /// Defines the fonts used for the scoreboard scene.
        /// </summary>
		void SetScoreFonts(string textFont, string normalFont, string highlightFont, int selectedBrightness, int unselectedBrightness);
		
        /// <summary>
        /// Defines the fonts used for scenes with 2 lines of text.
        /// </summary>
		void SetTwoLineFonts(string topFont, string bottomFont);
		
        /// <summary>
        /// Defines the fonts used for scenes with a single line of text. This is a list of decreasing 
		/// size fonts used to find a size that allows to fit the text in the DMD.
        /// </summary>
		void SetSingleLineFonts(string[] fonts);

        /// <summary>
        /// Display a scene composed of a background image and 2 lines of images. This scene corresponds 
		/// to the simple DMD JPSalas include in some of his original tables (Miraculous, Serious Sam I 
		/// & II, Pokemon,...). In these tables, VPX renders the DMD using either EMReel or Flasher objects 
		/// on the cab apron or on the desktop background. This scene allows to add a DMD display to these 
		/// tables very easily. Modified scripts for these tables are provided with the FlexDMD project.
		///
		/// If the scene, identified by its id, is already in the scene queue, it will be modified and not 
		/// reseted to avoid restarting any ongoing animation (video background for example).
        /// </summary>
		void DisplayJPSScene(string id, string background, string[] top, string[] bottom, Int32 animateIn, Int32 pauseTime, Int32 animateOut);
		
        #endregion

		// Below this point, you will find the original UltraDMD API
		
        #region Methods

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
        void DisplayScoreboard(Int32 cPlayers, Int32 highlightedPlayer, Int32 score1, Int32 score2, Int32 score3, Int32 score4, string lowerLeft, string lowerRight);

        void DisplayScoreboard00(int cPlayers, int highlightedPlayer, int score1, int score2, int score3, int score4, string lowerLeft, string lowerRight);

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

    [Guid("83fbf3e4-b4f4-415a-9a5b-7c2f635ff83b"), ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IDMDObjectEvents
    {
        #region Events

        [DispId(1)]
        void onDMDChanged();

        #endregion
    }

    #endregion

}
